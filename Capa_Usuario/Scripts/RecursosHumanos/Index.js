var mostarMasInfo = false;
const porPagina = 12;
var totalPaginas = Math.ceil(cantidadEmpleados / porPagina);                    // cantidadEmpleados es tipo 'var' y se encuentra en el index.cshtml
var pagActual = 1;
document.addEventListener('DOMContentLoaded', function () {
    generarBotonesPaginacion();

    document.querySelectorAll(".prev").forEach(prevBtn => {
        prevBtn.addEventListener("click", e => {
            e.preventDefault();
            if (pagActual > 1) {
                pagActual--;
                mostrarResultadosPaginacion(pagActual);
            }
        });
    });

    document.querySelectorAll(".next").forEach(nextBtn => {
        nextBtn.addEventListener("click", e => {
            e.preventDefault();
            if (pagActual < totalPaginas) {
                pagActual++;
                mostrarResultadosPaginacion(pagActual);
            }
        });
    });

    document.addEventListener("click", function (event) {
        if (event.target.classList.contains("btn-paginacion")) {
            pagActual = event.target.dataset.pag;
            mostrarResultadosPaginacion(pagActual);
        }
    });

    document.getElementById("filtro_dpto").addEventListener("change", function () {
        cargarAreas(this.value, "filtro_area");        // idDepartamento
    });

    cargarSedes();
    cargarCargos();
});


function limpiarCampos(arrCampos) {
    arrCampos.forEach(function (id) {
        document.getElementById(id).value = '';
    });
}

function mostrarOcultarElemento(idElemento, accion) {
    document.getElementById(idElemento).style.display = accion;
}

function abrirModalEmpleado(propiedadesModal) {
    limpiarCampos(camposEmpleado);
    cargarAreas("", "areaLaboral");

    document.getElementById("div_botonesAccion").innerHTML = propiedadesModal.botonAccion;
    document.getElementById("_ModalRegistrarEmpleado").textContent = propiedadesModal.tituloModal;
    propiedadesModal.camposPermitidos.forEach(function (id) {
        document.getElementById(id).readOnly = false;
    });

    propiedadesModal.camposBloqueados.forEach(function (id) {
        document.getElementById(id).readOnly = true;
    });

    if (propiedadesModal.selectsBloqueados.length > 0) {
        propiedadesModal.selectsBloqueados.forEach(function (id) {
            document.getElementById(id).style.pointerEvents = 'none';
            document.getElementById(id).style.backgroundColor = '#e9ecef';
        });
    }

    mostrarOcultarElemento("btn_buscarDNI", propiedadesModal.btnBuscarDNI);
    mostrarOcultarElemento("div_estadoEmpleado", propiedadesModal.divEstadoEmpleado);
    $('#modalRegistrarEmpleado').modal('show');

    // Agregar evento click al botón de agregar empleado
    var btnAgregarEmpleado = document.getElementById("btn_agregarEmpleado");
    if (btnAgregarEmpleado) {
        btnAgregarEmpleado.addEventListener("click", agregarEmpleado);
    }

    // Agregar evento click al botón de editar empleado
    var btnEditarEmpleado = document.getElementById("btn_editarEmpleado");
    if (btnEditarEmpleado) {
        btnEditarEmpleado.addEventListener("click", editarEmpleado);
    }
}

function generarBotonesPaginacion() {
    let bg = "";
    let btn = "";
    document.querySelectorAll(".numbers").forEach(function (element) {
        element.innerHTML = "";
    });

    for (let i = 1; i <= totalPaginas; i++) {
        if (i == pagActual) {
            bg = 'bg-cobefar-3 text-white';
            btn = 'btn';
        } else {
            bg = 'bg-light text-dark border border-turquesa';
            btn = 'btn btn-sm';
        }

        $(".numbers").append(`<button type="button" data-pag="${i}" class="${btn} mx-1 ${bg} btn-paginacion" class="active">${i}</button>`);
    }
}

function mostrarResultadosPaginacion(pag) {
    const divListaEmpleados = document.getElementById("div_listaEmpleados");
    let urlListaEmpleados = divListaEmpleados.dataset.url;

    let separator = urlListaEmpleados.includes('?') ? '&' : '?';
    urlListaEmpleados += `${separator}PaginacionResultados=${pag}`;

    if (vistaExterna) {
        urlListaEmpleados += '&VistaExterna=SI';
    }

    $(divListaEmpleados).load(urlListaEmpleados).fadeIn("fast");
    generarBotonesPaginacion();
}

function cargarCargos() {
    $.ajax({
        url: '/RecursosHumanos/CargarCargos',
        type: 'POST'
    }).done(function (response) {
        if (response.Mensaje == "OK" && response.Lista.length > 0) {
            let dataList = ``;
            let select = `<option value="">SELECCIONAR</option>`;

            for (let indice in response.Lista) {
                dataList += `<option data-idCargo="${response.Lista[indice].Id}" value="${response.Lista[indice].Nombre}"></option>`;
                select += `<option value="${response.Lista[indice].Id}">${response.Lista[indice].Nombre}</option>`;
            }

            document.getElementById("list_puestos").innerHTML = dataList;
            document.getElementById("filtro_cargo").innerHTML = select;
        }
    }).fail(function (jqXHR, textStatus, errorThrown) {
        console.error(jqXHR.status, textStatus, errorThrown);
    });
}

function cargarSedes() {
    $.ajax({
        url: '/RecursosHumanos/CargarSedes',
        type: 'POST'
    }).done(function (response) {
        if (response.Mensaje == "OK" && response.Lista.length > 0) {
            let selectSede = `<option value="">SELECCIONAR</option>`;

            for (let indice in response.Lista) {
                selectSede += `<option value="${response.Lista[indice].Id}">${response.Lista[indice].Nombre}</option>`;
            }

            document.getElementById("sedeLaboral").innerHTML = selectSede;
            document.getElementById("filtro_sede").innerHTML = selectSede;
        }
    }).fail(function (jqXHR, textStatus, errorThrown) {
        console.error(jqXHR.status, textStatus, errorThrown);
    });
}

function cargarAreas(idDpto, idSelectDestino) {
    let selectArea = `<option value="">SELECCIONAR</option>`;

    if (idDpto * 1 <= 0) {
        document.getElementById(idSelectDestino).innerHTML = selectArea;
        document.getElementById("idAreaLaboral").value = '';
        return;
    }

    $.ajax({
        url: '/RecursosHumanos/CargarAreas',
        type: 'POST',
        data: { idDepartamento: idDpto }
    }).done(function (response) {
        if (response.Mensaje == "OK" && response.Lista.length > 0) {
            for (let indice in response.Lista) {
                selectArea += `<option value="${response.Lista[indice].IdArea}">${response.Lista[indice].Nombre}</option>`;
            }

            document.getElementById(idSelectDestino).innerHTML = selectArea;
        }

        // Seleccionar el area despues que haya cargado el departamento
        if (idSelectDestino === "areaLaboral" && document.getElementById(idSelectDestino).length > 1) {        // Si hay opciones
            document.getElementById(idSelectDestino).value = document.getElementById("idAreaLaboral").value;        // campo oculto de referencia
            document.getElementById("idAreaLaboral").value = '';        //Limpiamos input
        }
    }).fail(function (jqXHR, textStatus, errorThrown) {
        console.error(jqXHR.status, textStatus, errorThrown);
    });
}

function renderizarEstiloFiltros() {
    document.querySelectorAll(".btn-atajoFiltro").forEach(function (btn) {
        // Eliminamos la clase 'bg-colorComplementario' del botón que coincida con el filtro anterior
        btn.classList.remove('btn-success');
        btn.classList.add('btn-dark');
    });
}