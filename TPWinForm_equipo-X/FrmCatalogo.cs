using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using BusinessLogic;
using Dominio;

namespace TPWinForm_equipo_X
{
    public partial class FrmCatalogo : Form
    {
        private ArticuloBL negocio;
        private bool ocupado;
        private bool opcionesCargadas;
        private List<Articulo> articulos = new List<Articulo>();
        public FrmCatalogo()
        {
            InitializeComponent();
            ActualizarBotones();
        }

        private Articulo Seleccionado()
        {
            var fila = dgvArticulos.CurrentRow == null ? null : dgvArticulos.CurrentRow.DataBoundItem as FilaArticulo;
            return fila == null ? null : articulos.FirstOrDefault(a => a.Id == fila.Id);
        }

        private void ActualizarBotones()
        {
            btnModificar.Enabled = btnEliminar.Enabled = btnDetalle.Enabled = !ocupado && Seleccionado() != null;
        }

        private void VerDetalle()
        {
            var articulo = Seleccionado();
            if (ocupado || articulo == null)
                return;
            using (var form = new FrmDetalle(articulo))
                form.ShowDialog(this);
        }

        private void Ocupado(bool valor)
        {
            ocupado = valor;
            foreach (Control control in Controls)
                if (!(control is Label))
                    control.Enabled = !valor;
            UseWaitCursor = valor;
            ActualizarBotones();
        }

        private void Limpiar()
        {
            txtBusqueda.Clear();
            if (opcionesCargadas)
            {
                cboMarca.SelectedIndex = 0;
                cboCategoria.SelectedIndex = 0;
            }
        }

        private async Task Cargar(int? seleccionar = null)
        {
            if (ocupado)
                return;
            Ocupado(true);
            lblResultado.Text = "Buscando artículos…";
            try
            {
                if (negocio == null)
                    negocio = new ArticuloBL();
                if (!opcionesCargadas)
                {
                    int marcaAnterior = cboMarca.SelectedItem is Marca ? ((Marca)cboMarca.SelectedItem).Id : 0;
                    int categoriaAnterior = cboCategoria.SelectedItem is Categoria ? ((Categoria)cboCategoria.SelectedItem).Id : 0;
                    var marcas = await Task.Run(() => new MarcaBL().Listar());
                    var categorias = await Task.Run(() => new CategoriaBL().Listar());
                    if (IsDisposed)
                        return;
                    marcas.Insert(0, new Marca { Id = 0, Descripcion = "Todas las marcas" });
                    categorias.Insert(0, new Categoria { Id = 0, Descripcion = "Todas las categorías" });
                    cboMarca.DisplayMember = "Descripcion";
                    cboMarca.ValueMember = "Id";
                    cboMarca.DataSource = marcas;
                    cboCategoria.DisplayMember = "Descripcion";
                    cboCategoria.ValueMember = "Id";
                    cboCategoria.DataSource = categorias;
                    cboMarca.SelectedValue = marcas.Any(x => x.Id == marcaAnterior) ? marcaAnterior : 0;
                    cboCategoria.SelectedValue = categorias.Any(x => x.Id == categoriaAnterior) ? categoriaAnterior : 0;
                    opcionesCargadas = true;
                }

                var filtro = new FiltroArticulo
                {
                    Texto = txtBusqueda.Text,
                    IdMarca = ((Marca)cboMarca.SelectedItem).Id == 0 ? (int? )null : ((Marca)cboMarca.SelectedItem).Id,
                    IdCategoria = ((Categoria)cboCategoria.SelectedItem).Id == 0 ? (int? )null : ((Categoria)cboCategoria.SelectedItem).Id
                };
                var lista = await Task.Run(() => negocio.Buscar(filtro));
                if (IsDisposed)
                    return;
                MostrarArticulos(lista, seleccionar);
            }
            catch (Exception ex)
            {
                if (!IsDisposed)
                {
                    articulos.Clear();
                    dgvArticulos.DataSource = null;
                    lblResultado.Text = "No se pudo completar la búsqueda.";
                    MessageBox.Show(this, ex.Message, "Error al buscar");
                }
            }
            finally
            {
                if (!IsDisposed)
                    Ocupado(false);
            }
        }

        private void MostrarArticulos(List<Articulo> lista, int? seleccionar)
        {
            articulos = lista;
            lblResultado.Text = lista.Count + " artículo(s)";
            dgvArticulos.DataSource = lista.Select(a => new FilaArticulo { Id = a.Id, Codigo = a.Codigo, Nombre = a.Nombre, Descripcion = a.Descripcion, Marca = a.Marca == null ? "(Sin marca)" : a.Marca.Descripcion, Categoria = a.Categoria == null ? "(Sin categoría)" : a.Categoria.Descripcion, Precio = a.Precio, Imagenes = a.Imagenes.Count }).ToList();
            dgvArticulos.Columns["Id"].Visible = false;
            dgvArticulos.Columns["Precio"].DefaultCellStyle.Format = "N4";
            if (dgvArticulos.Rows.Count > 0)
                dgvArticulos.CurrentCell = dgvArticulos.Rows[0].Cells["Codigo"];
            if (seleccionar.HasValue)
                foreach (DataGridViewRow fila in dgvArticulos.Rows)
                    if (((FilaArticulo)fila.DataBoundItem).Id == seleccionar)
                    {
                        dgvArticulos.CurrentCell = fila.Cells["Codigo"];
                        break;
                    }

        }

        private async Task AbrirEditor(bool nuevo)
        {
            if (ocupado)
                return;
            var seleccionado = Seleccionado();
            if (!nuevo && seleccionado == null)
                return;
            Ocupado(true);
            int? guardado = null;
            bool clasificacionesActualizadas = false;
            try
            {
                var articulo = nuevo ? new Articulo() : await Task.Run(() => negocio.ObtenerPorId(seleccionado.Id));
                if (articulo == null)
                    throw new InvalidOperationException("El artículo ya no existe. Actualizá la lista.");
                var marcas = await Task.Run(() => new MarcaBL().Listar());
                var categorias = await Task.Run(() => new CategoriaBL().Listar());
                if (IsDisposed)
                    return;
                using (var form = new FrmArticulo(articulo, marcas, categorias))
                {
                    if (form.ShowDialog(this) == DialogResult.OK)
                        guardado = form.ArticuloId;
                    clasificacionesActualizadas = form.ClasificacionesActualizadas;
                }
            }
            catch (Exception ex)
            {
                if (!IsDisposed)
                    MessageBox.Show(this, ex.Message, "No se pudo abrir el artículo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            finally
            {
                if (!IsDisposed)
                    Ocupado(false);
            }

            if (!IsDisposed)
            {
                if (guardado.HasValue)
                    Limpiar();
                if (clasificacionesActualizadas)
                    opcionesCargadas = false;
                if (guardado.HasValue)
                    await Cargar(guardado);
                else if (clasificacionesActualizadas)
                    await Cargar();
            }
        }

        private async Task Eliminar()
        {
            var articulo = Seleccionado();
            if (ocupado || articulo == null)
                return;
            if (MessageBox.Show(this, "¿Eliminar el artículo " + articulo.Nombre + "?", "Confirmar eliminación", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) != DialogResult.Yes)
                return;
            Ocupado(true);
            bool eliminado = false;
            try
            {
                await Task.Run(() => negocio.Eliminar(articulo.Id));
                eliminado = true;
            }
            catch (Exception ex)
            {
                if (!IsDisposed)
                    MessageBox.Show(this, ex.Message, "No se pudo eliminar", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            finally
            {
                if (!IsDisposed)
                    Ocupado(false);
            }

            if (eliminado && !IsDisposed)
                await Cargar();
        }

        private class FilaArticulo
        {
            public int Id { get; set; }
            public string Codigo { get; set; }
            public string Nombre { get; set; }
            public string Descripcion { get; set; }
            public string Marca { get; set; }
            public string Categoria { get; set; }
            public decimal Precio { get; set; }
            public int Imagenes { get; set; }
        }

        private void dgvArticulos_SelectionChanged(object sender, EventArgs e)
        {
            ActualizarBotones();
        }

        private void dgvArticulos_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && !ocupado)
                VerDetalle();
        }

        private async void btnBuscar_Click(object sender, EventArgs e)
        {
            await Cargar();
        }

        private async void btnLimpiar_Click(object sender, EventArgs e)
        {
            Limpiar();
            await Cargar();
        }

        private async void btnAgregar_Click(object sender, EventArgs e)
        {
            await AbrirEditor(true);
        }

        private async void btnModificar_Click(object sender, EventArgs e)
        {
            await AbrirEditor(false);
        }

        private async void btnEliminar_Click(object sender, EventArgs e)
        {
            await Eliminar();
        }

        private void btnDetalle_Click(object sender, EventArgs e)
        {
            VerDetalle();
        }

        private async void FrmCatalogo_Shown(object sender, EventArgs e)
        {
            if (System.ComponentModel.LicenseManager.UsageMode == System.ComponentModel.LicenseUsageMode.Designtime || DesignMode)
                return;
            await Cargar();
        }
    }
}
