function enviarValSelect(idi, idf, attrf) {
    $("#" + idf).val($("#" + idi + " option:selected").attr(attrf));
}

function datosTraslado(orden) {
    $('#contenido-productos' + orden).html('<img src="/imagenes/index/ReportesDigemid/cargando.gif" />');
    var parametros = {
        "guia": $('#DetRRU1' + orden + 'Guia').val(),
        "orden": orden,
        "Origen": $("#Origen").val()
    };
    $.ajax('/Rutas/infoDatosTraslado',
        {
            data: parametros,
            dataType: 'html',
            cache: false,
            type: 'post',
        })
        .done(function (response) {
            $('#contenido-productos' + orden).html(response);
    
        });
}

function actualizarCajas() {
    var docEntry = $("#doc_entry").val();

    if (typeof docEntry === "undefined" || docEntry === "" || docEntry === null) {

        var totalCajaMaster = 0 * 1;
        var totalCajas = 0 * 1;
        var i = 0;
        var j = 0;

        while ($("#Linea" + j).val() > 0) {
            while ($("#ldrr" + j + "rru" + i).val() > 0) {
                if ($("#ldrr" + j + "rruC" + i).val() > 0) {
                    totalCajaMaster += $("#ldrr" + j + "rruC" + i).val() * 1;
                }
                i++;
            }
            $("#DetRRU1" + j + "Cajas").val(totalCajaMaster);
            totalCajas += totalCajaMaster * 1
            totalCajaMaster = 0; i = 0;
            j++;
        }
        $("#TotalCajas").val(totalCajas);

    } else { return null; }
}

function validar() {
    var form = $("#form");
    $.ajax('/Rutas/validarNuevaHojaDeRepartoOTransferencia', {
        data: form.serialize(),
        dataType: 'html',
        cache: false,
        type: 'post'
    })
        .done(function (response) {
            Swal.fire(response);
        });
}

function listarGuias() {
    $(".select-guias").html();
    var htm = "<option value='' DocNum='' NumAtCard=''>Seleccione</option>";
    var parametros = {
        "Origen": $("#Origen").val()
    };
    $.ajax('/Rutas/infoGuiasTransferencia',
        {
            data: parametros,
            dataType: 'json',
            cache: false,
            type: 'post',
        })
        .done(function (response) {
            $.each(response, function (key, value) {
                htm += "<option value='" + value.NumAtCard + "' DocNum='" + value.DocNum + "' SlpName='" + value.SlpName + "'>" + value.NumAtCard + "</option>";

            });
            $(".select-guias").html(htm);
        })
}

/*funciones de edicion*/
function listarDetalleGuia(baseEntry, linea, idTR, btnEditarDetalle) {
  
    $.ajax({
        url: '/Rutas/ObtenerDetalleOrdenRuta',
        type: 'POST',
        data: { BaseEntry: baseEntry, BaseLinea: linea, HabilitarBotonEditar: btnEditarDetalle },
    }).done(function (response) {
        Swal.fire(response.mensaje);
        $('#baselinea_RRU11').val(linea);
        if (response.existeDatos) {
            $(".cls-colorFilaGuia").css('background-color', 'white');
            $("#" + idTR).css('background-color', 'yellow');
        }
        $("#tbl_detRRu11 > tbody").html(response.lista);
    });
}

function editarDetalle() {
    $.ajax({
        url: '/Rutas/EditarDetalleOrdenRuta',
        type: 'POST',
        data: $('#frm_detalle').serialize() + '&BaseEntry=' + $('#doc_entry').val() + '&BaseLinea=' + $('#baselinea_RRU11').val(),
        dataType: 'JSON',
    }).done(function (response) {
        Swal.fire({
            confirmButtonColor: '#26653D',
            closeOnCancel: false,
            allowOutsideClick: false,
            showCancelButton: false,
            text: response.mensaje,
        }).then((result) => {
            if (result.isConfirmed && response.nrocajas == true) {
                location.reload();
            }
        })

    });
}

function validarEnviarFormulario(estado) {
    var object = $("#form").serialize();
    if (estado === 'CREADO') {
        $.ajax('/Rutas/validarDatosEncabezadoRuta',
            {
                data: object,
                dataType: 'html',
                cache: false,
                type: 'post'
            })
            .done(function (response) {
                if (response != "OK") { swal.fire({ title: response, text: "Presione OK para continuar", icon: "warning" }); return false }
                else { $("#form").submit(); }
            });
    } else {
        $.ajax('/Rutas/validarNuevaHojaDeRepartoOTransferencia',
            {
                data: object,
                dataType: 'html',
                cache: false,
                type: 'post'
            })
            .done(function (response) {
                if (response != "OK") { swal.fire({ title: response, text: "Presione OK para continuar", icon: "warning" }); return false }
                else { $("#form").submit(); }
            });
    }
}