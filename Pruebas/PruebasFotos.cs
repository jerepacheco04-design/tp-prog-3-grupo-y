using System;
using System.Drawing;
using System.IO;
using System.Linq;
using TPWinForm_equipo_X;

class PruebasFotos
{
    static int Main()
    {
        string carpeta=Path.Combine(Path.GetTempPath(),"PruebasFotos_"+Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(carpeta);
        string original=Path.Combine(carpeta,"foto.png");
        string copia=null;
        try
        {
            using(var dibujo=new Bitmap(160,100)) { using(var g=Graphics.FromImage(dibujo)) {g.Clear(Color.SteelBlue);g.FillEllipse(Brushes.Gold,40,10,80,80);} dibujo.Save(original,System.Drawing.Imaging.ImageFormat.Png); }
            copia=ArchivosImagen.Importar(original);
            if(!Dominio.RutaImagen.EsLocal(copia)) throw new Exception("Referencia no portable");
            if(!File.ReadAllBytes(original).SequenceEqual(File.ReadAllBytes(ArchivosImagen.Resolver(copia)))) throw new Exception("La copia difiere");
            File.Delete(original);
            using(var foto=ArchivosImagen.LeerArchivo(ArchivosImagen.Resolver(copia)))
                if(foto.Width!=160 || foto.Height!=100) throw new Exception("No se puede leer la copia");
            Console.WriteLine("OK: la foto sigue disponible aunque se quite el original de Descargas.");
            string corrupta=Path.Combine(carpeta,"corrupta.png"); File.WriteAllText(corrupta,"no es una imagen");
            bool rechazo=false; try { ArchivosImagen.Importar(corrupta); } catch(ArgumentException) {rechazo=true;}
            if(!rechazo) throw new Exception("No rechazó la foto inválida");
            File.Delete(corrupta);
            Console.WriteLine("OK: archivos que no son fotos rechazados.");
            ArchivosImagen.DescartarCopiaNueva(copia);
            if(File.Exists(ArchivosImagen.Resolver(copia))) throw new Exception("No eliminó la copia temporal");
            copia=null;
            Console.WriteLine("OK: limpieza de una importación fallida sin tocar otros archivos.");
            return 0;
        }
        catch(Exception ex) {Console.WriteLine(ex);return 1;}
        finally {if(copia!=null) ArchivosImagen.DescartarCopiaNueva(copia); if(File.Exists(original)) File.Delete(original); if(Directory.GetFileSystemEntries(carpeta).Length==0) Directory.Delete(carpeta);}
    }
}
