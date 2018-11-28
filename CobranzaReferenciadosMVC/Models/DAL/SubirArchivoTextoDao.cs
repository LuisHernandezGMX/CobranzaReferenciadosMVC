using System.Linq;
using System.Collections.Generic;
using CobranzaReferenciadosMVC.Models.Entity;

namespace CobranzaReferenciadosMVC.Models.DAL
{
    public static class SubirArchivoTextoDao
    {
        /// <summary>
        /// Inserta todos los registros indicados en la tabla [ReciboPago] e indica la cantidad de registros validados, no validados
        /// y duplicados.
        /// </summary>
        /// <param name="registros">La lista de registros a insertar.</param>
        /// <param name="validados">Guarda la cantidad de registros que fueron validados.</param>
        /// <param name="sinValidar">Guarda la cantidad de registros que no fueron validados.</param>
        /// <param name="duplicados">Guarda la cantidad de registros que ya existían en la BD y por lo tanto no fueron insertados.</param>
        public static void InsertarRegistrosEnDB(IEnumerable<ReciboPago> registros, ref int validados, ref int sinValidar, ref int duplicados)
        {
            using (var db = new SCVEntities())
            using (var transaction = db.Database.BeginTransaction()) {
                db.Configuration.EnsureTransactionsForFunctionsAndCommands = false;

                try {
                    foreach (var registro in registros) {
                        bool validado = false;
                        string referencia = null;
                        int longitudReferenciaRequerida = 5;

                        // Nos aseguramos de que el registro no haya sido insertado previamente en la base de datos.
                        // Se compara la Fecha, el Monto, la Referencia 1 y la Referencia 2
                        bool registroExistente = db.ReciboPago.Any(rp =>
                            rp.Fecha == registro.Fecha
                            && rp.Monto == registro.Monto
                            && rp.Referencia1 == registro.Referencia1
                            && rp.Referencia2 == registro.Referencia2);

                        if (!registroExistente) {
                            // Buscamos en la tabla [Referencia_Fovi] por medio de las dos referencias del registro
                            bool existeReferencia1 =
                                !string.IsNullOrWhiteSpace(registro.Referencia1)              // No se aceptan referencias inexistentes...
                                && registro.Referencia1.Length >= longitudReferenciaRequerida // ... ni cuya longitud sea menor a 5 (valor arbitrario; puede ser modificado)
                                && db.Referencia_Fovi
                                    .Any(rf => rf.No_Referencia.Contains(registro.Referencia1) && rf.Estatus_Pago == "0");

                            if (existeReferencia1) {
                                validado = true;
                                referencia = registro.Referencia1;
                            } else {
                                bool existeReferencia2 =
                                    !string.IsNullOrWhiteSpace(registro.Referencia2)
                                    && registro.Referencia2.Length >= longitudReferenciaRequerida
                                    && db.Referencia_Fovi
                                        .Any(rf => rf.No_Referencia.Contains(registro.Referencia2) && rf.Estatus_Pago == "0");

                                if (existeReferencia2) {
                                    validado = true;
                                    referencia = registro.Referencia2;
                                }
                            }

                            // Agregamos los registros encontrados por su número de referencia y su prima total calculada.
                            if (validado) {
                                var calculado = db.CalcularPrimaTotalPorReferencia(referencia).FirstOrDefault();

                                // Si la prima total calculada es igual al monto del registro, actualizamos tablas [Referencia_Fovi] y [ReciboPago]
                                if (calculado != null && calculado.PrimaTotal == registro.Monto) {
                                    db.UpdPagoReferenFoviDate(calculado.NoReferencia, "proceso", registro.FechaProceso);
                                    registro.Validado = true;

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

                                        registro.RFC = polizaFactura[0].RFC;
                                        registro.CodRamoNroPol = $"{polizaFactura[0].Cod_ramo}-[{string.Join(",", numerosPoliza)}]";
                                    }

                                    validados++;
                                } else {
                                    sinValidar++;
                                }
                            } else {
                                sinValidar++;
                            }

                            db.ReciboPago.Add(registro);
                        } else {
                            duplicados++;
                        }
                    }

                    db.SaveChanges();
                    transaction.Commit();
                } catch {
                    transaction.Rollback();
                    throw;
                }
            }
        }
    }
}