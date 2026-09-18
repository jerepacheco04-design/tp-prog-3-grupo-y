using System.Collections.Generic;
using DAO;
using Dominio;
namespace BusinessLogic
{
    // Las pantallas usan esta capa; el DAO conserva las validaciones y transacciones.
    public class ArticuloBL
    {
        private readonly ArticuloDAO dao;
        public ArticuloBL() { dao = new ArticuloDAO(); }
        public ArticuloBL(AccesoDatos datos) { dao = new ArticuloDAO(datos); }
        public List<Articulo> Buscar(FiltroArticulo filtro) { return dao.Buscar(filtro); }
        public Articulo ObtenerPorId(int id) { return dao.ObtenerPorId(id); }
        public int Agregar(Articulo articulo) { return dao.Agregar(articulo); }
        public void Modificar(Articulo articulo) { dao.Modificar(articulo); }
        public void Eliminar(int id) { dao.Eliminar(id); }
    }
}
