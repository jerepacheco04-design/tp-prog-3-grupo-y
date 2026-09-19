using DAO;
using Dominio;
using System;
using System.Collections.Generic;


namespace BusinessLogic
{
    public class CategoriaBL
    {
        private const int MaxDescripcionLength = 50;

        public int Agregar(Categoria categoria)
        {
            if (categoria == null) throw new ArgumentNullException("categoria");
            ValidacionBL.Texto(categoria.Descripcion, "La categoria", MaxDescripcionLength);
            CategoriaDAO dao = new CategoriaDAO();
            return dao.Agregar(categoria);
        }

        public void Modificar(Categoria categoria)
        {
            if (categoria == null) throw new ArgumentNullException("categoria");
            ValidacionBL.Texto(categoria.Descripcion, "La categoria", MaxDescripcionLength);
            CategoriaDAO dao = new CategoriaDAO();
            dao.Modificar(categoria);
        }

        public void Eliminar(int id)
        {
            ValidacionBL.Id(id);
            CategoriaDAO dao = new CategoriaDAO();
            dao.Eliminar(id);
        }

        public List<Categoria> Listar()
        {
            CategoriaDAO dao = new CategoriaDAO();
            return dao.Listar();
        }
    }
}
