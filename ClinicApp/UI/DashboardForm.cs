using System;
using System.Drawing;
using System.Windows.Forms;
using ClinicApp.Data;

namespace ClinicApp.UI
{
    public class DashboardForm : Form
    {
        private TabControl tabControl;

        public DashboardForm()
        {
            InitializeUI();
        }

        private void InitializeUI()
        {
            var config = DataManager.LoadData().Config;
            this.Text = $"Employee Dashboard - {config.ClinicName}";
            this.Size = new Size(600, 400);
            this.StartPosition = FormStartPosition.CenterScreen;

            tabControl = new TabControl { Dock = DockStyle.Fill, Font = new Font("Segoe UI", 10) };

            string[] categories = { "Normal", "Special", "VIP" };
            foreach (var cat in categories)
            {
                var tab = new TabPage(cat);
                var panel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(50) };
                
                var lbl = new Label { Text = $"Manage {cat} Appointments", Font = new Font("Segoe UI", 14, FontStyle.Bold), AutoSize = true, Location = new Point(50, 50) };
                
                var btnBook = new Button { Text = "Book Appointment", Width = 200, Height = 50, Location = new Point(150, 120), BackColor = Color.DodgerBlue, ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
                var btnView = new Button { Text = "View Appointments", Width = 200, Height = 50, Location = new Point(150, 190), BackColor = Color.SeaGreen, ForeColor = Color.White, FlatStyle = FlatStyle.Flat };

                btnBook.Click += (s, e) => new BookAppointmentForm(cat).ShowDialog();
                btnView.Click += (s, e) => new ViewAppointmentsForm(cat).ShowDialog();

                panel.Controls.AddRange(new Control[] { lbl, btnBook, btnView });
                tab.Controls.Add(panel);
                tabControl.TabPages.Add(tab);
            }

            this.Controls.Add(tabControl);
        }
    }
}
