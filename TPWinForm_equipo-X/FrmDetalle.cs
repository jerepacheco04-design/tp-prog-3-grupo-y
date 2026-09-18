using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Dominio;

namespace TPWinForm_equipo_X
{
    public class FrmDetalle : Form
    {
        private readonly List<FotoArticulo> fotos;
        private readonly PictureBox vista=new PictureBox { SizeMode=PictureBoxSizeMode.Zoom, BackColor=Color.White, BorderStyle=BorderStyle.FixedSingle };
        private readonly Label estado=new Label { TextAlign=ContentAlignment.MiddleCenter };
        private readonly Label contador=new Label { TextAlign=ContentAlignment.MiddleCenter };
        private readonly Button anterior=new Button { Text="← Anterior" };
        private readonly Button siguiente=new Button { Text="Siguiente →" };

        private CancellationTokenSource carga;
        private int indice;

        public FrmDetalle(Articulo articulo) : this(articulo.Nombre,
            "Código: "+articulo.Codigo+"     Marca: "+(articulo.Marca==null ? "Sin marca" : articulo.Marca.Descripcion)+
            "     Categoría: "+(articulo.Categoria==null ? "Sin categoría" : articulo.Categoria.Descripcion)+
            "     Precio: "+articulo.Precio.ToString("N4")+Environment.NewLine+articulo.Descripcion,
            articulo.Imagenes.Select((x,i)=>new FotoArticulo { Referencia=x.ImagenUrl, Nombre="Imagen "+(i+1) }).ToList(),0) { }

        public FrmDetalle(string titulo,string descripcion,List<FotoArticulo> fotos,int inicial)
        {
            this.fotos=fotos;
            indice=fotos.Count==0 ? 0 : Math.Max(0,Math.Min(inicial,fotos.Count-1));
            Text="Detalle e imágenes — "+titulo;
            ClientSize=new Size(900,680); MinimumSize=new Size(800,600);
            StartPosition=FormStartPosition.CenterParent;
            Font=new Font("Segoe UI",10); BackColor=Color.FromArgb(247,249,252);
            Controls.Add(new Label { Text=titulo, Font=new Font(Font.FontFamily,20,FontStyle.Bold), Location=new Point(24,16), Size=new Size(850,38), AutoEllipsis=true, Anchor=AnchorStyles.Top|AnchorStyles.Left|AnchorStyles.Right });
            Controls.Add(new Label { Text=descripcion, Location=new Point(24,64), Size=new Size(852,80), AutoEllipsis=true, Anchor=AnchorStyles.Top|AnchorStyles.Left|AnchorStyles.Right });
            vista.SetBounds(24,150,852,410); vista.Anchor=AnchorStyles.Top|AnchorStyles.Bottom|AnchorStyles.Left|AnchorStyles.Right;
            estado.SetBounds(24,566,852,45); estado.Anchor=AnchorStyles.Bottom|AnchorStyles.Left|AnchorStyles.Right;
            anterior.SetBounds(24,628,135,32); anterior.Anchor=AnchorStyles.Left|AnchorStyles.Bottom;
            contador.SetBounds(164,628,550,32); contador.Anchor=AnchorStyles.Left|AnchorStyles.Right|AnchorStyles.Bottom;
            siguiente.SetBounds(741,628,135,32); siguiente.Anchor=AnchorStyles.Right|AnchorStyles.Bottom;
            Controls.AddRange(new Control[] { vista, estado, anterior, contador, siguiente });
            anterior.Click+=async(s,e)=>{indice--; await Mostrar();};
            siguiente.Click+=async(s,e)=>{indice++; await Mostrar();};
            Shown+=async(s,e)=>await Mostrar();
        }
        private async Task Mostrar()
        {
            if(carga!=null) { carga.Cancel(); carga.Dispose(); }
            carga=new CancellationTokenSource(); var token=carga.Token;
            var anteriorImagen=vista.Image; vista.Image=null; if(anteriorImagen!=null) anteriorImagen.Dispose();
            anterior.Enabled=indice>0; siguiente.Enabled=indice<fotos.Count-1;
            contador.Text=fotos.Count==0 ? "Sin imágenes" : "Imagen "+(indice+1)+" de "+fotos.Count;
            estado.ForeColor=Color.DimGray; estado.Text=fotos.Count==0 ? "Este artículo no tiene fotos." : "Cargando imagen…";
            if(fotos.Count==0) return;
            try
            {
                var foto=fotos[indice]; Bitmap imagen;
                if(foto.ArchivoPendiente!=null || RutaImagen.EsLocal(foto.Referencia))
                {
                    string archivo=foto.ArchivoPendiente ?? ArchivosImagen.Resolver(foto.Referencia);
                    imagen=await Task.Run(()=>{ArchivosImagen.ValidarArchivo(archivo); return ArchivosImagen.LeerArchivo(archivo);},token);
                }
                else
                {
                    imagen=await ImagenRemota.Cargar(foto.Referencia,token);
                }
                if(token.IsCancellationRequested || IsDisposed) { imagen.Dispose(); return; }
                vista.Image=imagen; estado.Text=foto.ArchivoPendiente!=null || RutaImagen.EsLocal(foto.Referencia) ? "Foto de la PC" : "Imagen desde un enlace";
            }
            catch(Exception ex)
            {
                if(token.IsCancellationRequested || IsDisposed) return;
                estado.ForeColor=Color.Firebrick;
                estado.Text=ex is OperationCanceledException ? "El sitio tardó demasiado. Volvé a intentar o elegí otra foto." : "No se pudo mostrar la foto: "+ex.GetBaseException().Message;
            }
        }
        protected override void Dispose(bool disposing)
        {
            if(disposing)
            {
                if(carga!=null) { carga.Cancel(); carga.Dispose(); carga=null; }
                if(vista.Image!=null) { vista.Image.Dispose(); vista.Image=null; }
            }
            base.Dispose(disposing);
        }
    }
}


