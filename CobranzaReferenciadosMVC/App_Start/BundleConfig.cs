using System.Web.Optimization;

namespace CobranzaReferenciadosMVC
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            /**************************************** APLICACIÓN GENERAL ****************************************/
            bundles.Add(new ScriptBundle("~/bundles/general").Include(
                "~/Scripts/jquery-{version}.js",
                "~/Scripts/bootstrap.js",
                "~/Scripts/fontawesome/all.js",
                "~/Scripts/respond.js",
                "~/Scripts/bootstrap-dialog.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                "~/Content/bootstrap.css",
                "~/Content/fontawesome/all.css",
                "~/Content/bootstrap-dialog.css",
                "~/Content/site.css"));
            /**************************************** APLICACIÓN GENERAL ****************************************/


            /**************************************** CONTROLLER: SubirArchivoTexto ****************************************/
            bundles.Add(new ScriptBundle("~/bundles/SubirArchivoTexto/Index").Include(
                "~/Scripts/SubirArchivoTexto/Index.js"));
            /**************************************** CONTROLLER: SubirArchivoTexto ****************************************/


            /**************************************** CONTROLLER: ActualizarReferencia ****************************************/
            bundles.Add(new ScriptBundle("~/bundles/ActualizarReferencia/Index").Include(
                "~/Scripts/datatables.js",
                "~/Scripts/ActualizarReferencia/Index.js"));

            bundles.Add(new StyleBundle("~/Content/ActualizarReferencia/Index").Include(
                "~/Content/datatables.css"));
            /**************************************** CONTROLLER: ActualizarReferencia ****************************************/


            /**************************************** CONTROLLER: ExportarRecibos ****************************************/
            bundles.Add(new ScriptBundle("~/bundles/ExportarRecibos/Index").Include(
                "~/Scripts/moment.min.js",
                "~/Scripts/bootstrap-datetimepicker.js",
                "~/Scripts/datatables.js",
                "~/Scripts/ExportarRecibos/Index.js"));

            bundles.Add(new StyleBundle("~/Content/ExportarRecibos").Include(
                "~/Content/bootstrap-datetimepicker.css",
                "~/Content/datatables.css"));
            /**************************************** CONTROLLER: ExportarRecibos ****************************************/
        }
    }
}
