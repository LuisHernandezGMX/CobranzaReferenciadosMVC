// [IsPost] es el valor traído de ASP.NET por medio de la propiedad <WebPageRenderingBase.IsPost>
// [DTSpanishLanguageUrl] es la ruta del JSON necesario para traducir al español los controles del Data Table.

(function (IsPost, DTSpanishLanguageUrl) {
    // Inicialización de los DateTimePickers
    $("#fechaInicialGroup").datetimepicker({
        format: "YYYY-MM-DD",
    });

    $("#fechaFinalGroup").datetimepicker({
        format: "YYYY-MM-DD"
    });

    // Sincronizar los rangos mínimos y máximos de las fechas
    $("#fechaInicialGroup").on("dp.change", function (ev) {
        $("#fechaFinalGroup").data("DateTimePicker").minDate(ev.date);
    });

    $("#FechaFinalGroup").on("dp.change", function (ev) {
        $("#FechaInicialGroup").data("DateTimePicker").maxDate(ev.date);
    });

    if (IsPost) {
        // Inicialización del DataTable
        $("#tablaRecibos").DataTable({
            scrollX: true,
            dom: "lBfrtip",
            buttons: ["copy", "excel"],
            language: {
                url: DTSpanishLanguageUrl
            }
        });

        // Estilos personalizados para los botones de exportación
        $("#tablaRecibos").on("init.dt", function () {
            $(".buttons-html5").removeClass("btn-default").addClass("btn-danger");
            $(".buttons-copy").prepend('<span class="glyphicon glyphicon-copy"></span> ');
            $(".buttons-excel").prepend('<span class="fas fa-file-excel"></span> ');
        });
    }
})(IS_POST, DATATABLES_SPANISH_LANG);