using System;
using System.Data.SQLite;
using System.IO;
using ClinicApp.Core;

namespace ClinicApp.Data
{
    public static class DatabaseHelper
    {
        private static readonly string DbPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "ClinicApp", "clinic.db");

        public static void Initialize()
        {
            try
            {
                string dir = Path.GetDirectoryName(DbPath);
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                    File.SetAttributes(dir, File.GetAttributes(dir) | FileAttributes.Hidden);
                }

                using (var conn = new SQLiteConnection($"Data Source={DbPath};Version=3;"))
                {
                    conn.Open();
                    // Config Table
                    string configTable = @"CREATE TABLE IF NOT EXISTS Config (
                        Key TEXT PRIMARY KEY, Value TEXT)";
                    // Appointments Table with UNIQUE constraint to prevent double-booking
                    string apptTable = @"CREATE TABLE IF NOT EXISTS Appointments (
                        Id TEXT PRIMARY KEY,
                        PatientName TEXT,
                        PhoneNumber TEXT,
                        Date TEXT,
                        TimeSlot TEXT,
                        Doctor TEXT,
                        Category TEXT,
                        CreatedAt TEXT,
                        UNIQUE(Date, TimeSlot, Doctor, Category) ON CONFLICT FAIL)";
                    
                    using (var cmd = new SQLiteCommand(configTable, conn)) cmd.ExecuteNonQuery();
                    using (var cmd = new SQLiteCommand(apptTable, conn)) cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Database initialization failed", ex);
                throw;
            }
        }

        public static bool SaveAppointment(string id, string name, string phone, string date, string time, string doctor, string category)
        {
            try
            {
                using (var conn = new SQLiteConnection($"Data Source={DbPath};Version=3;"))
                {
                    conn.Open();
                    string sql = @"INSERT INTO Appointments (Id, PatientName, PhoneNumber, Date, TimeSlot, Doctor, Category, CreatedAt) 
                                   VALUES (@id, @name, @phone, @date, @time, @doctor, @category, @created)";
                    
                    using (var cmd = new SQLiteCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", id);
                        cmd.Parameters.AddWithValue("@name", Security.Encrypt(name));
                        cmd.Parameters.AddWithValue("@phone", Security.Encrypt(phone));
                        cmd.Parameters.AddWithValue("@date", date);
                        cmd.Parameters.AddWithValue("@time", time);
                        cmd.Parameters.AddWithValue("@doctor", doctor);
                        cmd.Parameters.AddWithValue("@category", category);
                        cmd.Parameters.AddWithValue("@created", DateTime.Now.ToString("O"));
                        
                        cmd.ExecuteNonQuery();
                    }
                }
                return true;
            }
            catch (SQLiteException ex) when (ex.Message.Contains("UNIQUE constraint failed"))
            {
                Logger.Error("Double booking attempt blocked");
                return false; // Double booking prevented by DB
            }
            catch (Exception ex)
            {
                Logger.Error("Failed to save appointment", ex);
                return false;
            }
        }
        
        // Add similar methods for GetConfig, SaveConfig, GetAppointments...
    }
}
