using System;
using System.Linq;
using Dominio;

namespace DAO
{
    internal static class Validacion
    {
        internal static void Id(int id)
        {
            if (id <= 0)
                throw new ArgumentException("El identificador debe ser mayor que cero.");
        }

        internal static void Texto(string valor, string campo, int maximo)
        {
            if (string.IsNullOrWhiteSpace(valor))
                throw new ArgumentException(campo + " es obligatorio.");
            if (valor.Trim().Length > maximo)
                throw new ArgumentException(campo + " es demasiado largo.");
        }

        internal static void Url(string valor)
        {
            Texto(valor, "La imagen", 1000);
            if (RutaImagen.EsLocal(valor.Trim()))
                return;
            Uri uri;
            if (!Uri.TryCreate(valor.Trim(), UriKind.Absolute, out uri) || (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
                throw new ArgumentException("Agregá una foto desde la PC o una URL http o https válida.");
        }

        internal static void Precio(decimal valor)
        {
            if (valor < 0 || valor > 922337203685477.5807m || decimal.Round(valor, 4) != valor)
                throw new ArgumentException("Revisá el precio ingresado.");
        }

        internal static void Articulo(Articulo articulo)
        {
            if (articulo == null)
                throw new ArgumentNullException("articulo");
            Texto(articulo.Codigo, "El código", 50);
            Texto(articulo.Nombre, "El nombre", 50);
            Texto(articulo.Descripcion, "La descripción", 150);
            if (articulo.Marca == null || articulo.Marca.Id <= 0)
                throw new ArgumentException("Seleccioná una marca.");
            if (articulo.Categoria == null || articulo.Categoria.Id <= 0)
                throw new ArgumentException("Seleccioná una categoría.");
            Precio(articulo.Precio);
            if (articulo.Imagenes == null || articulo.Imagenes.Count == 0)
                throw new ArgumentException("El artículo debe tener al menos una imagen.");
            foreach (var imagen in articulo.Imagenes)
            {
                if (imagen == null)
                    throw new ArgumentException("La lista contiene una imagen vacía.");
                Url(imagen.ImagenUrl);
            }

            if (articulo.Imagenes.Select(i => i.ImagenUrl.Trim()).Distinct(StringComparer.Ordinal).Count() != articulo.Imagenes.Count)
                throw new ArgumentException("No se puede repetir una URL en el mismo artículo.");
        }
    }
}
