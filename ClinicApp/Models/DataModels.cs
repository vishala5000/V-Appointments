using System;
using System.Collections.Generic;

namespace ClinicApp.Models
{
    public class ClinicConfig
    {
        public string ClinicName { get; set; }
        public List<string> Doctors { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public int NormalDurationMins { get; set; }
        public int SpecialDurationMins { get; set; }
        public int VipDurationMins { get; set; }

        public ClinicConfig() { Doctors = new List<string>(); }
    }

    public class Appointment
    {
        public string Id { get; set; }
        public string PatientName { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan TimeSlot { get; set; }
        public string Doctor { get; set; }
        public string Category { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
