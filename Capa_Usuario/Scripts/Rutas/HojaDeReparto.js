
$(document).ready(function () {
    //ocultar boton de tickets no enviados al iniciar el DOM
    $("#btn-NoEnviados").hide();
});

//Funcion principal para agregar lineas al detalle
function agregarTicketsDetalle(esEdicion) {
    let tablaDeTicketsPorEnviar = $('#detalleRuta').DataTable();

    var nrows = 0;

    $("#Detalles tr").each(function () {
        nrows++;
    })
   /* console.log(nrows);*/
    let checkboxesSeleccionados = tablaDeTicketsPorEnviar.rows({ page: 'all' }).nodes().to$().find('.cls-chkVerif:checked');

    if (checkboxesSeleccionados.length > 0) {
        checkboxesSeleccionados.each(function () {
            var tr = $(this).closest('tr');
            // Ver si esa fila tiene vinculados
            var vinculados = tr.find('input[id^="Vinculados"]').val();

            // Verificamos si hay valores en vinculados
            if (vinculados != null && vinculados != "") {
                // Separamos los valores por la coma
                var valoresVinculados = vinculados.split(",");
                // Iteramos sobre los docnum vinculados
                for (var i = 0; i < valoresVinculados.length; i++) {
                    var docnum = valoresVinculados[i].trim();

                    var found = false;

                    checkboxesSeleccionados.each(function () {
                        var tr = $(this).closest('tr');
                        if (tr.find('input[id^="DocNum"]').val() === docnum && $(this).prop('checked')) {
                            found = true;
                            return false;
                        }
                    });

                    //buscamos en los valores que ya han sido agregados a la tabla llamada Detalles
                    var tablaDetallesAgregados = $('#Detalles');
                    if (tablaDetallesAgregados.find('tbody tr').length > 0) {
                        // Recorrer todas las filas de la tabla
                        tablaDetallesAgregados.find('tbody tr').each(function () {
                            // Obtener el valor del campo con id que comienza con "DocNumTDet" en la fila actual
                            var valorDocNum = $(this).find('input[id^="DocNumTDet"]').val();
                            // Verificar si el valor obtenido es igual a la variable "docnum"
                            if (valorDocNum === docnum) {
                                found = true;
                                return false;
                            }
                        });
                    }

                    if (!found && vinculados !== "") {
                        Swal.fire("Debe agregar a su detalle tambien los vinculados: '" + vinculados + "'");
                        return;
                    }
                }
            }

            // Obtener los valores relevantes de la fila y pasarlos a agregarItem
            var docEntry = tr.find('input[id^="DocEntry"]').val();
            var docNum = tr.find('input[id^="DocNum"]').val();
            var cardName = tr.find('input[id^="CardName"]').val();
            var guias = tr.find('input[id^="Guías"]').val();
            var cajas = tr.find('input[id^="Cajas"]').val();
            var obs = tr.find('input[id^="Obs"]').val();
            var direcciones = tr.find('input[id^="Direcciones"]').val();
            var montoFinal = tr.find('input[id^="MontoFinal"]').val();
            var envio = tr.find('input[id^="Envio"]').val();

            agregarItem(docEntry, docNum, cardName, guias, cajas, obs, direcciones, montoFinal, envio, esEdicion);
        });

        checkboxesSeleccionados.prop('checked', false);
        $("#chk_verifTodos").prop('checked', false);
        // Se posiciona en la tabla de detallado final
        $('html, body').animate({
            scrollTop: $('#divDetalleRuta').offset().top
        }, 1000);
    }

    else {
        // Si no hay tickets seleccionados, mostrar mensaje de alerta
        Swal.fire('Sin tickets seleccionados', 'Por favor, selecciona al menos un ticket.', 'warning');
    }
}

// Función unificada que agrega un item al detalle principal
function agregarItem(docEntry, docNum, cardName, guias, cajas, obs, direcciones, montoFinal, envio, esEdicion) {
    
    // Verificar si el campo ya existe
    if (validarUnicoCampoTabla(docEntry, "Detalles") == false) {
        return false;
    } else {
        if (docEntry == null || docEntry == "") {
            return false;
        }
        // Construir la fila HTML de la tabla
        var fila = "<tr>" +
            "<td><input name='DetRRU0[" + contDet + "].Linea' type='text' value='" + (contDet + 1) + "' style='width:40px' class='form-control' readonly/></td>" +
            "<td hidden><input name='DetRRU0[" + contDet + "].DocEntryTicket' id='DocEntryTDet" + contDet + "' type='text' value='" + docEntry + "'  class='form-control'  readonly/></td>" +
            "<td><input name='DetRRU0[" + contDet + "].DocNumTicket' id='DocNumTDet" + contDet + "' type='text' value='" + docNum + "' style='width:110px' class='form-control' readonly/></td>" +
            "<td><input name='DetRRU0[" + contDet + "].Socio' type='text' value='" + cardName + "' style='width:200px' class='form-control' readonly/></td>" +
            "<td><textarea name='DetRRU0[" + contDet + "].Guias' class='form-control' readonly>" + guias + "</textarea></td>" +
            "<td><input name='DetRRU0[" + contDet + "].Cajas' type='text' value='" + cajas + "' style='width:40px' class='form-control' readonly/></td>" +
            "<td><input name='DetRRU0[" + contDet + "].Observaciones' type='text' value='" + obs + "' class='form-control' readonly/></td>" +
            "<td><textarea name='DetRRU0[" + contDet + "].Direcciones' class='form-control' readonly>" + direcciones + "</textarea></td>" +
            "<td><input name='DetRRU0[" + contDet + "].MontoFinal' type='text' value='" + montoFinal + "' class='form-control' readonly/></td>" +
            "<td><input name='DetRRU0[" + contDet + "].Envio' type='text' value='" + envio + "' class='form-control' readonly/></td>" +
            "<td><a href='SeguimientoDeTicket?DocEntry=" + docEntry + "' target='_blank' style='cursor:pointer' class='btn btn-outline-primary' onclick=window.open(this.href,this.target,'width=500,height=350,top=120,left=100,toolbar=no,location=no,status=no,menubar=no');><i class='icon-truck'></i></a></td>";

        // Agregar botones según si es para grabar o no
        if (esEdicion) {
            var docEntryElement = document.getElementById("doc_entry");

            if (docEntryElement) {
                docEntryOrru = docEntryElement.value;
            }

            fila += "<td colspan='2' class='d-flex justify-content-center' id='acc" + (contDet + 1) + "'>" +
                "<button type='button' class='btn btn-outline-success mr-2' onclick='grabarLineaTabla(" + docEntryOrru + ","+(contDet + 1) + "," + docEntry + ")'><i class='icon icon-lock'></i> Grabar</button>" +
                "<button type='button' class='btn btn-outline-danger' onclick='borrarLineaTabla(this)'><i class='icon icon-bin'></i></button>" +
                "</td>";
        } else {
            fila += "<td><button type='button' class='btn btn-outline-danger' onclick='borrarLineaTabla(this)'><i class='icon icon-bin'></i></button></td>";
        }

        fila += "</tr>";
       
        // Agregar la fila al detalle de la tabla
        $("#Detalles").append(fila);

        // Calcular el total de cajas
        calcularTotalCajas();

        // Incrementar el contador de detalle
        contDet++;

        // Verificar si es edición y recargar la página
        //if (esEdicion) {
        //    window.location.reload();
        //}
        return true;
    }
}

function borrarLineaTabla(dom) {
    Swal.fire({
        title: '¿Estás seguro?',
        text: 'Esta acción no se puede deshacer.',
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Sí, eliminar',
        cancelButtonText: 'Cancelar'
    }).then((result) => {
        if (result.isConfirmed) {
            var i = -1;

            $(dom).closest('tr').remove();

            contDet = contDet - 1;
            $("#Detalles tr").each(function (index, htm) {
                iden1 = $(htm).find("td:eq(0) input");
                iden1.attr("name", "DetRRU0[" + i + "].Linea");
                iden1.val(i + 1);
                iden1 = $(htm).find("td:eq(1) input");
                iden1.attr("name", "DetRRU0[" + i + "].DocEntryTicket");
                iden1 = $(htm).find("td:eq(2) input");
                iden1.attr("name", "DetRRU0[" + i + "].DocNumTicket");
                iden1 = $(htm).find("td:eq(3) input");
                iden1.attr("name", "DetRRU0[" + i + "].Socio");
                iden1 = $(htm).find("td:eq(4) textarea");
                iden1.attr("name", "DetRRU0[" + i + "].Guias");
                iden1 = $(htm).find("td:eq(5) input");
                iden1.attr("name", "DetRRU0[" + i + "].Cajas");
                iden1 = $(htm).find("td:eq(6) input");
                iden1.attr("name", "DetRRU0[" + i + "].Observaciones");
                iden1 = $(htm).find("td:eq(7) textarea");
                iden1.attr("name", "DetRRU0[" + i + "].Direcciones");
                iden1 = $(htm).find("td:eq(8) input");
                iden1.attr("name", "DetRRU0[" + i + "].MontoFinal");
                iden1 = $(htm).find("td:eq(9) input");
                iden1.attr("name", "DetRRU0[" + i + "].Envio");
                i = i + 1;
            });
            calcularTotalCajas();

            //Swal.fire('Eliminado', 'El item ha sido eliminado correctamente.', 'success');
        }
    });
}

//funciones que solo se usan para Editar Hoja de reparto
function liberarLineaTabla(DocEntry, Linea, DocEntryTicket, idreg) {
    Swal.fire({
        title: '¿Está seguro(a) de liberar?',
        text: "Este proceso es irreversible!",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Si'
    }).then((result) => {
        if (result.isConfirmed) {
            var parametros = { "DocEntry": DocEntry, "Linea": Linea, "DocEntryTicket": DocEntryTicket };
            $.ajax('/Rutas/liberarRRU0',
                {
                    data: parametros,
                    dataType: 'html',
                    cache: false,
                    type: 'post'
                })
                .done(function (response) {
                    if (response == "ok") {
                        $("#" + idreg).css("background", "lightgray");
                        Swal.fire(
                            'Ticket liberado exitosamente',
                            '',
                            'success'
                        )
                    }
                    else {
                        swal.fire({ title: response, text: "Presione OK para continuar", icon: "warning" }); return false;
                    }
                });
        }
    })
}

function grabarLineaTabla(DocEntryRu, Linea, DocEntryTi) {
    let Guias = $("textarea[name='DetRRU0[" + (Linea - 1) + "].Guias']").val();
    Swal.fire({
        title: '¿Está seguro(a) de grabar?',
        text: "Este proceso es irreversible!",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Si'
    }).then((result) => {
        if (result.isConfirmed) {
            var object = { "DocEntry": DocEntryRu, "Linea": Linea, "DocEntryTicket": DocEntryTi, "Guias": Guias };
            $.ajax('/Rutas/agregarRRU0',
                {
                    data: object,
                    dataType: 'html',
                    cache: false,
                    type: 'post'
                })
                .done(function (response) {
                    if (response != "ok") { swal.fire({ title: response, text: "Presione OK para continuar", icon: "warning" }); return false }
                    else {
                        swal.fire({ title: 'Ticket agregado al detalle de ruta', text: "Presione OK para continuar", icon: "success" });
                        $("#acc" + Linea).html("<button type='button' class='btn btn-blue btn-sm' onclick=liberarTrTable1(" + DocEntryRu + "," + Linea + "," + DocEntryTi + "," + '"' + 'reg' + Linea + '"' + ")><i class='icon icon-unlocked'>  </i>Liberar</button> ");
                        return false
                    }
                });
        }
    })
}

function validarTipoRuta(tipo, estado) {
    if (tipo === 'VG') {
        $('#div_almacen-destino').hide();
        $('#div_almacen-origen').hide();
        //limpiar valores de almacen destino
        $('#AlmDestinoDesc option[value=" "]').prop('selected', true);
        $('#AlmDestinoCod').val(''); $('#AlmDestinoDesc2').val('');
        //limpiar valores de almacen origen
        $('#AlmOrigenDesc option[value=" "]').prop('selected', true);
        $('#AlmOrigenCod').val(''); $('#AlmOrigenDesc2').val('');
        if (estado !== "CREADO") {
            $('#Zona option[value="AGENCIA"]').prop('selected', true);
        }
        buscarConductorYPlaca('AGENCIA');
    } else {
        $('#div_almacen-destino').show();
        $('#div_almacen-origen').show();
        //enviar como parametro default el valor de combobox a almacen destino
        if (tipo === "VA") {
            $('#AlmDestinoDesc option[value="ALMACÉN N°5 (Arriola)"]').prop('selected', true);
            if (estado !== "CREADO") {
                $('#Zona option[value="ARRIOLA"]').prop('selected', true);
            }
            buscarConductorYPlaca('ARRIOLA');
        }
        else if (tipo === "VC") {
            $('#AlmDestinoDesc option[value="ALMACÉN N°1"]').prop('selected', true);
            if (estado !== "CREADO") {
                $('#Zona option[value="CONO CENTRO"]').prop('selected', true);
            }
            buscarConductorYPlaca('CONO CENTRO');
        }
        else if (tipo === "VD") {
            $('#AlmDestinoDesc option[value="DOMICILIOS"]').prop('selected', true);
        }
        if (estado !== "CREADO") {
            enviarValSelect('AlmDestinoDesc', 'AlmDestinoCod', 'AlmDestinoCod');
            enviarValSelect('AlmDestinoDesc', 'AlmDestinoDesc2', 'AlmDestinoDesc2');
        }
    }
    if (estado === "CREADO") {
        //ocultar el detallado de RRU11 cuando el tipo de ruta es diferente a transferencia de almacenes
        if (tipo != 'TA') { $("#tbl_detRRu11").css("display", "none"); }
    }
}

function buscarConductorYPlaca(zona) {
    var parametros = { "zona": zona }
    var selectConductor = $("#TransDesc");
    var selectPlaca = $("#Placa");
    $.ajax({
        url: '/Repartos/buscarConductorYPlaca',
        data: parametros,
        dataType: 'json',
        cache: false,
        type: 'post'
    })
        .done(function (response) {
            if (response.Placa !== '' && response.Conductor !== '') {
                selectConductor.val(response.Conductor).change();
                selectPlaca.val(response.Placa).change();
            } else {
                selectConductor.val('').change();
                selectPlaca.val('').change();
            }

        }).fail(function () {
            Swal.fire('Error', 'Hubo un problema al cargar los datos', 'error');
        });
}

function enviarValSelect(idi, idf, attrf) {
    $("#" + idf).val($("#" + idi + " option:selected").attr(attrf));
}

var xhr;
function listarTickets(estado) {
    var FechaSapTicket = $('#FechaSapTicket').val();
    var TipoRuta = $('#TipoRuta').val();
    var AlmOrigenCod = $('#AlmOrigenCod').val();
    var Zona = $('#Zona').val();
    var tabla = $('#detalleRuta').DataTable();

    if (TipoRuta !== '' && TipoRuta == 'VG') {
        if (FechaSapTicket !== '' && Zona !== '') {
            if (xhr && xhr.readyState !== 4) {
                xhr.abort();
            }
            Swal.fire({
                title: 'Trayendo información de tickets',
                text: 'Si cierra la ventana, la carga de datos seguirá en proceso.',
                allowEscapeKey: false,
                allowOutsideClick: false,
                showCloseButton: false,
                showConfirmButton: false,
                timer: 3000
            });
            var parametros = { "FechaSapTicket": FechaSapTicket, "TipoRuta": TipoRuta, "Zona": Zona, "AlmOrigenCod": AlmOrigenCod }
            if (estado === 'CREADO') {
                // Destruir la tabla antes de volver a cargar los datos
                tabla.clear().destroy();
            }
            xhr = $.ajax({
                url: '/Rutas/infoTicketsReparto',
                data: parametros,
                dataType: 'json',
                cache: false,
                type: 'post'
            })
                .done(function (response) {
                    $("#btn-NoEnviados a").text('');
                    $("#btn-NoEnviados").hide();
                    if (estado === 'CREADO') {
                        // Volver a inicializar DataTable después de destruirlo
                        tabla = $('#detalleRuta').DataTable();
                    } else {
                        if (tabla.rows().count() > 0) {
                            tabla.clear().draw();
                        }
                    }
                    var msjNoEnviados = "";
                    response.Resultado.forEach(function (item, index) {
                        var fila = $('<tr>');
                        // Agregar el ID a la fila
                        fila.attr('id', 'fila' + index);

                        // Crear las celdas con inputs y agregarlas a la fila
                        fila.append('<td class="text-center"><input type="checkbox" class="cls-chkVerif ml-3" id="check' + index + '" autocomplete="off"></td>');
                        fila.append('<td class="text-center"><input id="Linea' + index + '" type="text" value="' + (index + 1) + '" readonly hidden/>' + (index + 1) + '</td>');
                        fila.append('<td hidden><input id="DocEntry' + index + '" type="text" value="' + item.DocEntry + '" readonly hidden/></td>');
                        fila.append('<td class="text-center"><input id="DocNum' + index + '" type="text" value="' + item.DocNum + '" hidden/>' + item.DocNum + '</td>');
                        fila.append('<td hidden><input id="CardCode' + index + '" type="text" value="' + item.CardCode + '" hidden/></td>');
                        fila.append('<td class="text-center" style="width:250px"><input id="CardName' + index + '" type="text" value="' + item.CardName + '" hidden/>' + item.CardName + '</td>');
                        fila.append('<td class="text-center" style="width:450px"><input id="Guías' + index + '" type="text" value="' + item.Guias + '" readonly hidden/>' + item.Guias + '</td>');
                        fila.append('<td class="text-center"><input id="Cajas' + index + '" type="text" value="' + item.Cajas + '" readonly hidden/>' + item.Cajas + '</td>');
                        fila.append('<td hidden><input id="Obs' + index + '" type="text" value="' + (item.Observaciones != null ? item.Observaciones : '') + '" readonly hidden/>' + (item.Observaciones != null ? item.Observaciones : '') + '</td>');
                        fila.append('<td hidden><input id="Direcciones' + index + '" type="text" value="' + (item.DirDestino != null ? item.DirDestino : '') + '" readonly hidden/>' + (item.DirDestino != null ? item.DirDestino : '') + '</td>');
                        fila.append('<td class="text-center" style="width:70px"><input  type="text" value="' + item.Agencia + '" readonly hidden/>' + (item.Agencia != null ? item.Agencia : '') + '</td>');
                        fila.append('<td class="text-center"><input id="MontoFinal' + index + '" type="text" value="' + item.MontoFinal.toFixed(2) + '" readonly hidden/>' + item.MontoFinal.toFixed(2) + '</td>');
                        fila.append('<td hidden><input id="Envio' + index + '" type="text" value="' + item.GastoEnvio.toFixed(2) + '" readonly hidden/>' + item.GastoEnvio.toFixed(2) + '</td>');
                        fila.append('<td class="text-center"><input id="TipoVenta' + index + '" type="text" value="' + item.TipoVenta + '" readonly hidden/>' + item.TipoVenta + '</td>');
                        fila.append('<td class="text-center">' + (item.FechaPago != null ? item.FechaPago + '<br>' + item.HoraPago : 'PENDIENTE') + '</td>');
                        fila.append('<td hidden><input id="Vinculados' + index + '" type="text" value="' + (item.Vinculados != null ? item.Vinculados : '') + '" readonly hidden/>' + (item.Vinculados != null ? item.Vinculados : '') + '</td>');

                        tabla.row.add(fila);
                    });

                    tabla.draw(); // Dibujar la tabla una vez que se han agregado todas las filas
                    if (response.CantidadTicketsNoEnviados != 0) {
                        msjNoEnviados = "Tiene " + response.CantidadTicketsNoEnviados + " tickets pendiente de envio";
                        $("#btn-NoEnviados a").text(msjNoEnviados);
                        $("#btn-NoEnviados").show();
                    }
                    Swal.close();
                })
                .fail(function () {
                    // Manejar error si es necesario
                    Swal.fire('Error', 'Hubo un problema al cargar los datos', 'error');
                });
        }
    } else if (TipoRuta !== '' && TipoRuta !== 'VG') {
        if (FechaSapTicket !== '' && Zona !== '' && AlmOrigenCod !== '') {
            if (xhr && xhr.readyState !== 4) {
                xhr.abort();
            }
            Swal.fire({
                title: 'Trayendo información de tickets',
                text: 'Si cierra la ventana, la carga de datos seguirá en proceso.',
                allowEscapeKey: false,
                allowOutsideClick: false,
                showCloseButton: false,
                showConfirmButton: false,
                timer: 3000
            });
            var parametros = { "FechaSapTicket": FechaSapTicket, "TipoRuta": TipoRuta, "Zona": Zona, "AlmOrigenCod": AlmOrigenCod }
            if (estado === 'CREADO') {
                // Destruir la tabla antes de volver a cargar los datos
                tabla.clear().destroy();
            }
            xhr = $.ajax({
                url: '/Rutas/infoTicketsReparto',
                data: parametros,
                dataType: 'json',
                cache: false,
                type: 'post'
            })
                .done(function (response) {
                    $("#btn-NoEnviados a").text('');
                    $("#btn-NoEnviados").hide();

                    if (estado === 'CREADO') {
                        // Volver a inicializar DataTable después de destruirlo
                        tabla = $('#detalleRuta').DataTable();
                    } else {
                        if (tabla.rows().count() > 0) {
                            tabla.clear().draw();
                        }
                    }

                    var msjNoEnviados = "";
                    response.Resultado.forEach(function (item, index) {
                        var fila = $('<tr>');
                        // Agregar el ID a la fila
                        fila.attr('id', 'fila' + index);

                        // Crear las celdas con inputs y agregarlas a la fila
                        fila.append('<td class="text-center"><input type="checkbox" class="cls-chkVerif ml-3" id="check' + index + '" autocomplete="off"></td>');
                        fila.append('<td class="text-center"><input id="Linea' + index + '" type="text" value="' + (index + 1) + '" readonly hidden/>' + (index + 1) + '</td>');
                        fila.append('<td hidden><input id="DocEntry' + index + '" type="text" value="' + item.DocEntry + '" readonly hidden/></td>');
                        fila.append('<td class="text-center"><input id="DocNum' + index + '" type="text" value="' + item.DocNum + '" hidden/>' + item.DocNum + '</td>');
                        fila.append('<td hidden><input id="CardCode' + index + '" type="text" value="' + item.CardCode + '" hidden/></td>');
                        fila.append('<td class="text-center" style="width:250px"><input id="CardName' + index + '" type="text" value="' + item.CardName + '" hidden/>' + item.CardName + '</td>');
                        fila.append('<td class="text-center" style="width:450px"><input id="Guías' + index + '" type="text" value="' + item.Guias + '" readonly hidden/>' + item.Guias + '</td>');
                        fila.append('<td class="text-center"><input id="Cajas' + index + '" type="text" value="' + item.Cajas + '" readonly hidden/>' + item.Cajas + '</td>');
                        fila.append('<td hidden><input id="Obs' + index + '" type="text" value="' + (item.Observaciones != null ? item.Observaciones : '') + '" readonly hidden/>' + (item.Observaciones != null ? item.Observaciones : '') + '</td>');
                        fila.append('<td hidden><input id="Direcciones' + index + '" type="text" value="' + (item.DirDestino != null ? item.DirDestino : '') + '" readonly hidden/>' + (item.DirDestino != null ? item.DirDestino : '') + '</td>');
                        fila.append('<td class="text-center" style="width:70px"><input  type="text" value="' + item.Agencia + '" readonly hidden/>' + (item.Agencia != null ? item.Agencia : '') + '</td>');
                        fila.append('<td class="text-center"><input id="MontoFinal' + index + '" type="text" value="' + item.MontoFinal.toFixed(2) + '" readonly hidden/>' + item.MontoFinal.toFixed(2) + '</td>');
                        fila.append('<td hidden><input id="Envio' + index + '" type="text" value="' + item.GastoEnvio.toFixed(2) + '" readonly hidden/>' + item.GastoEnvio.toFixed(2) + '</td>');
                        fila.append('<td class="text-center"><input id="TipoVenta' + index + '" type="text" value="' + item.TipoVenta + '" readonly hidden/>' + item.TipoVenta + '</td>');
                        fila.append('<td class="text-center">' + (item.FechaPago != null ? item.FechaPago + '<br>' + item.HoraPago : 'PENDIENTE') + '</td>');
                        fila.append('<td hidden><input id="Vinculados' + index + '" type="text" value="' + (item.Vinculados != null ? item.Vinculados : '') + '" readonly hidden/>' + (item.Vinculados != null ? item.Vinculados : '') + '</td>');

                        tabla.row.add(fila);
                    });

                    tabla.draw(); // Dibujar la tabla una vez que se han agregado todas las filas
                    if (response.CantidadTicketsNoEnviados != 0) {
                        msjNoEnviados = "Tiene " + response.CantidadTicketsNoEnviados + " tickets pendiente de envio";
                        $("#btn-NoEnviados a").text(msjNoEnviados);
                        $("#btn-NoEnviados").show();
                    }
                    Swal.close();
                })
                .fail(function () {
                    // Manejar error si es necesario
                    Swal.fire('Error', 'Hubo un problema al cargar los datos', 'error');
                });
        }
    }
}

function seleccionarVerif() {
    let chk = $('#chk_verifTodos').is(':checked');
    let tabla = $('#detalleRuta').DataTable();
    if (chk) {
        tabla.rows({ search: 'applied' }).nodes().to$().find('.cls-chkVerif').prop('checked', chk);
    } else {
        tabla.rows({ search: 'applied' }).nodes().to$().find('.cls-chkVerif').prop('checked', chk);
    }
}

function verTicketsNoEnviados() {
    let FechaSapTicket = $('#FechaSapTicket').val();
    let TipoRuta = $('#TipoRuta').val();
    let Zona = $('#Zona').val();
    let AlmOrigenCod = $('#AlmOrigenCod').val();
    let tablaModal = $('#tableModalNE');
    let tbody = tablaModal.find('tbody');

    if (tbody.children().length > 0) {
        tbody.empty();
    }
    if (FechaSapTicket !== '' && TipoRuta !== '' && Zona !== '') {
        var parametros = { "FechaSapTicket": FechaSapTicket, "TipoRuta": TipoRuta, "Zona": Zona, "AlmOrigenCod": AlmOrigenCod }
        $.ajax({
            url: '/Rutas/listarTicketsRepartosNoEnviados',
            data: parametros,
            dataType: 'json',
            cache: false,
            type: 'post'
        })
            .done(function (response) {
                response.forEach(function (item, index) {
                    var fila = $('<tr>');
                    fila.append('<td class="text-center">' + item.DocNum + '  </td>');
                    fila.append('<td class="text-center">' + item.FechaSapTicket + '</td>');
                    fila.append('<td class="text-center">' + item.CardName + '</td>');
                    fila.append('<td class="text-center">' + item.MontoFinal.toFixed(2) + '</td></tr>');
                    tbody.append(fila);
                });

            }).fail(function () {
                // Manejar error si es necesario
                Swal.fire('Error', 'Hubo un problema al cargar los datos', 'error');
            });
        $("#ModalTicketsNE").modal('show');
    }
}

function calcularTotalCajas() {
    cajas = 0;
    $("#Detalles > tbody tr").find('td:eq(5) input').each(function () {
        cajas += $(this).val() * 1;
    })
    $("#TotalCajas").val(cajas);
}

function validarUnicoCampoTabla(identificador, idTabla) {

    var sinRegistro = true;
    $("#" + idTabla + " tr").find('td:eq(1) input').each(function () {
        iden = $(this).val();
        if (iden == identificador) {
            Swal.fire("El valor ya se encuentra en la tabla", "Verifique", "warning");
            sinRegistro = false;
        }
    });
    return sinRegistro;
}

function validarEnviarFormulario(estado) {
    var object = $("#form").serialize();
    if (estado === 'CREADO') {
        $.ajax('/Rutas/validarDatosEncabezadoRuta',
            {
                data: object,
                dataType: 'html',
                cache: false,
                type: 'post'
            })
            .done(function (response) {
                if (response != "OK") { swal.fire({ title: response, text: "Presione OK para continuar", icon: "warning" }); return false }
                else { $("#form").submit(); }
            });
    } else {
        $.ajax('/Rutas/validarNuevaHojaDeRepartoOTransferencia',
            {
                data: object,
                dataType: 'html',
                cache: false,
                type: 'post'
            })
            .done(function (response) {
                if (response != "OK") { swal.fire({ title: response, text: "Presione OK para continuar", icon: "warning" }); return false }
                else { $("#form").submit(); }
            });
    }
}


