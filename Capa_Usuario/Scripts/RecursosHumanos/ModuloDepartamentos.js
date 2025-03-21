var urlListaDptos = $('#div_listaDepartamentos').data('url');

document.addEventListener('DOMContentLoaded', function () {
    document.getElementById("div_admDepartamentos").addEventListener("click", function () {
        $('#div_listaDepartamentos').load(urlListaDptos).fadeIn("fast");
        $('#modalAdmDepartamentos').modal('show');
    });
});
//document.getElementById("btn_agregarDpto").addEventListener("click", function () {
//    let selectDpto = document.getElementById("codDepartamento");
//    let nombreDpto = selectDpto.options[selectDpto.selectedIndex].dataset.nombre;       // Obtenemos el nombre del dataset del área

//    if (selectDpto.value.length <= 0) {
//        Swal.fire('Debe seleccionar un DEPARTAMENTO.');
//        return;
//    }

//    if (validarTexto(nombreDpto) == false) {
//        Swal.fire('Texto ingresado no es v&aacutelido.');
//        return;
//    }

//    $.ajax({
//        url: '/RecursosHumanos/AgregarDepartamento',
//        type: 'POST',
//        data: {
//            coddepartamento: selectDpto.value,
//            nombre: nombreDpto
//        }
//    }).done(function (response) {
//        Swal.fire({
//            title: response.Mensaje,
//            html: response.Comentario.join('<br>'),
//            icon: response.Icono,
//            allowEscapeKey: false,
//            allowOutsideClick: false,
//            showConfirmButton: true
//        });

//        // Limpiamos los campos solo cuando se registró con éxito
//        if (response.Icono == 'success') {
//            selectDpto.value = "";
//            $('#div_listaDepartamentos').load(urlListaDptos).fadeIn("fast");
//        }

//        //$('#modalInfo').modal('hide');
//    }).fail(function (jqXHR, textStatus, errorThrown) {
//        console.error(jqXHR.status, textStatus, errorThrown);
//    });
//});

//function cambiarEstadoDpto(id, estado) {
//    $.ajax({
//        url: '/RecursosHumanos/EditarDepartamento',
//        type: 'POST',
//        data: { iddepartamento: id, estado: estado }
//    }).done(function (response) {
//        Swal.fire({
//            title: response.Mensaje,
//            html: response.Comentario && response.Comentario.length > 0 ? response.Comentario.join('<br>') : "",
//            icon: response.Icono,
//            allowEscapeKey: false,
//            allowOutsideClick: false,
//            showConfirmButton: true
//        });

//        if (response.Icono == "success") {
//            $('#div_listaDepartamentos').load(urlListaDptos).fadeIn("fast");
//        }

//    }).fail(function (jqXHR, textStatus, errorThrown) {
//        console.error(jqXHR.status, textStatus, errorThrown);
//    });
//}

//function eliminarDpto(btn) {
//    let idDpto = btn.dataset.iddepartamento;
//    let nombreDpto = btn.dataset.nombre;

//    if (idDpto <= 0 || nombreDpto == "") {
//        Swal.fire("Error al eliminar el área seleccionado.");
//        return;
//    }

//    swalWithBootstrapButtons.fire({
//        title: '&iquest;Est&aacute seguro(a) de ELIMINAR el departamento?',
//        html: 'El departamento ' + nombreDpto + ' ser&aacute;  eliminado de forma permanente.',
//        icon: 'warning',
//        allowEscapeKey: false,
//        showCancelButton: true,
//        confirmButtonText: "<i class='icon-checkmark'></i> Si, estoy seguro(a)",
//        cancelButtonText: "<i class='icon-cross'></i> Cancelar",
//        reverseButtons: true
//    }).then((result) => {
//        if (result.isConfirmed) {
//            $.ajax({
//                url: '/RecursosHumanos/EliminarDepartamento',
//                type: 'POST',
//                dataType: 'json',
//                data: { iddepartamento: idDpto }
//            }).done(function (response) {
//                Swal.fire({
//                    title: response.Mensaje,
//                    html: (response.Comentario && response.Comentario.length > 0) ? response.Comentario : "",
//                    icon: response.Icono,
//                    allowEscapeKey: false,
//                    allowOutsideClick: false,
//                    showConfirmButton: true
//                });

//                if (response.Icono == "success") {
//                    $('#div_listaDepartamentos').load(urlListaDptos).fadeIn("fast");
//                }
//            }).fail(function (jqXHR, textStatus, errorThrown) {
//                console.error(jqXHR.status, textStatus, errorThrown);
//            });
//        }
//    });
//}