using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Dominio;

namespace DAO
{
    public class ImagenDAO
    {
        private readonly AccesoDatos datos;
        public ImagenDAO() : this(new AccesoDatos())
        {
        }

        public ImagenDAO(AccesoDatos datos)
        {
            if (datos == null)
                throw new ArgumentNullException("datos");
            this.datos = datos;
        }

        public List<Imagen> ListarPorArticulo(int idArticulo)
        {
            Validacion.Id(idArticulo);
            var lista = new List<Imagen>();
            using (var conexion = datos.AbrirConexion())
            using (var comando = new SqlCommand("SELECT Id, IdArticulo, ImagenUrl FROM IMAGENES WHERE IdArticulo=@Id ORDER BY Id", conexion))
            {
                comando.Parameters.Add("@Id", SqlDbType.Int).Value = idArticulo;
                using (var lector = comando.ExecuteReader())
                    while (lector.Read())
                        lista.Add(new Imagen { Id = (int)lector["Id"], IdArticulo = (int)lector["IdArticulo"], ImagenUrl = (string)lector["ImagenUrl"] });
            }

            return lista;
        }

        public int Agregar(Imagen imagen)
        {
            if (imagen == null)
                throw new ArgumentNullException("imagen");
            if (imagen.Id != 0)
                throw new ArgumentException("Una imagen nueva debe tener Id cero.");
            Validacion.Id(imagen.IdArticulo);
            Validacion.Url(imagen.ImagenUrl);
            using (var conexion = datos.AbrirConexion())
            using (var transaccion = conexion.BeginTransaction(IsolationLevel.Serializable))
            {
                AccesoDatos.VerificarExistencia(conexion, transaccion, "ARTICULOS", imagen.IdArticulo);
                VerificarDuplicado(conexion, transaccion, imagen);
                int id = Insertar(conexion, transaccion, imagen.IdArticulo, imagen.ImagenUrl.Trim());
                transaccion.Commit();
                imagen.Id = id;
                return id;
            }
        }

        public void Modificar(Imagen imagen)
        {
            if (imagen == null)
                throw new ArgumentNullException("imagen");
            Validacion.Id(imagen.Id);
            Validacion.Id(imagen.IdArticulo);
            Validacion.Url(imagen.ImagenUrl);
            using (var conexion = datos.AbrirConexion())
            using (var transaccion = conexion.BeginTransaction(IsolationLevel.Serializable))
            {
                AccesoDatos.VerificarExistencia(conexion, transaccion, "ARTICULOS", imagen.IdArticulo);
                VerificarDuplicado(conexion, transaccion, imagen);
                using (var comando = new SqlCommand("UPDATE IMAGENES SET ImagenUrl=@Url WHERE Id=@Id AND IdArticulo=@Articulo", conexion, transaccion))
                {
                    comando.Parameters.Add("@Id", SqlDbType.Int).Value = imagen.Id;
                    comando.Parameters.Add("@Articulo", SqlDbType.Int).Value = imagen.IdArticulo;
                    AccesoDatos.AgregarTexto(comando, "@Url", imagen.ImagenUrl.Trim(), 1000);
                    if (comando.ExecuteNonQuery() != 1)
                        throw new InvalidOperationException("La imagen no existe o pertenece a otro artículo.");
                }

                transaccion.Commit();
            }
        }

        public void Eliminar(int id)
        {
            Validacion.Id(id);
            using (var conexion = datos.AbrirConexion())
            using (var transaccion = conexion.BeginTransaction(IsolationLevel.Serializable))
            {
                int idArticulo;
                using (var comando = new SqlCommand("SELECT IdArticulo FROM IMAGENES WITH (UPDLOCK, HOLDLOCK) WHERE Id=@Id", conexion, transaccion))
                {
                    comando.Parameters.Add("@Id", SqlDbType.Int).Value = id;
                    object valor = comando.ExecuteScalar();
                    if (valor == null)
                        throw new InvalidOperationException("La imagen ya no existe.");
                    idArticulo = (int)valor;
                }

                using (var comando = new SqlCommand("SELECT COUNT(*) FROM IMAGENES WHERE IdArticulo=@Articulo", conexion, transaccion))
                {
                    comando.Parameters.Add("@Articulo", SqlDbType.Int).Value = idArticulo;
                    if ((int)comando.ExecuteScalar() <= 1)
                        throw new InvalidOperationException("El artículo debe conservar al menos una imagen.");
                }

                using (var comando = new SqlCommand("DELETE FROM IMAGENES WHERE Id=@Id", conexion, transaccion))
                {
                    comando.Parameters.Add("@Id", SqlDbType.Int).Value = id;
                    comando.ExecuteNonQuery();
                }

                transaccion.Commit();
            }
        }

        private static void VerificarDuplicado(SqlConnection conexion, SqlTransaction transaccion, Imagen imagen)
        {
            using (var comando = new SqlCommand("SELECT COUNT(*) FROM IMAGENES WITH (UPDLOCK, HOLDLOCK) WHERE IdArticulo=@Articulo AND ImagenUrl COLLATE Latin1_General_100_BIN2=@Url AND Id<>@Id", conexion, transaccion))
            {
                comando.Parameters.Add("@Articulo", SqlDbType.Int).Value = imagen.IdArticulo;
                comando.Parameters.Add("@Id", SqlDbType.Int).Value = imagen.Id;
                AccesoDatos.AgregarTexto(comando, "@Url", imagen.ImagenUrl.Trim(), 1000);
                if ((int)comando.ExecuteScalar() > 0)
                    throw new ArgumentException("Esa imagen ya está asociada al artículo.");
            }
        }

        private static int Insertar(SqlConnection conexion, SqlTransaction transaccion, int idArticulo, string url)
        {
            using (var comando = new SqlCommand("INSERT INTO IMAGENES (IdArticulo, ImagenUrl) VALUES (@Articulo,@Url); SELECT CAST(SCOPE_IDENTITY() AS int);", conexion, transaccion))
            {
                comando.Parameters.Add("@Articulo", SqlDbType.Int).Value = idArticulo;
                AccesoDatos.AgregarTexto(comando, "@Url", url, 1000);
                return (int)comando.ExecuteScalar();
            }
        }

        
        internal static List<Imagen> Reemplazar(SqlConnection conexion, SqlTransaction transaccion, int idArticulo, List<Imagen> imagenes)
        {
            using (var comando = new SqlCommand("DELETE FROM IMAGENES WHERE IdArticulo=@Articulo", conexion, transaccion))
            {
                comando.Parameters.Add("@Articulo", SqlDbType.Int).Value = idArticulo;
                comando.ExecuteNonQuery();
            }

            var resultado = new List<Imagen>();
            foreach (var imagen in imagenes)
            {
                string url = imagen.ImagenUrl.Trim();
                int id = Insertar(conexion, transaccion, idArticulo, url);
                resultado.Add(new Imagen { Id = id, IdArticulo = idArticulo, ImagenUrl = url });
            }

            return resultado;
        }
    }
}
