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

    let form = { IdUbig: ubigeoSede, Nombre: nombreSede };
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

//function cargarSedes() {
//    $.ajax({
//        url: '/RecursosHumanos/CargarSedes',
//        type: 'POST'
//    }).done(function (response) {
//        if (response.Mensaje == "OK" && response.Lista.length > 0) {
//            let selectSede = `<option value="">SELECCIONAR</option>`;

//            for (let indice in response.Lista) {
//                selectSede += `<option value="${response.Lista[indice].IdSede}">${response.Lista[indice].Nombre}</option>`;
//            }

//            document.getElementById("sedeLaboral").innerHTML = selectSede;
//            document.getElementById("filtro_sede").innerHTML = selectSede;
//        }
//    }).fail(function (jqXHR, textStatus, errorThrown) {
//        console.error(jqXHR.status, textStatus, errorThrown);
//    });
//}

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