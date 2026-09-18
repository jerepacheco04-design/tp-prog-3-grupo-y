using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;

namespace DAO
{
    internal static class BaseInicial
    {
        internal static void Preparar(SqlConnectionStringBuilder opciones)
        {
            if (File.Exists(opciones.AttachDBFilename)) return;

            string respaldo = BuscarRespaldo();
            if (respaldo == null)
                throw new InvalidOperationException("Falta DatosIniciales/Catalogo.bak. Extraé la carpeta completa del ZIP.");

            var servidor = new SqlConnectionStringBuilder(opciones.ConnectionString);
            servidor.InitialCatalog = "master";
            servidor.AttachDBFilename = "";
            using (var conexion = new SqlConnection(servidor.ConnectionString))
            {
                conexion.Open();
                using (var comprobar = new SqlCommand("SELECT COUNT(*) FROM sys.databases WHERE name=@nombre", conexion))
                {
                    comprobar.Parameters.AddWithValue("@nombre", opciones.InitialCatalog);
                    if ((int)comprobar.ExecuteScalar() != 0)
                        throw new InvalidOperationException("Ya existe una base con ese nombre. Revisá la conexión antes de continuar.");
                }

                string nombreDatos = null;
                string nombreLog = null;
                using (var archivos = new SqlCommand("RESTORE FILELISTONLY FROM DISK=@respaldo", conexion))
                {
                    archivos.Parameters.AddWithValue("@respaldo", respaldo);
                    using (var lector = archivos.ExecuteReader())
                    {
                        while (lector.Read())
                        {
                            string tipo = (string)lector["Type"];
                            if (tipo == "D" && nombreDatos == null) nombreDatos = (string)lector["LogicalName"];
                            else if (tipo == "L" && nombreLog == null) nombreLog = (string)lector["LogicalName"];
                            else throw new InvalidOperationException("El respaldo no tiene el formato esperado.");
                        }
                    }
                }
                if (nombreDatos == null || nombreLog == null)
                    throw new InvalidOperationException("El respaldo está incompleto.");

                string archivoLog = Path.ChangeExtension(opciones.AttachDBFilename, ".ldf");
                if (File.Exists(archivoLog))
                    throw new InvalidOperationException("Ya hay archivos de una base anterior. No se reemplazaron.");
                Directory.CreateDirectory(Path.GetDirectoryName(opciones.AttachDBFilename));
                string nombreBase = "[" + opciones.InitialCatalog.Replace("]", "]]") + "]";
                using (var restaurar = new SqlCommand("RESTORE DATABASE " + nombreBase +
                    " FROM DISK=@respaldo WITH MOVE @datos TO @mdf, MOVE @log TO @ldf, CHECKSUM", conexion))
                {
                    restaurar.CommandTimeout = 120;
                    restaurar.Parameters.AddWithValue("@respaldo", respaldo);
                    restaurar.Parameters.AddWithValue("@datos", nombreDatos);
                    restaurar.Parameters.AddWithValue("@log", nombreLog);
                    restaurar.Parameters.AddWithValue("@mdf", opciones.AttachDBFilename);
                    restaurar.Parameters.AddWithValue("@ldf", archivoLog);
                    restaurar.ExecuteNonQuery();
                }
            }
        }

        private static string BuscarRespaldo()
        {
            string encontrado = null;
            var carpeta = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
            while (carpeta != null)
            {
                string archivo = Path.Combine(carpeta.FullName, "DatosIniciales", "Catalogo.bak");
                if (File.Exists(archivo)) encontrado = archivo;
                if (Directory.GetFiles(carpeta.FullName, "*.sln").Length > 0) break;
                carpeta = carpeta.Parent;
            }
            return encontrado;
        }
    }
}
