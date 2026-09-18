using System;
using DAO;
using Dominio;
class PruebasValidacion
{
    static int cantidad;
    static void Rechaza<T>(string caso, Action accion) where T : Exception
    {
        try { accion(); throw new Exception("No rechazado: " + caso); }
        catch (T) { cantidad++; Console.WriteLine("OK: " + caso); }
    }
    static Articulo Nuevo()
    {
        var a = new Articulo { Codigo="A01", Nombre="Prueba", Descripcion="Detalle", Marca=new Marca {Id=1}, Categoria=new Categoria {Id=1}, Precio=15.25m };
        a.Imagenes.Add(new Imagen { ImagenUrl="https://example.com/a.jpg" });
        return a;
    }
    static int Main()
    {
        try {
            var datos = new AccesoDatos("Data Source=servidor-no-utilizado;Initial Catalog=pruebas;Integrated Security=True;Connect Timeout=1");
            var dao = new ArticuloDAO(datos);
            Rechaza<ArgumentNullException>("articulo nulo", () => dao.Agregar(null));
            Rechaza<ArgumentException>("codigo obligatorio", () => { var a=Nuevo(); a.Codigo=" "; dao.Agregar(a); });
            Rechaza<ArgumentException>("nombre largo", () => { var a=Nuevo(); a.Nombre=new string('x',51); dao.Agregar(a); });
            Rechaza<ArgumentException>("descripcion larga", () => { var a=Nuevo(); a.Descripcion=new string('x',151); dao.Agregar(a); });
            Rechaza<ArgumentException>("marca obligatoria", () => { var a=Nuevo(); a.Marca=null; dao.Agregar(a); });
            Rechaza<ArgumentException>("categoria obligatoria", () => { var a=Nuevo(); a.Categoria.Id=0; dao.Agregar(a); });
            Rechaza<ArgumentException>("precio negativo", () => { var a=Nuevo(); a.Precio=-1; dao.Agregar(a); });
            Rechaza<ArgumentException>("precision money", () => { var a=Nuevo(); a.Precio=0.00001m; dao.Agregar(a); });
            Rechaza<ArgumentException>("imagen obligatoria", () => { var a=Nuevo(); a.Imagenes.Clear(); dao.Agregar(a); });
            Rechaza<ArgumentException>("URL no permitida", () => { var a=Nuevo(); a.Imagenes[0].ImagenUrl="file:///C:/imagen.jpg"; dao.Agregar(a); });
            Rechaza<ArgumentException>("URL repetida", () => { var a=Nuevo(); a.Imagenes.Add(new Imagen {ImagenUrl=a.Imagenes[0].ImagenUrl}); dao.Agregar(a); });
            Rechaza<ArgumentException>("alta con Id", () => { var a=Nuevo(); a.Id=1; dao.Agregar(a); });
            Rechaza<ArgumentException>("modificacion sin Id", () => dao.Modificar(Nuevo()));
            Rechaza<ArgumentException>("rango invertido", () => dao.Buscar(new FiltroArticulo {PrecioMinimo=20,PrecioMaximo=10}));
            Rechaza<ArgumentException>("filtro de precio negativo", () => dao.Buscar(new FiltroArticulo {PrecioMinimo=-1}));
            Rechaza<ArgumentException>("Id de baja invalido", () => dao.Eliminar(0));
            Rechaza<ArgumentException>("marca vacia", () => new MarcaDAO(datos).Agregar(new Marca()));
            Rechaza<ArgumentException>("categoria vacia", () => new CategoriaDAO(datos).Agregar(new Categoria()));
            Rechaza<ArgumentException>("imagen sin articulo", () => new ImagenDAO(datos).Agregar(new Imagen {ImagenUrl="https://example.com/a.jpg"}));
            Console.WriteLine(cantidad + " validaciones correctas. Sin conexion a SQL Server.");
            return 0;
        } catch (Exception ex) { Console.WriteLine(ex); return 1; }
    }
}
