using System.Collections.Generic;
using DAO;
using Dominio;

namespace BusinessLogic
{
    public class CategoriaBL
    {
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
            return dao.Agregar(categoria);
        }
    }
}
