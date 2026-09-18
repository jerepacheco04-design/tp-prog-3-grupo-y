using System;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using DAO;
using Dominio;
using BusinessLogic;

// Ejecutar con la ruta del SQL entregado. Usa una base temporal propia, nunca la del TP.
class PruebasCatalogo
{
    static int pruebas;
    static void Verificar(bool condicion,string mensaje)
    {
        if(!condicion) throw new Exception(mensaje);
        pruebas++; Console.WriteLine("OK: "+mensaje);
    }
    static void Rechaza(Action accion,string mensaje)
    {
        try { accion(); } catch(ArgumentException) { Verificar(true,mensaje); return; }
        throw new Exception("No rechazó: "+mensaje);
    }
    static int Main(string[] args)
    {
        string db="CatalogoTest_"+Guid.NewGuid().ToString("N");
        string master=@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=master;Integrated Security=True";
        bool creado=false;
        try
        {
            using(var cn=new SqlConnection(master))
            {
                cn.Open(); using(var q=cn.CreateCommand())
                {
                    q.CommandText="CREATE DATABASE ["+db+"]"; q.ExecuteNonQuery(); creado=true;
                    string sql=File.ReadAllText(args[0]);
                    sql=Regex.Replace(sql,@"(?im)^create database CATALOGO_P3_DB\s*$","").Replace("CATALOGO_P3_DB",db);
                    foreach(string batch in Regex.Split(sql,@"(?im)^\s*GO\s*$")) if(!string.IsNullOrWhiteSpace(batch)) { q.CommandText=batch; q.ExecuteNonQuery(); }
                }
            }
            var datos=new AccesoDatos(master.Replace("Initial Catalog=master","Initial Catalog="+db));
            var bl=new ArticuloBL(datos);
            var marcasBl=new MarcaBL(datos);
            var categoriasBl=new CategoriaBL(datos);
            var marcaNueva=new Marca { Descripcion="  Stanley  " };
            var categoriaNueva=new Categoria { Descripcion="Termos" };
            int marcaId=marcasBl.Agregar(marcaNueva);
            int categoriaId=categoriasBl.Agregar(categoriaNueva);
            Verificar(new MarcaBL(datos).Listar().Any(x=>x.Id==marcaId && x.Descripcion=="Stanley"),"Nueva marca persistida y sin espacios sobrantes");
            Verificar(new CategoriaBL(datos).Listar().Any(x=>x.Id==categoriaId && x.Descripcion=="Termos"),"Nueva categoría persistida");
            Rechaza(()=>marcasBl.Agregar(new Marca {Descripcion="Stanley"}),"Marca duplicada rechazada");
            Rechaza(()=>categoriasBl.Agregar(new Categoria {Descripcion=" Termos "}),"Categoría duplicada rechazada");
            Rechaza(()=>marcasBl.Agregar(new Marca {Descripcion=" "}),"Marca vacía rechazada");
            Rechaza(()=>categoriasBl.Agregar(new Categoria {Descripcion=new string('x',51)}),"Categoría demasiado larga rechazada");
            Verificar(bl.Buscar(new FiltroArticulo()).Count==5,"Lista inicial de cinco artículos");
            var samsung=bl.Buscar(new FiltroArticulo { Texto="samsung" });
            Verificar(samsung.Count==2 && samsung.All(x=>x.Marca.Descripcion=="Samsung"),"Búsqueda por marca en el texto (dos artículos según el SQL entregado)");
            Verificar(bl.Buscar(new FiltroArticulo { Texto="Galaxy" }).Single().Codigo=="S01","Búsqueda por nombre");
            Verificar(bl.Buscar(new FiltroArticulo { Texto="lindo loro" }).Single().Codigo=="A23","Búsqueda por descripción");
            Verificar(bl.Buscar(new FiltroArticulo { Texto="Televisores" }).Single().Codigo=="S56","Búsqueda por categoría en el texto");
            Verificar(bl.Buscar(new FiltroArticulo { Codigo="S99" }).Single().Nombre=="Play 4","Código exacto");
            Verificar(bl.Buscar(new FiltroArticulo { IdMarca=3, IdCategoria=2, PrecioMinimo=49000, PrecioMaximo=50000 }).Single().Codigo=="S56","Filtros combinados de marca, categoría y rango de precios");
            Verificar(bl.Buscar(new FiltroArticulo { Texto="%" }).Count==0,"Comodines tratados como texto literal");
            Rechaza(()=>bl.Buscar(new FiltroArticulo { PrecioMinimo=20, PrecioMaximo=10 }),"Rango de precios inválido");
            var a=new Articulo { Codigo="TEST_CRUD", Nombre="Prueba", Descripcion="Artículo de prueba", Marca=new Marca {Id=marcaId}, Categoria=new Categoria {Id=categoriaId}, Precio=123.4567m };
            a.Imagenes.Add(new Imagen {ImagenUrl="https://example.com/uno.jpg"});
            a.Imagenes.Add(new Imagen {ImagenUrl="https://example.com/dos.jpg"});
            int id=bl.Agregar(a);
            var leido=bl.ObtenerPorId(id);
            Verificar(leido.Marca.Descripcion=="Stanley" && leido.Categoria.Descripcion=="Termos", "Artículo asociado a la marca y categoría recién creadas");
            Verificar(bl.Buscar(new FiltroArticulo {IdMarca=marcaId,IdCategoria=categoriaId}).Single().Id==id,"Filtro por nueva marca y categoría");
            Verificar(leido.Nombre=="Prueba" && leido.Imagenes.Count==2 && leido.Precio==123.4567m,"Alta persistida con dos imágenes y cuatro decimales");
            var duplicado=new Articulo {Codigo=a.Codigo,Nombre=a.Nombre,Descripcion=a.Descripcion,Marca=a.Marca,Categoria=a.Categoria,Precio=a.Precio,Imagenes=a.Imagenes};
            Rechaza(()=>bl.Agregar(duplicado),"Código duplicado rechazado");
            a.Nombre="Prueba modificada"; a.Precio=999; a.Marca=new Marca {Id=2}; a.Imagenes.RemoveAt(0);
            bl.Modificar(a); leido=bl.ObtenerPorId(id);
            Verificar(leido.Nombre==a.Nombre && leido.Precio==999 && leido.Marca.Id==2 && leido.Imagenes.Count==1,"Modificación persistida, incluida la lista de imágenes");
            a.Imagenes.Clear(); Rechaza(()=>bl.Modificar(a),"No se permite guardar sin imágenes");
            Verificar(bl.ObtenerPorId(id).Imagenes.Count==1,"Una validación fallida conserva los datos guardados");
            a.Imagenes.Add(new Imagen {ImagenUrl="Imagenes/0123456789abcdef0123456789abcdef.png"});
            bl.Modificar(a);
            Verificar(bl.ObtenerPorId(id).Imagenes.Single().ImagenUrl==a.Imagenes[0].ImagenUrl,"Referencia local portable guardada y leída");
            a.Imagenes[0].ImagenUrl="Imagenes/../../secreto.png";
            Rechaza(()=>bl.Modificar(a),"No se admiten rutas fuera de la carpeta de fotos");
            a.Imagenes[0].ImagenUrl=@"C:\Users\alguien\Downloads\foto.png";
            Rechaza(()=>bl.Modificar(a),"No se guardan rutas absolutas de otra computadora");
            bl.Eliminar(id);
            Verificar(bl.ObtenerPorId(id)==null && bl.Buscar(new FiltroArticulo()).Count==5,"Eliminación persistida y registros originales conservados");
            using(var cn=datos.AbrirConexion()) using(var q=cn.CreateCommand()) {q.CommandText="SELECT COUNT(*) FROM IMAGENES WHERE IdArticulo="+id; Verificar((int)q.ExecuteScalar()==0,"Eliminación de las imágenes asociadas");}
            Console.WriteLine(pruebas+" comprobaciones correctas."); return 0;
        }
        catch(Exception ex) { Console.WriteLine(ex); return 1; }
        finally
        {
            SqlConnection.ClearAllPools();
            if(creado) using(var cn=new SqlConnection(master)) { cn.Open(); using(var q=cn.CreateCommand()) {q.CommandText="ALTER DATABASE ["+db+"] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE ["+db+"]"; q.ExecuteNonQuery();} }
        }
    }
}
