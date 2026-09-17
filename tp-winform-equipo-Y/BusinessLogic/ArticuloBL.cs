
using System.Collections.Generic;
using System.Linq;
using Dominio;
using DAO;


namespace BusinessLogic
{
    public class ArticuloBL
    {
        public List<Articulo> Buscar(FiltroArticulo filtro)
        {
            ArticuloDAO dao = new ArticuloDAO();

            return dao.Buscar(filtro);
        }
               
    }
}
