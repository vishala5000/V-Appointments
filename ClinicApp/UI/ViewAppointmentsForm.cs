using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using ClinicApp.Data;

namespace ClinicApp.UI
{
    public class ViewAppointmentsForm : Form
    {
        private string _category;
        private DataGridView dgv;
        private TextBox txtSearch;

        public ViewAppointmentsForm(string category) { _category = category; InitializeUI(); }

        private void InitializeUI()
        {
            this.Text = $"{_category} Appointments";
            this.Size = new Size(900, 550);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.MinimumSize = new Size(700, 400);

            // Top Panel for Search and Actions
            Panel topPanel = new Panel { Dock = DockStyle.Top, Height = 60, BackColor = Color.WhiteSmoke, Padding = new Padding(20, 15, 20, 10) };
            txtSearch = new TextBox { Width = 300, Font = new Font("Segoe UI", 11) };
            txtSearch.TextChanged += (s, e) => ApplyFilter();
            
            Button btnRefresh = new Button { Text = "Show All", Width = 90, Height = 30, Font = new Font("Segoe UI", 10) };
            btnRefresh.Click += (s, e) => { txtSearch.Clear(); LoadData(); };

            Button btnCancel = new Button { Text = "Cancel Selected", Width = 130, Height = 30, BackColor = Color.IndianRed, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 10, FontStyle.Bold) };
            btnCancel.Click += BtnCancel_Click;

            FlowLayoutPanel flow = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.LeftToRight, WrapContents = false };
            flow.Controls.AddRange(new Control[] { new Label { Text = "Search Patient:", AutoSize = true, Padding = new Padding(0, 7, 10, 0) }, txtSearch, btnRefresh, btnCancel });
            topPanel.Controls.Add(flow);

            // DataGridView
            dgv = new DataGridView { Dock = DockStyle.Fill, ReadOnly = true, AllowUserToAddRows = false, SelectionMode = DataGridViewSelectionMode.FullRowSelect, BackgroundColor = Color.White, Font = new Font("Segoe UI", 10), RowHeadersVisible = false };
            
            this.Controls.Add(dgv);
            this.Controls.Add(topPanel);

            LoadData();
        }

        private void LoadData()
        {
            var appointments = DatabaseHelper.GetAppointments(_category);
            var viewData = appointments.Select(a => new 
            { 
                a.Id, // Hidden column used for deletion
                a.PatientName, 
                a.PhoneNumber, 
                Date = a.Date.ToString("yyyy-MM-dd"), 
                Time = a.TimeSlot.ToString(@"hh\:mm"), 
                a.Doctor,
                BookedOn = a.CreatedAt.ToString("yyyy-MM-dd HH:mm")
            }).ToList();

            dgv.DataSource = viewData;
            if(dgv.Columns.Contains("Id")) dgv.Columns["Id"].Visible = false;
            if(dgv.Columns.Contains("Date")) dgv.Columns["Date"].HeaderText = "Appointment Date";
            if(dgv.Columns.Contains("Time")) dgv.Columns["Time"].HeaderText = "Time Slot";
            if(dgv.Columns.Contains("BookedOn")) dgv.Columns["BookedOn"].HeaderText = "Booked On";
        }

        private void ApplyFilter()
        {
            if (string.IsNullOrWhiteSpace(txtSearch.Text)) { LoadData(); return; }
            var appointments = DatabaseHelper.GetAppointments(_category);
            var filtered = appointments.Where(a => a.PatientName.IndexOf(txtSearch.Text, StringComparison.OrdinalIgnoreCase) >= 0 || a.PhoneNumber.Contains(txtSearch.Text)).Select(a => new 
            { 
                a.Id, a.PatientName, a.PhoneNumber, Date = a.Date.ToString("yyyy-MM-dd"), Time = a.TimeSlot.ToString(@"hh\:mm"), a.Doctor, BookedOn = a.CreatedAt.ToString("yyyy-MM-dd HH:mm")
            }).ToList();
            dgv.DataSource = filtered;
            if(dgv.Columns.Contains("Id")) dgv.Columns["Id"].Visible = false;
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            if (dgv.SelectedRows.Count == 0) { MessageBox.Show("Please select an appointment to cancel.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
            
            string id = dgv.SelectedRows[0].Cells["Id"].Value.ToString();
            string patient = dgv.SelectedRows[0].Cells["PatientName"].Value.ToString();

            if (MessageBox.Show($"Are you sure you want to cancel the appointment for {patient}?\nThis will free up the time slot.", "Confirm Cancellation", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                DatabaseHelper.DeleteAppointment(id);
                LoadData();
                MessageBox.Show("Appointment cancelled successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}
