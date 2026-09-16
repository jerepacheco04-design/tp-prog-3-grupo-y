using System;
using System.Collections.Generic;
using System.Linq;
using Dominio;
namespace DAO
{
    public class MarcaDAO
    {
        private readonly ClasificacionDAO dao;
        public MarcaDAO() : this(new AccesoDatos()) { }
        public MarcaDAO(AccesoDatos datos) { dao = new ClasificacionDAO(datos, true); }
        public List<Marca> Listar() { return dao.Listar().Select(x => new Marca { Id = x.Key, Descripcion = x.Value }).ToList(); }
        public int Agregar(Marca marca)
        {
            if (marca == null) throw new ArgumentNullException("marca");
            marca.Id = dao.Guardar(marca.Id, marca.Descripcion, true);
            return marca.Id;
        }
        public void Modificar(Marca marca)
        {
            if (marca == null) throw new ArgumentNullException("marca");
            dao.Guardar(marca.Id, marca.Descripcion, false);
        }
        public void Eliminar(int id) { dao.Eliminar(id); }
    }
}
