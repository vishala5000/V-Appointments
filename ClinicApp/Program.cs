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

                DatabaseHelper.Initialize();

                if (!DataManager.IsConfigured())
                    Application.Run(new AdminSetupForm());
                else
                    Application.Run(new DashboardForm());
            }
            catch (Exception ex)
            {
                Core.Logger.Error("Fatal startup error", ex);
                MessageBox.Show(
                    $"A critical error occurred:\n\n{ex.Message}\n\nStack Trace:\n{ex.StackTrace}", 
                    "V Soft - Fatal Error", 
                    MessageBoxButtons.OK, 
                    MessageBoxIcon.Error);
            }
        }
    }
}
