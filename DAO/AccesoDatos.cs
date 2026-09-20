using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace DAO
{

    public sealed class AccesoDatos
    {
        private readonly string cadenaConexion;
        public AccesoDatos() : this(LeerConfiguracion())
        {
        }

        public AccesoDatos(string cadenaConexion)
        {
            if (string.IsNullOrWhiteSpace(cadenaConexion))
                throw new ArgumentException("La cadena de conexión es obligatoria.");
            this.cadenaConexion = cadenaConexion;
        }

        private static string LeerConfiguracion()
        {
            var configuracion = ConfigurationManager.ConnectionStrings["Catalogo"];
            if (configuracion == null)
                throw new ConfigurationErrorsException("Agregá la conexión Catalogo en App.config del proyecto de inicio.");
            return configuracion.ConnectionString;
        }

        public SqlConnection AbrirConexion()
        {
            var conexion = new SqlConnection(cadenaConexion);
            try
            {
                conexion.Open();
                return conexion;
            }
            catch
            {
                conexion.Dispose();
                throw;
            }
        }

        internal static void AgregarTexto(SqlCommand comando, string nombre, string valor, int longitud)
        {
            comando.Parameters.Add(nombre, SqlDbType.VarChar, longitud).Value = valor;
        }

        internal static void VerificarExistencia(SqlConnection conexion, SqlTransaction transaccion, string tabla, int id)
        {
            
            using (var comando = new SqlCommand("SELECT COUNT(*) FROM " + tabla + " WHERE Id = @Id", conexion, transaccion))
            {
                comando.Parameters.Add("@Id", SqlDbType.Int).Value = id;
                if ((int)comando.ExecuteScalar() != 1)
                    throw new InvalidOperationException("No existe el registro seleccionado en " + tabla + ".");
            }
        }
    }
}
