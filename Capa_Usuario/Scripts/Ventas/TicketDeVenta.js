$(document).ready(function () {
    //para casos en donde la Zona es un desplegable (pachacamac y lima) se bloquea la escritura
    function bloquearEscritura(event) {
        event.preventDefault();
    }
    $("#Zona").on("keydown", bloquearEscritura);
});
function enviarValSelect(idi, idf, attrf) {
    $("#" + idf).val($("#" + idi + " option:selected").attr(attrf) || "");
}
function verTarifario() {
    window.open('/Rutas/GestionarTarifarios?TipoRep=Re&ticketventa=1', 'popUpWindow', 'height=500,width=850,left=100,top=100,resizable=yes,scrollbars=yes,toolbar=no,menubar=no,location=no,directories=no, status=yes');
}
function reubicarZona(direccion) {
    var DirDestino = $('#DirDestino');
    var selectedOption = DirDestino.find('option:selected');
    var Zona;
    if (selectedOption.length === 0 || selectedOption.val() === "") {
        var direccionEscogida = $('#DireccionEscogida').text().trim();
        DirDestino.find('option:not(:first-child)').each(function () {
            if (direccion != "" && direccion != null) {
                var direccionCompleta = direccion;
            } else {
                var direccionCompleta = $(this).val();
            }
            if (direccionCompleta === direccionEscogida) {
                Zona = $(this).attr('Zona');
                return false;
            }
        });
    } else {
        Zona = selectedOption.attr('Zona');
    }
    $("#Zona").val(Zona);

}
function busqueda(dom, tabla) {
    let desc = dom.toLowerCase();
    let textoCapitalizado = desc.charAt(0).toUpperCase() + desc.slice(1);
    let buscarxDepartamento = $("#chk_buscarxdepartamento" + tabla).is(':checked');
    let buscarxProvincia = $("#chk_buscarxprovincia" + tabla).is(':checked');
    let buscarxDistrito = $("#chk_buscarxdistrito" + tabla).is(':checked');
    $("#" + tabla + " tr").each(function (index, htm) {
        if (index > 0) {
            if (desc.length == 0) {
                $(htm).attr("class", " ");
            }
            else {
                if (buscarxDepartamento && $(htm).find("td:eq(3)").html() == textoCapitalizado) {
                    $(htm).attr("class", " ");
                } else if (buscarxProvincia && $(htm).find("td:eq(2)").html() == textoCapitalizado) {
                    $(htm).attr("class", " ");
                } else if (buscarxDistrito && $(htm).find("td:eq(1)").html() == textoCapitalizado) {
                    $(htm).attr("class", " ");
                } else {
                    if ((!buscarxDepartamento && !buscarxProvincia && !buscarxDistrito) && $(htm).text().toLowerCase().indexOf(desc) != -1) {
                        $(htm).attr("class", " ");
                    }
                    else {
                        $(htm).attr("class", "d-none")
                    }
                }

            }
        }

    });
}
//v
function agregarDatos(ubi, dis, pro, dep, Zona) {
    //caso Pachacamac, si la direccion fiscal es pachacamac o lima y el distrito 2 es igual a la 1, la Zona debe ser la de SAP
    if ((dis === 'Pachacamac' && $("#Distrito").val() === 'Pachacamac') || (dis === 'Lima' && $("#Distrito").val() === 'Lima')) {
        var DirDestino = $('#DirDestino');
        var selectedOption = DirDestino.find('option:selected');
        var Zona;
        if (selectedOption.length === 0 || selectedOption.val() === "") {
            var direccionEscogida = $('#DireccionEscogida').text().trim();
            DirDestino.find('option:not(:first-child)').each(function () {
                var direccionCompleta = $(this).val();
                if (direccionCompleta === direccionEscogida) {
                    Zona = $(this).attr('Zona');
                    return false;
                }
            });
        } else {
            Zona = selectedOption.attr('Zona');
        }
        $("#Zona").val(Zona);
        $("#Zona").removeAttr("list");
        $("#Zona").prop("readonly", true);
    } else if ($("#Zona").val() != "PREVENTA" &&
        ((dis !== 'Pachacamac' && $("#Distrito").val() !== 'Pachacamac'
        ) && (dis !== 'Lima' && $("#Distrito").val() !== 'Lima'))) {
        $("#Zona").val(Zona);
        $("#Zona").removeAttr("list");
        $("#Zona").prop("readonly", true);
    } else if ($("#Zona").val() != "PREVENTA" &&
        ((dis === 'Pachacamac' && $("#Distrito").val() !== 'Pachacamac'
        ) || (dis === 'Lima' && $("#Distrito").val() !== 'Lima'))) {
        //se activa datalist para Zona, primero se borra el valor y readonly false
        $("#Zona").val('');
        $("#Zona").prop("readonly", false);
        if (dis === 'Pachacamac') {
            $("#Zona").attr("list", "opcionesZonaPachacamac");
        } else if (dis === 'Lima') {
            $("#Zona").attr("list", "opcionesZonaLima");
        }
    } else {
        $("#Zona").val(Zona);
        $("#Zona").removeAttr("list");
        $("#Zona").prop("readonly", true);
    }
    $("#Ubigeo2").val(ubi);
    $("#Distrito2").val(dis);
    $("#Provincia2").val(pro);
    $("#Departamento2").val(dep);
    $("#DirDestino2").attr('readonly', false);
    $("#DirDestino2").focus();
}
function agregarDatosOficina(cal, ubi, dis, pro, dep) {
    $("#DirDestino2").val(cal);
    $("#Ubigeo2").val(ubi);
    $("#Distrito2").val(dis);
    $("#Provincia2").val(pro);
    $("#Departamento2").val(dep);
}
function mostrarOficinas() {
    let envio = $("#EnvioAgencia").val();
    let lugardestino = $("#LugarDestino").val();
    if (envio == "Oficina de agencia" && lugardestino == "Agencia Courier") {
        $("#btnUbigeo").hide();
        $("#btnOficinas").show();
    }
    else {
        $("#btnUbigeo").show();
        $("#btnOficinas").hide();
    }
}
function validacionDirDestino(estado) {
    var CardCode;
    if (estado === '') {
        $('#infoListaClientes input').on('change', function () {
            CardCode = $("#infoListaClientes #ListaClientes option[value='" + $("#infoListaClientes input").val() + "']").attr("CardCode");
            var Docnum = $("#DocNum").val(); // <-- Obtiene el valor del input DocNum
            if (!CardCode) { CardCode = ""; }
            if (!Docnum) { Docnum = ""; }
            var parametros = { "CardCode": CardCode, "DocNum": Docnum }; // <-- Agrega DocNum al objeto
            $.ajax('/Ventas/infoDirDestino',
                {
                    data: parametros,
                    dataType: 'html',
                    cache: false,
                    type: 'post',
                })
                .done(function (response) {
                    $('#DirDestino').html(response);
                    // Selecciona automáticamente la primera opción válida
                    var $dirDestino = $('#DirDestino');
                    var $firstValidOption = $dirDestino.find('option:not([value=""]):first');
                    if ($firstValidOption.length) {
                        $dirDestino.val($firstValidOption.val());
                        var zonaDefault = $firstValidOption.attr('Zona') || "";
                        $("#Zona").val(zonaDefault);
                        $dirDestino.trigger('change');
                    }
                });

        });
    } else {
        CardCode = $("#CardCode").val();
        var Docnum = $("#DocNum").val(); // <-- Obtiene el valor del input DocNum
        //si CardCode no encuenta un valor
        if (!CardCode) { CardCode = "" };
        var parametros = { "CardCode": CardCode, "DocNum": Docnum }; // <-- Agrega DocNum al objeto
        $.ajax('/Ventas/infoDirDestino',
            {
                data: parametros,
                dataType: 'html',
                cache: false,
                type: 'post',
            })
            .done(function (response) {
                $('#DirDestino').html(response);
                // Selecciona automáticamente la primera opción válida
                var $dirDestino = $('#DirDestino');
                var $firstValidOption = $dirDestino.find('option:not([value=""]):first');
                if ($firstValidOption.length) {
                    $dirDestino.val($firstValidOption.val());
                    var zonaDefault = $firstValidOption.attr('Zona') || "";
                    $("#Zona").val(zonaDefault);
                    $dirDestino.trigger('change');
                }
            });
    }
}
function validarDirDestino2(textoIngresado) {
    let cantidadRestante = parseInt(200) - parseInt(textoIngresado.length);
    if (cantidadRestante <= 0) {
        $('#DirDestino2').addClass('bg-danger text-white');
        $('#span_caracteresDirDestino2').html(`Excediste el límite de caracteres permitidos.`);
    } else {
        $('#DirDestino2').removeClass('bg-danger text-white');
        $('#span_caracteresDirDestino2').html(``);
    }

}
function reporteLibroSaldo(estado) {
    if (estado === '') {
        ObtieneDeudasSaldos();
    }
    window.open("/Ventas/ReporteLibroSaldo?Vista=DetallePago&CardCode=" + $("#CardCode").val(), null, 'width=900,height=550,top=100,left=100,toolbar=no,location=no,status=no,menubar=no')
}
//Tickets vinculados :
//funcion para mostrar buscador de tickets a vincular

function mostrarTableTicketsVinculados(valor) {
    var LugarDestino = $("#LugarDestino").val();
    //mostrar el llenado de tickets vinculados si la eleccion es SI
    if (valor == "SI" && (LugarDestino === "LOCAL" || LugarDestino === "EXTERNO")) { $("#divVinculados").show(); }
    else { $("#selectVinculado").val('NO'); $("#divVinculados").hide(); $("#DetallesVinculados  tbody").html(''); cont = 0; }
}
function buscarTicketAVincular(DocNum) {
    var DocNumPrincipal = $("#DocNum").val();
    if (DocNum == DocNumPrincipal) { swal.fire("No puede vincular el mismo número de ticket, hace redundancia."); } else {
        var parametros = { "DocNum": DocNum };
        $.ajax('/Ventas/buscarTicketAVincular',
            {
                data: parametros,
                dataType: 'json',
                cache: false,
                type: 'post'
            })
            .done(function (response) {
                var almacenSalida = "";
                var tabladetallesPedidos = $("#detallesPedidos");

                if (tabladetallesPedidos.find('tbody').children().length > 0) {
                    var encontrado = false;
                    tabladetallesPedidos.find('tbody').children().each(function () {
                        var almacenSalidaValue = $(this).find("input[id^='AlmacenSalida']").val();

                        if ((almacenSalidaValue === "16" || almacenSalidaValue === "03" ) && !encontrado && $(this).find("input[id^='Verificar']:checked").length > 0) {
                            encontrado = true;
                            almacenSalida = almacenSalidaValue;
                        }
                    });

                    if (encontrado && almacenSalida != "" && almacenSalida != null) {
                        var Zona = $("#Zona").val();
                        var Referencia = $("#Referencia").val();
                        var DireccionActual = "";
                        if ($.trim($("#DirDestino2").val()) !== "") {
                            DireccionActual = $("#DirDestino2").val();
                        } else {
                            DireccionActual = $("#Calle").val();
                        }
                        if (response != null && response != "" && DireccionActual != null && DireccionActual != "") {
                            var DireccionEnvio;
                            DireccionEnvio = response.Det3[0].Calle;
                            if (response.Det3.length > 1) {
                                DireccionEnvio = response.Det3[1].Calle;
                            }
                            if (response.Det2.length > 0) {
                                if (response.LugarDestino === 'LOCAL') {
                                    if (response.Det2[0].AlmacenSalida === almacenSalida) {
                                        if (DireccionEnvio === DireccionActual) {
                                            if ((response.Zona && response.Zona.trim().toLowerCase() === Zona.trim().toLowerCase()) || (!response.Zona && !Zona.trim())) {
                                                $("#DocNumVinculado").val(response.DocNum);
                                                $("#RucClienteVinculado").val(response.CardCode);
                                                $("#ClienteVinculado").val(response.CardName);
                                                $("#MontoFinalVinculado").val(response.MontoFinal);
                                            }
                                            else { swal.fire("No puede vincular, las ZONAS de envio son diferentes."); }
                                        } else { swal.fire("No puede vincular, las direcciones de envio son diferentes."); }
                                    }
                                    else {
                                        swal.fire("Error: almacén de salida distintos para envio a domicilio.");
                                    }
                                }
                                else if (response.LugarDestino === 'EXTERNO') {
                                    if ((response.Zona && response.Zona.trim().toLowerCase() === Zona.trim().toLowerCase()) || (!response.Zona && !Zona.trim()) &&
                                        (response.Referencia && response.Referencia.trim().toLowerCase() === Referencia.trim().toLowerCase()) || (!response.Referencia && !Referencia.trim())) {

                                        $("#DocNumVinculado").val(response.DocNum);
                                        $("#RucClienteVinculado").val(response.CardCode);
                                        $("#ClienteVinculado").val(response.CardName);
                                        $("#MontoFinalVinculado").val(response.MontoFinal);
                                    }
                                    else { swal.fire("No puede vincular, las ZONAS de envio o REFERENCIA son diferentes."); }
                                }
                            }
                            else {
                                swal.fire("Error: El ticket no tiene pedidos con almacen de origen Domicilio y Agencia.");
                            }

                        }
                        else { swal.fire("No puede vincular, no existe direccion actual"); }
                    }
                    else { Swal.fire("No tiene orden de venta checkeada.") }
                }
            });
    }
}
var cont = 0;
function agregarTicketVinculado(DocEntry, DocNum, CardCode, CardName, MontoFinal) {
    if (DocNum == null || DocNum == "") { swal.fire("No es valido el DocNum a vincular", "Verifique", "warning"); return; }
    if (CardCode == null || CardCode == "" || CardName == null || CardName == "") { swal.fire("No es valido el cliente de ticket vincular.", "Verifique", "warning"); return; }
    if (MontoFinal == 0 || MontoFinal == "") { swal.fire("No es valido el monto final.", "Verifique", "warning"); return; }
    //validamos ticket no repetido en la tabla
    var valorDup = false;
    $("#DetallesVinculados tr").find('td:eq(2) input').each(function () {
        iden = $(this).val();
        if (iden == DocNum) { swal.fire("El valor ya se encuentra en la tabla", "Verifique", "warning"); valorDup = true; }
    });
    if (valorDup == false) {
        $("#DetallesVinculados > tbody").append("<tr><td hidden><input class='form-control text-center' name='Det7[" + cont + "].DocEntry' value='" + DocEntry + "' readonly /></td><td><input class='form-control text-center' name='Det7[" + cont + "].Linea' value='" + (cont + 1) + " ' readonly /></td><td><input class= 'form-control text-center' name = 'Det7[" + cont + "].DocNumVinc' value = '" + DocNum + "'  readonly /></td ><td hidden><input class='form-control text-center' name='Det7[" + cont + "].CardCode' value='" + CardCode + "'  readonly  /></td><td><input class='form-control text-center' name='Det7[" + cont + "].CardName' value='" + CardName + "'  readonly  /></td><td><input class='form-control text-center' name='Det7[" + cont + "].MontoFinal' value='" + MontoFinal + "'  readonly  /></td><td>" +
            "<button type='button' onclick='borrarTrTablaVinculados(this)' class='btn btn-danger btn-sm p-2'><i class='icon icon-bin'></i></button></td></tr>");
        cont = cont + 1;

        $("#searchInput").val("");
        $("#DocNumVinculado").val("");
        $("#MontoFinalVinculado").val("");
        $("#ClienteVinculado").val("");
        $("#RucClienteVinculado").val("");
    }
}
function borrarTrTablaVinculados(dom) {
    var i = -1;
    $(dom).closest('tr').remove();
    cont = cont - 1;
    $("#DetallesVinculados" + " tr").each(function (index, htm) {
        iden = $(htm).find("td:eq(0) input");
        iden.attr("name", "Det7[" + i + "].DocEntry");
        iden1 = $(htm).find("td:eq(1) input");
        iden1.attr("name", "Det7[" + i + "].Linea");
        iden1.val(i + 1);
        iden2 = $(htm).find("td:eq(2) input");
        iden2.attr("name", "Det7[" + i + "].DocNumVinc");
        iden3 = $(htm).find("td:eq(3) input");
        iden3.attr("name", "Det7[" + i + "].CardCode");
        iden4 = $(htm).find("td:eq(4) input");
        iden4.attr("name", "Det7[" + i + "].CardName");
        iden5 = $(htm).find("td:eq(5) input");
        iden5.attr("name", "Det7[" + i + "].MontoFinal");
        i = i + 1;
    })
}

function validacionVerificarMontos(estado) {
    var object;
    if (estado === '') {
        object = $("#CreaTicketVenta").serialize();
    } else { object = $("#EditaTicketVenta").serialize(); }
    $.ajax('/Ventas/CalcularMontos',
        {
            data: object,
            dataType: 'json',
            cache: false,
            type: 'post'
        })
        .done(function (response) {
            if (estado === '') { 
                $('#MontoTotal').val(response.MontoTotal);
                $("#DescuentoNC").val(response.DescuentoNC);
                $("#MontoFinal").val(response.MontoFinal);
            } else {
                $("#MontoFinal").val(response.MontoFinal);
            }
        });
}
function clienteRegalos() {
    window.open("/Ventas/ReporteClienteRegalos?CardCode=" + $("#CardCode").val(), null, 'width=800,height=450,top=100,left=100,toolbar=no,location=no,status=no,menubar=no')
}
function muestraCampo() {

    if ($("#LugarDestino").val() == 'RECOJO') {
        $('#divNombrePer').hide();
        $('#divTelfPer').hide();
        $('#divTipoDocPer').hide();
        $('#divDocPer').hide();
    } else {
        $('#divNombrePer').show();
        $('#divTelfPer').show();
        $('#divTipoDocPer').show();
        $('#divDocPer').show();
    }

    if ($("#LugarDestino").val() == 'EXTERNO' || $("#LugarDestino").val() == 'Agencia Courier') {
        $("#CamposAgDom").show(); $("#CamposAgencia").show(); $("#Referencia").val(""); $("#Zona").val("AGENCIA")
    }
    else if ($("#LugarDestino").val() == 'LOCAL') {
        $("#Agencia").val(""); $("#EnvioAgencia").val(""); $("#Referencia").val("");
        $("#CamposAgDom").show(); $("#CamposAgencia").hide();
        var zonaDefault = $("#DirDestino option:selected").attr("Zona") || "";
        $("#Zona").val(zonaDefault);
    }
    else {
        $("#Agencia").val(""); $("#EnvioAgencia").val("");$("#Referencia").val("");$("#DirDestino").val("");$("#CamposAgDom").hide(); $("#DirDestino").val(""); $("#DirDestino2").val(""); $("#Ubigeo").val(""); $("#Calle").val(""); $("#Distrito").val(""); $("#Provincia").val(""); $("#Departamento").val(""); $("#Ubigeo2").val(""); $("#Calle2").val(""); $("#Distrito2").val(""); $("#Provincia2").val(""); $("#Departamento2").val("");

    }
}
//Solo se puede accionar cuando colocamos Agencia Courier y Modo envio (oficina de agencia)
function buscarOficina() {
    $.ajax({
        url: 'buscarOficinas',
        type: 'POST',
        data: { nombreAgencia: $('#Agencia').val() },
        success: function (response) {

            let filaTabla = '';
            $('#Oficinas > tbody').html('');

            if (response.length > 0) {
                for (let indice in response) {
                    filaTabla += `<tr>
                                    <td>${response[indice].Calle}</td>
                                    <td>${response[indice].Distrito}</td>
                                    <td>${response[indice].Provincia}</td>
                                    <td>${response[indice].Departamento}</td>
                                    <td>${response[indice].Ubigeo}</td>
                                    <td><button class="btn btn-primary btn-sm" data-dismiss="modal" onclick="agregarDatosOficina('${response[indice].Calle}','${response[indice].Ubigeo}','${response[indice].Distrito}','${response[indice].Provincia}','${response[indice].Departamento}')">Seleccionar</button></td>
                                </tr>`;
                }


            } else {
                filaTabla += `<tr>
                                    <td colspan="6" class="text-danger"><b>No se encontraron registros con la agencia seleccionada.</b></td>
                                </tr>`;
            }
            $('#Oficinas > tbody').html(filaTabla);
        }
    });
}
//se usa para PO y para crear ticket venta 
function validacionCliente(estado) {
    if (estado === '') {
        $('#infoFechaTicket').on('change', function () {
            //limpiamos datos de un posible cliente anterior
            $('#infoListaClientes input').val("");
            $('#infoCliente input').val("");
            $('#MontoTotal').val("0.00");
            $('#infoGastoEnvio input').val("");
            $('#infoFlete input').val("");
            $('#infoDescuentoNC input').val("");
            $('#infoDeudaCliente input').val("");
            $('#infoDeudaEmpresa input').val("");
            $('#infoMontoFinal input').val("");
            $('#detallesPedidos').html("");
            $('#div_detallesPedidos').css('display', 'none');
            $('#DirDestino').html('');
            var parametros = { "Fecha": $('#infoFechaTicket input').val() };
            $.ajax('/Ventas/infoListaClientes',
                {
                    data: parametros,
                    dataType: 'html',
                    cache: false,
                    type: 'post'
                })
                .done(function (response) {
                    $('#infoListaClientes input').html(response);
                    verificarClientePO();     //Inicializamos para tickets que vienen desde PedidoOnline
                });
        });
        $('#CardName').on('change', function () {
            $('#infoCliente input').val("");

            $('#infoCliente input').val($("#infoListaClientes #ListaClientes option[value='" + $("#infoListaClientes input").val() + "']").attr("CardCode"));
            verificarClientePO();     //Inicializamos para tickets que vienen desde PedidoOnline
            $('#infoGastoEnvio input').val("");
            $('#infoFlete input').val("");
            $('#infoDescuentoNC input').val("");
            $('#infoDeudaCliente input').val("");
            $('#infoDeudaEmpresa input').val("");
            $('#MontoTotal').val("");
            $('#infoMontoFinal input').val("");
            ObtieneDeudasSaldos();
            listarNotasDeCreditoV();
        });
    }
}
/*********************************** */
//Funcion unica para pedidos online
function verificarClientePO() {
    let tipo = `@Request["Tipo"]`;

    if (tipo == 'PedidoOnline') {
        let clienteCorrecto = 1;
        $('.cls-pedidoOnline').addClass('disabled');
        $('#nav-det-pedidos-tab').attr('onclick', '');
        $.ajax({
            url: '/Ventas/VerificarCliente',
            type: 'POST',
            dataType: 'json',
            data: { DocEntry: $('#DocEntry').val(), CardCode: $('#CardCode').val() },
        }).done(function (response) {
            if (response != '') {
                clienteCorrecto = 0;
                Swal.fire(response);
            } else {
                clienteCorrecto = 1;
                $('.cls-pedidoOnline').removeClass('disabled');
                $('#nav-det-pedidos-tab').attr('onclick', 'validarTabDetPedido()');
            }

        }).fail(function (jqXHR, textStatus, errorThrown) {
            console.log(jqXHR.status + textStatus + errorThrown);
        });
        return clienteCorrecto;
    }
}
/************************************ */
//solo es usado en el crear ticket venta
function listarNotasDeCreditoV() {
    var object = $("#CreaTicketVenta").serialize();
    $.ajax('/Ventas/infoListaNotasDeCreditoV',
        {
            data: object,
            dataType: 'html',
            cache: false,
            type: 'post'
        })
        .done(function (response) {
            if (response != '') {
                $("#div_detallesNC").css('display', 'block');
                $("#detallesNC > tbody").html(response);
            }
            else {
                $("#div_detallesNC").css('display', 'none');
                $("#detallesNC > tbody").html('');
            }
            validacionVerificarMontos('');
        });
}
function ObtieneDeudasSaldos() {
    var parametros = { "CardCode": $("#CardCode").val() };
    $.ajax('/Ventas/ObtieneDeudasSaldos',
        {
            data: parametros,
            dataType: 'json',
            cache: false,
            type: 'post'
        })
        .done(function (response) {
            $("#RptSaldo").html("Saldo:  S/" + response.SaldoActual);
            if (response.SaldoActual >= 0) { $("#DeudaEmpresa").val(response.SaldoActual); }
            else { $("#DeudaCliente").val(response.SaldoActual * -1); }
        });
}
function verificacionDatos(estado) {
    $("#Verificar").on('click', function (e) {
        e.preventDefault();
        var tipoVenta;

        if (estado === '') {
            var tipoVentaInput = $('#TipoVentaInput');
            var tipoVentaSelect = $('#TipoVentaSelect');


            if (tipoVentaSelect.val() != '') {
                tipoVenta = tipoVentaSelect.val();
            } else {
                tipoVenta = tipoVentaInput.val();
            }
        } else { tipoVenta = $('#TipoVenta').val(); }
        var formSelector = estado === '' ? "#CreaTicketVenta" : "#EditaTicketVenta";
        var object = $(formSelector).serializeArray();

        var dataObject = {};
        $.each(object, function (i, field) {
            dataObject[field.name] = field.value;
        });

        dataObject.TipoVenta = tipoVenta;

        $.ajax({
            url: '/Ventas/ValidarDatosTicket',
            data: dataObject,
            dataType: 'html',
            cache: false,
            type: 'post'
        })
            .done(function (response) {
                if (response != "true") {
                    Swal.fire(response);
                } else {
                    Swal.fire({
                        title: 'Datos Ok',
                        html: '<button type="button" id="cancelar" class="btn btn-secondary">Cancelar</button> <button type="button" id="enviarForm" class="btn btn-success"><i class="icon icon-plus"></i> Enviar </button>',
                        showCloseButton: false,
                        showCancelButton: false,
                        showConfirmButton: false
                    });

                    $("#enviarForm").on('click', function () {
                        enviarFormulario();
                    });

                    $("#cancelar").on('click', function () {
                        Swal.close();
                    });
                }
            })
            .fail(function (xhr, status, error) {
                console.error('Error:', error);
            });
    });
}
function openPreliminar(docEntry) {
    // Construir la URL para la acción en el servidor
    var url = '@Url.Action("PreliminarLayoutOV_Ticket", "Ventas")' + '?DocEntry=' + docEntry;

    // Abrir una nueva pestaña con el PDF generado
    window.open(url, '_blank', 'noopener,noreferrer');
}


function validarLugarDestino(valor) {
    if (valor == 'Centro' || valor == 'Arriola') {
        document.querySelector('#div_AlmProcedencia').style.display = 'block'
    } else {
        document.querySelector('#div_AlmProcedencia').style.display = 'none'
    }

    return;
}

function limpiarAlmProcedencia(lugarDestino) {
    if (lugarDestino !== 'Arriola' && lugarDestino !== 'Centro') {
        document.getElementById('AlmProcedencia').value = '';
    }
}