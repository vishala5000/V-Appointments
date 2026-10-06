using System;
using System.Drawing;
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
            this.Text = $"Employee Dashboard - {config.ClinicName}";
            this.WindowState = FormWindowState.Maximized;
            this.FormBorderStyle = FormBorderStyle.Sizable;

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
                outerPanel.Resize += (s, e) => {
                    innerPanel.Location = new Point((outerPanel.Width - innerPanel.Width) / 2, (outerPanel.Height - innerPanel.Height) / 2);
                };

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
        }

        private Button CreateStyledButton(string text, Point location, Color bgColor)
        {
            return new Button { Text = text, Size = new Size(350, 60), Location = location, BackColor = bgColor, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 12, FontStyle.Bold), Cursor = Cursors.Hand };
        }
    }
}
