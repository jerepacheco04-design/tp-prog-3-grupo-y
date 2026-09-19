using DAO;
using Dominio;
using System;
using System.Collections.Generic;


namespace BusinessLogic
{
    public class ImagenBL
    {
        public List<Imagen> ListarPorArticulo(int idArticulo)
        {
            ValidacionBL.Id(idArticulo);
            return new ImagenDAO().ListarPorArticulo(idArticulo);
        }

        public int agregar(Imagen imagen)
        {
            if (imagen == null) throw new ArgumentNullException("imagen");
            if (imagen.Id != 0) throw new ArgumentException("Una imagen nueva debe tener Id cero.");
            ValidacionBL.Id(imagen.IdArticulo);
            ValidacionBL.Url(imagen.ImagenUrl);
            return new ImagenDAO().Agregar(imagen);
        }

        public void Modificar(Imagen imagen)
        {
            if (imagen == null) throw new ArgumentNullException("imagen");
            ValidacionBL.Id(imagen.Id);
            ValidacionBL.Id(imagen.IdArticulo);
            ValidacionBL.Url(imagen.ImagenUrl);
            new ImagenDAO().Modificar(imagen);
        }

        public void Eliminar(int idImagen, int idArticulo)
        {
            ValidacionBL.Id(idImagen);
            ValidacionBL.Id(idArticulo);
            new ImagenDAO().Eliminar(idImagen);
        }


    }
}
