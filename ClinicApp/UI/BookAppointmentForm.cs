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

        public BookAppointmentForm(string category) { _category = category; InitializeUI(); }

        private void InitializeUI()
        {
            this.Text = $"Book {_category} Appointment";
            this.Size = new Size(450, 400);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            var layout = new TableLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(30), ColumnCount = 2 };
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 110));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

            AddRow(layout, "Patient Name:", txtName = new TextBox { Dock = DockStyle.Fill, Font = new Font("Segoe UI", 11) });
            AddRow(layout, "Phone:", txtPhone = new TextBox { Dock = DockStyle.Fill, Font = new Font("Segoe UI", 11) });
            AddRow(layout, "Date:", dtpDate = new DateTimePicker { Dock = DockStyle.Fill, Font = new Font("Segoe UI", 11), MinDate = DateTime.Today });
            
            var config = DataManager.LoadConfig();
            cmbDoctor = new ComboBox { Dock = DockStyle.Fill, DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 11) };
            cmbDoctor.Items.AddRange(config.Doctors.ToArray());
            AddRow(layout, "Doctor:", cmbDoctor);

            cmbSlot = new ComboBox { Dock = DockStyle.Fill, DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 11) };
            AddRow(layout, "Time Slot:", cmbSlot);

            dtpDate.ValueChanged += (s, e) => LoadSlots();
            cmbDoctor.SelectedIndexChanged += (s, e) => LoadSlots();

            var btnBook = new Button { Text = "Confirm Booking", Width = 150, Height = 40, BackColor = Color.DodgerBlue, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 11, FontStyle.Bold) };
            btnBook.Click += BtnBook_Click;
            layout.Controls.Add(btnBook);
            layout.SetColumnSpan(btnBook, 2);
            layout.SetCellStyle(btnBook, new TableLayoutPanelCellPadding(0, 20, 0, 0));

            this.Controls.Add(layout);
            LoadSlots();
        }

        private void AddRow(TableLayoutPanel layout, string label, Control ctrl)
        {
            layout.Controls.Add(new Label { Text = label, AutoSize = true, Anchor = AnchorStyles.Left, Padding = new Padding(0, 8, 0, 0), Font = new Font("Segoe UI", 11) });
            layout.Controls.Add(ctrl);
        }

        private void LoadSlots()
        {
            cmbSlot.Items.Clear();
            if (cmbDoctor.SelectedItem == null) return;
            var slots = SlotGenerator.GetAvailableSlots(_category, dtpDate.Value.Date, cmbDoctor.SelectedItem.ToString());
            foreach (var slot in slots) cmbSlot.Items.Add(slot.ToString(@"hh\:mm"));
            if (cmbSlot.Items.Count > 0) cmbSlot.SelectedIndex = 0;
        }

        private void BtnBook_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtName.Text) || cmbDoctor.SelectedItem == null || cmbSlot.SelectedItem == null)
            {
                MessageBox.Show("Please fill all fields.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning); return;
            }
            if (!Regex.IsMatch(txtPhone.Text, @"^(\+\d{1,2}\s)?\(?\d{3}\)?[\s.-]?\d{3}[\s.-]?\d{4}$"))
            {
                MessageBox.Show("Invalid phone number format.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning); return;
            }

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

            if (DatabaseHelper.SaveAppointment(appt))
            {
                MessageBox.Show("Appointment Booked Successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            }
            else
            {
                MessageBox.Show("This slot was just booked by someone else. Please select another.", "Booking Conflict", MessageBoxButtons.OK, MessageBoxIcon.Error);
                LoadSlots();
            }
        }
    }
}
