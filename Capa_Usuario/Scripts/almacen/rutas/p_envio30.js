$(document).ready(function () {
    crearRutas(30);
});

function crearRutas(lineas) {
    $('#contenido-crear').on('click', function () {
        //alert($("#contenido-origen select option:selected").text());
        //detallesJsonProductos($('#rru12-subLineas'+1).attr('subLineas'),$('#contenido-nroDocumento input').attr('DocEntry'), $("#contenido-linea" + 1).attr("value"))
        if (confirm("¿Esta seguro/a de Crear Documento?")) {
            var parametros = {

                "DocEntry": $('#contenido-nroDocumento input').attr('DocEntry')
                , "DocSerie": $('#contenido-serieDocumento input').val()
                , "DocNum": $('#contenido-nroDocumento input').val()
                , "TipoRuta": $('#contenido-tipoRuta select').val()
                , "TransCod": $("#contenido-transportista select option:selected").val()
                , "TransDesc": $("#contenido-transportista select option:selected").text()
                , "PlacaCod": $("#contenido-placa select option:selected").val()
                , "PlacaDesc": $("#contenido-placa select option:selected").text()
                , "CopilCod": ($("#contenido-copiloto #ListaCopilotos option[value='" + $("#contenido-copiloto input").val() + "']").attr("CopilCod")) ? $("#contenido-copiloto #ListaCopilotos option[value='" + $("#contenido-copiloto input").val() + "']").attr("CopilCod") : "NULL"
                , "CopilDesc": ($("#contenido-copiloto input").val()) ? $("#contenido-copiloto input").val() : "NULL"
                , "Copil2Cod": ($("#contenido-copiloto2 #ListaCopilotos2 option[value='" + $("#contenido-copiloto2 input").val() + "']").attr("Copil2Cod")) ? $("#contenido-copiloto2 #ListaCopilotos2 option[value='" + $("#contenido-copiloto2 input").val() + "']").attr("Copil2Cod") : "NULL"
                , "Copil2Desc": ($("#contenido-copiloto2 input").val()) ? $("#contenido-copiloto2 input").val() : "NULL"
                , "Copil3Cod": ($("#contenido-copiloto3 #ListaCopilotos3 option[value='" + $("#contenido-copiloto3 input").val() + "']").attr("Copil3Cod")) ? $("#contenido-copiloto3 #ListaCopilotos3 option[value='" + $("#contenido-copiloto3 input").val() + "']").attr("Copil3Cod") : "NULL"
                , "Copil3Desc": ($("#contenido-copiloto3 input").val()) ? $("#contenido-copiloto3 input").val() : "NULL"
                , "Copil4Cod": ($("#contenido-copiloto4 #ListaCopilotos4 option[value='" + $("#contenido-copiloto4 input").val() + "']").attr("Copil4Cod")) ? $("#contenido-copiloto4 #ListaCopilotos4 option[value='" + $("#contenido-copiloto4 input").val() + "']").attr("Copil4Cod") : "NULL"
                , "Copil4Desc": ($("#contenido-copiloto4 input").val()) ? $("#contenido-copiloto4 input").val() : "NULL"
                , "FechaCont": $('#contenido-FechaCont input').val()
                , "FechaDoc": $('#contenido-FechaDoc input').val()
                , "AlmOrigenCod": $("#contenido-origen select option:selected").val()
                , "AlmOrigenDesc": $("#contenido-origen select option:selected").text()
                , "AlmOrigenDesc2": $("#contenido-origen select option:selected").text()
                , "AlmDestinoCod": $("#contenido-destino select option:selected").val()
                , "AlmDestinoDesc": $("#contenido-destino select option:selected").text()
                , "AlmDestinoDesc2": $("#contenido-destino select option:selected").text()
                , "PropietarioCod": ($("#contenido-propietario #ListaPropietarios option[value='" + $("#contenido-propietario input").val() + "']").attr("PropietarioCod")) ? $("#contenido-propietario #ListaPropietarios option[value='" + $("#contenido-propietario input").val() + "']").attr("PropietarioCod") : ""
                , "PropietarioDesc": $("#contenido-propietario input").val()
                , "HoraI": $('#contenido-horaInicio input').val()
                , "HoraT": $('#contenido-horaTermino input').val()
                , "Estado": $('#contenido-estadoDocumento #estado').attr('value')
                , "TotalCajas": ($('#contenido-totalCajas').text() == "") ? "0" : $('#contenido-totalCajas').text()
                , "Observaciones": ($('#contenido-observaciones input').val()) ? $('#contenido-observaciones input').val() : ""
                , "ListaDetalleRegistroRutas": detallesJson(lineas)
            };
            $.ajax('/Rutas/CreaOrdenRegistroRutas',
                {
                    data: parametros,
                    dataType: 'json',
                    cache: false,
                    type: 'post'
                })
                .done(function (response) {
                    if (response.mensaje == "") {
                        alert(response.mensajeError);
                    }
                    else {
                        alert(response.mensaje + "-" + response.nroDocumento);
                        $('#contenido-estadoDocumento #estado').html(response.estado);
                        $('#contenido-nroDocumento input').val(response.nroDocumento);  
                        $('#contenedorId').css("background", response.estiloContenedorId);
                        $('#tabla3Id').html(response.tabla3Id);
                        generarPdfs(response.nroDocumento);
                    }
                    
                })
                .fail(function (jqXHR, textStatus, errorThrown) {
                    //alert("Error en Creacion Comuniquese con soporte");
                    if (jqXHR.status === 0) {
                        alert('Not connect: Verify Network. Encabezado');
                    }
                    else if (jqXHR.status == 404) {
                        alert('Requested page not found [404] Encabezado');
                    }
                    else if (jqXHR.status == 500) {
                        alert('Internal Server Error [500]. Encabezado');
                    }
                    else if (textStatus === 'parsererror') {
                        alert('Requested JSON parse failed . Encabezado');
                    }
                    else if (textStatus === 'timeout') {
                        alert('Time out error. Encabezado');
                    }
                    else if (textStatus === 'abort') {

                        alert('Ajax request aborted. Encabezado');
                    }
                    else {
                        alert('Uncaught Error: ' + jqXHR.responseText + ' Encabezado');
                    }
                });
        }
    });

    $('#contenido-generarNuevo').on('click', function () {
        location.reload();
    });
    $('#contenido-volver').on('click', function () {
        if (confirm("¿volver? se borraran los datos no creados")) {
            location.href = "../../";
        }
    });
}
function detallesJson(lineas) {
    var ListaJson = new Array(lineas);
    for (var j = 1; j <= lineas; j++) {
        ListaJson[j - 1] = generaJson(j);
    }
    return ListaJson;
}
function generaJson(i) {
    var verifica = "NO";
    if ($("#contenido-verificado" + i + " input").is(":checked")) {
        verifica = "SI";
    }
    else {
        verifica = "NO";
    }
    Json = {
        "DocEntry": $('#contenido-nroDocumento input').attr('DocEntry')
        ,"Linea": $("#contenido-linea" + i).attr("value")
        , "SocioCod": ($("#contenido-SocioNegocio" + i + " #ListaSocios option[value='" + $("#contenido-SocioNegocio" + i + " input").val() + "']").attr("SocioCod"))
            ?
            $("#contenido-SocioNegocio" + i + " #ListaSocios option[value='" + $("#contenido-SocioNegocio" + i + " input").val() + "']").attr("SocioCod")
            : "NULL"
        , "SocioDesc": $("#contenido-SocioNegocio" + i + " input").val() ? $("#contenido-SocioNegocio" + i + " input").val() : "NULL"
        , "Ruc": ($("#contenido-ruc" + i).text() != "") ? $("#contenido-ruc" + i).text() : "NULL"
        , "Otros": ($("#contenido-otros" + i + " input").val()) ? $("#contenido-otros" + i + " input").val() : "NULL"
        , "Telefono": ($("#contenido-telefonoRS" + i + " input").val()) ? $("#contenido-telefonoRS" + i + " input").val() : "NULL"
        , "DirDest": ($("#contenido-direccionD" + i + " input").val()) ? $("#contenido-direccionD" + i + " input").val() : ""
        , "MontoTotal": (($("#contenido-montoTotalVenta" + i + " select").val() != "0") && ($("#contenido-montoTotalVenta" + i + " select").val()))
            ?
            $("#contenido-montoTotalVenta" + i + " select").val()
            : "NULL"
        , "Ventas": ($("#contenido-listaVentas" + i + " p").text()) ? $("#contenido-listaVentas" + i + " p").text() : "NULL"
        , "Guias": ($("#contenido-listaGuias" + i + " p").text()) ? $("#contenido-listaGuias" + i + " p").text() : $("#contenido-listaGuias" + i + " select").val()
        , "Montos": ($("#contenido-monto" + i + " p").text()) ? $("#contenido-monto" + i + " p").text() : "NULL"
        , "VendedorCod": ($("#contenido-vendedor" + i + " p").attr("VendedorCod")) ? $("#contenido-vendedor" + i + " p").attr("VendedorCod") : "NULL"
        , "VendedorDesc": (($("#contenido-vendedor" + i + " p").text() != "") && ($("#contenido-vendedor" + i + " p").text()))
            ?
            $("#contenido-vendedor" + i + " p").text()
            : "NULL"
        , "Cajas": ($("#contenido-nroCajas" + i + " input").val()) ? $("#contenido-nroCajas" + i + " input").val() : "NULL"
        , "PerEnc": ($("#contenido-personaEncargada" + i + " input").val()) ? $("#contenido-personaEncargada" + i + " input").val() : "NULL"
        , "DniPerEnc": ($("#contenido-dniPE" + i + " input").val()) ? $("#contenido-dniPE" + i + " input").val() : "NULL"
        , "CostoEnvio": ($("#contenido-costoEnvio" + i + " input").val()) ? $("#contenido-costoEnvio" + i + " input").val() : "NULL"
        , "Verificado": verifica
        , "ListaRRU12": detallesJsonProductos($('#rru12-subLineas'+i).attr('subLineas'),$('#contenido-nroDocumento input').attr('DocEntry'), $("#contenido-linea" + i).attr("value"))
    }
    return Json;
}
function generaJsonProductos(i, BaseEntry, BaseLinea) {
    Json = {
        "BaseEntry": BaseEntry
        , "BaseLinea": BaseLinea
        , "Linea": $("#contenido-productos" + BaseLinea +" #rru12-Linea" + i).attr("value")
        , "ItemCode": $("#contenido-productos" + BaseLinea + " #rru12-ItemName" + i + " p").attr("ItemCode")
        , "ItemName": $("#contenido-productos" + BaseLinea + " #rru12-ItemName" + i + " p").text()
        , "Lote": $("#contenido-productos" + BaseLinea + " #rru12-Lote" + i + " p").text()
        , "CantidadL": $("#contenido-productos" + BaseLinea + " #rru12-CantidadL" + i + " p").text()
        , "LaboCod": $("#contenido-productos" + BaseLinea + " #rru12-LaboDesc" + i + " p").attr("LaboCod")
        , "LaboDesc": $("#contenido-productos" + BaseLinea + " #rru12-LaboDesc" + i + " p").text()
        , "UnitMed": $("#contenido-productos" + BaseLinea + " #rru12-UnitMed" + i + " p").text()
        , "CantUnitMed": $("#contenido-productos" + BaseLinea + " #rru12-UnitMed" + i + " p").attr("CantUnitMed")
        , "Cajas": $("#contenido-productos" + BaseLinea + " #rru12-Cajas" + i + " input").val()

    }

    return Json;
}
function detallesJsonProductos(subLineas, BaseEntry, BaseLinea) {   
    var ListaJson = new Array(subLineas);
    for (var j = 1; j <= subLineas; j++) {
        ListaJson[j - 1] = generaJsonProductos(j, BaseEntry, BaseLinea);
    }
    return ListaJson;
}
//pdfs
function generarPdfs(DocNum) {
    $('#documento').on('click', function () {
        window.location.href = '/Rutas/pdfRutas?DocNum=' + DocNum;
    });
}