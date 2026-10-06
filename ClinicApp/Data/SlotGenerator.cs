using System;
using System.Collections.Generic;
using System.Linq;
using ClinicApp.Models;

namespace ClinicApp.Data
{
    public static class SlotGenerator
    {
        public static List<TimeSpan> GenerateSlots(TimeSpan start, TimeSpan end, int durationMins)
        {
            var slots = new List<TimeSpan>();
            var current = start;
            var duration = TimeSpan.FromMinutes(durationMins);

            // Mathematical division: strictly non-overlapping
            while (current.Add(duration) <= end)
            {
                slots.Add(current);
                current = current.Add(duration);
            }
            return slots;
        }

        public static List<TimeSpan> GetAvailableSlots(string category, DateTime date, string doctor)
        {
            var config = DataManager.LoadData().Config;
            int duration = category == "Normal" ? config.NormalDurationMins :
                           category == "Special" ? config.SpecialDurationMins : config.VipDurationMins;

            var allSlots = GenerateSlots(config.StartTime, config.EndTime, duration);
            var appointments = DataManager.LoadData().Appointments;

            // Filter out slots already booked by this specific doctor on this date for this category
            var bookedSlots = appointments
                .Where(a => a.Category == category && a.Date.Date == date.Date && a.Doctor == doctor)
                .Select(a => a.TimeSlot)
                .ToHashSet();

            return allSlots.Where(s => !bookedSlots.Contains(s)).ToList();
        }
    }
}
