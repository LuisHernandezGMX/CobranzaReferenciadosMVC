using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using CobranzaReferenciadosMVC.Models.Entity;

namespace CobranzaReferenciadosMVC.Models.ViewModels.ExportarRecibos
{
    public enum TiposRecibo
    {
        Validados,
        NoValidados,
        Todos
    }

    public class BuscarRecibosViewModel
    {
        [Required(ErrorMessage = "Debe ingresar esta fecha.")]
        public DateTime? FechaInicial { get; set; }

        [Required(ErrorMessage = "Debe ingresar esta fecha.")]
        public DateTime? FechaFinal { get; set; }

        public TiposRecibo TiposRecibo { get; set; }

        public IEnumerable<ReciboPago> RecibosAExportar { get; set; }
    }
}