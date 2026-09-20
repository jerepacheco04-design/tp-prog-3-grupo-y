using System;
using System.Collections.Generic;
using DAO;
using Dominio;

namespace BusinessLogic
{
    public class CategoriaBL
    {
        private const int DescripcionMaxLength = 50;

        private readonly CategoriaDAO dao;

        public CategoriaBL()
        {
            dao = new CategoriaDAO();
        }

        public CategoriaBL(AccesoDatos datos)
        {
            dao = new CategoriaDAO(datos);
        }

        public List<Categoria> Listar()
        {
            return dao.Listar();
        }

        public int Agregar(Categoria categoria)
        {
            ValidarCategoria(categoria);
            if (categoria.Id != 0) throw new ArgumentException("Un registro nuevo debe tener Id cero.");
            return dao.Agregar(categoria);
        }

        public void Modificar(Categoria categoria)
        {
            ValidarCategoria(categoria);
            ValidacionBL.Id(categoria.Id);
            dao.Modificar(categoria);
        }

        public void Eliminar(int id)
        {
            ValidacionBL.Id(id);
            dao.Eliminar(id);
        }

        private static void ValidarCategoria(Categoria categoria)
        {
            if (categoria == null) throw new ArgumentNullException("categoria");
            ValidacionBL.Texto(categoria.Descripcion, "La categoria", DescripcionMaxLength);
        }
    }
}
