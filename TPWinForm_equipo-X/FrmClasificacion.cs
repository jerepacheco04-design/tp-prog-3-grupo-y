using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using BusinessLogic;
using Dominio;

namespace TPWinForm_equipo_X
{
    public partial class FrmClasificacion : Form
    {
        private readonly bool esMarca;
        private bool guardando;
        public int RegistroId { get; private set; }
        public string Descripcion { get; private set; }

        public FrmClasificacion()
        {
            InitializeComponent();
        }

        public FrmClasificacion(bool esMarca) : this()
        {
            this.esMarca = esMarca;
            Text = esMarca ? "Nueva marca" : "Nueva categoría";
        }

        private async Task Guardar()
        {
            if (guardando)
                return;
            string valor = txtNombre.Text.Trim();
            if (string.IsNullOrWhiteSpace(valor))
            {
                MessageBox.Show(this, "Ingresá un nombre.");
                txtNombre.Focus();
                return;
            }

            guardando = true;
            btnGuardar.Enabled = btnCancelar.Enabled = txtNombre.Enabled = false;
            bool correcto = false;
            try
            {
                RegistroId = await Task.Run(() => esMarca ? new MarcaBL().Agregar(new Marca { Descripcion = valor }) : new CategoriaBL().Agregar(new Categoria { Descripcion = valor }));
                Descripcion = valor;
                correcto = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "No se pudo guardar");
            }
            finally
            {
                guardando = false;
                btnGuardar.Enabled = btnCancelar.Enabled = txtNombre.Enabled = true;
            }

            if (correcto)
            {
                DialogResult = DialogResult.OK;
                Close();
            }
        }

        private async void btnGuardar_Click(object sender, EventArgs e)
        {
            await Guardar();
        }

        private void FrmClasificacion_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (guardando)
                e.Cancel = true;
        }
    }
}
