using System;
using System.Drawing;
using System.IO;
using Dominio;

namespace TPWinForm_equipo_X
{
    public sealed class FotoArticulo
    {
        public string Referencia { get; set; }
        public string ArchivoPendiente { get; set; }
        public string Nombre { get; set; }

        public override string ToString()
        {
            return Nombre ?? Referencia;
        }
    }

    public static class ArchivosImagen
    {
        public const int MaximoBytes = 15 * 1024 * 1024;
        public static string CarpetaRaiz
        {
            get
            {
                var carpeta = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
                while (carpeta != null)
                {
                    if (File.Exists(Path.Combine(carpeta.FullName, "TPWinForm_equipo-X.sln")))
                        return carpeta.FullName;
                    carpeta = carpeta.Parent;
                }

                return AppDomain.CurrentDomain.BaseDirectory;
            }
        }

        public static string Resolver(string referencia)
        {
            if (!RutaImagen.EsLocal(referencia))
                throw new ArgumentException("La referencia de la foto local no es válida.");
            return Path.Combine(CarpetaRaiz, "Imagenes", referencia.Substring("Imagenes/".Length));
        }

        public static void ValidarArchivo(string archivo)
        {
            string extension = Path.GetExtension(archivo).ToLowerInvariant();
            if (extension != ".jpg" && extension != ".jpeg" && extension != ".png" && extension != ".gif" && extension != ".bmp" && extension != ".webp")
                throw new ArgumentException("Elegí una imagen JPG, PNG, GIF, BMP o WebP.");
            var info = new FileInfo(archivo);
            if (!info.Exists)
                throw new FileNotFoundException("No se encontró la foto seleccionada.");
            if (info.Length > MaximoBytes)
                throw new ArgumentException("Cada foto puede ocupar hasta 15 MB.");
            using (var foto = LeerArchivo(archivo))
            {
            }
        }

        public static Bitmap LeerArchivo(string archivo)
        {
            using (var stream = File.OpenRead(archivo))
                return Leer(stream);
        }

        public static Bitmap Leer(Stream stream)
        {
            // WIC utiliza los decodificadores de Windows, incluido WebP.
            try
            {
                var decodificador = System.Windows.Media.Imaging.BitmapDecoder.Create(stream, System.Windows.Media.Imaging.BitmapCreateOptions.PreservePixelFormat, System.Windows.Media.Imaging.BitmapCacheOption.OnLoad);
                var cuadro = decodificador.Frames[0];
                if ((long)cuadro.PixelWidth * cuadro.PixelHeight > 40000000)
                    throw new ArgumentException("La foto supera 40 megapíxeles.");
                var encoder = new System.Windows.Media.Imaging.PngBitmapEncoder();
                encoder.Frames.Add(System.Windows.Media.Imaging.BitmapFrame.Create(cuadro));
                using (var memoria = new MemoryStream())
                {
                    encoder.Save(memoria);
                    memoria.Position = 0;
                    using (var foto = Image.FromStream(memoria, true, true))
                        return new Bitmap(foto);
                }
            }
            catch (NotSupportedException)
            {
                throw new ArgumentException("El archivo no es compatible).");
            }
        }

        public static string Importar(string archivo)
        {
            ValidarArchivo(archivo);
            string referencia = "Imagenes/" + Guid.NewGuid().ToString("N") + Path.GetExtension(archivo).ToLowerInvariant();
            string destino = Resolver(referencia);
            Directory.CreateDirectory(Path.GetDirectoryName(destino));
            try
            {
                File.Copy(archivo, destino, false);
            }
            catch
            {
                if (File.Exists(destino))
                    File.Delete(destino);
                throw;
            }

            return referencia;
        }

        public static void DescartarCopiaNueva(string referencia)
        {
            
            string archivo = Resolver(referencia);
            if (File.Exists(archivo))
                File.Delete(archivo);
        }
    }
}
