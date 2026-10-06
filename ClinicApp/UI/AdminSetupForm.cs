using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using ClinicApp.Data;

namespace ClinicApp.UI
{
    public class AdminSetupForm : Form
    {
        private TextBox txtClinicName;
        private ListBox lstDoctors;
        private TextBox txtNewDoctor;
        private DateTimePicker dtpStart, dtpEnd;
        private NumericUpDown nudNormal, nudSpecial, nudVip;

        public AdminSetupForm()
        {
            InitializeUI();
        }

        private void InitializeUI()
        {
            this.Text = "Clinic Admin Setup";
            this.Size = new Size(500, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            var layout = new TableLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(20), ColumnCount = 1 };
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

            layout.Controls.Add(new Label { Text = "Clinic Name:", AutoSize = true });
            txtClinicName = new TextBox { Width = 400 };
            layout.Controls.Add(txtClinicName);

            layout.Controls.Add(new Label { Text = "Doctors:", AutoSize = true, Padding = new Padding(0, 10, 0, 0) });
            var docPanel = new FlowLayoutPanel { FlowDirection = FlowDirection.LeftToRight, Width = 400, Height = 30 };
            txtNewDoctor = new TextBox { Width = 250 };
            var btnAddDoc = new Button { Text = "Add", Width = 60 };
            var btnRemDoc = new Button { Text = "Remove", Width = 60 };
            docPanel.Controls.AddRange(new Control[] { txtNewDoctor, btnAddDoc, btnRemDoc });
            layout.Controls.Add(docPanel);

            lstDoctors = new ListBox { Width = 400, Height = 100 };
            layout.Controls.Add(lstDoctors);

            var timePanel = new FlowLayoutPanel { Width = 400, Height = 30 };
            dtpStart = new DateTimePicker { Format = DateTimePickerFormat.Time, Width = 100 };
            dtpEnd = new DateTimePicker { Format = DateTimePickerFormat.Time, Width = 100 };
            timePanel.Controls.AddRange(new Control[] { new Label { Text = "Start:", AutoSize=true, Padding=new Padding(0,5,0,0) }, dtpStart, 
                new Label { Text = "End:", AutoSize=true, Padding=new Padding(20,5,0,0) }, dtpEnd });
            layout.Controls.Add(timePanel);

            layout.Controls.Add(new Label { Text = "Slot Durations (Minutes):", AutoSize = true, Padding = new Padding(0, 10, 0, 0) });
            var durPanel = new FlowLayoutPanel { Width = 400, Height = 30 };
            nudNormal = new NumericUpDown { Width = 80, Minimum = 10, Maximum = 120, Value = 30 };
            nudSpecial = new NumericUpDown { Width = 80, Minimum = 10, Maximum = 120, Value = 45 };
            nudVip = new NumericUpDown { Width = 80, Minimum = 10, Maximum = 120, Value = 60 };
            durPanel.Controls.AddRange(new Control[] { new Label{Text="Normal:",AutoSize=true}, nudNormal, 
                new Label{Text="Special:",AutoSize=true, Padding=new Padding(10,0,0,0)}, nudSpecial, 
                new Label{Text="VIP:",AutoSize=true, Padding=new Padding(10,0,0,0)}, nudVip });
            layout.Controls.Add(durPanel);

            var btnSave = new Button { Text = "Save & Start System", Width = 200, Height = 40, BackColor = Color.LightGreen };
            btnSave.Click += BtnSave_Click;
            layout.Controls.Add(btnSave);

            this.Controls.Add(layout);

            // Events
            btnAddDoc.Click += (s, e) => { if(!string.IsNullOrWhiteSpace(txtNewDoctor.Text)) lstDoctors.Items.Add(txtNewDoctor.Text.Trim()); txtNewDoctor.Clear(); };
            btnRemDoc.Click += (s, e) => { if(lstDoctors.SelectedItem != null) lstDoctors.Items.Remove(lstDoctors.SelectedItem); };
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtClinicName.Text) || lstDoctors.Items.Count == 0)
            {
                MessageBox.Show("Please enter Clinic Name and add at least one Doctor.", "Validation Error");
                return;
            }

            var data = DataManager.LoadData();
            data.Config.ClinicName = txtClinicName.Text.Trim();
            data.Config.Doctors = lstDoctors.Items.Cast<string>().ToList();
            data.Config.StartTime = dtpStart.Value.TimeOfDay;
            data.Config.EndTime = dtpEnd.Value.TimeOfDay;
            data.Config.NormalDurationMins = (int)nudNormal.Value;
            data.Config.SpecialDurationMins = (int)nudSpecial.Value;
            data.Config.VipDurationMins = (int)nudVip.Value;
            data.IsConfigured = true;

            DataManager.SaveData();
            MessageBox.Show("Setup Complete! Opening Dashboard...", "Success");
            
            this.Hide();
            new DashboardForm().ShowDialog();
            this.Close();
        }
    }
}
