using System;
using System.IO;
using System.Threading;
using System.Windows.Forms;

using YemekhaneBakiyeKart.Sys;

namespace YemekhaneBakiyeKart
{
    static class Program
    {
        private static string appGuid = "c0a76b5a-12ab-45c5-b9d9-d693faa6e7b9";

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            using (Mutex mutex = new Mutex(false, "Global\\" + appGuid))
            {
                if (!mutex.WaitOne(0, false))
                {
                    return;
                }

                if (!ReadSettings()) return;

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new Login
            }
        }

        private static Boolean ReadSettings()
        {
            Boolean result = false;

            try
            {
                StreamReader sr = new StreamReader("Server.txt");
                Settings.ServerType = sr.ReadLine();
                Settings.ServerIP = sr.ReadLine();

                Settings.ConnectionStrLocal = "Server=" + Settings.ServerIP + ";Database=YEMEKHANE_" + Settings.ServerType + ";User Id=***;Password=***;";
               
                result = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            return result;
        }
    }
}
