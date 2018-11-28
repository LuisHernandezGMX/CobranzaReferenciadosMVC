using System.Web.Mvc;
using CobranzaReferenciadosMVC.Models.DAL;

namespace CobranzaReferenciadosMVC.Controllers
{
    public class ActualizarReferenciaController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            var recibos = ActualizarReferenciaDao.ObtenerRecibosNoValidados();

            return View(recibos);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult NuevaReferencia(string nuevaReferencia, int? idRecibo)
        {
            var message = ActualizarReferenciaDao.ActualizarReferenciaExistente(nuevaReferencia, idRecibo);

            return Json(message);
        }
    }
}