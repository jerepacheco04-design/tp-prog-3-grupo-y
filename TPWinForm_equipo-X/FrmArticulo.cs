using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using BusinessLogic;
using Dominio;

namespace TPWinForm_equipo_X
{
    public class FrmArticulo : Form
    {
        private readonly TextBox codigo=new TextBox { MaxLength=50 };
        private readonly TextBox nombre=new TextBox { MaxLength=50 };
        private readonly TextBox descripcion=new TextBox { MaxLength=150, Multiline=true, ScrollBars=ScrollBars.Vertical };
        private readonly ComboBox marca=new ComboBox { DropDownStyle=ComboBoxStyle.DropDownList };
        private readonly ComboBox categoria=new ComboBox { DropDownStyle=ComboBoxStyle.DropDownList };
        private readonly NumericUpDown precio=new NumericUpDown { DecimalPlaces=4, Maximum=922337203685477.5807m, ThousandsSeparator=true };
        private readonly ListBox imagenes=new ListBox { HorizontalScrollbar=true, IntegralHeight=false };
        private readonly TextBox enlace=new TextBox { MaxLength=1000 };
        private readonly Label error=new Label { ForeColor=Color.Firebrick };
        private readonly Button guardar=new Button { Text="Guardar artículo" };
        private readonly Button cancelar=new Button { Text="Cancelar", DialogResult=DialogResult.Cancel };
        private readonly int id;
        private bool guardando;
        public int ArticuloId { get; private set; }
        public bool ClasificacionesActualizadas { get; private set; }
        public FrmArticulo(Articulo articulo,List<Marca> marcas,List<Categoria> categorias)
        {
            id=articulo.Id; Text=id==0 ? "Agregar artículo" : "Modificar artículo";
            ClientSize=new Size(740,720); FormBorderStyle=FormBorderStyle.FixedDialog;
            MaximizeBox=false; MinimizeBox=false; StartPosition=FormStartPosition.CenterParent;
            Font=new Font("Segoe UI",10); BackColor=Color.FromArgb(247,249,252);
            Controls.Add(new Label { Text=Text, Font=new Font(Font.FontFamily,20,FontStyle.Bold), Location=new Point(24,16), AutoSize=true });
            Campo("Código *",codigo,24,72,210,28); Campo("Nombre *",nombre,254,72,460,28);
            Campo("Descripción *",descripcion,24,136,690,72);
            Campo("Marca *",marca,24,246,205,28); Campo("Categoría *",categoria,374,246,205,28);
            var nuevaMarca = new Button { Text="Nueva marca", Location=new Point(235,268), Size=new Size(114,32) };
            var nuevaCategoria = new Button { Text="Nueva categoría", Location=new Point(585,268), Size=new Size(129,32) };
            nuevaMarca.Click += (s,e) => NuevaClasificacion(true);
            nuevaCategoria.Click += (s,e) => NuevaClasificacion(false);
            Controls.AddRange(new Control[] { nuevaMarca, nuevaCategoria });
            Campo("Precio * (hasta cuatro decimales)",precio,24,310,325,28);
            Campo("Imágenes *: agregá fotos de la PC o enlaces",imagenes,24,374,690,96);
            enlace.SetBounds(24,504,525,28);
            enlace.AccessibleName="Enlace de una imagen http o https";
            var agregarEnlace=new Button { Text="Agregar URL", Location=new Point(559,502), Size=new Size(155,32) };
            var agregarFotos=new Button { Text="Agregar fotos desde la PC", Location=new Point(24,542), Size=new Size(228,34) };
            var verFotos=new Button { Text="Vista previa", Location=new Point(262,542), Size=new Size(150,34) };
            var quitar=new Button { Text="Quitar de la lista", Location=new Point(422,542), Size=new Size(155,34) };
            Controls.AddRange(new Control[] { enlace, agregarEnlace, agregarFotos, verFotos, quitar });
            agregarEnlace.Click+=async(s,e)=>{ agregarEnlace.Enabled=false; try { await AgregarEnlace(); } finally { if(!IsDisposed) agregarEnlace.Enabled=true; } };
            agregarFotos.Click+=(s,e)=>AgregarFotos();
            verFotos.Click+=(s,e)=>VerFotos();
            imagenes.DoubleClick+=(s,e)=>VerFotos();
            quitar.Click+=(s,e)=>{ if(imagenes.SelectedIndex>=0) imagenes.Items.RemoveAt(imagenes.SelectedIndex); };
            error.SetBounds(24,586,690,66); Controls.Add(error);
            guardar.SetBounds(394,668,170,34); cancelar.SetBounds(584,668,130,34);
            Controls.AddRange(new Control[] { guardar,cancelar }); CancelButton=cancelar;
            codigo.Text=articulo.Codigo; nombre.Text=articulo.Nombre; descripcion.Text=articulo.Descripcion;
            marca.DisplayMember="Descripcion"; marca.ValueMember="Id"; marca.DataSource=marcas;
            categoria.DisplayMember="Descripcion"; categoria.ValueMember="Id"; categoria.DataSource=categorias;
            marca.SelectedIndex=-1; categoria.SelectedIndex=-1;
            if(articulo.Marca!=null && marcas.Any(x=>x.Id==articulo.Marca.Id)) marca.SelectedValue=articulo.Marca.Id;
            if(articulo.Categoria!=null && categorias.Any(x=>x.Id==articulo.Categoria.Id)) categoria.SelectedValue=articulo.Categoria.Id;
            precio.Value=Math.Max(0,Math.Min(precio.Maximum,articulo.Precio));
            foreach(var foto in articulo.Imagenes)
                imagenes.Items.Add(new FotoArticulo { Referencia=foto.ImagenUrl, Nombre=RutaImagen.EsLocal(foto.ImagenUrl) ? "Foto de la PC "+(imagenes.Items.Count+1) : foto.ImagenUrl });
            if(imagenes.Items.Count>0) imagenes.SelectedIndex=0;
            if(id!=0 && (marca.SelectedIndex<0 || categoria.SelectedIndex<0)) error.Text="El artículo tiene una marca o categoría inexistente. Seleccioná una válida antes de guardar.";
            guardar.Click+=async(s,e)=>await Guardar();
            FormClosing+=(s,e)=>{ if(guardando) e.Cancel=true; };
        }
        private void Campo(string etiqueta,Control control,int x,int y,int ancho,int alto)
        {
            Controls.Add(new Label { Text=etiqueta, Location=new Point(x,y), AutoSize=true });
            control.SetBounds(x,y+24,ancho,alto); Controls.Add(control);
        }
        private void NuevaClasificacion(bool esMarca)
        {
            using(var form = new FrmClasificacion(esMarca))
            {
                if(form.ShowDialog(this) != DialogResult.OK) return;
                // Solo se actualiza la lista elegida; los demás campos del artículo se conservan.
                if(esMarca)
                {
                    var lista = ((List<Marca>)marca.DataSource).ToList();
                    lista.Add(new Marca { Id=form.RegistroId, Descripcion=form.Descripcion });
                    marca.DataSource = lista.OrderBy(x=>x.Descripcion).ToList();
                    marca.SelectedValue = form.RegistroId;
                }
                else
                {
                    var lista = ((List<Categoria>)categoria.DataSource).ToList();
                    lista.Add(new Categoria { Id=form.RegistroId, Descripcion=form.Descripcion });
                    categoria.DataSource = lista.OrderBy(x=>x.Descripcion).ToList();
                    categoria.SelectedValue = form.RegistroId;
                }
                ClasificacionesActualizadas = true;
                error.ForeColor = Color.DarkGreen;
                error.Text = (esMarca ? "Marca" : "Categoría") + " creada y seleccionada. Completá el artículo y guardalo.";
            }
        }
        private async Task Guardar()
        {
            if(guardando) return;
            if(!string.IsNullOrWhiteSpace(enlace.Text))
            {
                error.ForeColor=Color.Firebrick; error.Text="Tenés un enlace pendiente: tocá Agregar URL o borrá ese texto antes de guardar."; return;
            }
            var fotos=imagenes.Items.Cast<FotoArticulo>().ToList();
            var articulo=new Articulo { Id=id, Codigo=codigo.Text.Trim(), Nombre=nombre.Text.Trim(), Descripcion=descripcion.Text.Trim(),
                Marca=marca.SelectedItem as Marca, Categoria=categoria.SelectedItem as Categoria, Precio=precio.Value,
                Imagenes=new List<Imagen>() };
            guardando=true; foreach(Control c in Controls) if(!(c is Label)) c.Enabled=false;
            error.ForeColor=Color.Firebrick;
            error.Text="Guardando…"; bool correcto=false;
            var copiasNuevas=new List<string>();
            try
            {
                await Task.Run(()=>
                {
                    foreach(var foto in fotos)
                    {
                        string referencia=foto.Referencia;
                        if(foto.ArchivoPendiente!=null)
                        {
                            referencia=ArchivosImagen.Importar(foto.ArchivoPendiente);
                            copiasNuevas.Add(referencia);
                        }
                        articulo.Imagenes.Add(new Imagen { ImagenUrl=referencia });
                    }
                    var bl=new ArticuloBL(); if(id==0) bl.Agregar(articulo); else bl.Modificar(articulo);
                });
                ArticuloId=articulo.Id; correcto=true;
            }
            catch(Exception ex)
            {
                foreach(string copia in copiasNuevas) { try { ArchivosImagen.DescartarCopiaNueva(copia); } catch(IOException) { } catch(UnauthorizedAccessException) { } }
                error.Text=ex.Message;
            }
            finally { guardando=false; foreach(Control c in Controls) c.Enabled=true; }
            if(correcto) { DialogResult=DialogResult.OK; Close(); }
        }
        private async Task AgregarEnlace()
        {
            string valor; Uri uri; try { valor=ImagenRemota.Normalizar(enlace.Text); } catch(Exception ex) { error.ForeColor=Color.Firebrick; error.Text=ex.Message; return; }
            error.ForeColor=Color.Firebrick;
            if(!Uri.TryCreate(valor,UriKind.Absolute,out uri) || (uri.Scheme!="http" && uri.Scheme!="https")) { error.Text="Pegá un enlace http o https de una imagen y tocá Agregar URL."; return; }
            if(imagenes.Items.Cast<FotoArticulo>().Any(x=>x.Referencia==valor)) { error.Text="Ese enlace ya está en la lista."; return; }
            error.ForeColor=Color.DimGray; error.Text="Comprobando la foto…";
            try { using(var foto=await ImagenRemota.Cargar(valor,System.Threading.CancellationToken.None)) { } }
            catch(Exception ex) { if(!IsDisposed) { error.ForeColor=Color.Firebrick; error.Text="No se agregó: "+ex.GetBaseException().Message; } return; }
            if(IsDisposed) return;
            if(imagenes.Items.Cast<FotoArticulo>().Any(x=>x.Referencia==valor)) return;
            imagenes.Items.Add(new FotoArticulo {Referencia=valor,Nombre=valor});
            imagenes.SelectedIndex=imagenes.Items.Count-1; enlace.Clear(); error.Text="";
        }
        private void AgregarFotos()
        {
            using(var dialogo=new OpenFileDialog { Title="Elegir fotos del artículo", Filter="Fotos (JPG, PNG, GIF, BMP, WebP)|*.jpg;*.jpeg;*.png;*.gif;*.bmp;*.webp", Multiselect=true, CheckFileExists=true })
            {
                if(dialogo.ShowDialog(this)!=DialogResult.OK) return;
                try
                {
                    foreach(string archivo in dialogo.FileNames) ArchivosImagen.ValidarArchivo(archivo);
                    foreach(string archivo in dialogo.FileNames)
                    {
                        if(imagenes.Items.Cast<FotoArticulo>().Any(x=>string.Equals(x.ArchivoPendiente,archivo,StringComparison.OrdinalIgnoreCase))) continue;
                        imagenes.Items.Add(new FotoArticulo { ArchivoPendiente=archivo, Nombre=Path.GetFileName(archivo)+" (desde la PC)" });
                    }
                    if(imagenes.Items.Count>0) imagenes.SelectedIndex=imagenes.Items.Count-1;
                    error.ForeColor=Color.DarkGreen; error.Text="Fotos seleccionadas. Se copiarán al proyecto cuando guardes el artículo.";
                }
                catch(Exception ex) { error.ForeColor=Color.Firebrick; error.Text="No se pudo agregar la foto: "+ex.Message; }
            }
        }
        private void VerFotos()
        {
            var fotos=imagenes.Items.Cast<FotoArticulo>().ToList();
            using(var form=new FrmDetalle(string.IsNullOrWhiteSpace(nombre.Text) ? "Vista previa" : nombre.Text,"Vista previa de las fotos del artículo",fotos,Math.Max(0,imagenes.SelectedIndex))) form.ShowDialog(this);
        }
    }
}


