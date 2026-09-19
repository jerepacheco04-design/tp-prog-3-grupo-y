
using DAO;
using Dominio;
using System;
using System.Collections.Generic;
using System.Linq;


namespace BusinessLogic
{
    public class ArticuloBL
    {
        // Constantes para la validación de longitudes máximas, para evitar repetir el hardcodeo de valores y facilitar cambios futuros.
        private const int CodigoMaxLength = 50;
        private const int NombreMaxLength = 50;
        private const int DescripcionMaxLength = 150;

        public List<Articulo> Buscar(FiltroArticulo filtro)
        {
            ArticuloDAO dao = new ArticuloDAO();

            if (filtro == null)
            {
                throw new ArgumentNullException("filtro");
            }

            if (filtro.Texto != null && filtro.Texto.Length > 150)
            {
                throw new ArgumentException("La búsqueda admite hasta 150 caracteres.");
            }

            if (filtro.Codigo != null && filtro.Codigo.Length > 50)
            {
                throw new ArgumentException("El código admite hasta 50 caracteres.");
            }

            if (filtro.PrecioMinimo > filtro.PrecioMaximo)
            {
                throw new ArgumentException("El precio mínimo no puede superar al máximo.");
            }

            if (filtro.IdMarca.HasValue) ValidacionBL.Id(filtro.IdMarca.Value);
            if (filtro.IdCategoria.HasValue) ValidacionBL.Id(filtro.IdCategoria.Value);
            if (filtro.PrecioMinimo.HasValue) ValidacionBL.Precio(filtro.PrecioMinimo.Value);
            if (filtro.PrecioMaximo.HasValue) ValidacionBL.Precio(filtro.PrecioMaximo.Value);

            return dao.Buscar(filtro);
        }

        internal static void ValidarArticulo(Articulo articulo)
        {
            if (articulo == null) throw new ArgumentNullException("articulo");
            ValidacionBL.Texto(articulo.Codigo, "El código", CodigoMaxLength);
            ValidacionBL.Texto(articulo.Nombre, "El nombre", NombreMaxLength);
            ValidacionBL.Texto(articulo.Descripcion, "La descripción", DescripcionMaxLength);
            if (articulo.Marca == null || articulo.Marca.Id <= 0) throw new ArgumentException("Seleccioná una marca.");
            if (articulo.Categoria == null || articulo.Categoria.Id <= 0) throw new ArgumentException("Seleccioná una categoría.");
            ValidacionBL.Precio(articulo.Precio);
            if (articulo.Imagenes == null || articulo.Imagenes.Count == 0)
                throw new ArgumentException("El artículo debe tener al menos una imagen.");
            foreach (var imagen in articulo.Imagenes)
            {
                if (imagen == null) throw new ArgumentException("La lista contiene una imagen vacía.");
                ValidacionBL.Url(imagen.ImagenUrl);
            }
            if (articulo.Imagenes.Select(i => i.ImagenUrl.Trim()).Distinct(StringComparer.Ordinal).Count() != articulo.Imagenes.Count)
                throw new ArgumentException("No se puede repetir una URL en el mismo artículo.");
        }

    }
}
