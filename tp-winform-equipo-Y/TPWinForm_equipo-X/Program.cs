using System;
using System.Windows.Forms;
namespace TPWinForm_equipo_X
{
    internal static class Program
    {
        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FrmCatalogo());
        }
    }
}
