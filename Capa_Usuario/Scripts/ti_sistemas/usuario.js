function mostrarMensaje(mensaje) {
    if (typeof mensaje === 'string' && mensaje.trim() !== '') {
        Swal.fire(mensaje);
    }
}

function validarCodigoSAP(input) {
    if (input.value <= 0) {
        $('#CodigoSap').val('');
        Swal.fire("El valor debe ser mayor que cero.");
    }
}

function infoIdUsuario(idRol) {
    var idR = $(idRol).val();
    if (idR == null || idR == "") { idR = 0; }
    var parametros = { "idRol": idR };

    $.ajax('/TI_Sistemas/infoIdUsuario',
        {
            data: parametros,
            dataType: 'html',
            cache: false,
            type: 'post',
        })
        .done(function (response) {
            let result = JSON.parse(response);
            $('#usuario').val(result.prefijo + result.id);
            $('#prefijo').val(result.prefijo);
            $('#id').val(result.id);
        });
}