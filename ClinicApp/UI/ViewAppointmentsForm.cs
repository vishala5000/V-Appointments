using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using ClinicApp.Data;

namespace ClinicApp.UI
{
    public class ViewAppointmentsForm : Form
    {
        private DataGridView dgv;

        public ViewAppointmentsForm(string category)
        {
            InitializeUI(category);
        }

        private void InitializeUI(string category)
        {
            this.Text = $"{category} Appointments";
            this.Size = new Size(800, 500);
            this.StartPosition = FormStartPosition.CenterParent;

            dgv = new DataGridView { Dock = DockStyle.Fill, ReadOnly = true, AllowUserToAddRows = false, SelectionMode = DataGridViewSelectionMode.FullRowSelect };
            
            var data = DataManager.LoadData();
            var filtered = data.Appointments.Where(a => a.Category == category).OrderBy(a => a.Date).ThenBy(a => a.TimeSlot).ToList();

            dgv.DataSource = filtered.Select(a => new 
            { 
                a.PatientName, 
                a.PhoneNumber, 
                Date = a.Date.ToShortDateString(), 
                Time = a.TimeSlot.ToString(@"hh\:mm"), 
                a.Doctor,
                BookedOn = a.CreatedAt.ToShortDateString()
            }).ToList();

            dgv.Columns["Date"].HeaderText = "Appointment Date";
            dgv.Columns["Time"].HeaderText = "Time Slot";
            dgv.Columns["BookedOn"].HeaderText = "Booked On";

            this.Controls.Add(dgv);
        }
    }
}
