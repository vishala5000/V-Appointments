using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Globalization;
using System.IO;
using ClinicApp.Core;
using ClinicApp.Models;

namespace ClinicApp.Data
{
    public static class DatabaseHelper
    {
        // Added BusyTimeout to handle transient locks gracefully
        private static readonly string ConnString = $"Data Source={Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "ClinicApp", "clinic.db")};Version=3;BusyTimeout=3000;";

        public static void Initialize()
        {
            try
            {
                string dir = Path.GetDirectoryName(ConnString.Split('=')[1].Split(';')[0]);
                if (!Directory.Exists(dir)) { Directory.CreateDirectory(dir); File.SetAttributes(dir, File.GetAttributes(dir) | FileAttributes.Hidden); }

                using (var conn = new SQLiteConnection(ConnString))
                {
                    conn.Open();
                    using (var cmd = new SQLiteCommand("CREATE TABLE IF NOT EXISTS Config (Key TEXT PRIMARY KEY, Value TEXT)", conn)) cmd.ExecuteNonQuery();
                    using (var cmd = new SQLiteCommand("CREATE TABLE IF NOT EXISTS Appointments (Id TEXT PRIMARY KEY, PatientName TEXT, PhoneNumber TEXT, Date TEXT, TimeSlot TEXT, Doctor TEXT, Category TEXT, CreatedAt TEXT, UNIQUE(Date, TimeSlot, Doctor, Category) ON CONFLICT FAIL)", conn)) cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex) { Logger.Error("DB Init Failed", ex); throw; }
        }

        public static void SaveConfigValue(string key, string value)
        {
            using (var conn = new SQLiteConnection(ConnString)) { conn.Open(); using (var cmd = new SQLiteCommand("INSERT OR REPLACE INTO Config (Key, Value) VALUES (@key, @val)", conn)) { cmd.Parameters.AddWithValue("@key", key); cmd.Parameters.AddWithValue("@val", value); cmd.ExecuteNonQuery(); } }
        }

        public static string GetConfigValue(string key)
        {
            using (var conn = new SQLiteConnection(ConnString)) { conn.Open(); using (var cmd = new SQLiteCommand("SELECT Value FROM Config WHERE Key=@key", conn)) { cmd.Parameters.AddWithValue("@key", key); return cmd.ExecuteScalar()?.ToString(); } }
        }

        public static bool SaveAppointment(Appointment appt)
        {
            try
            {
                using (var conn = new SQLiteConnection(ConnString))
                {
                    conn.Open();
                    using (var cmd = new SQLiteCommand("INSERT INTO Appointments (Id, PatientName, PhoneNumber, Date, TimeSlot, Doctor, Category, CreatedAt) VALUES (@id, @name, @phone, @date, @time, @doctor, @category, @created)", conn))
                    {
                        cmd.Parameters.AddWithValue("@id", appt.Id);
                        cmd.Parameters.AddWithValue("@name", Security.Encrypt(appt.PatientName));
                        cmd.Parameters.AddWithValue("@phone", Security.Encrypt(appt.PhoneNumber));
                        cmd.Parameters.AddWithValue("@date", appt.Date.ToString("yyyy-MM-dd"));
                        cmd.Parameters.AddWithValue("@time", appt.TimeSlot.ToString(@"hh\:mm"));
                        cmd.Parameters.AddWithValue("@doctor", appt.Doctor);
                        cmd.Parameters.AddWithValue("@category", appt.Category);
                        cmd.Parameters.AddWithValue("@created", DateTime.Now.ToString("O"));
                        cmd.ExecuteNonQuery();
                    }
                }
                return true;
            }
            catch (SQLiteException ex) when (ex.Message.Contains("UNIQUE constraint failed")) { return false; }
            catch (Exception ex) { Logger.Error("Save Appt Failed", ex); return false; }
        }

        // NEW: Delete Appointment
        public static void DeleteAppointment(string id)
        {
            using (var conn = new SQLiteConnection(ConnString)) { conn.Open(); using (var cmd = new SQLiteCommand("DELETE FROM Appointments WHERE Id=@id", conn)) { cmd.Parameters.AddWithValue("@id", id); cmd.ExecuteNonQuery(); } }
        }

        public static List<Appointment> GetAppointments(string category)
        {
            var list = new List<Appointment>();
            using (var conn = new SQLiteConnection(ConnString))
            {
                conn.Open();
                using (var cmd = new SQLiteCommand("SELECT * FROM Appointments WHERE Category=@cat ORDER BY Date DESC, TimeSlot", conn))
                {
                    cmd.Parameters.AddWithValue("@cat", category);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new Appointment
                            {
                                Id = reader["Id"].ToString(),
                                PatientName = Security.Decrypt(reader["PatientName"].ToString()),
                                PhoneNumber = Security.Decrypt(reader["PhoneNumber"].ToString()),
                                // Fixed: Use ParseExact to prevent regional date format crashes
                                Date = DateTime.ParseExact(reader["Date"].ToString(), "yyyy-MM-dd", CultureInfo.InvariantCulture),
                                TimeSlot = TimeSpan.ParseExact(reader["TimeSlot"].ToString(), "hh\\:mm", CultureInfo.InvariantCulture),
                                Doctor = reader["Doctor"].ToString(),
                                Category = reader["Category"].ToString(),
                                CreatedAt = DateTime.Parse(reader["CreatedAt"].ToString())
                            });
                        }
                    }
                }
            }
            return list;
        }

        public static List<string> GetBookedSlots(string category, string date, string doctor)
        {
            var slots = new List<string>();
            using (var conn = new SQLiteConnection(ConnString)) { conn.Open(); using (var cmd = new SQLiteCommand("SELECT TimeSlot FROM Appointments WHERE Category=@cat AND Date=@date AND Doctor=@doc", conn)) { cmd.Parameters.AddWithValue("@cat", category); cmd.Parameters.AddWithValue("@date", date); cmd.Parameters.AddWithValue("@doc", doctor); using (var reader = cmd.ExecuteReader()) while (reader.Read()) slots.Add(reader["TimeSlot"].ToString()); } }
            return slots;
        }
    }
}
