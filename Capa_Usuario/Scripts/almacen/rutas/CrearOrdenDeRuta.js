$(document).ready(function () {

});
function enviarValSelect(idi, idf,attrf) {
    $("#" + idf).val($("#" + idi + " option:selected").attr(attrf));
}
function enviarValList(idi,idList,idf,attrf) {
    $("#" + idf).val($("#"+idList+" option[value='"+$("#"+idi).val()+"']").attr(attrf));
}
function solicitarSocios(Idfecha,lineas) {  
    var parametros = { "FechaCont": $(Idfecha).val()};
    $.ajax('/Rutas/infoListaSocios',
        {
            data: parametros,
            dataType: 'html',
            cache: false,
            type: 'post'
        })
        .done(function (response) {
            $("#ListaSocios").html(response);
            for (var i = 1; i <= lineas; i++) {
                $("#Socio" + i).val(""); $("#ListaDetalleRegistroRutas" + (i - 1) + "DocEntryTicket").html("<option value=''>Seleccione</option>");
                $("#ListaDetalleRegistroRutas" + (i - 1) + "Guias").val("");
                $("#ListaDetalleRegistroRutas" + (i - 1) + "Cajas").val("");
                $("#ListaDetalleRegistroRutas" + (i - 1) + "Obs").text("");
                $("#ListaDetalleRegistroRutas" + (i - 1) + "Direcciones").text("");
                $("#ListaDetalleRegistroRutas" + (i - 1) + "MontoFinal").text("");
                $("#ListaDetalleRegistroRutas" + (i - 1) + "GastoEnvio").text("");
                $("#SeguimientoDeTicket" + (i - 1)).attr("href", null); $("#SeguimientoDeTicket" + (i - 1)).attr("target", "_blank");
            }
            calcularTotalCajas(lineas);
        });
}
function solicitarTickets(IdFecha,IdCardName,linea,lineas) {
    var parametros = {
        "FechaCont": $("#" + IdFecha).val(), "CardCode": $("#ListaSocios option[value='" + $("#" + IdCardName).val() + "']").attr("CardCode")
    };
    $.ajax('/Rutas/infoListaTicketsVenta',
        {
            data: parametros,
            dataType: 'html',
            cache: false,
            type: 'post'
        })
        .done(function (response) {
            $("#ListaDetalleRegistroRutas" + linea + "DocEntryTicket").html(response);
            solicitarGuias("#ListaDetalleRegistroRutas" + linea + "DocEntryTicket", linea);
            solicitarDatosDetalles("#ListaDetalleRegistroRutas" + linea + "DocEntryTicket", linea, lineas);
        });
}
function solicitarGuias(IdDocEntry,linea) {
    var parametros = { "DocEntry": $(IdDocEntry).val() };
    $.ajax('/Rutas/infoGuiasTicketsVenta',
        {
            data: parametros,
            dataType: 'html',
            cache: false,
            type: 'post'
        })
        .done(function (response) {
            $("#ListaDetalleRegistroRutas"+linea+"Guias").val(response);
        });
}
function solicitarDatosDetalles(IdDocEntry, linea,lineas) {
    var parametros = { "DocEntry": $(IdDocEntry).val() };
    $.ajax('/Rutas/infoTicket',
        {
            data: parametros,
            dataType: 'json',
            cache: false,
            type: 'post'
        })
        .done(function (response) {
            $("#ListaDetalleRegistroRutas" + linea + "Cajas").val(response.Cajas);
            $("#ListaDetalleRegistroRutas" + linea + "Obs").text(muestraVacio(response.Observaciones));
            $("#ListaDetalleRegistroRutas" + linea + "Direcciones").text("1:" + muestraVacio(response.DirDestino1) + ";2:" + muestraVacio(response.DirDestino2));
            $("#ListaDetalleRegistroRutas" + linea + "MontoFinal").text(response.MontoFinal);
            $("#ListaDetalleRegistroRutas" + linea + "GastoEnvio").text(muestraVacio(response.GastoEnvio));
            $("#SeguimientoDeTicket" + linea).attr("href", "SeguimientoDeTicket?DocEntry=" + response.DocEntry);
            $("#SeguimientoDeTicket" + linea).attr("target", "_blank");
            calcularTotalCajas(lineas);
        });
}
function validar() {
    var form = $("#form");
    $.ajax('/Rutas/validarRuta', {
        data: form.serialize(),
        dataType: 'html',
        cache: false,
        type:'post'
    })
    .done(function (response) {
        alert(response);
    });
}
function calcularTotalCajas(lineas) {
    var detalles = new Array();
    for (var i=1; i <= lineas;i++){
        detalles[i - 1] = {
            "Cajas": $("#ListaDetalleRegistroRutas"+(i-1)+"Cajas").val()
        };
    }
    var parametros = { "l":detalles };
    $.ajax('/Rutas/calcularTotalCajas',
        {
            data: parametros,
            dataType: 'html',
            cache: false,
            type: 'post'
        })
        .done(function (response) {
            $("#TotalCajas").val(response);
        });
}
function muestraVacio(x) { if (x == null) { return ""; }else{ return x;}}