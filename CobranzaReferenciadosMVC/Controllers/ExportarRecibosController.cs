using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CobranzaReferenciadosMVC.Models.DAL;
using CobranzaReferenciadosMVC.Models.ViewModels.ExportarRecibos;

namespace CobranzaReferenciadosMVC.Controllers
{
    public class ExportarRecibosController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult BuscarRecibosAExportar([Bind(Include = "FechaInicial,FechaFinal,TiposRecibo")]BuscarRecibosViewModel model)
        {
            if (ModelState.IsValid) {
                model.RecibosAExportar = ExportarRecibosDao.BuscarRecibosPorRango(model);
            }

            return View("Index", model);
        }
    }
}