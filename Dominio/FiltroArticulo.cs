namespace Dominio
{
   
    public class FiltroArticulo
    {
        public string Texto { get; set; }
        public string Codigo { get; set; }
        public int? IdMarca { get; set; }
        public int? IdCategoria { get; set; }
        public decimal? PrecioMinimo { get; set; }
        public decimal? PrecioMaximo { get; set; }
    }
}
