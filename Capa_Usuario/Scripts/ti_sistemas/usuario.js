document.addEventListener("DOMContentLoaded", function () {
    document.getElementById('emplCobefar1').addEventListener('change', function () {
        if (this.checked) {
            document.getElementById('inputRoles').value = "";
            document.getElementById('idRol').value = 0;
            document.getElementById('usuario').value = "";
            document.getElementById('CodigoSap').value = "";

            document.getElementById('div_inputsForm').style.display = "block";
            document.getElementById('div_listaEmpleados').style.display = "block";

            document.getElementById('Nombres').value = "";
            document.getElementById('div_nombres').style.display = "none";

            document.getElementById('Apellidos').value = "";
            document.getElementById('div_apellidos').style.display = "none";
        }
    });

    document.getElementById('emplCobefar2').addEventListener('change', function () {
        if (this.checked) {
            document.getElementById('inputRoles').value = "";
            document.getElementById('idRol').value = 0;
            document.getElementById('usuario').value = "";
            document.getElementById('CodigoSap').value = "";

            document.getElementById('div_inputsForm').style.display = "block";
            document.getElementById('div_listaEmpleados').style.display = "none";

            document.getElementById('Nombres').value = "";
            document.getElementById('div_nombres').style.display = "block";

            document.getElementById('Apellidos').value = "";
            document.getElementById('div_apellidos').style.display = "block";
        }
    });

    gestionarValorDatalist('inputEmpleados', 'listaEmpleados', 'empleadoId', 'idempleado');
    gestionarValorDatalist('inputRoles', 'listaRoles', 'idRol', 'idrol');
});

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

function infoIdUsuario(elemento) {
    const opcionSeleccionada = Array.from(document.querySelectorAll('#listaRoles option'))
        .find(option => option.value === elemento.value);

    if (opcionSeleccionada) {
        const idRol = opcionSeleccionada.dataset.idrol || 0;
        const parametros = { idRol };

        $.ajax('/TI_Sistemas/infoIdUsuario', {
            data: parametros,
            dataType: 'html',
            cache: false,
            type: 'post',
        })
            .done(response => {
                try {
                    const result = JSON.parse(response);
                    $('#usuario').val(`${result.prefijo}${result.id}`);
                    $('#prefijo').val(result.prefijo);
                    $('#id').val(result.id);
                } catch (e) {
                    console.error('Error parsing response:', e);
                }
            })
            .fail((jqXHR, textStatus, errorThrown) => {
                console.error('AJAX request failed:', textStatus, errorThrown);
            });
    } else {
        $('#usuario').val('');
        $('#prefijo').val('');
        $('#id').val('');
        //console.error('No se encontró una opción válida');
    }
}