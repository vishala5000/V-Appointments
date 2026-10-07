using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using ClinicApp.Data;

namespace ClinicApp.UI
{
    public class DashboardForm : Form
    {
        public DashboardForm() { InitializeUI(); }

        private void InitializeUI()
        {
            var config = DataManager.LoadConfig();
            this.Text = $"V Soft Dashboard - {config.ClinicName}";
            this.WindowState = FormWindowState.Maximized;
            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.BackColor = Color.White;

            // HEADER PANEL
            Panel headerPanel = new Panel { Dock = DockStyle.Top, Height = 80, BackColor = Color.FromArgb(245, 245, 245) };
            
            PictureBox picLogo = new PictureBox { Size = new Size(60, 60), SizeMode = PictureBoxSizeMode.Zoom };
            string logoPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logo.png");
            
            try
            {
                if (File.Exists(logoPath))
                {
                    using (var fs = new FileStream(logoPath, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        picLogo.Image = Image.FromStream(fs);
                    }
                }
                else
                {
                    picLogo.BackColor = Color.DodgerBlue; // Fallback if logo is missing
                }
            }
            catch
            {
                picLogo.BackColor = Color.LightGray; // Safe fallback on any image error
            }
            
            picLogo.Location = new Point(20, 10);

            Label lblTitle = new Label { Text = config.ClinicName, Font = new Font("Segoe UI", 18, FontStyle.Bold), ForeColor = Color.DodgerBlue, AutoSize = true, Location = new Point(90, 20) };
            
            Button btnSettings = new Button { Text = "⚙ Settings", Size = new Size(120, 40), BackColor = Color.Gray, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 10, FontStyle.Bold), Anchor = AnchorStyles.Top | AnchorStyles.Right };
            btnSettings.Location = new Point(headerPanel.Width - btnSettings.Width - 20, 20);
            headerPanel.Resize += (s, e) => btnSettings.Location = new Point(headerPanel.Width - btnSettings.Width - 20, 20);
            btnSettings.Click += (s, e) => new AdminSetupForm(true).ShowDialog();

            headerPanel.Controls.Add(picLogo);
            headerPanel.Controls.Add(lblTitle);
            headerPanel.Controls.Add(btnSettings);

            // TABS
            TabControl tabControl = new TabControl { Dock = DockStyle.Fill, Font = new Font("Segoe UI", 11) };
            string[] categories = { "Normal", "Special", "VIP" };
            Color[] colors = { Color.DodgerBlue, Color.MediumPurple, Color.Goldenrod };

            for (int i = 0; i < categories.Length; i++)
            {
                var cat = categories[i];
                var tab = new TabPage(cat) { BackColor = Color.WhiteSmoke };
                var outerPanel = new Panel { Dock = DockStyle.Fill };
                var innerPanel = new Panel { Size = new Size(450, 350), Anchor = AnchorStyles.None };
                
                outerPanel.Controls.Add(innerPanel);
                outerPanel.Resize += (s, e) => innerPanel.Location = new Point((outerPanel.Width - innerPanel.Width) / 2, (outerPanel.Height - innerPanel.Height) / 2);

                var lbl = new Label { Text = $"Manage {cat} Appointments", Font = new Font("Segoe UI", 18, FontStyle.Bold), AutoSize = true, Location = new Point(50, 50), ForeColor = colors[i] };
                var btnBook = CreateStyledButton("Book Appointment", new Point(50, 150), colors[i]);
                var btnView = CreateStyledButton("View Appointments", new Point(50, 230), Color.SeaGreen);

                btnBook.Click += (s, e) => new BookAppointmentForm(cat).ShowDialog();
                btnView.Click += (s, e) => new ViewAppointmentsForm(cat).ShowDialog();

                innerPanel.Controls.AddRange(new Control[] { lbl, btnBook, btnView });
                tab.Controls.Add(outerPanel);
                tabControl.TabPages.Add(tab);
            }

            this.Controls.Add(tabControl); 
            this.Controls.Add(headerPanel); 
        }

        private Button CreateStyledButton(string text, Point location, Color bgColor)
        {
            return new Button { Text = text, Size = new Size(350, 60), Location = location, BackColor = bgColor, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 12, FontStyle.Bold), Cursor = Cursors.Hand };
        }
    }
}
