using System;
using System.Windows.Forms;

namespace TPWinForm_equipo_X
{
    internal static class Program
    {
        [STAThread]
        private static int Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            try
            {
                Application.Run(new FrmCatalogo());
                return 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudo abrir el catálogo. Si es la primera vez, usá INICIAR.\n\n" + ex.Message, "Catálogo");
                return 1;
            }
        }
    }
}
