using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using BusinessLogic;
using Dominio;

namespace TPWinForm_equipo_X
{
    public class FrmClasificacion : Form
    {
        private readonly bool esMarca;
        private readonly TextBox nombre = new TextBox { MaxLength=50 };
        private readonly Label error = new Label { ForeColor=Color.Firebrick };
        private readonly Button guardar = new Button { Text="Crear y seleccionar" };
        private readonly Button cancelar = new Button { Text="Cancelar", DialogResult=DialogResult.Cancel };
        private bool guardando;
        public int RegistroId { get; private set; }
        public string Descripcion { get; private set; }

        public FrmClasificacion(bool esMarca)
        {
            this.esMarca = esMarca;
            Text = esMarca ? "Nueva marca" : "Nueva categoría";
            ClientSize = new Size(520,270);
            Font = new Font("Segoe UI",10);
            BackColor = Color.FromArgb(247,249,252);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = MinimizeBox = false;
            StartPosition = FormStartPosition.CenterParent;
            Controls.Add(new Label { Text=Text, Font=new Font(Font.FontFamily,18,FontStyle.Bold), Location=new Point(22,16), AutoSize=true });
            Controls.Add(new Label { Text=esMarca ? "Nombre de la marca (ejemplo: Stanley)" : "Nombre de la categoría (ejemplo: Termos)", Location=new Point(24,64), AutoSize=true });
            nombre.SetBounds(24,91,472,28);
            Controls.Add(new Label { Text="Quedará disponible para todos los artículos, aunque canceles el artículo actual.", Location=new Point(24,129), Size=new Size(472,42) });
            error.SetBounds(24,173,472,40);
            guardar.SetBounds(197,221,177,34); cancelar.SetBounds(384,221,112,34);
            Controls.AddRange(new Control[] { nombre, error, guardar, cancelar });
            AcceptButton = guardar; CancelButton = cancelar;
            guardar.Click += async(s,e) => await Guardar();
            FormClosing += (s,e) => { if(guardando) e.Cancel=true; };
            Shown += (s,e) => nombre.Focus();
        }
        private async Task Guardar()
        {
            if(guardando) return;
            string valor = nombre.Text.Trim();
            if(string.IsNullOrWhiteSpace(valor)) { error.Text="Ingresá un nombre."; nombre.Focus(); return; }
            guardando=true; guardar.Enabled=cancelar.Enabled=nombre.Enabled=false;
            error.Text="Guardando…";
            bool correcto=false;
            try
            {
                RegistroId = await Task.Run(() => esMarca
                    ? new MarcaBL().Agregar(new Marca { Descripcion=valor })
                    : new CategoriaBL().Agregar(new Categoria { Descripcion=valor }));
                Descripcion=valor; correcto=true;
            }
            catch(ArgumentException ex) { error.Text=ex.Message; }
            catch(Exception ex) { error.Text="No se pudo guardar. Revisá la conexión e intentá nuevamente."; error.AccessibleDescription=ex.Message; }
            finally { guardando=false; guardar.Enabled=cancelar.Enabled=nombre.Enabled=true; }
            if(correcto) { DialogResult=DialogResult.OK; Close(); }
        }
    }
}
