using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using BusinessLogic;
using Dominio;

namespace TPWinForm_equipo_X
{
    public partial class FrmArticulo : Form
    {
        private readonly int id;
        private bool guardando;
        public int ArticuloId { get; private set; }
        public bool ClasificacionesActualizadas { get; private set; }

        public FrmArticulo()
        {
            InitializeComponent();
        }

        public FrmArticulo(Articulo articulo, List<Marca> marcas, List<Categoria> categorias) : this()
        {
            id = articulo.Id;
            Text = id == 0 ? "Agregar artículo" : "Modificar artículo";
            CargarDatos(articulo, marcas, categorias);
        }

        private void CargarDatos(Articulo articulo, List<Marca> marcas, List<Categoria> categorias)
        {
            codigo.Text = articulo.Codigo;
            nombre.Text = articulo.Nombre;
            descripcion.Text = articulo.Descripcion;
            marca.DisplayMember = "Descripcion";
            marca.ValueMember = "Id";
            marca.DataSource = marcas;
            categoria.DisplayMember = "Descripcion";
            categoria.ValueMember = "Id";
            categoria.DataSource = categorias;
            marca.SelectedIndex = -1;
            categoria.SelectedIndex = -1;
            if (articulo.Marca != null && marcas.Any(x => x.Id == articulo.Marca.Id))
                marca.SelectedValue = articulo.Marca.Id;
            if (articulo.Categoria != null && categorias.Any(x => x.Id == articulo.Categoria.Id))
                categoria.SelectedValue = articulo.Categoria.Id;
            precio.Value = Math.Max(0, Math.Min(precio.Maximum, articulo.Precio));
            foreach (var foto in articulo.Imagenes)
                imagenes.Items.Add(new FotoArticulo { Referencia = foto.ImagenUrl, Nombre = RutaImagen.EsLocal(foto.ImagenUrl) ? "Foto de la PC " + (imagenes.Items.Count + 1) : foto.ImagenUrl });
            if (imagenes.Items.Count > 0)
                imagenes.SelectedIndex = 0;
            if (id != 0 && (marca.SelectedIndex < 0 || categoria.SelectedIndex < 0))
                error.Text = "Seleccioná marca y categoría.";
        }

        private void NuevaClasificacion(bool esMarca)
        {
            using (var form = new FrmClasificacion(esMarca))
            {
                if (form.ShowDialog(this) != DialogResult.OK)
                    return;
                if (esMarca)
                {
                    var lista = ((List<Marca>)marca.DataSource).ToList();
                    lista.Add(new Marca { Id = form.RegistroId, Descripcion = form.Descripcion });
                    marca.DataSource = lista.OrderBy(x => x.Descripcion).ToList();
                    marca.SelectedValue = form.RegistroId;
                }
                else
                {
                    var lista = ((List<Categoria>)categoria.DataSource).ToList();
                    lista.Add(new Categoria { Id = form.RegistroId, Descripcion = form.Descripcion });
                    categoria.DataSource = lista.OrderBy(x => x.Descripcion).ToList();
                    categoria.SelectedValue = form.RegistroId;
                }

                ClasificacionesActualizadas = true;
                error.Text = "";
            }
        }

        private async Task Guardar()
        {
            if (guardando)
                return;
            if (!string.IsNullOrWhiteSpace(enlace.Text))
            {
                error.Text = "Agregá o borrá el enlace pendiente.";
                return;
            }

            var fotos = imagenes.Items.Cast<FotoArticulo>().ToList();
            var articulo = new Articulo
            {
                Id = id,
                Codigo = codigo.Text.Trim(),
                Nombre = nombre.Text.Trim(),
                Descripcion = descripcion.Text.Trim(),
                Marca = marca.SelectedItem as Marca,
                Categoria = categoria.SelectedItem as Categoria,
                Precio = precio.Value,
                Imagenes = new List<Imagen>()
            };
            guardando = true;
            foreach (Control c in Controls)
                if (!(c is Label))
                    c.Enabled = false;
            error.Text = "Guardando…";
            bool correcto = false;
            var copiasNuevas = new List<string>();
            try
            {
                await Task.Run(() =>
                {
                    foreach (var foto in fotos)
                    {
                        string referencia = foto.Referencia;
                        if (foto.ArchivoPendiente != null)
                        {
                            referencia = ArchivosImagen.Importar(foto.ArchivoPendiente);
                            copiasNuevas.Add(referencia);
                        }

                        articulo.Imagenes.Add(new Imagen { ImagenUrl = referencia });
                    }

                    var bl = new ArticuloBL();
                    if (id == 0)
                        bl.Agregar(articulo);
                    else
                        bl.Modificar(articulo);
                });
                ArticuloId = articulo.Id;
                correcto = true;
            }
            catch (Exception ex)
            {
                foreach (string copia in copiasNuevas)
                {
                    try
                    {
                        ArchivosImagen.DescartarCopiaNueva(copia);
                    }
                    catch (IOException)
                    {
                    }
                    catch (UnauthorizedAccessException)
                    {
                    }
                }

                error.Text = ex.Message;
            }
            finally
            {
                guardando = false;
                foreach (Control c in Controls)
                    c.Enabled = true;
            }

            if (correcto)
            {
                DialogResult = DialogResult.OK;
                Close();
            }
        }

        private async Task AgregarEnlace()
        {
            string valor;
            try
            {
                valor = ImagenRemota.Normalizar(enlace.Text);
            }
            catch (Exception ex)
            {
                error.Text = ex.Message;
                return;
            }

            if (imagenes.Items.Cast<FotoArticulo>().Any(x => x.Referencia == valor))
            {
                error.Text = "Ese enlace ya está en la lista.";
                return;
            }

            error.Text = "Comprobando la foto…";
            try
            {
                using (var foto = await ImagenRemota.Cargar(valor, System.Threading.CancellationToken.None))
                {
                }
            }
            catch (Exception ex)
            {
                if (!IsDisposed)
                {
                    error.Text = "No se agregó: " + ex.GetBaseException().Message;
                }

                return;
            }

            if (IsDisposed)
                return;
            if (imagenes.Items.Cast<FotoArticulo>().Any(x => x.Referencia == valor))
                return;
            imagenes.Items.Add(new FotoArticulo { Referencia = valor, Nombre = valor });
            imagenes.SelectedIndex = imagenes.Items.Count - 1;
            enlace.Clear();
            error.Text = "";
        }

        private void AgregarFotos()
        {
            using (var dialogo = new OpenFileDialog
            {
                Title = "Elegir fotos del artículo",
                Filter = "Fotos (JPG, PNG, GIF, BMP, WebP)|*.jpg;*.jpeg;*.png;*.gif;*.bmp;*.webp",
                Multiselect = true,
                CheckFileExists = true
            }

            )
            {
                if (dialogo.ShowDialog(this) != DialogResult.OK)
                    return;
                try
                {
                    foreach (string archivo in dialogo.FileNames)
                        ArchivosImagen.ValidarArchivo(archivo);
                    foreach (string archivo in dialogo.FileNames)
                    {
                        if (imagenes.Items.Cast<FotoArticulo>().Any(x => string.Equals(x.ArchivoPendiente, archivo, StringComparison.OrdinalIgnoreCase)))
                            continue;
                        imagenes.Items.Add(new FotoArticulo { ArchivoPendiente = archivo, Nombre = Path.GetFileName(archivo) + " (desde la PC)" });
                    }

                    if (imagenes.Items.Count > 0)
                        imagenes.SelectedIndex = imagenes.Items.Count - 1;
                    error.Text = "";
                }
                catch (Exception ex)
                {
                    error.Text = "No se pudo agregar la foto: " + ex.Message;
                }
            }
        }

        private void VerFotos()
        {
            var fotos = imagenes.Items.Cast<FotoArticulo>().ToList();
            using (var form = new FrmDetalle(string.IsNullOrWhiteSpace(nombre.Text) ? "Vista previa" : nombre.Text, "Vista previa de las fotos del artículo", fotos, Math.Max(0, imagenes.SelectedIndex)))
                form.ShowDialog(this);
        }

        private void nuevaMarca_Click(object sender, EventArgs e)
        {
            NuevaClasificacion(true);
        }

        private void nuevaCategoria_Click(object sender, EventArgs e)
        {
            NuevaClasificacion(false);
        }

        private async void agregarEnlace_Click(object sender, EventArgs e)
        {
            agregarEnlace.Enabled = false;
            try
            {
                await AgregarEnlace();
            }
            finally
            {
                if (!IsDisposed)
                    agregarEnlace.Enabled = true;
            }
        }

        private void agregarFotos_Click(object sender, EventArgs e)
        {
            AgregarFotos();
        }

        private void verFotos_Click(object sender, EventArgs e)
        {
            VerFotos();
        }

        private void imagenes_DoubleClick(object sender, EventArgs e)
        {
            VerFotos();
        }

        private void quitar_Click(object sender, EventArgs e)
        {
            if (imagenes.SelectedIndex >= 0)
                imagenes.Items.RemoveAt(imagenes.SelectedIndex);
        }

        private async void guardar_Click(object sender, EventArgs e)
        {
            await Guardar();
        }

        private void FrmArticulo_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (guardando)
                e.Cancel = true;
        }
    }
}
