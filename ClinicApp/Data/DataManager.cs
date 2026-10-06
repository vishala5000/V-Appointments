using System;
using System.IO;
using System.Windows.Forms;
using Newtonsoft.Json;
using ClinicApp.Models;

namespace ClinicApp.Data
{
    public static class DataManager
    {
        private static readonly string AppDir = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "ClinicApp");
        
        private static readonly string DataFile = Path.Combine(AppDir, "clinic_data.json");
        private static MasterData _cache;

        public static MasterData LoadData()
        {
            if (_cache != null) return _cache;

            try
            {
                if (!Directory.Exists(AppDir))
                {
                    Directory.CreateDirectory(AppDir);
                    // Hide directory from average users to prevent accidental deletion
                    File.SetAttributes(AppDir, File.GetAttributes(AppDir) | FileAttributes.Hidden);
                }

                if (File.Exists(DataFile))
                {
                    string json = File.ReadAllText(DataFile);
                    _cache = JsonConvert.DeserializeObject<MasterData>(json) ?? new MasterData();
                }
                else
                {
                    _cache = new MasterData();
                    SaveData(); // Create initial file
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading data: {ex.Message}", "Critical Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                _cache = new MasterData();
            }
            return _cache;
        }

        public static void SaveData()
        {
            if (_cache == null) return;

            try
            {
                string json = JsonConvert.SerializeObject(_cache, Formatting.Indented);
                File.WriteAllText(DataFile, json);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving data: {ex.Message}", "Critical Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
