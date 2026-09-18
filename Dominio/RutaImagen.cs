using System.Text.RegularExpressions;
namespace Dominio
{
    public static class RutaImagen
    {
        // Se guarda una referencia portable, nunca C:\Users\... ni rutas arbitrarias.
        public static bool EsLocal(string valor)
        {
            return valor != null && Regex.IsMatch(valor, @"\AImagenes/[a-f0-9]{32}\.(jpg|jpeg|png|gif|bmp|webp)\z", RegexOptions.IgnoreCase);
        }
    }
}

