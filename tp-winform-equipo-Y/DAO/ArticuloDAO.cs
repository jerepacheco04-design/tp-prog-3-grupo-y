// Acceso a ARTICULOS y sus IMAGENES en CATALOGO_P3_DB.
// Listar y Buscar devuelven objetos Articulo con todas sus imagenes.
// Agregar devuelve el Id generado; Modificar guarda la lista completa de imagenes.
// Las escrituras usan parametros y una transaccion para evitar guardados parciales.

using Dominio;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace DAO
{
    public class ArticuloDAO
    {
        private readonly AccesoDatos datos;
        public ArticuloDAO() : this(new AccesoDatos()) { }
        public ArticuloDAO(AccesoDatos datos)
        {
            if (datos == null) throw new ArgumentNullException("datos");
            this.datos = datos;
        }

        public Articulo ObtenerPorId(int id)
        {
            // TODO:
            // esta bien cargar una lista y retornar solo el primero? deberia validar id unico para que esto no pase
            // pero tambien deberia ser tolerante si es que la DB ya hay datos asi. Retornar y al mismo tiempo avisar con
            // algun exception?
            var articulos = Consultar(new FiltroArticulo(), id);

            if (articulos.Count == 0)
            {
                return null;
            }
            else
            {
                return articulos[0];
            }
        }

        public List<Articulo> Buscar(FiltroArticulo filtro)
        {
            return Consultar(filtro, null);
        }

        // TODO:
        // Hay que separar este método en doso mas? Vale la pena tener todo junto? Filtro de lista, obtener por id, todo apunta aca.
        private List<Articulo> Consultar(FiltroArticulo filtro, int? id)
        {
            var resultado = new List<Articulo>();
            var porId = new Dictionary<int, Articulo>();
            using (var conexion = datos.AbrirConexion())
            using (var comando = conexion.CreateCommand())
            {
                comando.CommandText = @"SELECT A.Id, A.Codigo, A.Nombre, A.Descripcion, A.IdMarca, A.IdCategoria, A.Precio,
                                        M.Descripcion AS Marca, C.Descripcion AS Categoria, I.Id AS IdImagen, I.ImagenUrl
                                        FROM ARTICULOS A
                                        LEFT JOIN MARCAS M ON M.Id = A.IdMarca
                                        LEFT JOIN CATEGORIAS C ON C.Id = A.IdCategoria
                                        LEFT JOIN IMAGENES I ON I.IdArticulo = A.Id
                                        WHERE 1 = 1";
                if (id.HasValue) AgregarFiltro(comando, " AND A.Id = @Id", "@Id", SqlDbType.Int, id.Value);
                if (!string.IsNullOrWhiteSpace(filtro.Texto))
                {
                    comando.CommandText += " AND (A.Codigo LIKE @Texto ESCAPE '~' OR A.Nombre LIKE @Texto ESCAPE '~' OR A.Descripcion LIKE @Texto ESCAPE '~')";
                    // Los comodines ingresados por el usuario se buscan como texto literal.
                    string texto = filtro.Texto.Trim().Replace("~", "~~").Replace("%", "~%").Replace("_", "~_").Replace("[", "~[");
                    AccesoDatos.AgregarTexto(comando, "@Texto", "%" + texto + "%", 302);
                }
                if (!string.IsNullOrWhiteSpace(filtro.Codigo))
                {
                    comando.CommandText += " AND A.Codigo = @Codigo";
                    AccesoDatos.AgregarTexto(comando, "@Codigo", filtro.Codigo.Trim(), 50);
                }
                if (filtro.IdMarca.HasValue) AgregarFiltro(comando, " AND A.IdMarca = @Marca", "@Marca", SqlDbType.Int, filtro.IdMarca.Value);
                if (filtro.IdCategoria.HasValue) AgregarFiltro(comando, " AND A.IdCategoria = @Categoria", "@Categoria", SqlDbType.Int, filtro.IdCategoria.Value);
                if (filtro.PrecioMinimo.HasValue) AgregarFiltro(comando, " AND A.Precio >= @Minimo", "@Minimo", SqlDbType.Money, filtro.PrecioMinimo.Value);
                if (filtro.PrecioMaximo.HasValue) AgregarFiltro(comando, " AND A.Precio <= @Maximo", "@Maximo", SqlDbType.Money, filtro.PrecioMaximo.Value);
                comando.CommandText += " ORDER BY A.Nombre, A.Id, I.Id";
                using (var lector = comando.ExecuteReader())
                    while (lector.Read())
                    {
                        int articuloId = (int)lector["Id"];
                        Articulo articulo;
                        if (!porId.TryGetValue(articuloId, out articulo))
                        {
                            articulo = new Articulo
                            {
                                Id = articuloId,
                                Codigo = Convert.ToString(lector["Codigo"]),
                                Nombre = Convert.ToString(lector["Nombre"]),
                                Descripcion = Convert.ToString(lector["Descripcion"]),
                                Precio = lector["Precio"] == DBNull.Value ? 0 : (decimal)lector["Precio"],
                                Marca = lector["IdMarca"] == DBNull.Value ? null : new Marca { Id = (int)lector["IdMarca"], Descripcion = lector["Marca"] == DBNull.Value ? "(Marca inexistente)" : (string)lector["Marca"] },
                                Categoria = lector["IdCategoria"] == DBNull.Value ? null : new Categoria { Id = (int)lector["IdCategoria"], Descripcion = lector["Categoria"] == DBNull.Value ? "(Categoría inexistente)" : (string)lector["Categoria"] }
                            };
                            porId.Add(articuloId, articulo);
                            resultado.Add(articulo);
                        }
                        if (lector["IdImagen"] != DBNull.Value)
                            articulo.Imagenes.Add(new Imagen { Id = (int)lector["IdImagen"], IdArticulo = articuloId, ImagenUrl = (string)lector["ImagenUrl"] });
                    }
            }
            return resultado;
        }

        private static void AgregarFiltro(SqlCommand comando, string condicion, string parametro, SqlDbType tipo, object valor)
        {
            comando.CommandText += condicion;
            comando.Parameters.Add(parametro, tipo).Value = valor;
        }

        public int Agregar(Articulo articulo)
        {
            Validacion.Articulo(articulo);
            if (articulo.Id != 0) throw new ArgumentException("Un artículo nuevo debe tener Id igual a cero.");
            return Guardar(articulo, true);
        }

        public void Modificar(Articulo articulo)
        {
            Validacion.Articulo(articulo);
            Validacion.Id(articulo.Id);
            Guardar(articulo, false);
        }

        private int Guardar(Articulo articulo, bool nuevo)
        {
            // Artículo e imágenes se guardan juntos: si algo falla, using revierte la transacción.
            using (var conexion = datos.AbrirConexion())
            using (var transaccion = conexion.BeginTransaction(IsolationLevel.Serializable))
            {
                AccesoDatos.VerificarExistencia(conexion, transaccion, "MARCAS", articulo.Marca.Id);
                AccesoDatos.VerificarExistencia(conexion, transaccion, "CATEGORIAS", articulo.Categoria.Id);
                using (var comando = new SqlCommand("SELECT COUNT(*) FROM ARTICULOS WITH (UPDLOCK, HOLDLOCK) WHERE Codigo = @Codigo AND Id <> @Id", conexion, transaccion))
                {
                    AccesoDatos.AgregarTexto(comando, "@Codigo", articulo.Codigo.Trim(), 50);
                    comando.Parameters.Add("@Id", SqlDbType.Int).Value = articulo.Id;
                    if ((int)comando.ExecuteScalar() > 0) throw new ArgumentException("Ya existe un artículo con ese código.");
                }
                int id = articulo.Id;
                string sql = nuevo
                    ? @"INSERT INTO ARTICULOS (Codigo, Nombre, Descripcion, IdMarca, IdCategoria, Precio)
VALUES (@Codigo, @Nombre, @Descripcion, @Marca, @Categoria, @Precio); SELECT CAST(SCOPE_IDENTITY() AS int);"
                    : @"UPDATE ARTICULOS SET Codigo=@Codigo, Nombre=@Nombre, Descripcion=@Descripcion,
IdMarca=@Marca, IdCategoria=@Categoria, Precio=@Precio WHERE Id=@Id";
                using (var comando = new SqlCommand(sql, conexion, transaccion))
                {
                    AccesoDatos.AgregarTexto(comando, "@Codigo", articulo.Codigo.Trim(), 50);
                    AccesoDatos.AgregarTexto(comando, "@Nombre", articulo.Nombre.Trim(), 50);
                    AccesoDatos.AgregarTexto(comando, "@Descripcion", articulo.Descripcion.Trim(), 150);
                    comando.Parameters.Add("@Marca", SqlDbType.Int).Value = articulo.Marca.Id;
                    comando.Parameters.Add("@Categoria", SqlDbType.Int).Value = articulo.Categoria.Id;
                    comando.Parameters.Add("@Precio", SqlDbType.Money).Value = articulo.Precio;
                    comando.Parameters.Add("@Id", SqlDbType.Int).Value = id;
                    if (nuevo) id = (int)comando.ExecuteScalar();
                    else if (comando.ExecuteNonQuery() != 1) throw new InvalidOperationException("El artículo ya no existe.");
                }
                var imagenesGuardadas = ImagenDAO.Reemplazar(conexion, transaccion, id, articulo.Imagenes);
                transaccion.Commit();
                articulo.Id = id;
                articulo.Imagenes = imagenesGuardadas;
                return id;
            }
        }

        public void Eliminar(int id)
        {
            Validacion.Id(id);
            using (var conexion = datos.AbrirConexion())
            using (var transaccion = conexion.BeginTransaction(IsolationLevel.Serializable))
            {
                AccesoDatos.VerificarExistencia(conexion, transaccion, "ARTICULOS", id);
                using (var comando = new SqlCommand("DELETE FROM IMAGENES WHERE IdArticulo=@Id; DELETE FROM ARTICULOS WHERE Id=@Id", conexion, transaccion))
                {
                    comando.Parameters.Add("@Id", SqlDbType.Int).Value = id;
                    comando.ExecuteNonQuery();
                }
                transaccion.Commit();
            }
        }
    }
}
