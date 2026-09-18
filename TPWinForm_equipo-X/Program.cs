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
                if (args.Length == 1 && args[0] == "--respaldar")
                {
                    GuardarCopia();
                    return 0;
                }
                Application.Run(new FrmCatalogo());
                try
                {
                    GuardarCopia();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Los datos siguen en tu base, pero no se pudo actualizar la copia para compartir.\n" + ex.Message, "Copia para compartir");
                    return 1;
                }
                return 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudo abrir el catálogo. Si es la primera vez, usá INICIAR.\n\n" + ex.Message, "Catálogo");
                return 1;
            }
        }

        private static void GuardarCopia()
        {
            string destino = System.IO.Path.Combine(ArchivosImagen.CarpetaRaiz, "DatosIniciales", "Catalogo.bak");
            new BusinessLogic.ArticuloBL().GuardarRespaldo(destino);
        }
    }
}
