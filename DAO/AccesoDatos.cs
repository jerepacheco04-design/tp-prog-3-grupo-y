using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;

namespace DAO
{

    public sealed class AccesoDatos
    {
        private readonly string cadenaConexion;
        private readonly string archivoEsperado;
        public AccesoDatos() : this(LeerConfiguracion())
        {
        }

        
        public AccesoDatos(string cadenaConexion)
        {
            if (string.IsNullOrWhiteSpace(cadenaConexion))
                throw new ArgumentException("La cadena de conexión es obligatoria.");
            this.cadenaConexion = cadenaConexion;
            var opciones = new SqlConnectionStringBuilder(cadenaConexion);
            if (!string.IsNullOrEmpty(opciones.AttachDBFilename))
                archivoEsperado = Path.GetFullPath(opciones.AttachDBFilename);
        }

        private static string LeerConfiguracion()
        {
            var configuracion = ConfigurationManager.ConnectionStrings["Catalogo"];
            if (configuracion == null)
                throw new ConfigurationErrorsException("Agregá la conexión Catalogo en App.config del proyecto de inicio.");
            var opciones = new SqlConnectionStringBuilder(configuracion.ConnectionString);
            if (opciones.AttachDBFilename.StartsWith("|DataDirectory|", StringComparison.OrdinalIgnoreCase))
            {
                
                string carpeta = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "CatalogoEquipoY", "Datos");
                opciones.AttachDBFilename = Path.Combine(carpeta, opciones.AttachDBFilename.Substring("|DataDirectory|".Length).TrimStart('\\', '/'));
                BaseInicial.Preparar(opciones);
            }

            return opciones.ConnectionString;
        }

        public SqlConnection AbrirConexion()
        {
            var conexion = new SqlConnection(cadenaConexion);
            try
            {
                conexion.Open();
                if (archivoEsperado != null)
                {
                    using (var comando = new SqlCommand("SELECT physical_name FROM sys.database_files WHERE file_id=1", conexion))
                    {
                        string archivoReal = Convert.ToString(comando.ExecuteScalar());
                        if (!string.Equals(Path.GetFullPath(archivoReal), archivoEsperado, StringComparison.OrdinalIgnoreCase))
                            throw new InvalidOperationException("La base de datos no es la correcta.");
                    }
                }

                return conexion;
            }
            catch
            {
                conexion.Dispose();
                throw;
            }
        }

        public void GuardarRespaldo(string destino)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(destino));
            string temporal = destino + "." + Guid.NewGuid().ToString("N") + ".tmp";
            try
            {
                using (var conexion = AbrirConexion())
                using (var comando = conexion.CreateCommand())
                {
                    comando.CommandTimeout = 60;
                    string baseActual = "[" + conexion.Database.Replace("]", "]]") + "]";
                    comando.CommandText = "BACKUP DATABASE " + baseActual + " TO DISK=@archivo WITH COPY_ONLY, INIT, CHECKSUM";
                    comando.Parameters.AddWithValue("@archivo", temporal);
                    comando.ExecuteNonQuery();
                    comando.CommandText = "RESTORE VERIFYONLY FROM DISK=@archivo WITH CHECKSUM";
                    comando.ExecuteNonQuery();
                }
                if (File.Exists(destino)) File.Replace(temporal, destino, null);
                else File.Move(temporal, destino);
            }
            finally
            {
                if (File.Exists(temporal)) File.Delete(temporal);
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
