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
                }
            }

            return lineas.Select(linea => ParseLinea(linea));
        }

        /// <summary>
        /// Descarta la información innecesaria de la línea y deja
        /// solo lo necesario para el registro.
        /// </summary>
        /// <param name="linea">La línea leída del archivo.</param>
        /// <returns></returns>
        private RegistroViewModel ParseLinea(string linea)
        {
            var campos = linea.Split('\t');

            return LeerRegistro(campos);
        }

        /// <summary>
        /// Lee los campos del registro leído y regresa un objeto
        /// con sus campos correspondientes, aplicando las reglas necesarias para
        /// procesar la segunda referencia, el banco y la leyenda.
        /// </summary>
        /// <param name="campos">La línea leída del archivo separada en sus campos correspondientes.</param>
        /// <returns>Un nuevo objecto RegistroViewModel.</returns>
        private RegistroViewModel LeerRegistro(string[] campos)
        {
            var registro = new RegistroViewModel {
                Fecha = DateTime.Parse(campos[4]),
                Referencia1 = campos[5]?.Trim() ?? string.Empty,
                Monto = decimal.Parse(campos[6]),
                TipoMovimiento = campos[9]?.Trim() ?? string.Empty
            };

            if (registro.TipoMovimiento.Equals("TRANSFERENCIA INTERBANCARIA", StringComparison.InvariantCultureIgnoreCase)) {
                registro.Referencia2 = campos[10]?.Trim() ?? string.Empty;
                registro.Banco = campos[11]?.Trim() ?? string.Empty;
                registro.Leyenda = campos[13]?.Trim() ?? string.Empty;
            } else if (registro.TipoMovimiento.Equals("Deposito Efectivo", StringComparison.InvariantCultureIgnoreCase)) {
                registro.Referencia2 = campos[10]?.Trim() ?? string.Empty;
                registro.Banco = registro.Leyenda = string.Empty;
            } else if (registro.TipoMovimiento.Equals("ABONO POR TRANSF SPEI", StringComparison.InvariantCultureIgnoreCase)) {
                registro.Referencia2 = campos[11]?.Trim() ?? string.Empty;
                registro.Banco = campos[10]?.Trim() ?? string.Empty;
                registro.Leyenda = campos[13]?.Trim() ?? string.Empty;
            } else {
                registro.Referencia2 = registro.Banco = registro.Leyenda = string.Empty;
            }

            return registro;
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