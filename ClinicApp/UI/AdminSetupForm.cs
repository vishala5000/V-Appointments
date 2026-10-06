using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using ClinicApp.Data;
using ClinicApp.Models;

namespace ClinicApp.UI
{
    public class AdminSetupForm : Form
    {
        private TextBox txtClinicName, txtNewDoctor;
        private ListBox lstDoctors;
        private DateTimePicker dtpStart, dtpEnd;
        private NumericUpDown nudNormal, nudSpecial, nudVip;

        public AdminSetupForm() { InitializeUI(); }

        private void InitializeUI()
        {
            this.Text = "Clinic Admin Setup";
            this.BackColor = Color.FromArgb(245, 245, 245);
            this.WindowState = FormWindowState.Maximized;
            this.FormBorderStyle = FormBorderStyle.Sizable;

            var mainLayout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 3, RowCount = 1 };
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 600));
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));

            var formContainer = new Panel { Dock = DockStyle.Fill, AutoScroll = true };
            mainLayout.Controls.Add(formContainer, 1, 0);

            var layout = new TableLayoutPanel { Dock = DockStyle.Top, ColumnCount = 1, AutoSize = true };
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

            layout.Controls.Add(new Label { Text = "Initial Clinic Configuration", Font = new Font("Segoe UI", 20, FontStyle.Bold), AutoSize = true, Padding = new Padding(0, 20, 0, 20), ForeColor = Color.DodgerBlue });

            AddLabel(layout, "Clinic / Hospital Name:");
            txtClinicName = new TextBox { Font = new Font("Segoe UI", 11), Height = 30 };
            layout.Controls.Add(txtClinicName);

            AddLabel(layout, "Add Doctors:");
            var docPanel = new FlowLayoutPanel { FlowDirection = FlowDirection.LeftToRight, Height = 40 };
            txtNewDoctor = new TextBox { Width = 350, Font = new Font("Segoe UI", 11) };
            var btnAddDoc = new Button { Text = "Add", Width = 80, Height = 30, BackColor = Color.DodgerBlue, ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            var btnRemDoc = new Button { Text = "Remove", Width = 80, Height = 30, BackColor = Color.IndianRed, ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            docPanel.Controls.AddRange(new Control[] { txtNewDoctor, btnAddDoc, btnRemDoc });
            layout.Controls.Add(docPanel);

            lstDoctors = new ListBox { Height = 120, Font = new Font("Segoe UI", 11) };
            layout.Controls.Add(lstDoctors);

            AddLabel(layout, "Operating Hours:");
            var timePanel = new FlowLayoutPanel { Height = 40 };
            dtpStart = new DateTimePicker { Format = DateTimePickerFormat.Time, Width = 150, Font = new Font("Segoe UI", 11) };
            dtpEnd = new DateTimePicker { Format = DateTimePickerFormat.Time, Width = 150, Font = new Font("Segoe UI", 11) };
            timePanel.Controls.AddRange(new Control[] { new Label{Text="Start:", AutoSize=true, Padding=new Padding(0,8,0,0)}, dtpStart, 
                new Label { Text = "End:", AutoSize = true, Padding = new Padding(30, 8, 0, 0) }, dtpEnd });
            layout.Controls.Add(timePanel);

            AddLabel(layout, "Appointment Slot Durations (Minutes):");
            var durPanel = new FlowLayoutPanel { Height = 40 };
            nudNormal = new NumericUpDown { Width = 100, Minimum = 10, Maximum = 120, Value = 30, Font = new Font("Segoe UI", 11) };
            nudSpecial = new NumericUpDown { Width = 100, Minimum = 10, Maximum = 120, Value = 45, Font = new Font("Segoe UI", 11) };
            nudVip = new NumericUpDown { Width = 100, Minimum = 10, Maximum = 120, Value = 60, Font = new Font("Segoe UI", 11) };
            durPanel.Controls.AddRange(new Control[] { new Label{Text="Normal:",AutoSize=true, Padding=new Padding(0,8,0,0)}, nudNormal, 
                new Label{Text="Special:",AutoSize=true, Padding=new Padding(30,8,0,0)}, nudSpecial, 
                new Label{Text="VIP:",AutoSize=true, Padding=new Padding(30,8,0,0)}, nudVip });
            layout.Controls.Add(durPanel);

            var btnSave = new Button { Text = "Save Configuration & Launch Dashboard", Height = 50, BackColor = Color.SeaGreen, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 12, FontStyle.Bold), Margin = new Padding(0, 30, 0, 0) };
            btnSave.Click += BtnSave_Click;
            layout.Controls.Add(btnSave);

            formContainer.Controls.Add(layout);
            this.Controls.Add(mainLayout);

            btnAddDoc.Click += (s, e) => { if(!string.IsNullOrWhiteSpace(txtNewDoctor.Text)) lstDoctors.Items.Add(txtNewDoctor.Text.Trim()); txtNewDoctor.Clear(); };
            btnRemDoc.Click += (s, e) => { if(lstDoctors.SelectedItem != null) lstDoctors.Items.Remove(lstDoctors.SelectedItem); };
        }

        private void AddLabel(TableLayoutPanel layout, string text)
        {
            layout.Controls.Add(new Label { Text = text, AutoSize = true, Font = new Font("Segoe UI", 11, FontStyle.Bold), Padding = new Padding(0, 15, 0, 5) });
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtClinicName.Text) || lstDoctors.Items.Count == 0)
            {
                MessageBox.Show("Please enter Clinic Name and add at least one Doctor.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var config = new ClinicConfig
            {
                ClinicName = txtClinicName.Text.Trim(),
                Doctors = lstDoctors.Items.Cast<string>().ToList(),
                StartTime = dtpStart.Value.TimeOfDay,
                EndTime = dtpEnd.Value.TimeOfDay,
                NormalDurationMins = (int)nudNormal.Value,
                SpecialDurationMins = (int)nudSpecial.Value,
                VipDurationMins = (int)nudVip.Value
            };

            DataManager.SaveConfig(config);
            this.Hide();
            new DashboardForm().ShowDialog();
            this.Close();
        }
    }
}
