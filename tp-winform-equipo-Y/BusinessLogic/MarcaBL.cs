using DAO;
using Dominio;
using System;
using System.Collections.Generic;


namespace BusinessLogic
{
    public class MarcaBL
    {
        public List<Marca> Listar()
        {
            return new MarcaDAO().Listar();
        }

        public int Agregar(Marca marca)
        {
            if (marca == null) throw new ArgumentNullException("marca");

            ValidacionBL.Texto(marca.Descripcion, "La marca", 50);
            return new MarcaDAO().Agregar(marca);
        }

        public void Modificar(Marca marca)
        {
            if (marca == null) throw new ArgumentNullException("marca");
            ValidacionBL.Id(marca.Id);
            ValidacionBL.Texto(marca.Descripcion, "La marca", 50);
            new MarcaDAO().Modificar(marca);
        }
        public void eliminar(int id)
        {
            ValidacionBL.Id(id);
            new MarcaDAO().Eliminar(id);
        }
    }
}
