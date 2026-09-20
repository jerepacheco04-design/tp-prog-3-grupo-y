using System;
using Dominio;

namespace BusinessLogic
{
    internal static class ValidacionBL
    {
        // Helper de validacion de id generico. (por el momento solo valida int > 0)
        internal static void Id(int id)
        {
            if (id <= 0) throw new ArgumentException("El identificador debe ser mayor que cero.");
        }

        // Helper de validacion de texto generico.
        internal static void Texto(string valor, string campo, int maximo)
        {
            if (string.IsNullOrWhiteSpace(valor) || valor.Trim().Length > maximo)
                throw new ArgumentException(campo + " es obligatorio y admite hasta " + maximo + " caracteres.");
        }

        // Helper de validacion de URL generico. ¿esta bien que seteemos el valor max en 1000? si hay mas tablas con otro lenght no seria generico esto.
        internal static void Url(string valor)
        {
            Texto(valor, "La imagen", 1000);
            if (RutaImagen.EsLocal(valor.Trim()))
                return;
            Uri uri;
            if (!Uri.TryCreate(valor.Trim(), UriKind.Absolute, out uri) ||
                (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
                throw new ArgumentException("Agregá una foto desde la PC o una URL http o https válida.");
        }

        internal static void Precio(decimal valor)
        {
            if (valor < 0 || valor > 922337203685477.5807m || decimal.Round(valor, 4) != valor)
                throw new ArgumentException("El precio debe ser no negativo, tener hasta cuatro decimales y estar dentro del rango SQL money.");
        }
    }
}
