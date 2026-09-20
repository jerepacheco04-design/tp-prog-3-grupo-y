using System;
using System.Collections.Generic;
using DAO;
using Dominio;

namespace BusinessLogic
{
    public class ImagenBL
    {
        private readonly ImagenDAO dao;

        public ImagenBL()
        {
            dao = new ImagenDAO();
        }

        public ImagenBL(AccesoDatos datos)
        {
            dao = new ImagenDAO(datos);
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
