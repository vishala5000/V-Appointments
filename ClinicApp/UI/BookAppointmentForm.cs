using System;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using ClinicApp.Data;
using ClinicApp.Models;

namespace ClinicApp.UI
{
    public class BookAppointmentForm : Form
    {
        private string _category;
        private TextBox txtName, txtPhone;
        private DateTimePicker dtpDate;
        private ComboBox cmbDoctor, cmbSlot;

        public BookAppointmentForm(string category)
        {
            _category = category;
            InitializeUI();
        }

        private void InitializeUI()
        {
            this.Text = $"Book {_category} Appointment";
            this.Size = new Size(400, 400);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;

            var layout = new TableLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(20), ColumnCount = 2 };
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 100));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

            AddRow(layout, "Patient Name:", txtName = new TextBox { Dock = DockStyle.Fill });
            AddRow(layout, "Phone:", txtPhone = new TextBox { Dock = DockStyle.Fill });
            AddRow(layout, "Date:", dtpDate = new DateTimePicker { Dock = DockStyle.Fill, MinDate = DateTime.Today });
            
            var config = DataManager.LoadData().Config;
            cmbDoctor = new ComboBox { Dock = DockStyle.Fill, DropDownStyle = ComboBoxStyle.DropDownList };
            cmbDoctor.Items.AddRange(config.Doctors.ToArray());
            AddRow(layout, "Doctor:", cmbDoctor);

            cmbSlot = new ComboBox { Dock = DockStyle.Fill, DropDownStyle = ComboBoxStyle.DropDownList };
            AddRow(layout, "Time Slot:", cmbSlot);

            // Dynamic slot loading
            dtpDate.ValueChanged += (s, e) => LoadSlots();
            cmbDoctor.SelectedIndexChanged += (s, e) => LoadSlots();

            var btnBook = new Button { Text = "Confirm Booking", Width = 150, Height = 40, BackColor = Color.DodgerBlue, ForeColor = Color.White };
            btnBook.Click += BtnBook_Click;
            layout.Controls.Add(btnBook);
            layout.SetColumnSpan(btnBook, 2);

            this.Controls.Add(layout);
            LoadSlots();
        }

        private void AddRow(TableLayoutPanel layout, string label, Control ctrl)
        {
            layout.Controls.Add(new Label { Text = label, AutoSize = true, Anchor = AnchorStyles.Left, Padding = new Padding(0, 5, 0, 0) });
            layout.Controls.Add(ctrl);
        }

        private void LoadSlots()
        {
            cmbSlot.Items.Clear();
            if (cmbDoctor.SelectedItem == null) return;

            var slots = SlotGenerator.GetAvailableSlots(_category, dtpDate.Value.Date, cmbDoctor.SelectedItem.ToString());
            foreach (var slot in slots)
            {
                cmbSlot.Items.Add(slot.ToString(@"hh\:mm"));
            }
            if (cmbSlot.Items.Count > 0) cmbSlot.SelectedIndex = 0;
        }

        private void BtnBook_Click(object sender, EventArgs e)
        {
            // Validation
            if (string.IsNullOrWhiteSpace(txtName.Text) || cmbDoctor.SelectedItem == null || cmbSlot.SelectedItem == null)
            {
                MessageBox.Show("Please fill all fields.", "Validation Error"); return;
            }

            if (!Regex.IsMatch(txtPhone.Text, @"^(\+\d{1,2}\s)?\(?\d{3}\)?[\s.-]?\d{3}[\s.-]?\d{4}$"))
            {
                MessageBox.Show("Invalid phone number format.", "Validation Error"); return;
            }

            var data = DataManager.LoadData();
            var appt = new Appointment
            {
                Id = Guid.NewGuid().ToString("N"),
                PatientName = txtName.Text.Trim(),
                PhoneNumber = txtPhone.Text.Trim(),
                Date = dtpDate.Value.Date,
                TimeSlot = TimeSpan.Parse(cmbSlot.SelectedItem.ToString()),
                Doctor = cmbDoctor.SelectedItem.ToString(),
                Category = _category,
                CreatedAt = DateTime.Now
            };

            data.Appointments.Add(appt);
            DataManager.SaveData();

            MessageBox.Show("Appointment Booked Successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.Close();
        }
    }
}
