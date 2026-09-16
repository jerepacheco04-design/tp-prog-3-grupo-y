using System;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DAO;
using Dominio;

namespace TPWinForm_equipo_X
{
    // Ventana mínima para integrar y comprobar la lectura del DAO.
    public class FrmCatalogo : Form
    {
        private readonly TextBox txtBuscar = new TextBox();
        private readonly Button btnBuscar = new Button();
        private readonly DataGridView grilla = new DataGridView();
        private readonly Label estado = new Label();
        public FrmCatalogo()
        {
            Text = "TPWinForm_equipo-x - Catálogo";
            ClientSize = new Size(1000, 620);
            MinimumSize = new Size(800, 460);
            StartPosition = FormStartPosition.CenterScreen;
            Font = new Font("Segoe UI", 10);
            BackColor = Color.FromArgb(247, 249, 252);
            var titulo = new Label { Text = "Catálogo de artículos", Font = new Font("Segoe UI", 20, FontStyle.Bold), Location = new Point(24, 16), Size = new Size(800, 46) };
            var ayuda = new Label { Text = "Consulta de integración del DAO. Buscar por código, nombre o descripción.", Location = new Point(26, 70), Size = new Size(900, 25) };
            txtBuscar.SetBounds(26, 108, 770, 30);
            txtBuscar.MaxLength = 150;
            txtBuscar.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
            btnBuscar.Text = "Buscar / actualizar";
            btnBuscar.SetBounds(814, 104, 160, 36);
            btnBuscar.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnBuscar.Click += async (s, e) => await Cargar();
            grilla.SetBounds(26, 160, 948, 365);
            grilla.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            grilla.ReadOnly = true;
            grilla.AllowUserToAddRows = false;
            grilla.AllowUserToDeleteRows = false;
            grilla.RowHeadersVisible = false;
            grilla.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grilla.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            grilla.BackgroundColor = Color.White;
            estado.SetBounds(26, 544, 948, 60);
            estado.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            Controls.AddRange(new Control[] { titulo, ayuda, txtBuscar, btnBuscar, grilla, estado });
            AcceptButton = btnBuscar;
            Shown += async (s, e) => await Cargar();
        }
        private async Task Cargar()
        {
            btnBuscar.Enabled = false;
            estado.ForeColor = Color.DimGray;
            estado.Text = "Consultando…";
            string texto = txtBuscar.Text;
            try
            {
                var lista = await Task.Run(() => new ArticuloDAO().Buscar(new FiltroArticulo { Texto = texto }));
                if (IsDisposed) return;
                grilla.DataSource = lista.Select(a => new { a.Codigo, a.Nombre, a.Descripcion,
                    Marca = a.Marca == null ? "(Sin marca)" : a.Marca.Descripcion,
                    Categoria = a.Categoria == null ? "(Sin categoría)" : a.Categoria.Descripcion,
                    a.Precio, Imagenes = a.Imagenes.Count }).ToList();
                grilla.Columns["Precio"].DefaultCellStyle.Format = "N4";
                estado.ForeColor = Color.DarkGreen;
                estado.Text = lista.Count + " artículo(s). Conexión y lectura del DAO verificadas.";
            }
            catch (Exception ex)
            {
                if (IsDisposed) return;
                grilla.DataSource = null;
                estado.ForeColor = Color.Firebrick;
                estado.Text = ex is System.Data.SqlClient.SqlException
                    ? "No se pudo consultar SQL Server. Revisá Catalogo en App.config y ejecutá el script de la base. Instrucciones en LEEME.md."
                    : ex.Message;
            }
            finally { if (!IsDisposed) btnBuscar.Enabled = true; }
        }
    }
}
