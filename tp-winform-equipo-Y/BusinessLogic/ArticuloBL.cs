
using DAO;
using Dominio;
using System;
using System.Collections.Generic;


namespace BusinessLogic
{
    public class ArticuloBL
    {
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
            if (filtro.PrecioMinimo.HasValue) Validacion.Precio(filtro.PrecioMinimo.Value);
            if (filtro.PrecioMaximo.HasValue) Validacion.Precio(filtro.PrecioMaximo.Value);

            return dao.Buscar(filtro);
        }

    }
}
