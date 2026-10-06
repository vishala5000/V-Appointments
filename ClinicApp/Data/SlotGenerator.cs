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

            while (current.Add(duration) <= end)
            {
                slots.Add(current);
                current = current.Add(duration);
            }
            return slots;
        }

        public static List<TimeSpan> GetAvailableSlots(string category, DateTime date, string doctor)
        {
            var config = DataManager.LoadConfig();
            int duration = category == "Normal" ? config.NormalDurationMins :
                           category == "Special" ? config.SpecialDurationMins : config.VipDurationMins;

            var allSlots = GenerateSlots(config.StartTime, config.EndTime, duration);
            var bookedSlots = DatabaseHelper.GetBookedSlots(category, date.ToString("yyyy-MM-dd"), doctor);

            return allSlots.Where(s => !bookedSlots.Contains(s.ToString(@"hh\:mm"))).ToList();
        }
    }
}
