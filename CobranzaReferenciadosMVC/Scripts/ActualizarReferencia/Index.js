// [DTSpanishLanguageUrl] es la ruta del JSON necesario para traducir al español los controles del Data Table.
// [NuevaReferenciaUrl] es la ruta del Action que se invocará por medio de AJAX para actualizar la referencia.

(function (DTSpanishLanguageUrl, NuevaReferenciaUrl) {
    // Inicialización del DataTable
    var tableRegistros = $("#tableRegistros").DataTable({
        scrollX: true,
        select: {
            style: "single",
            info: false
        },
        language: {
            url: DTSpanishLanguageUrl
        }
    });

    // Mantener seleccionada siempre la primera fila de la tabla.
    $("#tableRegistros").on("init.dt", function () {
        tableRegistros.row().select();
    });

    // Botón para actualizar la referencia por medio de AJAX.
    $("#btnActualizarReferencia").click(function () {
        var idRecibo = tableRegistros.row(".selected").data()[0];

        BootstrapDialog.show({
            title: "Ingresar Nueva Referencia",
            message: 'Ingrese la nueva referencia para el registro con <b>ID = ' + idRecibo + '</b>:<br><br> <input type="text" class="form-control" />',
            type: BootstrapDialog.TYPE_PRIMARY,
            closable: false,
            buttons: [{
                label: "Cancelar",
                cssClass: "btn-danger",
                action: function (dialog) {
                    dialog.close();
                }
            }, {
                label: "Actualizar",
                cssClass: "btn-primary",
                action: function (dialog) {
                    var nuevaReferencia = dialog.getModalBody().find("input").val();
                    var token = document.getElementsByName("__RequestVerificationToken")[0].value

                    $.post(NuevaReferenciaUrl, {
                        idRecibo: idRecibo,
                        nuevaReferencia: nuevaReferencia,
                        __RequestVerificationToken: token
                    }, function (response) {
                        if (response.Status) {
                            BootstrapDialog.show({
                                title: "Referencia Actualizada",
                                message: response.Mensaje,
                                type: BootstrapDialog.TYPE_PRIMARY,
                                closable: false,
                                buttons: [{
                                    label: "Aceptar",
                                    cssClass: "btn-primary",
                                    action: function (innerDialog) {
                                        window.location.reload(true);
                                    }
                                }]
                            });
                        } else {
                            BootstrapDialog.show({
                                title: "Referencia No Actualizada",
                                message: response.Mensaje,
                                type: BootstrapDialog.TYPE_DANGER,
                                closable: false,
                                buttons: [{
                                    label: "Cerrar",
                                    cssClass: "btn-danger",
                                    action: function (innerDialog) {
                                        innerDialog.close();
                                    }
                                }]
                            });
                        }
                    });
                }
            }]
        });
    });
})(DATATABLES_SPANISH_LANG, RUTA_NUEVA_REFERENCIA);