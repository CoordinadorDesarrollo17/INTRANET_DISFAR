const camposEmpleado = ["nroDocEmpleado", "celularEmpleado", "nombresEmpleado", "apellidosEmpleado", "condicionLaboral", "nroCorporativoLaboral", "anexoCorporativoLaboral", "sedeLaboral", "dptoLaboral", "areaLaboral", "cargoLaboral", "correoLaboral"];
const camposFiltros = ["filtro_nombresApellidos", "filtro_nroDocumento", "filtro_dpto", "filtro_area", "filtro_cargo", "filtro_sede"];
const registroEmpleado = {
    botonAccion: `<button id="btn_agregarEmpleado" type="button" class="btn btn-success"><i class="icon-plus"></i> Agregar</button>`,
    tituloModal: 'Registrar EMPLEADO',
    camposPermitidos: ["nroDocEmpleado"],
    camposBloqueados: ["nombresEmpleado", "apellidosEmpleado", "nroCorporativoLaboral"],
    selectsBloqueados: [],
    btnBuscarDNI: "block",
    divEstadoEmpleado: "none"
};
var urlExportarExcel = '/RecursosHumanos/ExportarListadoEmpleados';
var paramsVistaExterna = '';

// Obtener la URL de la lista de empleados desde el atributo de datos del elemento div_listaEmpleados
var urlListaEmpleados = $('#div_listaEmpleados').data('url') + '?PaginacionResultados=1';

// Para aplicar filtros por default cuando es vista externa
if (vistaExterna) {
    urlListaEmpleados += '&VistaExterna=SI';
}

$('#div_listaEmpleados').load(urlListaEmpleados).fadeIn("fast");

document.addEventListener('DOMContentLoaded', function () {
    document.getElementById("btn_mostrarTodos").addEventListener("click", mostrarTodos);
    document.getElementById("btn_limpiarFiltrosBusqueda").addEventListener("click", limpiarFiltrosBusqueda);
    document.getElementById("div_RegistrarEmpleado").addEventListener("click", function () {
        abrirModalEmpleado(registroEmpleado);
    });
    document.getElementById("btn_mostrarBusquedaFiltros").addEventListener("click", function () {
        $('#modalBusquedaFiltros').modal('show');
    });
    document.getElementById("btn_filtrarEmpleado").addEventListener("click", function () {
        filtrarEmpleados();
    });
    document.getElementById("nroDocEmpleado").addEventListener("input", function () {
        limitarMaxCaracteres("nroDocEmpleado", "8");
        permitirSoloNumeros("nroDocEmpleado");
    });

    document.getElementById("celularEmpleado").addEventListener("input", function () {
        limitarMaxCaracteres("celularEmpleado", "9");
        permitirSoloNumeros("celularEmpleado");
    });

    document.getElementById("filtro_nombresApellidos").addEventListener("input", limitarMaxCaracteres("filtro_nombresApellidos", "300"));

    document.getElementById("anexoCorporativoLaboral").addEventListener("input", function () {
        limitarMaxCaracteres("anexoCorporativoLaboral", "3");
        permitirSoloNumeros("anexoCorporativoLaboral");
    });
    document.getElementById('nroCorporativoLaboral').addEventListener('input', definirNumCorporativo);
    document.getElementById('cargoLaboral').addEventListener('input', definirPuesto);

    if (vistaExterna == false) {
        document.getElementById("btn_descargarListadoEmpleado").addEventListener("click", exportarListadoEmpleados);

        document.getElementById("filtro_nroDocumento").addEventListener("input", function () {
            permitirSoloNumeros("filtro_nroDocumento");
            limitarMaxCaracteres("filtro_nroDocumento", "8");
        });
    }

    document.getElementById("btn_buscarDNI").addEventListener("click", function () {
        buscarDNI();
    })
});

//--- Funciones
function definirNumCorporativo() {
    permitirSoloNumeros("nroCorporativoLaboral");
    limitarMaxCaracteres("nroCorporativoLaboral", "9");
    let datalist = document.getElementById('list_numCorporativos');
    let idNumero = document.getElementById("idNumeroCorporativo");
    let opcion = Array.from(datalist.children).find(option => option.value === this.value);

    // Limpiamos el valor del campo
    idNumero.value = '';

    if (opcion && idNumero) {
        idNumero.value = opcion.dataset.idnumero;
    }
}

function definirPuesto() {
    let datalist = document.getElementById('list_puestos');
    let idPuesto = document.getElementById("idPuesto");
    let opcion = Array.from(datalist.children).find(option => option.value === this.value);

    // Limpiamos el valor del campo
    idPuesto.value = '';
    if (opcion && idPuesto) {
        idPuesto.value = opcion.dataset.idcargo;
    }
}

function filtrarEmpleados() {
    const urlBase = $('#div_listaEmpleados').data('url');
    const nombresApellidos = document.getElementById("filtro_nombresApellidos").value;
    const nroDocumento = document.getElementById("filtro_nroDocumento").value;
    const dpto = document.getElementById("filtro_dpto").value;
    const area = document.getElementById("filtro_area").value;
    const cargo = document.getElementById("filtro_cargo").value;
    const sede = document.getElementById("filtro_sede").value;
    
    if (nombresApellidos != '' || nroDocumento != '' || dpto != '' || area != '' || cargo != '' || sede != '') {
        let params = `?NombresApellidos=${encodeURIComponent(nombresApellidos)}&NroDocumento=${nroDocumento}&DatosLaborales.IdDepartamento=${dpto}&DatosLaborales.IdArea=${area}&DatosLaborales.IdCargo=${cargo}&DatosLaborales.IdSede=${sede}`;

        if (vistaExterna) {
            params += '&VistaExterna=SI';
        }
        cargando();
        
        urlExportarExcel = '/RecursosHumanos/ExportarListadoEmpleados' + params;
        const listaFiltros = urlBase + params;
        document.getElementById("div_listaEmpleados").setAttribute("data-url", listaFiltros);
        $('#div_listaEmpleados').load(listaFiltros, function (response, status, xhr) {
            if (status === 'success') {
                // Este callback se ejecutará después de que el contenido se haya cargado
                mostrarResultadosPaginacion(1);

                // Verifica si Swal está abierto y lo cierra
                if (Swal.isVisible()) {
                    Swal.close();
                } else {
                    console.warn('SweetAlert2 modal is not visible.');
                }
            } else {
                console.error('Failed to load content:', xhr.status, xhr.statusText);
            }
        });
        // Reiniciar la paginación
        pagActual = 1;
    } else {
        mostrarTodos();
    }

    $('#modalBusquedaFiltros').modal('hide');
}

function agregarEmpleado() {
    let form = {
        IdOEMPL: 0,
        NroDocumento: document.getElementById("nroDocEmpleado").value,
        Celular: document.getElementById("celularEmpleado").value,
        Nombres: document.getElementById("nombresEmpleado").value,
        Apellidos: document.getElementById("apellidosEmpleado").value,
        CondicionLaboral: document.getElementById("condicionLaboral").value,
        AnexoCorporativo: document.getElementById("anexoCorporativoLaboral").value,
        IdSede: document.getElementById("sedeLaboral").value,
        IdDepartamento: document.getElementById("dptoLaboral").value,
        IdArea: document.getElementById("areaLaboral").value,
        IdCargo: document.getElementById("idPuesto").value,
        CorreoCorporativo: document.getElementById("correoLaboral").value
    };

    buscarAnexoCorreo('/RecursosHumanos/AgregarEmpleado', form);
}

function editarEmpleado() {
    let form = {
        IdOEMPL: document.getElementById("idEmpleado").value,
        NroDocumento: document.getElementById("nroDocEmpleado").value,
        Celular: document.getElementById("celularEmpleado").value,
        Nombres: document.getElementById("nombresEmpleado").value,
        Apellidos: document.getElementById("apellidosEmpleado").value,
        CondicionLaboral: document.getElementById("condicionLaboral").value,
        IdNumeroCorporativo: document.getElementById("idNumeroCorporativo").value,
        NumeroCorporativo: document.getElementById("nroCorporativoLaboral").value,
        AnexoCorporativo: document.getElementById("anexoCorporativoLaboral").value,
        IdSede: document.getElementById("sedeLaboral").value,
        IdDepartamento: document.getElementById("dptoLaboral").value,
        IdArea: document.getElementById("areaLaboral").value,
        IdCargo: document.getElementById("idPuesto").value,
        CorreoCorporativo: document.getElementById("correoLaboral").value,
        Estado: document.getElementById("estadoEmpleado").value
    };

    buscarAnexoCorreo('/RecursosHumanos/EditarEmpleado', form);
}

// Función para agregar o editar un empleado
function buscarAnexoCorreo(url, form) {
    if (modoRRHH) {
        $.ajax({
            url: '/RecursosHumanos/AnexoCorreoCorporativoDuplicado',
            type: 'POST',
            data: { anexo: form.AnexoCorporativo, correoCorporativo: form.CorreoCorporativo, id: form.IdOEMPL }
        }).done(function (response) {
            if (response.Icono === 'success') {
                procesarEmpleado(url, form);
            } else {
                swalWithBootstrapButtons.fire({
                    title: response.Mensaje,
                    html: response.Comentario.join('<br>'),
                    icon: response.Icono,
                    allowEscapeKey: false,
                    showCancelButton: true,
                    confirmButtonText: "<i class='icon-checkmark'></i> Si, continuar",
                    cancelButtonText: "<i class='icon-cross'></i> Cancelar",
                    reverseButtons: true
                }).then((result) => {
                    if (result.isConfirmed) {
                        procesarEmpleado(url, form);
                    }
                });
            }
        }).fail(function (jqXHR, textStatus, errorThrown) {
            console.error(jqXHR.status, textStatus, errorThrown); // Imprimir cualquier error en la consola en caso de que la solicitud AJAX falle
        });
    } else {
        procesarEmpleado(url, form);
    }
}

function procesarEmpleado(url, data) {
    $.ajax({
        url: url,
        type: 'POST',
        data: data
    }).done(function (response) {
        // Mostrar un mensaje utilizando SweetAlert2 con la respuesta del servidor
        Swal.fire({
            title: response.Mensaje,
            html: response.Comentario.join('<br>'),
            icon: response.Icono,
            allowEscapeKey: false,
            allowOutsideClick: false,
            showConfirmButton: true
        });

        // Limpiar los campos y actualizar la lista de empleados si la respuesta es exitosa
        if (response.Icono === 'success') {
            $('#modalRegistrarEmpleado').modal('hide');
            limpiarCampos(camposEmpleado);
            $('#div_listaEmpleados').load(urlListaEmpleados).fadeIn("fast");
            cargarNumCorporativos();
        }
    }).fail(function (jqXHR, textStatus, errorThrown) {
        console.error(jqXHR.status, textStatus, errorThrown); // Imprimir cualquier error en la consola en caso de que la solicitud AJAX falle
    });
}

function obtenerDatosEmpleado(idEmpleado) {
    cargando();

    var xhr = new XMLHttpRequest();
    xhr.open('POST', '/RecursosHumanos/ObtenerDatosEmpleado?id=' + idEmpleado, true);
    xhr.setRequestHeader('Content-Type', 'application/json');

    xhr.onload = function () {          // Manejar la respuesta de la solicitud
        if (xhr.status >= 200 && xhr.status < 300) {
            var response = JSON.parse(xhr.responseText);
            let datos = {
                estadoEmpleado: response?.Empleado?.Estado || '',
                idEmpleado: response?.Empleado?.IdOEMPL || '',
                //id: response?.Empleado?.IdEMPL1 || '',
                nroDocEmpleado: response?.Empleado?.NroDocumento || '',
                celularEmpleado: response?.Empleado?.Celular || '',
                nombresEmpleado: response?.Empleado?.Nombres || '',
                apellidosEmpleado: response?.Empleado?.Apellidos || '',
                condicionLaboral: response?.Empleado?.DatosLaborales?.CondicionLaboral || '',
                dptoLaboral: response?.Empleado?.DatosLaborales?.IdDepartamento || '',
                idAreaLaboral: response?.Empleado?.DatosLaborales?.IdArea || '',
                sedeLaboral: response?.Empleado?.DatosLaborales?.IdSede || '',
                cargoLaboral: response?.Empleado?.DatosLaborales?.NombreCargo || '',
                idPuesto: response?.Empleado?.DatosLaborales?.IdCargo || '',
                idNumeroCorporativo: response?.Empleado?.DatosLaborales?.IdNumeroCorporativo || '',
                nroCorporativoLaboral: response?.Empleado?.DatosLaborales?.NumeroCorporativo || '',
                anexoCorporativoLaboral: response?.Empleado?.DatosLaborales?.AnexoCorporativo || '',
                correoLaboral: response?.Empleado?.DatosLaborales?.CorreoCorporativo || ''
            };

            Object.keys(datos).forEach(function (key) {
                //console.log('Clave: ', key, 'Valor: ', datos[key]);
                document.getElementById(key).value = datos[key];

                if (key === 'dptoLaboral') {
                    document.getElementById(key).dispatchEvent(new Event('change', { bubbles: true }));
                }
            });
            swal.close();
        } else {
            errorProcesarSolicitud();
        }
    };

    xhr.onerror = function () {
        errorProcesarSolicitud();           // Manejar errores de la solicitud
    };

    xhr.send();         // Enviar la solicitud
}

function exportarListadoEmpleados() {
    $(location).attr('href', urlExportarExcel);
}

function errorProcesarSolicitud() {
    Swal.fire({
        title: 'Error',
        text: 'Ha ocurrido un error al procesar la solicitud.',
        icon: 'error'
    });
}

function buscarDNI() {
    cargando();

    let dni = document.getElementById("nroDocEmpleado").value;
    const apiToken = '26dae51b5dbcd0dcfdd4117008edc0c6d56ac787f07ccf510cf7fa32d6fa45e8';
    const apiUrl = `https://apiperu.dev/api/dni/${dni}?api_token=${apiToken}`;

    fetch(apiUrl)
        .then(response => {
            if (!response.ok) {
                throw new Error('La respuesta de la red no fue correcta');
                document.getElementById("nombresEmpleado").readOnly = false;
                document.getElementById("apellidosEmpleado").readOnly = false;
                swal.close();
            }
            return response.json();
        })
        .then(data => {
            let datos = data.data;
            document.getElementById("nombresEmpleado").value = capitalizarTexto(datos.nombres.toLowerCase());
            document.getElementById("apellidosEmpleado").value = capitalizarTexto(`${datos.apellido_paterno.toLowerCase()} ${datos.apellido_materno.toLowerCase()}`);
            swal.close();
        })
        .catch(error => {
            console.log('Ha habido un problema con su operacion de busqueda:', error);
            document.getElementById("nombresEmpleado").readOnly = false;
            document.getElementById("apellidosEmpleado").readOnly = false;
            swal.close();
        });
}

function consultarDatosEmpleado(dni) {
    cargando();

    $.ajax({
        url: "/RecursosHumanos/ObtenerDatosEmpleado",
        type: 'POST',
        data: { id: 0, NroDocumento: dni }
    }).done(function (response) {
        swal.close();
        if (response.Empleado != null) {
            Swal.fire({
                title: "Datos",
                html: response.Empleado.NombresApellidos + '<br>' + response.Empleado.NroDocumento,
                icon: "info",
                allowEscapeKey: false,
                allowOutsideClick: false,
                showConfirmButton: true
            });
        } else {
            Swal.fire("Empleado no encontrado.");
        }
    }).fail(function (jqXHR, textStatus, errorThrown) {
        console.error(jqXHR.status, textStatus, errorThrown); // Imprimir cualquier error en la consola en caso de que la solicitud AJAX falle
    });
}

function mostrarTodos() {
    renderizarEstiloFiltros();

    document.getElementById("div_listaEmpleados").setAttribute("data-url", "/RecursosHumanos/ListarEmpleados");
    $('#div_listaEmpleados').load(urlListaEmpleados).fadeIn("fast");

    // Restablecer la URL de exportación
    urlExportarExcel = '/RecursosHumanos/ExportarListadoEmpleados';

    // Limpiar los campos de filtro
    limpiarCampos(camposFiltros);

    // Calcular el total de páginas y generar los botones de paginación
    totalPaginas = Math.ceil(totalEmpleados / porPagina);
    pagActual = 1;
    generarBotonesPaginacion();
    mostrarResultadosPaginacion(1);
}

function limpiarFiltrosBusqueda() {
    // Obtener el elemento filtro_area una sola vez
    const filtroArea = document.getElementById("filtro_area");

    // Limpiar las opciones de filtro_area
    filtroArea.innerHTML = `<option value="">SELECCIONAR</option>`;

    // Restablecer la URL de exportación
    urlExportarExcel = '/RecursosHumanos/ExportarListadoEmpleados';

    // Limpiar los campos de filtro
    limpiarCampos(camposFiltros);
}

// Filtros left-sidebar por DEPARTAMENTO
var atajoFiltro = document.querySelectorAll(".btn-atajoFiltro");

atajoFiltro.forEach(function (btn) {
    btn.addEventListener("click", function () {
        renderizarEstiloFiltros();

        // Elimina las clases de fondo y agrega la clase 'bg-colorComplementario' al botón actual
        this.classList.remove('btn-dark');
        this.classList.add('btn-success');

        // Obtiene el ID del departamento del botón actual
        let params = `?DatosLaborales.IdDepartamento=${this.dataset.id}&PaginacionResultados=1`;
        
        // Construimos la URL completa del filtro de departamento
        let filtroDpto = $('#div_listaEmpleados').data('url') + params;

        // Actualizamos la URL para exportar Excel con el nuevo filtro
        urlExportarExcel = '/RecursosHumanos/ExportarListadoEmpleados' + params;

        document.getElementById("div_listaEmpleados").setAttribute("data-url", "/RecursosHumanos/ListarEmpleados" + `?DatosLaborales.IdDepartamento=${this.dataset.id}`);

        // Carga los datos filtrados en el div_listaEmpleados
        $('#div_listaEmpleados').load(filtroDpto).fadeIn("fast");

        // Reiniciar la paginación
        pagActual = 1;
    });
});