using System.Linq;
using System.Text;

namespace CobranzaReferenciadosMVC.Models.Extensions
{
    public static class StringExtensions
    {
        /// <summary>
        /// Elimina las letras de la cadena indicada y deja solamente los caracteres numéricos.
        /// </summary>
        /// <param name="cadena">La cadena a filtrar.</param>
        /// <returns></returns>
        public static string EliminarLetras(this string cadena)
        {
            var builder = new StringBuilder();
            var filtrada = cadena
                .Where(c => char.IsDigit(c));

            foreach (var c in filtrada) {
                builder.Append(c);
            }

            return builder.ToString();
        }
    }
}