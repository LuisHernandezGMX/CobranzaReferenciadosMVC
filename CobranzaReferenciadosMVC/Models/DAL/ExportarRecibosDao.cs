using System.Linq;
using System.Collections.Generic;
using CobranzaReferenciadosMVC.Models.Entity;
using CobranzaReferenciadosMVC.Models.ViewModels.ExportarRecibos;

namespace CobranzaReferenciadosMVC.Models.DAL
{
    public class ExportarRecibosDao
    {
        /// <summary>
        /// Regresa todos los recibos de pago que se encuentren dentro del rango de fechas
        /// indicado y que estén validados de acuerdo a la enumeración TiposRecibo.
        /// </summary>
        /// <param name="model">El vista modelo obtenido desde el cliente.</param>
        /// <returns></returns>
        public static IEnumerable<ReciboPago> BuscarRecibosPorRango(BuscarRecibosViewModel model)
        {
            using (var db = new SCVEntities()) {
                var fechaFinalExclusiva = model.FechaFinal?.AddDays(1); // Agrega un día adicional, para emular el comportamiento de BETWEEN. 
                var query = db.ReciboPago
                    .Where(recibo => recibo.Fecha >= model.FechaInicial && recibo.Fecha < fechaFinalExclusiva);

                switch (model.TiposRecibo) {
                    case TiposRecibo.Todos:
                        return query.ToList();

                    case TiposRecibo.Validados:
                        return query.Where(recibo => recibo.Validado).ToList();

                    default:
                        return query.Where(recibo => !recibo.Validado).ToList();
                }
            }
        }
    }
}