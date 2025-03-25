const camposCargo = ["nombreCargo"];
var urlListaCargos = $('#div_listaCargos').data('url');

document.addEventListener('DOMContentLoaded', function () {
    document.getElementById("div_admCargos").addEventListener("click", function () {
        $('#div_listaCargos').load(urlListaCargos).fadeIn("fast");
        $('#modalAdmCargos').modal('show');
    });
    document.getElementById("btn_agregarCargo").addEventListener("click", function () {
        agregarCargo();
    })
});

function agregarCargo() {
    let nombreCargo = document.getElementById("nombreCargo").value;

    if (validarTexto(nombreCargo) == false) {
        Swal.fire('Texto ingresado no es v&aacutelido.');
        return;
    }

    procesarCargo("/RecursosHumanos/AgregarCargo", { Nombre: nombreCargo });
}

function eliminarCargo(nombreCargo, idCargo) {
    if (idCargo <= 0 || nombreCargo == "") {
        Swal.fire("Error al eliminar el cargo seleccionado.");
        return;
    }

    swalWithBootstrapButtons.fire({
        title: '&iquest;Est&aacute seguro(a) de ELIMINAR el cargo?',
        html: 'El cargo ' + nombreCargo + ' ser&aacute;  eliminado de forma permanente.',
        icon: 'warning',
        allowEscapeKey: false,
        showCancelButton: true,
        confirmButtonText: "<i class='icon-checkmark'></i> Si, estoy seguro(a)",
        cancelButtonText: "<i class='icon-cross'></i> Cancelar",
        reverseButtons: true
    }).then((result) => {
        if (result.isConfirmed) {
            procesarCargo("/RecursosHumanos/EliminarCargo", { idcargo: idCargo });
        }
    });
}

//function cargarCargos() {
//    $.ajax({
//        url: '/RecursosHumanos/CargarCargos',
//        type: 'POST'
//    }).done(function (response) {
//        if (response.Mensaje == "OK" && response.Lista.length > 0) {
//            let dataList = ``;

//            for (let indice in response.Lista) {
//                dataList += `<option data-idCargo="${response.Lista[indice].IdCargo}" value="${response.Lista[indice].Nombre}"></option>`;
//            }

//            document.getElementById("list_puestos").innerHTML = dataList;
//        }
//    }).fail(function (jqXHR, textStatus, errorThrown) {
//        console.error(jqXHR.status, textStatus, errorThrown);
//    });
//}

function procesarCargo(url, data) {
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
            limpiarCampos(camposCargo);
            $('#div_listaCargos').load(urlListaCargos).fadeIn("fast");
            cargarCargos();
        }
    }).fail(function (jqXHR, textStatus, errorThrown) {
        console.error(jqXHR.status, textStatus, errorThrown); // Imprimir cualquier error en la consola en caso de que la solicitud AJAX falle
    });
}