$(document).ready(function(){
    
    codProd();
    ProdLabo();
    rptKardex();
});
function codProd()
{
    $('#infoProducto input').on('change', function () {
        $('#ItemCode').val("");
        $('#ItemCode').val($("#infoProducto #ListaProductos option[value='" + $("#infoProducto input").val() + "']").attr("ItemCode"));
    });
}
function ProdLabo()
{
    $('#FirmCode').on('change', function () {
        var parametros = { "FirmCode": $('#FirmCode').val() };
        $.ajax('/ReportesDigemid/ListaProductosHtml',
            {
                data: parametros,
                dataType: 'html',
                cache: false,
                type: 'post'
            })
            .done(function (response) {
                $('#infoProducto div').html(response);
            });
    });
}
//eventos de reporte
function rptKardex() {
    $('#Formulario #Generar').on('click', function () {
        $('#Reporte').html('<img src="/imagenes/index/ReportesDigemid/cargando.gif" />');
        var parametros = $("#frmKardex").serialize();
        $.ajax('/ReportesDigemid/tbReporteKardex',
            {
                data : parametros,
                dataType: 'html',
                cache: false,
                type: 'post'
            })
            .done(function (response) {
                $('#Reporte').html(response);
            });
    });
}
