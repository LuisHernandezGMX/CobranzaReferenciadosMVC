using System;
using System.Web;
using System.Linq;
using System.Web.Mvc;
using CobranzaReferenciadosMVC.Models.DAL;
using CobranzaReferenciadosMVC.Models.Entity;
using CobranzaReferenciadosMVC.Models.Messages;
using CobranzaReferenciadosMVC.Models.Extensions;
using CobranzaReferenciadosMVC.Models.Business.SubirArchivoTexto;

namespace CobranzaReferenciadosMVC.Controllers
{
    public class SubirArchivoTextoController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult PrevisualizarArchivo(HttpPostedFileBase archivo)
        {
            var message = UtilityMethods.ProcesarRegistros(archivo, arch => {
                var registros = new LectorArchivo(arch).LeerArchivo();
                var html = UtilityMethods.ToHtmlString(registros);

                return new Message(true, html);
            });

            return Json(message);
        }

        [HttpPost]
        public JsonResult SubirArchivo(HttpPostedFileBase archivo)
        {
            var message = UtilityMethods.ProcesarRegistros(archivo, arch => {
                var registros = new LectorArchivo(arch)
                .LeerArchivo()
                .Select(registro => new ReciboPago {
                    Fecha = registro.Fecha,
                    Referencia1 = registro.Referencia1.EliminarLetras(),
                    Referencia2 = registro.Referencia2.EliminarLetras(),
                    Monto = registro.Monto,
                    TipoMovimiento = registro.TipoMovimiento,
                    Banco = registro.Banco,
                    Leyenda = registro.Leyenda,
                    FechaProceso = DateTime.Now,
                    Validado = false
                });

                int validados = 0;
                int sinValidar = 0;
                int duplicados = 0;

                SubirArchivoTextoDao.InsertarRegistrosEnDB(registros, ref validados, ref sinValidar, ref duplicados);

                return new Message(true, "Todos los registros se procesaron con éxito.\n\n"
                    + $"Total de registros: {registros.Count()}\n"
                    + $"Total de registros validados: {validados}\n"
                    + $"Total de registros sin validar: {sinValidar}\n"
                    + $"Total de registros duplicados: {duplicados}");
            });

            return Json(message);
        }
    }
}