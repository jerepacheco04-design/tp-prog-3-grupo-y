using System;
using System.Collections.Generic;
using System.Linq;
using Dominio;
namespace DAO
{
    public class CategoriaDAO
    {
        private readonly ClasificacionDAO dao;
        public CategoriaDAO() : this(new AccesoDatos()) { }
        public CategoriaDAO(AccesoDatos datos) { dao = new ClasificacionDAO(datos, false); }
        public List<Categoria> Listar() { return dao.Listar().Select(x => new Categoria { Id = x.Key, Descripcion = x.Value }).ToList(); }
        public int Agregar(Categoria categoria)
        {
            if (categoria == null) throw new ArgumentNullException("categoria");
            categoria.Id = dao.Guardar(categoria.Id, categoria.Descripcion, true);
            return categoria.Id;
        }
        public void Modificar(Categoria categoria)
        {
            if (categoria == null) throw new ArgumentNullException("categoria");
            dao.Guardar(categoria.Id, categoria.Descripcion, false);
        }
        public void Eliminar(int id) { dao.Eliminar(id); }
    }
}
