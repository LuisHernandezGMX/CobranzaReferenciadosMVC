using System;

namespace CobranzaReferenciadosMVC.Models.ViewModels.SubirArchivoTexto
{
    /// <summary>
    /// Un registro leído del archivo de texto.
    /// </summary>
    public class RegistroViewModel
    {
        public DateTime Fecha { get; set; }
        public string Referencia1 { get; set; }
        public string Referencia2 { get; set; }
        public decimal Monto { get; set; }
        public string TipoMovimiento { get; set; }
        public string Banco { get; set; }
        public string Leyenda { get; set; }
    }
}