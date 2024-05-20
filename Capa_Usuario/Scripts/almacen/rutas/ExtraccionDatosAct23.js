$(document).ready(function(){
    buscarDocumento(30); 
    actualizarFormulario();
});
function buscarDocumento(lineas)
{
    $('#buscar').on('click', function () {
        var parametros = { "DocNum": $('#DocNum').val() };
        $.ajax('/Rutas/BuscaOrdenRegistroRutas',
            {
                data : parametros,
                dataType: 'json',
                cache: false,
                type: 'post'
            })
            .done(function (response) {
                alert("buscando--"+ response.DocNum);
                if (response.DocEntry == -1) { alert("No ingreso Documento valido"); }
                if (response.DocEntry == 0) { alert("No Existe Documento");}
                if (response.DocEntry > 0)
                {
                    generarPdfs(response.DocNum)
                    validacionTipoRuta(response.TipoRuta);
                    valicionEstadoDocumento(response.Estado);
                    validacionTransportista(response.TransCod);
                    validacionPlaca(response.PlacaCod);
                    validacionCopiloto("1", response.CopilDesc);
                    validacionCopiloto("2", response.Copil2Desc);
                    validacionCopiloto("3", response.Copil3Desc);
                    validacionCopiloto("4", response.Copil4Desc);
                    validacionOrigen(response.AlmOrigenCod);
                    validacionDestino(response.AlmDestinoCod);
                    validacionPropietario(response.PropietarioDesc);
                    validacionSerieDocumento(response.DocSerie);
                    validacionNroDocumento(response.DocEntry,response.DocNum);
                    validacionObservaciones(response.Observaciones);
                    validacionFechas(response.FechaCont, response.FechaDoc);
                    validacionHoras(response.HoraI, response.HoraT);
                    detalles(lineas, response);
                    validacionSocio(lineas, response, response.ListaDetalleRegistroRutas);
                    /*aqui me QUEDE*/
                }
                /*
                detallesIni(lineas);
                validacionSocio(lineas);*/
            });
    });
}
function validacionTotalCajas(orden,lineas)
{   
        $('#contenido-nroCajas' + orden).on('change', function () {
            var cajas = 0;
            for (i = 1; i <= lineas; i++) {
                lineaCaja = $('#contenido-nroCajas' + i + ' input').val() * 1;
                if (lineaCaja < 0) {
                    alert("No puedes ingresar cajas negativas");
                    lineaCaja = 0;
                    $('#contenido-nroCajas' + i + ' input').val("");
                }
                cajas += lineaCaja;
            }
            $('#contenido-totalCajas').html("<strong>" + cajas + "</strong>");
        });
}
//usado en detalles
function validacionTabla2(lineas)
{         
    var parametros = { "lineas": lineas };
          $.ajax('/Rutas/infoTabla2',
                {
                data : parametros,
                dataType :'html',
                cache : false,
                type : 'post'
                })
          .done(function(response)
          {
              $('#tabla2').html(response);
          });
}
function validacionTipoRuta(TipoRuta)
{
    var parametros = {"TipoRuta": TipoRuta};
          $.ajax('/Rutas/infoTipoRuta',
              {
                data: parametros,
                dataType :'html',
                cache : false,
                type : 'post'
                })
          .done(function(response)
                {
                       $('#contenido-tipoRuta').html(response);
                });
}
function valicionEstadoDocumento(Estado)
{
    var parametros = { "Estado": Estado };
           $.ajax('/Rutas/infoEstadoDocumento',
               {
                data: parametros,
                dataType :'json',
                cache : false,
                type : 'post'
                })
          .done(function(response)
                {
                    $('#contenido-estadoDocumento #estado').attr('value',response['EstadoV']);
                    $('#contenido-estadoDocumento #estado').html(response['EstadoD']);
                    $('#contenido-estadoDocumento #documento').html(response['Documento']);
                    $('#contenido-estadoDocumento #documentoDomicilios').html(response['DocumentoDomicilios']);     
                });
}
function validacionTransportista(TransCod)
{       
    var parametros = { "TransCod": TransCod };
         $.ajax('/Rutas/infoTransportista',
             {
                data: parametros,
                dataType :'html',
                cache : false,
                type : 'post'
                })
          .done(function(response)
                {
                       $('#contenido-transportista').html(response);
                });        
}
function validacionPlaca(PlacaCod)
{
    var parametros = { "PlacaCod": PlacaCod };  
        $.ajax('/Rutas/infoPlaca',
            {
                data: parametros,
                dataType :'html',
                cache : false,
                type : 'post'
                })
          .done(function(response)
                {
                       $('#contenido-placa').html(response);
                });   
}
function validacionCopiloto(nro,desc)
{
    var nroId = "";
    if (nro != "1") { nroId = nro; }
    var parametros = { "nro": nroId, "desc": desc };
        $.ajax('/Rutas/infoCopiloto',
                {
                data : parametros,
                dataType :'html',
                cache : false,
                type : 'post'
                })
          .done(function(response)
                {
                       $('#contenido-copiloto'+nroId).html(response);            
                }); 
}
function validacionOrigen(AlmOrigenCod)
{
    var parametros = { "AlmOrigenCod": AlmOrigenCod };
        $.ajax('/Rutas/infoOrigen',
                {
                data : parametros,
                dataType :'html',
                cache : false,
                type : 'post'
                })
          .done(function(response)
                {
                       $('#contenido-origen').html(response);          
                });   
}
function validacionDestino(AlmDestinoCod)
{
    var parametros = { "AlmDestinoCod": AlmDestinoCod };
        $.ajax('/Rutas/infoDestino',
                {
                data : parametros,
                dataType :'html',
                cache : false,
                type : 'post'
                })
          .done(function(response)
                {
                       $('#contenido-destino').html(response);       
                });        
}
function validacionPropietario(PropietarioDesc)
{
    var parametros = { "PropietarioDesc": PropietarioDesc };
    $.ajax('/Rutas/infoPropietario',
        {
            data : parametros,
            dataType: 'html',
            cache: false,
            type: 'post'
        })
        .done(function (response) {
            $('#contenido-propietario').html(response);
        });
}
function validacionSerieDocumento(DocSerie)
{
    var parametros = { "DocSerie": DocSerie };
    $.ajax('/Rutas/infoSerieDocumento',
                {
                data : parametros,
                dataType :'html',
                cache : false,
                type : 'post'
                })
          .done(function(response)
                {
                       $('#contenido-serieDocumento').html(response); 
                });   
}
function validacionNroDocumento(DocEntry, DocNum)
{
    var parametros = { "DocEntry":DocEntry, "DocNum": DocNum };
    $.ajax('/Rutas/infoNroDocumento',
                {
                data : parametros,
                dataType :'html',
                cache : false,
                type : 'post'
                })
          .done(function(response)
                {
                       $('#contenido-nroDocumento').html(response);
                });   
}
function validacionObservaciones(Observaciones)
{
    var parametros = { "Observaciones": Observaciones };
    $.ajax('/Rutas/infoObservaciones',
                {
                data : parametros,
                dataType :'html',
                cache : false,
                type : 'post'
                })
          .done(function(response)
                {
                       $('#contenido-observaciones').html(response);
               
                });   
}
function validacionFechas(FechaCont, FechaDoc)
{
    $('#contenido-FechaCont input').val(FechaCont);
    $('#contenido-FechaDoc input').val(FechaDoc);
}
function validacionHoras(HoraI,HoraT)
{
    $('#contenido-horaInicio input').val(HoraI);
    $('#contenido-horaTermino input').val(HoraT);
}
//detalles de registro
function detalles(lineas, resp)
{
    validacionTabla2(lineas);
    var parametros = {"FechaCont": resp.FechaCont,"tipoRuta": resp.TipoRuta};
    $.ajax('/Rutas/infoDetalles',
        {
            data: parametros,
            dataType: 'json',
            cache: false,
            type: 'post'
        })
        .done(function (response) {
            $('#contenido-totalCajas').html("<strong>" + resp.TotalCajas + "</strong>");
            $('#contenido-FechaCont').css('display', response.EstiloFechaCont);
            $('#titulo-socioNegocio').html(response.InfoTituloSocioNegocio);
            $('#titulo-ruc').html(response.InfoTituloRuc);
            $('#titulo-otros').html(response.InfoTituloOtros);
            $('#titulo-telefonoRS').html(response.InfoTituloTelefonoRS);
            $('#titulo-direccionD').html(response.InfoTituloDireccionD);
            $('#titulo-nroVenta').html(response.InfoTituloNroVenta);
            $('#titulo-nroGuias').html(response.InfoTituloNroGuias);
            $('#titulo-monto').html(response.InfoTituloMonto);
            $('#titulo-vendedor').html(response.InfoTituloVendedor);
            $('#titulo-cajas').html(response.InfoTituloCajas);
            $('#titulo-personaEncargada').html(response.InfoTituloPersonaEncargada);
            $('#titulo-dniPE').html(response.InfoTituloDniPE);
            $('#titulo-costoEnvio').html(response.InfoTituloCostoEnvio);
            $('#titulo-verificado').html(response.InfoTituloVerificado);

            for (var h = 1; h <= lineas; h++) {
                $('#contenido-ruc' + h).html(response.InfoContenidoRuc);
                $('#contenido-otros' + h).html(response.InfoContenidoOtros);
                $('#contenido-telefonoRS' + h).html(response.InfoContenidoTelefonoRS);
                $('#contenido-montoTotalVenta' + h).html(response.InfoContenidoMontoTotalVenta);
                $('#contenido-listaVentas' + h).html(response.InfoContenidoListaVentas);
                $('#contenido-listaGuias' + h).html(response.InfoContenidoListaGuias);
                $('#contenido-monto' + h).html(response.InfoContenidoMonto);
                $('#contenido-vendedor' + h).html(response.InfoContenidoVendedor);
                $('#contenido-direccionD' +h).html(response.InfoContenidoDireccionD);
                $('#contenido-nroCajas' + h).html(response.InfoContenidoNroCajas);
                $('#contenido-personaEncargada' + h).html(response.InfoContenidoPersonaEncargada);
                $('#contenido-dniPE' + h + '').html(response.InfoContenidoDniPE);
                $('#contenido-costoEnvio' + h).html(response.InfoContenidoCostoEnvio);
                $('#contenido-verificado' + h).html(response.InfoContenidoVerificado);

                $('#contenido-ruc' + h).css('display', response.EstiloContenidoRuc);
                $('#contenido-telefonoRS' + h).css('display', response.EstiloContenidoTelefonoRS);
                $('#contenido-direccionD' + h).css('display', response.EstiloContenidoDireccionD);
                $('#contenido-personaEncargada' + h).css('display', response.EstiloContenidoPersonaEncargada);
                $('#contenido-dniPE' + h).css('display', response.EstiloContenidoDniPE);
                $('#contenido-costoEnvio' + h).css('display', response.EstiloContenidoCostoEnvio);
            }
        });
}
function validacionSocio(lineas, resp, ListDet)
{
        var parametros ={"FechaCont": resp.FechaCont };
        
        $.ajax('/Rutas/infoSocio',
                {
            data : parametros,
            dataType :'html',
            cache : false,
            type : 'post'
                })
          .done(function(response)
                {
                   for(var h=1;h<=lineas;h++)
                   {
                       $('#contenido-SocioNegocio' + h).html(response);
                       validacionRuc(h);
                       validaciondirDest(h);
                       validacionMontoTotalVenta(h);
                       //validacionTotalCajas(h, lineas);
                   }
                   for (var j = 0; j < ListDet.length; j++) 
                   {
                       $('#contenido-SocioNegocio' + (j + 1) + ' input').val(ListDet[j].SocioDesc);
                       $('#contenido-ruc' + (j + 1)).text(ListDet[j].Ruc);
                       $('#contenido-otros' + (j + 1) + ' input').val(ListDet[j].Otros);
                       $('#contenido-telefonoRS' + (j + 1) + ' input').val(ListDet[j].Telefono);
                       $('#contenido-direccionD' + (j + 1) + ' input').val(ListDet[j].DirDest);
                       muestraMontoTotalVenta(ListDet[j].SocioCod, resp.FechaCont, (j + 1), ListDet[j].MontoTotal);
                       muestraVentas(ListDet[j], j + 1);
                       $("#contenido-nroCajas" + (j + 1) + " input").val(ListDet[j].Cajas);
                       $("#contenido-personaEncargada" + (j + 1) + " input").val(ListDet[j].PerEnc);
                       $("#contenido-dniPE" + (j + 1) + " input").val(ListDet[j].DniPerEnc);
                       $("#contenido-costoEnvio" + (j + 1) + " input").val(ListDet[j].CostoEnvio);
                       $('#contenido-verificado' + (j + 1) + ' input').attr("checked", ListDet[j].Verificado);
                       
                   }
                });
}
function validacionRuc(orden)
{
    $('#contenido-SocioNegocio' + orden).on('change', function () {
        $('#contenido-listaVentas' + orden).html("<p></p>");
        $('#contenido-listaGuias' + orden).html("<p></p>");
        $('#contenido-monto' + orden).html("<p></p>");
        $('#contenido-vendedor' + orden).html("<p></p>");
                var CardCode = $("#contenido-SocioNegocio"+orden+" #ListaSocios option[value='"+$("#contenido-SocioNegocio"+orden+" input").val()+"']").attr("SocioCod");
                if(!CardCode){CardCode=""};
                var parametros = {"CardCode": CardCode};               
            $.ajax('/Rutas/infoRuc',
                        {
                                data : parametros,
                                dataType : 'html',
                                cache : false,
                                type : 'post',
                        })
                .done(function(response)
                {
                        $('#contenido-ruc' + orden).text(response);
                });
        });
}
function validaciondirDest(orden)
{
        $('#contenido-SocioNegocio'+orden).on('change',function(){
                var CardCode = $("#contenido-SocioNegocio"+orden+" #ListaSocios option[value='"+$("#contenido-SocioNegocio"+orden+" input").val()+"']").attr("SocioCod");
                if(!CardCode){CardCode=""};
                var parametros ={"CardCode": CardCode,"orden": orden};
                $.ajax('/Rutas/infoDireccionD',
                        {
                                data : parametros,
                                dataType : 'html',
                                cache : false,
                                type : 'post',
                        })
                .done(function(response)
                        {
                                $('#contenido-direccionD'+orden).html(response);
                        });
                
        });
}
function validacionMontoTotalVenta(orden)
{
        $('#contenido-SocioNegocio'+orden+'').on('change',function(){
                var CardCode = $("#contenido-SocioNegocio"+orden+" #ListaSocios option[value='"+$("#contenido-SocioNegocio"+orden+" input").val()+"']").attr("SocioCod");
                if(!CardCode){CardCode=""};
                var parametros={
                    "CardCode": CardCode,
                    "FechaCont": $('#contenido-FechaCont input').val(),
                    "orden": orden
                };
            $.ajax('/Rutas/infoMontoTotalVenta',
                        {
                            data : parametros,
                            dataType : 'html',
                            cache : false,
                            type : 'post',
                        })
                .done(function(response)
                        {
                                $('#contenido-montoTotalVenta'+orden).html(response);
                                validacionVentas(orden);
                        });
        });
}
function muestraMontoTotalVenta(CardCode, FechaCont, orden, MontoTotal) {
    var parametros = {
        "CardCode": CardCode,
        "FechaCont": FechaCont,
        "orden": orden,
        "MontoTotal": MontoTotal
    };
    $.ajax('/Rutas/infoMontoTotalVenta',
        {
            data: parametros,
            dataType: 'html',
            cache: false,
            type: 'post'
        })
        .done(function (response) {
            $('#contenido-montoTotalVenta' + orden).html(response);
        });
}
function validacionVentas(orden)
{
        $('#contenido-montoTotalVenta'+orden).on('change',function(){     
                var parametros ={
                        "CardCode": $("#contenido-SocioNegocio" + orden + " #ListaSocios option[value='" + $("#contenido-SocioNegocio" + orden + " input").val() + "']").attr("SocioCod"),
                        "FechaVenta": $('option:selected', '#contenido-montoTotalVenta'+orden+' select').attr('fe'),
                        "MontoTotalVenta": $('#contenido-montoTotalVenta' + orden + ' select').val()
                };
            $.ajax('/Rutas/infoListaVentas',
                        {
                                data : parametros,
                                dataType : 'json',
                                cache : false,
                                type : 'post',
                        })
                .done(function(response)
                        {
                    $('#contenido-listaVentas' + orden).html(response.InfoContenidoListaVentas);
                    $('#contenido-listaGuias' + orden).html(response.InfoContenidoListaGuias);
                    $('#contenido-monto' + orden).html(response.InfoContenidoMonto);
                    $('#contenido-vendedor' + orden).html(response.InfoContenidoVendedor);
                        });             
        });       
}
function muestraVentas(ListDet,orden)
{
    var parametros = {"CardCode": ListDet.CardCode,
        "FechaVenta": "",
        "MontoTotalVenta": ListDet.MontoTotalVenta,
        "Ventas": ListDet.Ventas,
        "Guias": ListDet.Guias,
        "Montos": ListDet.Montos,
        "VendedorCod": ListDet.VendedorCod,
        "VendedorDesc": ListDet.VendedorDesc
    };
    $.ajax('/Rutas/infoListaVentas',
        {
            data: parametros,
            dataType: 'json',
            cache: false,
            type: 'post',
        })
        .done(function (response) {
            $('#contenido-listaVentas' + orden).html(response.InfoContenidoListaVentas);
            $('#contenido-listaGuias' + orden).html(response.InfoContenidoListaGuias);
            $('#contenido-monto' + orden).html(response.InfoContenidoMonto);
            $('#contenido-vendedor' + orden).html(response.InfoContenidoVendedor);
        });
}

/*******************act********************/
function actualizarFormulario()
{
    $('#contenido-actualizar').on('click', function () {
        if (confirm("¿Esta seguro/a de Actualizar Documento?"))
        {
            var parametros = {

                "DocEntry": $('#contenido-nroDocumento input').attr('DocEntry')
                , "DocSerie": $('#contenido-serieDocumento input').val()
                , "DocNum": $('#contenido-nroDocumento input').val()
                , "TipoRuta": $('#contenido-tipoRuta select').val()
                , "TransCod": $("#contenido-transportista select option:selected").val()
                , "TransDesc": $("#contenido-transportista select option:selected").text()
                , "PlacaCod": $("#contenido-placa select option:selected").val()
                , "PlacaDesc": $("#contenido-placa select option:selected").text()
                , "CopilCod": ($("#contenido-copiloto #ListaCopilotos option[value='" + $("#contenido-copiloto input").val() + "']").attr("CopilCod")) ? $("#contenido-copiloto #ListaCopilotos option[value='" + $("#contenido-copiloto input").val() + "']").attr("CopilCod") : "NULL"
                , "CopilDesc": ($("#contenido-copiloto input").val()) ? $("#contenido-copiloto input").val() : "NULL"
                , "Copil2Cod": ($("#contenido-copiloto2 #ListaCopilotos2 option[value='" + $("#contenido-copiloto2 input").val() + "']").attr("Copil2Cod")) ? $("#contenido-copiloto2 #ListaCopilotos2 option[value='" + $("#contenido-copiloto2 input").val() + "']").attr("Copil2Cod") : "NULL"
                , "Copil2Desc": ($("#contenido-copiloto2 input").val()) ? $("#contenido-copiloto2 input").val() : "NULL"
                , "Copil3Cod": ($("#contenido-copiloto3 #ListaCopilotos3 option[value='" + $("#contenido-copiloto3 input").val() + "']").attr("Copil3Cod")) ? $("#contenido-copiloto3 #ListaCopilotos3 option[value='" + $("#contenido-copiloto3 input").val() + "']").attr("Copil3Cod") : "NULL"
                , "Copil3Desc": ($("#contenido-copiloto3 input").val()) ? $("#contenido-copiloto3 input").val() : "NULL"
                , "Copil4Cod": ($("#contenido-copiloto4 #ListaCopilotos4 option[value='" + $("#contenido-copiloto4 input").val() + "']").attr("Copil4Cod")) ? $("#contenido-copiloto4 #ListaCopilotos4 option[value='" + $("#contenido-copiloto4 input").val() + "']").attr("Copil4Cod") : "NULL"
                , "Copil4Desc": ($("#contenido-copiloto4 input").val()) ? $("#contenido-copiloto4 input").val() : "NULL"
                , "FechaCont": $('#contenido-FechaCont input').val()
                , "FechaDoc": $('#contenido-FechaDoc input').val()
                , "AlmOrigenCod": $("#contenido-origen select option:selected").val()
                , "AlmOrigenDesc": $("#contenido-origen select option:selected").text()
                , "AlmOrigenDesc2": $("#contenido-origen select option:selected").text()
                , "AlmDestinoCod": $("#contenido-destino select option:selected").val()
                , "AlmDestinoDesc": $("#contenido-destino select option:selected").text()
                , "AlmDestinoDesc2": $("#contenido-destino select option:selected").text()
                , "PropietarioCod": ($("#contenido-propietario #ListaPropietarios option[value='" + $("#contenido-propietario input").val() + "']").attr("PropietarioCod")) ? $("#contenido-propietario #ListaPropietarios option[value='" + $("#contenido-propietario input").val() + "']").attr("PropietarioCod") : ""
                , "PropietarioDesc": $("#contenido-propietario input").val()
                , "HoraI": $('#contenido-horaInicio input').val()
                , "HoraT": $('#contenido-horaTermino input').val()
                , "Estado": $('#contenido-estadoDocumento #estado').attr('value')
                , "TotalCajas": ($('#contenido-totalCajas').text() == "") ? "0" : $('#contenido-totalCajas').text()
                , "Observaciones": ($('#contenido-observaciones input').val()) ? $('#contenido-observaciones input').val() : ""
            };
            $.ajax('/Rutas/ActualizaOrdenRegistroRutas',
                {
                    data: parametros,
                    dataType: 'json',
                    cache: false,
                    type: 'post'
                })
                .done(function (response) {
                    if (response.mensaje == "") {
                        alert(response.mensajeError);
                    }
                    else {
                        alert(response.mensaje + "-actualizado" + response.nroDocumento);
                       /* $('#contenido-estadoDocumento #estado').html(response.estado);
                        $('#contenido-nroDocumento input').val(response.nroDocumento);
                        $('#contenedorId').css("background", response.estiloContenedorId);
                        $('#tabla3Id').html(response.tabla3Id);*/
                    }

                })
                .fail(function (jqXHR, textStatus, errorThrown) {
                    //alert("Error en Creacion Comuniquese con soporte");
                    if (jqXHR.status === 0) {
                        alert('Not connect: Verify Network. Encabezado');
                    }
                    else if (jqXHR.status == 404) {
                        alert('Requested page not found [404] Encabezado');
                    }
                    else if (jqXHR.status == 500) {
                        alert('Internal Server Error [500]. Encabezado');
                    }
                    else if (textStatus === 'parsererror') {
                        alert('Requested JSON parse failed . Encabezado');
                    }
                    else if (textStatus === 'timeout') {
                        alert('Time out error. Encabezado');
                    }
                    else if (textStatus === 'abort') {

                        alert('Ajax request aborted. Encabezado');
                    }
                    else {
                        alert('Uncaught Error: ' + jqXHR.responseText + ' Encabezado');
                    }
                });
        }
    });
}
/**************pdf*******************/

function generarPdfs(DocNum)
{
    $('#pdf').on('click', function(){
        window.location.href = '/Rutas/pdfRutas?DocNum='+DocNum;
    });
}