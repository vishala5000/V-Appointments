using System;
using System.IO;

namespace ClinicApp.Core
{
    public static class Logger
    {
        private static readonly string LogDir = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "ClinicApp", "Logs");

        public static void Info(string message) => Write("INFO", message);
        public static void Error(string message, Exception ex = null) => Write("ERROR", $"{message} | {ex?.Message}");

        private static void Write(string level, string message)
        {
            try
            {
                if (!Directory.Exists(LogDir)) Directory.CreateDirectory(LogDir);
                string logFile = Path.Combine(LogDir, $"ClinicApp_{DateTime.Now:yyyy-MM-dd}.log");
                string entry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [{level}] {message}{Environment.NewLine}";
                File.AppendAllText(logFile, entry);
            }
            catch { /* Fail silently to avoid crashing the app on logging errors */ }
        }
    }
}
