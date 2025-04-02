const camposNumCorporativos = ["numeroCorporativo", "operadorNumero"];
var urlListaNumCorporativos = $('#div_listaNumCorporativos').data('url');
$('#div_listaNumCorporativos').load(urlListaNumCorporativos).fadeIn("fast");

document.addEventListener('DOMContentLoaded', function () {
    document.getElementById("div_admNumCorporativos").addEventListener("click", function () {
        limpiarCampos(camposNumCorporativos);
        $('#div_listaNumCorporativos').load(urlListaNumCorporativos).fadeIn("fast");
        $('#modalAdmNumCorporativos').modal('show');
    });
    document.getElementById("numeroCorporativo").addEventListener("input", limitarMaxCaracteres("numeroCorporativo", "9"));
    document.getElementById("numeroCorporativo").addEventListener("input", permitirSoloNumeros("numeroCorporativo"));
    document.getElementById("btn_agregarNumero").addEventListener("click", agregarNumCorporativo);
    document.getElementById("btn_exportarExcel").addEventListener("click", exportarNumCorporativos);
});

function agregarNumCorporativo() {
    let form = {
        numerocorporativo: document.getElementById("numeroCorporativo").value,          // input
        operador: document.getElementById("operadorNumero").value                                // select
    };

    procesarNumCorporativo('/RecursosHumanos/AgregarNumero', form);
}

function auditarNumCorporativo(id) {
    $.ajax({
        url: '/RecursosHumanos/AuditarNumero',
        type: 'POST',
        data: { id: id }
    }).done(function (response) {
        if (response.Mensaje === "OK" && response.Lista.length > 0) {
            let num = 1;
            let lista = response.Lista.map(item => {
                return `<tr>
                    <td class="text-center">${num++}</td>
                    <td class="text-center">${item.Campo}</td>
                    <td class="text-center">${item.ValorAnterior}</td>
                    <td class="text-center">${item.ValorActual}</td>
                    <td class="text-center">${item.NomApeRegistradoPor}</td>
                    <td class="text-center">${item.FechaRegistro} ${item.HoraRegistro}</td>
                </tr>`;
            }).join("");

            const tbody = document.querySelector("#tbl_audNumeros tbody");
            tbody.innerHTML = lista;

            $("#modalAudNumeros").modal("show");
        } else {
            Swal.fire("Sin datos para mostrar.");
        }
    }).fail(function (jqXHR, textStatus, errorThrown) {
        console.error(jqXHR.status, textStatus, errorThrown);
    });
}

function eliminarNumCorporativo(numCorporativo, id) {
    swalWithBootstrapButtons.fire({
        title: '&iquest;Est&aacute seguro(a) de ELIMINAR n&uacute;mero?',
        html: 'El n&uacute;mero ' + numCorporativo + ' ser&aacute;  eliminado de forma permanente.',
        icon: 'warning',
        allowEscapeKey: false,
        showCancelButton: true,
        confirmButtonText: "<i class='icon-checkmark'></i> Si, estoy seguro(a)",
        cancelButtonText: "<i class='icon-cross'></i> Cancelar",
        reverseButtons: true
    }).then((result) => {
        if (result.isConfirmed) {
            procesarNumCorporativo('/RecursosHumanos/EliminarNumero', { id: id });
        }
    });
}

function liberarNumCorporativo(numCorporativo, id, nroDocumento) {
    swalWithBootstrapButtons.fire({
        title: '&iquest;Est&aacute seguro(a) de LIBERAR n&uacute;mero?',
        html: 'El n&uacute;mero ' + numCorporativo + ' ser&aacute;  liberado para su reasignaci&oacuten.',
        icon: 'warning',
        allowEscapeKey: false,
        showCancelButton: true,
        confirmButtonText: "<i class='icon-checkmark'></i> Si, estoy seguro(a)",
        cancelButtonText: "<i class='icon-cross'></i> Cancelar",
        reverseButtons: true
    }).then((result) => {
        if (result.isConfirmed) {
            procesarNumCorporativo('/RecursosHumanos/LiberarNumero', { id: id, nroDocumento: nroDocumento });            
        }
    });
}

function cargarNumCorporativos() {
    $.ajax({
        url: '/RecursosHumanos/CargarNumCorporativos',
        type: 'POST'
    }).done(function (response) {
        if (response.Mensaje == "OK" && response.Lista.length > 0) {
            let dataList = ``;

            for (let indice in response.Lista) {
                dataList += `<option data-idNumero="${response.Lista[indice].IdNumero}" value="${response.Lista[indice].NumeroCorporativo}"></option>`;
            }

            document.getElementById("list_numCorporativos").innerHTML = dataList;
        }
    }).fail(function (jqXHR, textStatus, errorThrown) {
        console.error(jqXHR.status, textStatus, errorThrown);
    });
}

function exportarNumCorporativos() {
    $(location).attr('href', `/RecursosHumanos/ExportarListadoNumerosCorporativos`);
}

function procesarNumCorporativo(url, data) {
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
            limpiarCampos(camposNumCorporativos);
            $('#div_listaNumCorporativos').load(urlListaNumCorporativos).fadeIn("fast");
            $('#div_listaEmpleados').load(urlListaEmpleados).fadeIn("fast");
            cargarNumCorporativos();
        }
    }).fail(function (jqXHR, textStatus, errorThrown) {
        console.error(jqXHR.status, textStatus, errorThrown); // Imprimir cualquier error en la consola en caso de que la solicitud AJAX falle
    });
}