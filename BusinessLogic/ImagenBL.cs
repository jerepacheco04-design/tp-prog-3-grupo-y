using System;
using System.Collections.Generic;
using DAO;
using Dominio;

namespace BusinessLogic
{
    // Sin uso actual: la UI no gestiona imagenes una por una, siempre guarda el Articulo completo
    // (ArticuloBL.Agregar/Modificar -> ArticuloDAO.Guardar -> ImagenDAO.Reemplazar reescribe toda la lista).
    // Se deja esta clase por si en el futuro se necesita administrar una imagen de forma individual.
    public class ImagenBL
    {
        private readonly ImagenDAO dao;

        public ImagenBL()
        {
            dao = new ImagenDAO();
        }

        public List<Imagen> ListarPorArticulo(int idArticulo)
        {
            ValidacionBL.Id(idArticulo);
            return dao.ListarPorArticulo(idArticulo);
        }

        public int Agregar(Imagen imagen)
        {
            if (imagen == null) throw new ArgumentNullException("imagen");
            if (imagen.Id != 0) throw new ArgumentException("Una imagen nueva debe tener Id cero.");
            ValidacionBL.Id(imagen.IdArticulo);
            ValidacionBL.Url(imagen.ImagenUrl);
            return dao.Agregar(imagen);
        }

        public void Modificar(Imagen imagen)
        {
            if (imagen == null) throw new ArgumentNullException("imagen");
            ValidacionBL.Id(imagen.Id);
            ValidacionBL.Id(imagen.IdArticulo);
            ValidacionBL.Url(imagen.ImagenUrl);
            dao.Modificar(imagen);
        }

        public void Eliminar(int idImagen, int idArticulo)
        {
            ValidacionBL.Id(idImagen);
            ValidacionBL.Id(idArticulo);
            dao.Eliminar(idImagen);
        }
    }
}
