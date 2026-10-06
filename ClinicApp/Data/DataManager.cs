using System;
using System.Collections.Generic;
using System.Linq;
using ClinicApp.Models;

namespace ClinicApp.Data
{
    public static class DataManager
    {
        private static ClinicConfig _cache;

        public static bool IsConfigured()
        {
            return DatabaseHelper.GetConfigValue("IsConfigured") == "true";
        }

        public static ClinicConfig LoadConfig()
        {
            if (_cache != null) return _cache;

            _cache = new ClinicConfig
            {
                ClinicName = DatabaseHelper.GetConfigValue("ClinicName") ?? "",
                Doctors = (DatabaseHelper.GetConfigValue("Doctors") ?? "").Split(',').Where(d => !string.IsNullOrEmpty(d)).ToList(),
                StartTime = TimeSpan.TryParse(DatabaseHelper.GetConfigValue("StartTime"), out var st) ? st : new TimeSpan(9, 0, 0),
                EndTime = TimeSpan.TryParse(DatabaseHelper.GetConfigValue("EndTime"), out var et) ? et : new TimeSpan(17, 0, 0),
                NormalDurationMins = int.TryParse(DatabaseHelper.GetConfigValue("NormalDur"), out var nd) ? nd : 30,
                SpecialDurationMins = int.TryParse(DatabaseHelper.GetConfigValue("SpecialDur"), out var sd) ? sd : 45,
                VipDurationMins = int.TryParse(DatabaseHelper.GetConfigValue("VipDur"), out var vd) ? vd : 60
            };
            return _cache;
        }

        public static void SaveConfig(ClinicConfig config)
        {
            DatabaseHelper.SaveConfigValue("IsConfigured", "true");
            DatabaseHelper.SaveConfigValue("ClinicName", config.ClinicName);
            DatabaseHelper.SaveConfigValue("Doctors", string.Join(",", config.Doctors));
            DatabaseHelper.SaveConfigValue("StartTime", config.StartTime.ToString());
            DatabaseHelper.SaveConfigValue("EndTime", config.EndTime.ToString());
            DatabaseHelper.SaveConfigValue("NormalDur", config.NormalDurationMins.ToString());
            DatabaseHelper.SaveConfigValue("SpecialDur", config.SpecialDurationMins.ToString());
            DatabaseHelper.SaveConfigValue("VipDur", config.VipDurationMins.ToString());
            
            _cache = config; // Update cache
        }
    }
}
