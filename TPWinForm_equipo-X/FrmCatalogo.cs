using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using BusinessLogic;
using Dominio;

namespace TPWinForm_equipo_X
{
    public class FrmCatalogo : Form
    {
        private readonly ArticuloBL negocio = new ArticuloBL();
        private readonly Panel filtros = new Panel();
        private readonly FlowLayoutPanel acciones = new FlowLayoutPanel();
        private readonly TextBox texto = new TextBox { MaxLength = 150 };
        private readonly TextBox codigo = new TextBox { MaxLength = 50 };
        private readonly ComboBox marca = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList };
        private readonly ComboBox categoria = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList };
        private readonly CheckBox usarMin = new CheckBox { Text = "Precio desde", AutoSize = true };
        private readonly CheckBox usarMax = new CheckBox { Text = "Precio hasta", AutoSize = true };
        private readonly NumericUpDown minimo = Precio();
        private readonly NumericUpDown maximo = Precio();
        private readonly DataGridView grilla = new DataGridView();
        private readonly Label estado = new Label();
        private readonly Button editar = new Button { Text = "Modificar", Width = 130, Height = 36 };
        private readonly Button eliminar = new Button { Text = "Eliminar", Width = 130, Height = 36 };
        private readonly Button detalle = new Button { Text = "Ver detalle / fotos", Width = 175, Height = 36 };
        private bool ocupado;
        private bool opcionesCargadas;
        private List<Articulo> articulos = new List<Articulo>();

        public FrmCatalogo()
        {
            Text = "Catálogo de artículos";
            ClientSize = new Size(1100, 720);
            MinimumSize = new Size(1100, 720);
            StartPosition = FormStartPosition.CenterScreen;
            Font = new Font("Segoe UI", 10);
            BackColor = Color.FromArgb(247, 249, 252);
            Controls.Add(new Label { Text = "Catálogo de artículos", Font = new Font(Font.FontFamily, 22, FontStyle.Bold), Location = new Point(24, 18), AutoSize = true });
            filtros.SetBounds(24, 78, 1052, 176);
            filtros.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            Campo("Buscar por código, nombre, descripción, marca o categoría", texto, 0, 0, 770);
            var buscar = new Button { Text = "Buscar", Location = new Point(800, 23), Size = new Size(118, 32) };
            var limpiar = new Button { Text = "Limpiar filtros", Location = new Point(926, 23), Size = new Size(126, 32) };
            filtros.Controls.AddRange(new Control[] { buscar, limpiar });
            Campo("Marca", marca, 0, 64, 250);
            Campo("Categoría", categoria, 270, 64, 250);
            Campo("Código exacto", codigo, 540, 64, 230);
            usarMin.Location = new Point(0, 138); minimo.SetBounds(125, 134, 210, 28);
            usarMax.Location = new Point(365, 138); maximo.SetBounds(490, 134, 210, 28);
            minimo.Enabled = maximo.Enabled = false;
            filtros.Controls.AddRange(new Control[] { usarMin, minimo, usarMax, maximo });
            usarMin.CheckedChanged += (s,e) => minimo.Enabled = usarMin.Checked;
            usarMax.CheckedChanged += (s,e) => maximo.Enabled = usarMax.Checked;
            buscar.Click += async (s,e) => await Cargar();
            limpiar.Click += async (s,e) => { Limpiar(); await Cargar(); };
            AcceptButton = buscar;
            grilla.SetBounds(24, 264, 1052, 342);
            grilla.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            grilla.ReadOnly = true; grilla.MultiSelect = false;
            grilla.AllowUserToAddRows = false; grilla.AllowUserToDeleteRows = false;
            grilla.RowHeadersVisible = false; grilla.AutoGenerateColumns = false;
            grilla.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grilla.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            grilla.BackgroundColor = Color.White;
            Columna("Codigo", "Código", 65); Columna("Nombre", "Nombre", 135);
            Columna("Descripcion", "Descripción", 180); Columna("Marca", "Marca", 90);
            Columna("Categoria", "Categoría", 100); Columna("Precio", "Precio", 100);
            Columna("Imagenes", "Imágenes", 60);
            grilla.Columns["Precio"].DefaultCellStyle.Format = "N4";
            grilla.SelectionChanged += (s,e) => ActualizarBotones();
            grilla.CellDoubleClick += (s,e) => { if(e.RowIndex >= 0 && !ocupado) VerDetalle(); };
            acciones.SetBounds(24, 620, 1052, 42);
            acciones.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            var agregar = new Button { Text = "Agregar artículo", Width = 160, Height = 36 };
            acciones.Controls.AddRange(new Control[] { agregar, editar, eliminar, detalle });
            detalle.Click += (s,e) => VerDetalle();
            agregar.Click += async (s,e) => await AbrirEditor(true);
            editar.Click += async (s,e) => await AbrirEditor(false);
            eliminar.Click += async (s,e) => await Eliminar();
            estado.SetBounds(24, 669, 1052, 42);
            estado.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            Controls.AddRange(new Control[] { filtros, grilla, acciones, estado });
            Shown += async (s,e) => await Cargar();
            ActualizarBotones();
        }
        private static NumericUpDown Precio() { return new NumericUpDown { DecimalPlaces = 4, Maximum = 922337203685477.5807m, ThousandsSeparator = true }; }
        private void Campo(string titulo, Control control, int x, int y, int ancho)
        {
            filtros.Controls.Add(new Label { Text = titulo, Location = new Point(x,y), AutoSize = true });
            control.SetBounds(x,y+24,ancho,28); filtros.Controls.Add(control);
        }
        private void Columna(string propiedad, string titulo, int ancho)
        {
            grilla.Columns.Add(new DataGridViewTextBoxColumn { Name = propiedad, DataPropertyName = propiedad, HeaderText = titulo, FillWeight = ancho, SortMode = DataGridViewColumnSortMode.NotSortable });
        }
        private Articulo Seleccionado()
        {
            var fila = grilla.CurrentRow == null ? null : grilla.CurrentRow.DataBoundItem as FilaArticulo;
            return fila == null ? null : articulos.FirstOrDefault(a => a.Id == fila.Id);
        }
        private void ActualizarBotones() { editar.Enabled = eliminar.Enabled = detalle.Enabled = !ocupado && Seleccionado() != null; }
        private void VerDetalle()
        {
            var articulo=Seleccionado(); if(ocupado || articulo==null) return;
            using(var form=new FrmDetalle(articulo)) form.ShowDialog(this);
        }
        private void Ocupado(bool valor)
        {
            ocupado = valor; filtros.Enabled = acciones.Enabled = grilla.Enabled = !valor;
            UseWaitCursor = valor; ActualizarBotones();
        }
        private void Limpiar() { texto.Clear(); codigo.Clear(); if(opcionesCargadas) { marca.SelectedIndex=0; categoria.SelectedIndex=0; } usarMin.Checked=usarMax.Checked=false; minimo.Value=maximo.Value=0; }
        private async Task Cargar(int? seleccionar = null, string mensaje = null)
        {
            if(ocupado) return;
            Ocupado(true); estado.ForeColor=Color.DimGray; estado.Text="Buscando artículos…";
            try
            {
                if(!opcionesCargadas)
                {
                    int marcaAnterior = marca.SelectedItem is Marca ? ((Marca)marca.SelectedItem).Id : 0;
                    int categoriaAnterior = categoria.SelectedItem is Categoria ? ((Categoria)categoria.SelectedItem).Id : 0;
                    var marcas=await Task.Run(() => new MarcaBL().Listar());
                    var categorias=await Task.Run(() => new CategoriaBL().Listar());
                    if(IsDisposed) return;
                    marcas.Insert(0,new Marca { Id=0, Descripcion="Todas las marcas" });
                    categorias.Insert(0,new Categoria { Id=0, Descripcion="Todas las categorías" });
                    marca.DisplayMember="Descripcion"; marca.ValueMember="Id"; marca.DataSource=marcas;
                    categoria.DisplayMember="Descripcion"; categoria.ValueMember="Id"; categoria.DataSource=categorias;
                    marca.SelectedValue=marcas.Any(x=>x.Id==marcaAnterior) ? marcaAnterior : 0;
                    categoria.SelectedValue=categorias.Any(x=>x.Id==categoriaAnterior) ? categoriaAnterior : 0;
                    opcionesCargadas=true;
                }
                var filtro=new FiltroArticulo { Texto=texto.Text, Codigo=codigo.Text,
                    IdMarca=((Marca)marca.SelectedItem).Id==0 ? (int?)null : ((Marca)marca.SelectedItem).Id,
                    IdCategoria=((Categoria)categoria.SelectedItem).Id==0 ? (int?)null : ((Categoria)categoria.SelectedItem).Id,
                    PrecioMinimo=usarMin.Checked ? minimo.Value : (decimal?)null,
                    PrecioMaximo=usarMax.Checked ? maximo.Value : (decimal?)null };
                var lista=await Task.Run(() => negocio.Buscar(filtro));
                if(IsDisposed) return;
                articulos=lista;
                grilla.DataSource=lista.Select(a=>new FilaArticulo { Id=a.Id, Codigo=a.Codigo, Nombre=a.Nombre, Descripcion=a.Descripcion,
                    Marca=a.Marca==null ? "(Sin marca)" : a.Marca.Descripcion,
                    Categoria=a.Categoria==null ? "(Sin categoría)" : a.Categoria.Descripcion, Precio=a.Precio, Imagenes=a.Imagenes.Count }).ToList();
                if(seleccionar.HasValue) foreach(DataGridViewRow fila in grilla.Rows) if(((FilaArticulo)fila.DataBoundItem).Id==seleccionar) { grilla.CurrentCell=fila.Cells[0]; break; }
                estado.ForeColor=Color.DarkGreen;
                estado.Text=(mensaje==null ? "" : mensaje+" ")+(lista.Count==0 ? "No hay artículos con estos filtros. Probá limpiar los filtros." : lista.Count+" artículo(s) encontrados.");
            }
            catch(Exception ex) { if(!IsDisposed) { articulos.Clear(); grilla.DataSource=null; estado.ForeColor=Color.Firebrick; estado.Text="No se pudo completar la búsqueda: "+ex.Message; } }
            finally { if(!IsDisposed) Ocupado(false); }
        }
        private async Task AbrirEditor(bool nuevo)
        {
            if(ocupado) return;
            var seleccionado=Seleccionado(); if(!nuevo && seleccionado==null) return;
            Ocupado(true);
            int? guardado=null;
            bool clasificacionesActualizadas=false;
            try
            {
                var articulo=nuevo ? new Articulo() : await Task.Run(()=>negocio.ObtenerPorId(seleccionado.Id));
                if(articulo==null) throw new InvalidOperationException("El artículo ya no existe. Actualizá la lista.");
                var marcas=await Task.Run(()=>new MarcaBL().Listar());
                var categorias=await Task.Run(()=>new CategoriaBL().Listar());
                if(IsDisposed) return;
                using(var form=new FrmArticulo(articulo,marcas,categorias))
                {
                    if(form.ShowDialog(this)==DialogResult.OK) guardado=form.ArticuloId;
                    clasificacionesActualizadas=form.ClasificacionesActualizadas;
                }
            }
            catch(Exception ex) { if(!IsDisposed) MessageBox.Show(this,ex.Message,"No se pudo abrir el artículo",MessageBoxButtons.OK,MessageBoxIcon.Warning); }
            finally { if(!IsDisposed) Ocupado(false); }
            if(!IsDisposed)
            {
                if(guardado.HasValue) Limpiar();
                if(clasificacionesActualizadas) opcionesCargadas=false;
                if(guardado.HasValue) await Cargar(guardado,"Artículo guardado.");
                else if(clasificacionesActualizadas) await Cargar(null,"Marcas y categorías actualizadas.");
            }
        }
        private async Task Eliminar()
        {
            var articulo=Seleccionado(); if(ocupado || articulo==null) return;
            if(MessageBox.Show(this,"¿Eliminar «"+articulo.Nombre+"» ("+articulo.Codigo+") y sus imágenes? Esta acción no se puede deshacer.","Confirmar eliminación",MessageBoxButtons.YesNo,MessageBoxIcon.Warning,MessageBoxDefaultButton.Button2)!=DialogResult.Yes) return;
            Ocupado(true); bool eliminado=false;
            try { await Task.Run(()=>negocio.Eliminar(articulo.Id)); eliminado=true; }
            catch(Exception ex) { if(!IsDisposed) MessageBox.Show(this,ex.Message,"No se pudo eliminar",MessageBoxButtons.OK,MessageBoxIcon.Warning); }
            finally { if(!IsDisposed) Ocupado(false); }
            if(eliminado && !IsDisposed) await Cargar(null,"Artículo eliminado.");
        }
        private class FilaArticulo
        {
            public int Id {get;set;} public string Codigo {get;set;} public string Nombre {get;set;}
            public string Descripcion {get;set;} public string Marca {get;set;} public string Categoria {get;set;}
            public decimal Precio {get;set;} public int Imagenes {get;set;}
        }
    }
}
