using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Dominio;

namespace TPWinForm_equipo_X
{
    public partial class FrmDetalle : Form
    {
        private readonly List<FotoArticulo> fotos = new List<FotoArticulo>();
        private CancellationTokenSource carga;
        private int indice;
        public FrmDetalle(Articulo articulo) : this(articulo.Nombre, "Código: " + articulo.Codigo + "     Marca: " + (articulo.Marca == null ? "Sin marca" : articulo.Marca.Descripcion) + "     Categoría: " + (articulo.Categoria == null ? "Sin categoría" : articulo.Categoria.Descripcion) + "     Precio: " + articulo.Precio.ToString("N4") + Environment.NewLine + articulo.Descripcion, articulo.Imagenes.Select((x, i) => new FotoArticulo { Referencia = x.ImagenUrl, Nombre = "Imagen " + (i + 1) }).ToList(), 0)
        {
        }

        public FrmDetalle()
        {
            InitializeComponent();
        }

        public FrmDetalle(string titulo, string descripcion, List<FotoArticulo> fotos, int inicial) : this()
        {
            this.fotos = fotos;
            indice = fotos.Count == 0 ? 0 : Math.Max(0, Math.Min(inicial, fotos.Count - 1));
            Text = "Detalle e imágenes — " + titulo;
            descripcionArticulo.Text = descripcion;
        }

        private async Task Mostrar()
        {
            if (carga != null)
            {
                carga.Cancel();
                carga.Dispose();
            }

            carga = new CancellationTokenSource();
            var token = carga.Token;
            var anteriorImagen = vista.Image;
            vista.Image = null;
            if (anteriorImagen != null)
                anteriorImagen.Dispose();
            anterior.Enabled = indice > 0;
            siguiente.Enabled = indice < fotos.Count - 1;
            contador.Text = fotos.Count == 0 ? "Sin imágenes" : "Imagen " + (indice + 1) + " de " + fotos.Count;
            estado.Text = fotos.Count == 0 ? "Este artículo no tiene fotos." : "Cargando imagen…";
            if (fotos.Count == 0)
                return;
            try
            {
                var foto = fotos[indice];
                Bitmap imagen;
                if (foto.ArchivoPendiente != null || RutaImagen.EsLocal(foto.Referencia))
                {
                    string archivo = foto.ArchivoPendiente ?? ArchivosImagen.Resolver(foto.Referencia);
                    imagen = await Task.Run(() =>
                    {
                        ArchivosImagen.ValidarArchivo(archivo);
                        return ArchivosImagen.LeerArchivo(archivo);
                    }, token);
                }
                else
                {
                    imagen = await ImagenRemota.Cargar(foto.Referencia, token);
                }

                if (token.IsCancellationRequested || IsDisposed)
                {
                    imagen.Dispose();
                    return;
                }

                vista.Image = imagen;
                estado.Text = "";
            }
            catch (Exception ex)
            {
                if (token.IsCancellationRequested || IsDisposed)
                    return;
                estado.Text = ex is OperationCanceledException ? "El sitio tardó demasiado. Volvé a intentar o elegí otra foto." : "No se pudo mostrar la foto: " + ex.GetBaseException().Message;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (carga != null)
                {
                    carga.Cancel();
                    carga.Dispose();
                    carga = null;
                }

                if (vista != null && vista.Image != null)
                {
                    vista.Image.Dispose();
                    vista.Image = null;
                }
            }

            base.Dispose(disposing);
        }

        private async void anterior_Click(object sender, EventArgs e)
        {
            indice--;
            await Mostrar();
        }

        private async void siguiente_Click(object sender, EventArgs e)
        {
            indice++;
            await Mostrar();
        }

        private async void FrmDetalle_Shown(object sender, EventArgs e)
        {
            await Mostrar();
        }
    }
}
