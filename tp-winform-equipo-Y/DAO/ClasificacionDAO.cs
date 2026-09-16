using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace DAO
{
    // Código común a marcas y categorías. Los nombres SQL se eligen internamente.
    internal class ClasificacionDAO
    {
        private readonly AccesoDatos datos;
        private readonly string tabla;
        private readonly string campo;
        internal ClasificacionDAO(AccesoDatos datos, bool marca)
        {
            if (datos == null) throw new ArgumentNullException("datos");
            this.datos = datos;
            tabla = marca ? "MARCAS" : "CATEGORIAS";
            campo = marca ? "IdMarca" : "IdCategoria";
        }
        internal List<KeyValuePair<int, string>> Listar()
        {
            var lista = new List<KeyValuePair<int, string>>();
            using (var cn = datos.AbrirConexion())
            using (var cmd = new SqlCommand("SELECT Id, Descripcion FROM " + tabla + " ORDER BY Descripcion, Id", cn))
            using (var dr = cmd.ExecuteReader())
                while (dr.Read()) lista.Add(new KeyValuePair<int, string>((int)dr["Id"], Convert.ToString(dr["Descripcion"])));
            return lista;
        }
        internal int Guardar(int id, string descripcion, bool nuevo)
        {
            Validacion.Texto(descripcion, "La descripción", 50);
            if (!nuevo) Validacion.Id(id);
            else if (id != 0) throw new ArgumentException("Un registro nuevo debe tener Id cero.");
            using (var cn = datos.AbrirConexion())
            using (var tx = cn.BeginTransaction(IsolationLevel.Serializable))
            {
                using (var cmd = new SqlCommand("SELECT COUNT(*) FROM " + tabla + " WITH (UPDLOCK,HOLDLOCK) WHERE Descripcion=@Descripcion AND Id<>@Id", cn, tx))
                {
                    AccesoDatos.AgregarTexto(cmd, "@Descripcion", descripcion.Trim(), 50);
                    cmd.Parameters.Add("@Id", SqlDbType.Int).Value = id;
                    if ((int)cmd.ExecuteScalar() > 0) throw new ArgumentException("Ya existe esa descripción en " + tabla + ".");
                }
                string sql = nuevo ? "INSERT INTO " + tabla + " (Descripcion) VALUES (@Descripcion); SELECT CAST(SCOPE_IDENTITY() AS int);"
                    : "UPDATE " + tabla + " SET Descripcion=@Descripcion WHERE Id=@Id";
                using (var cmd = new SqlCommand(sql, cn, tx))
                {
                    AccesoDatos.AgregarTexto(cmd, "@Descripcion", descripcion.Trim(), 50);
                    cmd.Parameters.Add("@Id", SqlDbType.Int).Value = id;
                    if (nuevo) id = (int)cmd.ExecuteScalar();
                    else if (cmd.ExecuteNonQuery() != 1) throw new InvalidOperationException("El registro ya no existe.");
                }
                tx.Commit();
                return id;
            }
        }
        internal void Eliminar(int id)
        {
            Validacion.Id(id);
            using (var cn = datos.AbrirConexion())
            using (var tx = cn.BeginTransaction(IsolationLevel.Serializable))
            {
                AccesoDatos.VerificarExistencia(cn, tx, tabla, id);
                using (var cmd = new SqlCommand("SELECT COUNT(*) FROM ARTICULOS WITH (UPDLOCK,HOLDLOCK) WHERE " + campo + "=@Id", cn, tx))
                {
                    cmd.Parameters.Add("@Id", SqlDbType.Int).Value = id;
                    if ((int)cmd.ExecuteScalar() > 0) throw new InvalidOperationException("Hay artículos que utilizan este registro. No se puede eliminar.");
                }
                using (var cmd = new SqlCommand("DELETE FROM " + tabla + " WHERE Id=@Id", cn, tx))
                {
                    cmd.Parameters.Add("@Id", SqlDbType.Int).Value = id;
                    cmd.ExecuteNonQuery();
                }
                tx.Commit();
            }
        }
    }
}
