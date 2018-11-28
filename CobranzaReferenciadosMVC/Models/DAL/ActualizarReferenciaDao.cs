using System;
using System.Linq;
using System.Collections.Generic;
using CobranzaReferenciadosMVC.Models.Entity;
using CobranzaReferenciadosMVC.Models.Messages;

namespace CobranzaReferenciadosMVC.Models.DAL
{
    public class ActualizarReferenciaDao
    {
        /// <summary>
        /// Regresa todos recibos de pago que no hayan sido validados aún.
        /// </summary>
        /// <returns>Una lista con los recibos de pago no validados.</returns>
        public static IEnumerable<ReciboPago> ObtenerRecibosNoValidados()
        {
            using (var db = new SCVEntities()) {
                return db.ReciboPago
                    .Where(rp => !rp.Validado)
                    .ToList();
            }
        }

        /// <summary>
        /// Actualiza la referencia del registro indicado y regresa un mensaje
        /// con el resultado de la operación.
        /// </summary>
        /// <param name="nuevaReferencia">El valor de la nueva referencia.</param>
        /// <param name="idRecibo">El Id del recibo a actualizar.</param>
        /// <returns>Un mensaje con el resultado de la actualización.</returns>
        public static Message ActualizarReferenciaExistente(string nuevaReferencia, int? idRecibo)
        {
            if (string.IsNullOrWhiteSpace(nuevaReferencia)) {
                return new Message(false, "Debe ingresar la nueva referencia.");
            }

            using (var db = new SCVEntities()) {
                db.Configuration.EnsureTransactionsForFunctionsAndCommands = false;
                
                if (!db.ReciboPago.Any(rp => rp.Id == idRecibo)) {
                    return new Message(false, $"El recibo de pago con Id = {idRecibo?.ToString() ?? "NULL"} no existe. Por favor, verifique de nuevo.");
                }

                using (var transaction = db.Database.BeginTransaction()) {
                    try {
                        var reciboPago = db.ReciboPago.Find(idRecibo);
                        reciboPago.Referencia1 = nuevaReferencia;

                        bool validado = false;
                        string referencia = null;
                        int longitudReferenciaRequerida = 5;

                        // Buscamos en la tabla [Referencia_Fovi] por medio de las dos referencias del registro
                        bool existeReferencia1 =
                            !string.IsNullOrWhiteSpace(reciboPago.Referencia1)              // No se aceptan referencias inexistentes...
                            && reciboPago.Referencia1.Length >= longitudReferenciaRequerida // ... ni cuya longitud sea menor a 5 (valor arbitrario; puede ser modificado)
                            && db.Referencia_Fovi
                                .Any(rf => rf.No_Referencia.Contains(reciboPago.Referencia1) && rf.Estatus_Pago == "0");

                        if (existeReferencia1) {
                            validado = true;
                            referencia = reciboPago.Referencia1;
                        } else {
                            bool existeReferencia2 =
                                !string.IsNullOrWhiteSpace(reciboPago.Referencia2)
                                && reciboPago.Referencia2.Length >= longitudReferenciaRequerida
                                && db.Referencia_Fovi
                                    .Any(rf => rf.No_Referencia.Contains(reciboPago.Referencia2) && rf.Estatus_Pago == "0");

                            if (existeReferencia2) {
                                validado = true;
                                referencia = reciboPago.Referencia2;
                            }
                        }

                        // Si no se encuentra la nueva referencia, cancelar la actualización.
                        if (!validado) {
                            transaction.Rollback();
                            return new Message(false, $"El registro no fue actualizado debido a que no se encontró la referencia indicada.");
                        }

                        // Verificamos que coincidan los montos del registro y de la prima total.
                        var calculado = db.CalcularPrimaTotalPorReferencia(referencia).FirstOrDefault();

                        if (calculado != null && calculado.PrimaTotal == reciboPago.Monto) {
                            db.UpdPagoReferenFoviDate(calculado.NoReferencia, "proceso", reciboPago.FechaProceso);
                            reciboPago.Validado = true;

                            // Agregamos el RFC, el código de ramo y el número de póliza obtenidos de [Poliza_Factura]
                            var polizaFactura = db.Poliza_Factura
                                .Where(pf =>
                                    pf.Cod_ramo == "102"
                                    && pf.TipoPoliza == "10"
                                    && pf.RFC == db.Referencia_Fovi.FirstOrDefault(rf =>
                                        rf.No_Referencia == calculado.NoReferencia).User_Reg)
                                .ToList();

                            if (polizaFactura.Count > 0) {
                                var numerosPoliza = polizaFactura.Select(pf => pf.nro_pol);

                                reciboPago.RFC = polizaFactura[0].RFC;
                                reciboPago.CodRamoNroPol = $"{polizaFactura[0].Cod_ramo}-[{string.Join(",", numerosPoliza)}]";
                            }

                            db.SaveChanges();
                            transaction.Commit();

                            return new Message(true, "La referencia se ha actualizado con éxito.");
                        } else {
                            transaction.Rollback();
                            return new Message(false, "El registro no fue actualizado debido a que la prima total no coincide con el monto registrado de la referencia indicada.");
                        }
                    } catch (Exception e) {
                        transaction.Rollback();
                        return new Message(false, $"Ocurrió un error al actualizar la referencia de este registro.\nERROR: <pre>{e}</pre>");
                    }
                }
            }
        }
    }
}