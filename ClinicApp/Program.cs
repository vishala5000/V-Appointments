using System;
using System.Windows.Forms;
using ClinicApp.Data;
using ClinicApp.UI;

namespace ClinicApp
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var data = DataManager.LoadData();

            if (!data.IsConfigured)
            {
                Application.Run(new AdminSetupForm());
            }
            else
            {
                Application.Run(new DashboardForm());
            }
        }
    }
}
