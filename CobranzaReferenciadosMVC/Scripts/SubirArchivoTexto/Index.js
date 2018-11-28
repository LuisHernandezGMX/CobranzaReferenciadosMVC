// [PrevisualizarArchivoUrl] es la ruta hacia el action para previsualizar el archivo de texto en forma de un DataTable.
// [SubirArchivoUrl] es la ruta hacia el action para subir el archivo de texto al servidor.

(function (PrevisualizarArchivoUrl, SubirArchivoUrl) {
    function LimpiarPantalla() {
        // Tabla
        $("#previewRegistros").empty();

        // Barra de progreso
        $("#barraProgreso").css("width", "0%");

        // Mensaje de barra de progreso
        $("#mensajeBarraProgreso").text("");

        // Botón de subir archivo
        $("#btnSubirArchivo").prop("disabled", true);
    }

    $("#archivo").change(function () {
        LimpiarPantalla();

        var fileInput = $(this);
        var btnLimpiarPantalla = $("#btnLimpiarPantalla");

        fileInput.prop("disabled", true);
        btnLimpiarPantalla.prop("disabled", true);

        var formData = new FormData();
        formData.append("archivo", this.files[0]);

        $.ajax({
            url: PrevisualizarArchivoUrl,
            type: "POST",
            data: formData,
            processData: false,
            contentType: false,
            success: function (response) {
                if (response.Status) {
                    $("#previewRegistros").append(response.Mensaje);
                    $("#btnSubirArchivo").prop("disabled", false);
                } else {
                    BootstrapDialog.show({
                        title: "Error al Leer Archivo",
                        type: BootstrapDialog.TYPE_DANGER,
                        message: response.Mensaje,
                        closable: false,
                        buttons: [{
                            label: "Cerrar",
                            cssClass: "btn-danger",
                            action: function (dialog) {
                                $("#btnLimpiarPantalla").click();
                                dialog.close();
                            }
                        }]
                    });
                }

                fileInput.prop("disabled", false);
                btnLimpiarPantalla.prop("disabled", false);
            }
        });
    });

    $("#btnSubirArchivo").click(function () {
        var btnSubirArchivo = $(this);
        var btnLimpiarPantalla = $("#btnLimpiarPantalla");
        var fileInput = $("#archivo");

        btnSubirArchivo.prop("disabled", true);
        btnLimpiarPantalla.prop("disabled", true);
        fileInput.prop("disabled", true);
        $("#barraProgreso").css("width", "100%");
        $("#mensajeBarraProgreso").text("Subiendo archivo al sistema. Un momento por favor...");

        var formData = new FormData();
        formData.append("archivo", fileInput[0].files[0]);

        $.ajax({
            url: SubirArchivoUrl,
            type: "POST",
            data: formData,
            processData: false,
            contentType: false,
            success: function (response) {
                if (response.Status) {
                    BootstrapDialog.show({
                        title: "Archivo Subido con Éxito",
                        type: BootstrapDialog.TYPE_PRIMARY,
                        message: response.Mensaje,
                        closable: false,
                        buttons: [{
                            label: "Aceptar",
                            cssClass: "btn-primary",
                            action: function (dialog) {
                                $("#btnLimpiarPantalla").click();
                                dialog.close();
                            }
                        }]
                    });
                } else {
                    BootstrapDialog.show({
                        title: "Error al Subir Archivo",
                        type: BootstrapDialog.TYPE_DANGER,
                        message: response.Mensaje,
                        closable: false,
                        buttons: [{
                            label: "Cerrar",
                            cssClass: "btn-danger",
                            action: function (dialog) {
                                $("#btnLimpiarPantalla").click();
                                dialog.close();
                            }
                        }]
                    });
                }

                fileInput.prop("disabled", false);
                btnLimpiarPantalla.prop("disabled", false);
            }
        });
    });

    $("#btnLimpiarPantalla").click(function () {
        // Limpiar el File Input
        $("#archivo").val("");
        LimpiarPantalla();
    });
})(PREVISUALIZAR_ARCHIVO_URL, SUBIR_ARCHIVO_URL);