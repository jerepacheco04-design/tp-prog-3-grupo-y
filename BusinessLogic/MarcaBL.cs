using System;
using System.Collections.Generic;
using DAO;
using Dominio;

namespace BusinessLogic
{
    public class MarcaBL
    {
        private const int DescripcionMaxLength = 50;

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
            ValidarMarca(marca);
            if (marca.Id != 0) throw new ArgumentException("Un registro nuevo debe tener Id cero.");
            return dao.Agregar(marca);
        }

        public void Modificar(Marca marca)
        {
            ValidarMarca(marca);
            ValidacionBL.Id(marca.Id);
            dao.Modificar(marca);
        }

        public void Eliminar(int id)
        {
            ValidacionBL.Id(id);
            dao.Eliminar(id);
        }

        private static void ValidarMarca(Marca marca)
        {
            if (marca == null) throw new ArgumentNullException("marca");
            ValidacionBL.Texto(marca.Descripcion, "La marca", DescripcionMaxLength);
        }
    }
}
