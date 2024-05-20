$(document).ready(function () {

});
function enviarValSelect(idSelect, idCampo, atrib) {
    $("#" + idCampo).val(
        $("#" + idSelect + " option:selected ").attr(atrib)
    );
}
function enviarValList(idi, idList, idf, attrf) {
    $("#" + idf).val($("#" + idList + " option[value='" + $("#" + idi).val() + "']").attr(attrf));
}
function datosTraslado(orden) {
    $('#contenido-productos' + orden).html('<img src="/imagenes/index/ReportesDigemid/cargando.gif" />');
    var parametros = {
        "guia": $('#RRU1' + orden + 'Guia').val()
        , "orden": orden
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
        $("#RRU1" + j + "Cajas").val(totalCajaMaster);
        totalCajas += totalCajaMaster * 1
        totalCajaMaster = 0; i = 0;
        j++;
    }
    $("#TotalCajas").val(totalCajas);
    
}
function validar() {
    var form = $("#form");
    $.ajax('/Rutas/validarRuta', {
        data: form.serialize(),
        dataType: 'html',
        cache: false,
        type: 'post'
    })
        .done(function (response) {
            alert(response);
        });
}