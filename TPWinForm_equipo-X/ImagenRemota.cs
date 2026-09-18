using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace TPWinForm_equipo_X
{
    public static class ImagenRemota
    {
        public static string Normalizar(string enlace)
        {
            Uri uri;
            if (!Uri.TryCreate((enlace ?? "").Trim(), UriKind.Absolute, out uri) || (uri.Scheme != "http" && uri.Scheme != "https"))
                throw new ArgumentException("Pegá un enlace http o https de una imagen.");
            var parametros = HttpUtility.ParseQueryString(uri.Query);
            // Google comparte la página de resultados; imgurl contiene la foto elegida.
            if (uri.AbsolutePath == "/imgres" && parametros["imgurl"] != null)
            {
                Uri foto;
                if (Uri.TryCreate(parametros["imgurl"], UriKind.Absolute, out foto) && (foto.Scheme == "http" || foto.Scheme == "https"))
                    uri = foto;
            }

            // El generador de miniaturas antiguo falla, pero el archivo original sigue disponible.
            if (uri.Host == "intercompras.com" && uri.AbsolutePath == "/product_thumb_keepratio_2.php" && parametros["img"] != null && parametros["img"].StartsWith("images/product/", StringComparison.Ordinal))
                uri = new Uri(new Uri("https://intercompras.com/"), parametros["img"]);
            return uri.AbsoluteUri;
        }

        public static async Task<Bitmap> Cargar(string enlace, CancellationToken token)
        {
            string url = Normalizar(enlace), archivo;
            using (var hash = SHA256.Create())
                archivo = Path.Combine(ArchivosImagen.CarpetaRaiz, "Imagenes", "Cache", BitConverter.ToString(hash.ComputeHash(Encoding.UTF8.GetBytes(url))).Replace("-", "") + ".png");
            if (File.Exists(archivo))
            {
                try
                {
                    return ArchivosImagen.LeerArchivo(archivo);
                }
                catch (ArgumentException)
                {
                }
                catch (IOException)
                {
                }
            }

            // TLS 1.2 es necesario también cuando se inicia desde herramientas antiguas de .NET.
            ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12;
            using (var cliente = new HttpClient())
            using (var limite = CancellationTokenSource.CreateLinkedTokenSource(token))
            {
                limite.CancelAfter(TimeSpan.FromSeconds(20));
                using (var respuesta = await cliente.GetAsync(url, HttpCompletionOption.ResponseHeadersRead, limite.Token).ConfigureAwait(false))
                {
                    if (!respuesta.IsSuccessStatusCode)
                        throw new InvalidOperationException("El sitio de la foto respondió HTTP " + (int)respuesta.StatusCode + ". Usá otro enlace o una foto descargada.");
                    string tipo = respuesta.Content.Headers.ContentType == null ? "" : respuesta.Content.Headers.ContentType.MediaType;
                    if (tipo == "text/html")
                        throw new InvalidOperationException("Ese enlace abre una página, no una foto. En el navegador elegí Copiar dirección de imagen.");
                    if (respuesta.Content.Headers.ContentLength > ArchivosImagen.MaximoBytes)
                        throw new ArgumentException("La foto supera 15 MB.");
                    using (var origen = await respuesta.Content.ReadAsStreamAsync().ConfigureAwait(false))
                    using (var memoria = new MemoryStream())
                    {
                        byte[] buffer = new byte[8192];
                        int cantidad;
                        while ((cantidad = await origen.ReadAsync(buffer, 0, buffer.Length, limite.Token).ConfigureAwait(false)) > 0)
                        {
                            if (memoria.Length + cantidad > ArchivosImagen.MaximoBytes)
                                throw new ArgumentException("La foto supera 15 MB.");
                            memoria.Write(buffer, 0, cantidad);
                        }

                        memoria.Position = 0;
                        Bitmap imagen = ArchivosImagen.Leer(memoria);
                        try
                        {
                          
                            Directory.CreateDirectory(Path.GetDirectoryName(archivo));
                            string temporal = archivo + "." + Guid.NewGuid().ToString("N") + ".tmp";
                            try
                            {
                                imagen.Save(temporal, ImageFormat.Png);
                                if (!File.Exists(archivo))
                                    File.Move(temporal, archivo);
                            }
                            finally
                            {
                                if (File.Exists(temporal))
                                    File.Delete(temporal);
                            }
                        }
                        catch (IOException)
                        {
                        }
                        catch (UnauthorizedAccessException)
                        {
                        }

                        return imagen;
                    }
                }
            }
        }
    }
}
