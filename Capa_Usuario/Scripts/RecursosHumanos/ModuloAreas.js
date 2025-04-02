
var urlListaAreas = $('#div_listaAreas').data('url');
document.addEventListener('DOMContentLoaded', function () {
    document.getElementById("div_admAreas").addEventListener("click", function () {
        $('#div_listaAreas').load(urlListaAreas).fadeIn("fast");
        $('#modalAdmAreas').modal('show');
    });

    document.getElementById("dptoLaboral").addEventListener("change", function () {
        cargarAreas(this.value, "areaLaboral");        // idDepartamento              
    });

    document.getElementById("filtro_dpto").addEventListener("change", function () {
        cargarAreas(this.value, "filtro_area");        // idDepartamento
    });
});

//function cargarAreas(idDpto, idSelectDestino) {
//    let selectArea = `<option value="">SELECCIONAR</option>`;

//    if (idDpto * 1 <= 0) {
//        document.getElementById(idSelectDestino).innerHTML = selectArea;
//        document.getElementById("idAreaLaboral").value = '';
//        return;
//    }

//    $.ajax({
//        url: '/RecursosHumanos/CargarAreas',
//        type: 'POST',
//        data: { iddepartamento: idDpto }
//    }).done(function (response) {
//        if (response.Mensaje == "OK" && response.Lista.length > 0) {
//            for (let indice in response.Lista) {
//                selectArea += `<option value="${response.Lista[indice].IdArea}">${response.Lista[indice].Nombre}</option>`;
//            }

//            document.getElementById(idSelectDestino).innerHTML = selectArea;
//        }

//        // Seleccionar el area despues que haya cargado el departamento
//        if (idSelectDestino === "areaLaboral" && document.getElementById(idSelectDestino).length > 1) {        // Si hay opciones
//            document.getElementById(idSelectDestino).value = document.getElementById("idAreaLaboral").value;        // campo oculto de referencia
//            document.getElementById("idAreaLaboral").value = '';        //Limpiamos input
//        }
//    }).fail(function (jqXHR, textStatus, errorThrown) {
//        console.error(jqXHR.status, textStatus, errorThrown);
//    });
//}

//function agregarArea() {
//    let selectDpto = document.getElementById("codDepartamentoArea");
//    let selectArea = document.getElementById("codArea");
//    let nombreArea = selectArea.options[selectArea.selectedIndex].dataset.nombre;       // Obtenemos el nombre del dataset del área

//    if (selectDpto.value.length <= 0) {
//        Swal.fire('Debe seleccionar un DEPARTAMENTO.');
//        return;
//    }

//    if (nombreArea.length <= 0) {
//        Swal.fire('Debe seleccionar un &Aacute;REA.');
//        return;
//    }

//    if (validarTexto(nombreArea) == false) {
//        Swal.fire('Texto ingresado no es v&aacutelido.');
//        return;
//    }

//    $.ajax({
//        url: '/RecursosHumanos/AgregarArea',
//        type: 'POST',
//        data: {
//            codarea: selectArea.value,
//            coddepartamento: selectDpto.value,
//            nombre: nombreArea
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
//            selectArea.value = "";
//            selectDpto.value = "";
//            $('#div_listaAreas').load(urlListaAreas).fadeIn("fast");
//        }

//        //$('#modalInfo').modal('hide');
//    }).fail(function (jqXHR, textStatus, errorThrown) {
//        console.error(jqXHR.status, textStatus, errorThrown);
//    });
//}

//function cambiarEstadoArea(id, estado) {
//    $.ajax({
//        url: '/RecursosHumanos/EditarArea',
//        type: 'POST',
//        data: { idarea: id, estado: estado }
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
//            $('#div_listaAreas').load(urlListaAreas).fadeIn("fast");
//        }

//    }).fail(function (jqXHR, textStatus, errorThrown) {
//        console.error(jqXHR.status, textStatus, errorThrown);
//    });
//}

//function eliminarArea(btn) {
//    let idArea = btn.dataset.idarea;
//    let nombreArea = btn.dataset.nombre;

//    if (idArea <= 0 || nombreArea == "") {
//        Swal.fire("Error al eliminar el área seleccionado.");
//        return;
//    }

//    swalWithBootstrapButtons.fire({
//        title: '&iquest;Est&aacute seguro(a) de ELIMINAR el &aacuterea?',
//        html: 'El &aacuterea ' + nombreArea + ' ser&aacute;  eliminada de forma permanente.',
//        icon: 'warning',
//        allowEscapeKey: false,
//        showCancelButton: true,
//        confirmButtonText: "<i class='icon-checkmark'></i> Si, estoy seguro(a)",
//        cancelButtonText: "<i class='icon-cross'></i> Cancelar",
//        reverseButtons: true
//    }).then((result) => {
//        if (result.isConfirmed) {
//            $.ajax({
//                url: '/RecursosHumanos/EliminarArea',
//                type: 'POST',
//                dataType: 'json',
//                data: { idarea: idArea }
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
//                    $('#div_listaAreas').load(urlListaAreas).fadeIn("fast");
//                }
//            }).fail(function (jqXHR, textStatus, errorThrown) {
//                console.error(jqXHR.status, textStatus, errorThrown);
//            });
//        }
//    });
//}