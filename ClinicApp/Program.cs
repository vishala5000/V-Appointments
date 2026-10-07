using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using ClinicApp.Data;
using ClinicApp.UI;

namespace ClinicApp
{
    static class Program
    {
        [DllImport("user32.dll")]
        private static extern bool SetProcessDPIAware();

        [STAThread]
        static void Main()
        {
            try
            {
                SetProcessDPIAware();
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                // Initialize Database first
                DatabaseHelper.Initialize();

                if (!DataManager.IsConfigured())
                    Application.Run(new AdminSetupForm());
                else
                    Application.Run(new DashboardForm());
            }
            catch (Exception ex)
            {
                // Log the error
                Core.Logger.Error("Fatal startup error", ex);
                
                // Show the EXACT error message to the user for debugging
                MessageBox.Show(
                    $"A critical error occurred:\n\n{ex.Message}\n\nStack Trace:\n{ex.StackTrace}", 
                    "V Soft - Fatal Error", 
                    MessageBoxButtons.OK, 
                    MessageBoxIcon.Error);
            }
        }
    }
}
