document.addEventListener("DOMContentLoaded", function () {

    // === Empleado Cobefar 1 ===
    let empl1 = document.getElementById('emplCobefar1');
    if (empl1) {
        empl1.addEventListener('change', function () {
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
    }

    // === Empleado Cobefar 2 ===
    let empl2 = document.getElementById('emplCobefar2');
    if (empl2) {
        empl2.addEventListener('change', function () {
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
    }

    // === Input empleados ===
    let inputEmpleados = document.getElementById('inputEmpleados');
    if (inputEmpleados) {
        inputEmpleados.addEventListener('change', function () {
            const sedeMapping = {
                "1": "01",
                "2": "03",
                "4": "06",
                "5": "07"
            };

            const idsede = document.getElementById("sedeId")?.value;
            const whsCode = sedeMapping[idsede];

            let whsCodeInput = document.getElementById("WhsCode");
            if (whsCodeInput) {
                whsCodeInput.value = whsCode || "";
            }
        });

        // Datalist relacionados con inputEmpleados
        gestionarValorDatalist('inputEmpleados', 'listaEmpleados', 'empleadoId', 'idempleado');
        gestionarValorDatalist('inputEmpleados', 'listaEmpleados', 'sedeId', 'idsede');
    }

    // === Input roles ===
    let inputRoles = document.getElementById('inputRoles');
    if (inputRoles) {
        gestionarValorDatalist('inputRoles', 'listaRoles', 'idRol', 'idrol');
    }
});


// === Funciones auxiliares ===

function mostrarMensaje(mensaje) {
    if (typeof mensaje === 'string' && mensaje.trim() !== '') {
        Swal.fire({
            icon: 'info',
            title: 'Mensaje',
            text: mensaje,
            showConfirmButton: true,
            confirmButtonText: 'Aceptar',
            confirmButtonColor: '#198754',
            background: '#f6fff7',
            color: '#198754',
            customClass: {
                popup: 'swal2-modern-popup',
                title: 'swal2-modern-title',
                confirmButton: 'swal2-modern-confirm'
            }
        });
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
    }
}
