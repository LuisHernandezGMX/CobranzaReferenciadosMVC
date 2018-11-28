using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CobranzaReferenciadosMVC.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult SubirArchivoTexto()
        {
            return RedirectToAction("Index", "SubirArchivoTexto");
        }

        [HttpGet]
        public ActionResult ActualizarReferencia()
        {
            return null;
        }
    }
}