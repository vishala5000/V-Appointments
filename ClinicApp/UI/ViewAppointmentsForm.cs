using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using ClinicApp.Data;

namespace ClinicApp.UI
{
    public class ViewAppointmentsForm : Form
    {
        public ViewAppointmentsForm(string category) { InitializeUI(category); }

        private void InitializeUI(string category)
        {
            this.Text = $"{category} Appointments";
            this.Size = new Size(850, 500);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.MinimumSize = new Size(600, 300);

            var dgv = new DataGridView { Dock = DockStyle.Fill, ReadOnly = true, AllowUserToAddRows = false, SelectionMode = DataGridViewSelectionMode.FullRowSelect, BackgroundColor = Color.White, Font = new Font("Segoe UI", 10) };
            
            var appointments = DatabaseHelper.GetAppointments(category);
            var viewData = appointments.Select(a => new 
            { 
                a.PatientName, 
                a.PhoneNumber, 
                Date = a.Date.ToString("yyyy-MM-dd"), 
                Time = a.TimeSlot.ToString(@"hh\:mm"), 
                a.Doctor,
                BookedOn = a.CreatedAt.ToString("yyyy-MM-dd HH:mm")
            }).ToList();

            dgv.DataSource = viewData;
            if(dgv.Columns.Contains("Date")) dgv.Columns["Date"].HeaderText = "Appointment Date";
            if(dgv.Columns.Contains("Time")) dgv.Columns["Time"].HeaderText = "Time Slot";
            if(dgv.Columns.Contains("BookedOn")) dgv.Columns["BookedOn"].HeaderText = "Booked On";

            this.Controls.Add(dgv);
        }
    }
}
