using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Lab_sp.View.Forms;
using Lab_sp.Core;

namespace Lab_sp
{
    static class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            new SplashScreen().ShowDialog();
            if (new AuthForm().ShowDialog() == DialogResult.OK)
                Application.Run(new Main());
            Environment.Exit(1);
        }
    }
}
