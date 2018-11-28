using System;
using System.IO;
using System.Web;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using CobranzaReferenciadosMVC.Models.Messages;
using CobranzaReferenciadosMVC.Models.ViewModels.SubirArchivoTexto;

namespace CobranzaReferenciadosMVC.Models.Business.SubirArchivoTexto
{
    /// <summary>
    /// Lee el archivo indicado y regresa la información requerida. Esta información es:
    /// * Fecha, Referencia1, Monto, Tipo de Movimiento, Referencia 2, Banco y Leyenda *
    /// </summary>
    public class LectorArchivo
    {
        private HttpPostedFileBase archivo;

        public LectorArchivo(HttpPostedFileBase archivo)
        {
            this.archivo = archivo;
        }

        /// <summary>
        /// Regresa una lista de registros leídos del archivo de texto.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<RegistroViewModel> LeerArchivo()
        {
            var lineas = new List<string>();

            using (var memStream = new MemoryStream()) {
                archivo.InputStream.CopyTo(memStream);
                memStream.Position = 0;

                using (var reader = new StreamReader(memStream)) {
                    string linea;

                    do {
                        linea = reader.ReadLine();

                        if (!linea?.Contains("Cargo") ?? false) {
                            lineas.Add(linea);
                        }

                    } while (linea != null);

                    /* MODIFICACIÓN (10/10/2018)
                     * 
                     *  
                    while ((linea = reader.ReadLine()) != null) {
                        lineas.Add(linea);
                    }

                    MODIFICACIÓN */
                }
            }

            return lineas.Select(linea => ParseLinea(linea));

            /* MODIFICACIÓN (10/10/2018): Debido a que el formato
             * de los recibos de pago cambió a partir de esta fecha,
             * el parsing también cambiará. El código debajo es el código
             * que se utilizaba para analizar los recibos de pago antes de
             * la fecha mencionada. No modificar, ya que el formato de los
             * recibos podría cambiar nuevamente sin previo aviso. 

            // Siempre se omite la última línea
            lineas.RemoveAt(lineas.Count() - 1);

            return lineas.Select(linea => ParseLinea(linea));

            MODIFICACIÓN  */
        }

        /// <summary>
        /// Descarta la información innecesaria de la línea y deja
        /// solo lo necesario para el registro.
        /// </summary>
        /// <param name="linea">La línea leída del archivo.</param>
        /// <returns></returns>
        private RegistroViewModel ParseLinea(string linea)
        {
            /*  MODIFICACIÓN (10/10/2018)
             * 
            var campos = linea.Split('\t');

            return new RegistroViewModel {
                Fecha = DateTime.Parse(campos[4]),
                Referencia1 = campos[5] ?? "",
                Referencia2 = campos[10] ?? "",
                Monto = decimal.Parse(campos[6]),
                TipoMovimiento = campos[9] ?? "",
                Banco = campos[11] ?? "",
                Leyenda = campos[13] ?? ""
            };

            MODIFICACIÓN*/

            var campos = linea.Split('\t');

            return new RegistroViewModel {
                Fecha = DateTime.Parse(campos[4]),
                Referencia1 = campos[3]?.Trim() ?? string.Empty,
                Referencia2 = campos[5]?.Trim() ?? string.Empty,
                Monto = decimal.Parse(campos[6]),
                TipoMovimiento = campos[9]?.Trim() ?? string.Empty,
                Banco = campos[10]?.Trim() ?? string.Empty,
                Leyenda = campos[13]?.Trim() ?? string.Empty
            };

        }
    }

    /// <summary>
    /// Contiene métodos de utilería para el controlador SubirArchivoTexto.
    /// </summary>
    public static class UtilityMethods
    {
        /// <summary>
        /// Regresa los registros indicados ordenados en filas y columnas HTML (TR y TD).
        /// </summary>
        /// <param name="registros">La lista de registros a convertir.</param>
        /// <returns></returns>
        public static string ToHtmlString(IEnumerable<RegistroViewModel> registros)
        {
            var builder = new StringBuilder();

            foreach (var registro in registros) {
                builder
                    .Append("<tr><td>")
                    .Append(registro.Fecha)
                    .Append("</td><td>")
                    .Append(registro.Referencia1)
                    .Append("</td><td>")
                    .Append(registro.Referencia2)
                    .Append("</td><td>")
                    .Append(registro.Monto)
                    .Append("</td><td>")
                    .Append(registro.TipoMovimiento)
                    .Append("</td><td>")
                    .Append(registro.Banco)
                    .Append("</td><td>")
                    .Append(registro.Leyenda)
                    .Append("</td></tr>");
            }

            return builder.ToString();
        }

        /// <summary>
        /// Toma el archivo subido desde el cliente, procesa la información contenida en él y regresa un mensaje de acuerdo
        /// al resultado. Si el archivo es nulo, no compatible con "text/plain" u alguna excepción ocurre regresa un mensaje de error. Si
        /// el procesamiento se hace de manera correcta regresa un mensaje exitoso.
        /// </summary>
        /// <param name="archivo">El archivo subido desde el cliente.</param>
        /// <param name="procesamiento">Función para procesar el archivo subido. El mensaje que regresa esta función es el mensaje que
        /// regresará ProcesarRegistros() si termina su ejecución sin errores.</param>
        /// <returns></returns>
        public static Message ProcesarRegistros(HttpPostedFileBase archivo, Func<HttpPostedFileBase, Message> procesamiento)
        {
            if (archivo == null)
                return new Message(false, "Por favor, seleccione un archivo para subir.");

            if (archivo.ContentType != "text/plain")
                return new Message(false, "Solo se aceptan archivos de texto plano (.txt).");

            try {
                return procesamiento.Invoke(archivo);
            } catch (Exception e) {
                return new Message(false, $"Ocurrió un error al leer el archivo.\n\nERROR:<pre>{e}</pre>");
            }
        }
    }
}