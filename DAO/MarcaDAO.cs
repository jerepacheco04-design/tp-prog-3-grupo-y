using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Dominio;

namespace DAO
{
    public class MarcaDAO
    {
        private readonly AccesoDatos datos;

        public MarcaDAO() : this(new AccesoDatos())
        {
        }

        public MarcaDAO(AccesoDatos datos)
        {
            if (datos == null)
                throw new ArgumentNullException("datos");
            this.datos = datos;
        }

        public List<Marca> Listar()
        {
            var lista = new List<Marca>();
            using (var cn = datos.AbrirConexion())
            using (var cmd = new SqlCommand("SELECT Id, Descripcion FROM MARCAS ORDER BY Descripcion, Id", cn))
            using (var dr = cmd.ExecuteReader())
                while (dr.Read())
                    lista.Add(new Marca { Id = (int)dr["Id"], Descripcion = Convert.ToString(dr["Descripcion"]) });
            return lista;
        }

        public int Agregar(Marca marca)
        {
            marca.Id = Guardar(marca.Id, marca.Descripcion, true);
            return marca.Id;
        }

        public void Modificar(Marca marca)
        {
            Guardar(marca.Id, marca.Descripcion, false);
        }

        private int Guardar(int id, string descripcion, bool nuevo)
        {
            using (var cn = datos.AbrirConexion())
            using (var tx = cn.BeginTransaction(IsolationLevel.Serializable))
            {
                using (var cmd = new SqlCommand("SELECT COUNT(*) FROM MARCAS WITH (UPDLOCK,HOLDLOCK) WHERE Descripcion=@Descripcion AND Id<>@Id", cn, tx))
                {
                    AccesoDatos.AgregarTexto(cmd, "@Descripcion", descripcion.Trim(), 50);
                    cmd.Parameters.Add("@Id", SqlDbType.Int).Value = id;
                    if ((int)cmd.ExecuteScalar() > 0)
                        throw new ArgumentException("Ya existe esa marca.");
                }

                string sql = nuevo
                    ? "INSERT INTO MARCAS (Descripcion) VALUES (@Descripcion); SELECT CAST(SCOPE_IDENTITY() AS int);"
                    : "UPDATE MARCAS SET Descripcion=@Descripcion WHERE Id=@Id";
                using (var cmd = new SqlCommand(sql, cn, tx))
                {
                    AccesoDatos.AgregarTexto(cmd, "@Descripcion", descripcion.Trim(), 50);
                    cmd.Parameters.Add("@Id", SqlDbType.Int).Value = id;
                    if (nuevo)
                        id = (int)cmd.ExecuteScalar();
                    else if (cmd.ExecuteNonQuery() != 1)
                        throw new InvalidOperationException("El registro ya no existe.");
                }

                tx.Commit();
                return id;
            }
        }

        public void Eliminar(int id)
        {
            using (var cn = datos.AbrirConexion())
            using (var tx = cn.BeginTransaction(IsolationLevel.Serializable))
            {
                AccesoDatos.VerificarExistencia(cn, tx, "MARCAS", id);
                using (var cmd = new SqlCommand("SELECT COUNT(*) FROM ARTICULOS WITH (UPDLOCK,HOLDLOCK) WHERE IdMarca=@Id", cn, tx))
                {
                    cmd.Parameters.Add("@Id", SqlDbType.Int).Value = id;
                    if ((int)cmd.ExecuteScalar() > 0)
                        throw new InvalidOperationException("Hay artículos que utilizan este registro. No se puede eliminar.");
                }

                using (var cmd = new SqlCommand("DELETE FROM MARCAS WHERE Id=@Id", cn, tx))
                {
                    cmd.Parameters.Add("@Id", SqlDbType.Int).Value = id;
                    cmd.ExecuteNonQuery();
                }

                tx.Commit();
            }
        }
    }
}
