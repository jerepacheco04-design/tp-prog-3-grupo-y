using System;
using System.Linq;
using Dominio;

namespace DAO
{
    internal static class Validacion
    {
        internal static void Id(int id)
        {
            if (id <= 0) throw new ArgumentException("El identificador debe ser mayor que cero.");
        }

        internal static void Texto(string valor, string campo, int maximo)
        {
            if (string.IsNullOrWhiteSpace(valor) || valor.Trim().Length > maximo)
                throw new ArgumentException(campo + " es obligatorio y admite hasta " + maximo + " caracteres.");
        }

        internal static void Url(string valor)
        {
            Texto(valor, "La URL de la imagen", 1000);
            Uri uri;
            if (!Uri.TryCreate(valor.Trim(), UriKind.Absolute, out uri) ||
                (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
                throw new ArgumentException("La imagen debe tener una URL http o https válida.");
        }
        //comentario prueba 
        internal static void Precio(decimal valor)
        {
            if (valor < 0 || valor > 922337203685477.5807m || decimal.Round(valor, 4) != valor)
                throw new ArgumentException("El precio debe ser no negativo, tener hasta cuatro decimales y estar dentro del rango SQL money.");
        }

        internal static void Articulo(Articulo articulo)
        {
            if (articulo == null) throw new ArgumentNullException("articulo");
            Texto(articulo.Codigo, "El código", 50);
            Texto(articulo.Nombre, "El nombre", 50);
            Texto(articulo.Descripcion, "La descripción", 150);
            if (articulo.Marca == null || articulo.Marca.Id <= 0) throw new ArgumentException("Seleccioná una marca.");
            if (articulo.Categoria == null || articulo.Categoria.Id <= 0) throw new ArgumentException("Seleccioná una categoría.");
            Precio(articulo.Precio);
            if (articulo.Imagenes == null || articulo.Imagenes.Count == 0)
                throw new ArgumentException("El artículo debe tener al menos una imagen.");
            foreach (var imagen in articulo.Imagenes)
            {
                if (imagen == null) throw new ArgumentException("La lista contiene una imagen vacía.");
                Url(imagen.ImagenUrl);
            }
            if (articulo.Imagenes.Select(i => i.ImagenUrl.Trim()).Distinct(StringComparer.Ordinal).Count() != articulo.Imagenes.Count)
                throw new ArgumentException("No se puede repetir una URL en el mismo artículo.");
        }
    }
}
