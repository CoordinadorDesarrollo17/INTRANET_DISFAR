$(document).ready(function () {
});
function filtrar() {
    var object = $("#frm").serialize();
    $(location).attr('href', "/Rutas/ListadoRutas?"+object);
}