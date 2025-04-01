const camposSede = ["nombreSede", "idUbigeo"];
var urlListaSedes = $('#div_listaSedes').data('url');

document.addEventListener('DOMContentLoaded', function () {
    document.getElementById("div_admSedes").addEventListener("click", function () {
        $('#div_listaSedes').load(urlListaSedes).fadeIn("fast");
        $('#modalAdmSedes').modal('show');
    });

    document.getElementById("btn_agregarSede").addEventListener("click", function () {
        agregarSede();
    });
});

function agregarSede() {
    let nombreSede = $('#nombreSede').val();
    let ubigeoSede = $('#idUbigeo').val();

    if (validarTexto(nombreSede) == false) {
        Swal.fire('Texto ingresado no es v&aacutelido.');
        return;
    }

    let form = { UbigeoID: ubigeoSede, Nombre: nombreSede };
    procesarSede('/RecursosHumanos/AgregarSede', form);
}

function eliminarSede(nombreSede, idSede) {
    if (idSede <= 0 || nombreSede == "") {
        Swal.fire("Error al eliminar la sede seleccionada.");
        return;
    }

    swalWithBootstrapButtons.fire({
        title: '&iquest;Est&aacute seguro(a) de ELIMINAR la sede?',
        html: 'La sede ' + nombreSede + ' ser&aacute;  eliminada de forma permanente.',
        icon: 'warning',
        allowEscapeKey: false,
        showCancelButton: true,
        confirmButtonText: "<i class='icon-checkmark'></i> Si, estoy seguro(a)",
        cancelButtonText: "<i class='icon-cross'></i> Cancelar",
        reverseButtons: true
    }).then((result) => {
        if (result.isConfirmed) {
            procesarSede('/RecursosHumanos/EliminarSede', { idSede: idSede });
        }
    });
}
function procesarSede(url, data) {
    $.ajax({
        url: url,
        type: 'POST',
        data: data
    }).done(function (response) {
        // Mostrar un mensaje utilizando SweetAlert2 con la respuesta del servidor
        Swal.fire({
            title: response.Mensaje,
            html: response.Comentario && response.Comentario.length > 0 ? response.Comentario.join('<br>') : "",
            icon: response.Icono,
            allowEscapeKey: false,
            allowOutsideClick: false,
            showConfirmButton: true
        });

        // Limpiar los campos y actualizar la lista de empleados si la respuesta es exitosa
        if (response.Icono === 'success') {
            limpiarCampos(camposSede);
            $('#div_listaSedes').load(urlListaSedes).fadeIn("fast");
            cargarSedes();
        }
    }).fail(function (jqXHR, textStatus, errorThrown) {
        console.error(jqXHR.status, textStatus, errorThrown); // Imprimir cualquier error en la consola en caso de que la solicitud AJAX falle
    });
}