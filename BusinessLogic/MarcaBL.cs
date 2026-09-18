using System.Collections.Generic;
using DAO;
using Dominio;

namespace BusinessLogic
{
    public class MarcaBL
    {
        private readonly MarcaDAO dao;
        public MarcaBL()
        {
            dao = new MarcaDAO();
        }

        public MarcaBL(AccesoDatos datos)
        {
            dao = new MarcaDAO(datos);
        }

        public List<Marca> Listar()
        {
            return dao.Listar();
        }

        public int Agregar(Marca marca)
        {
            return dao.Agregar(marca);
        }
    }
}
