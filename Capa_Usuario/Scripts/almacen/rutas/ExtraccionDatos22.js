$(document).ready(function(){

//variable global

      var lineas = 30;  
      validacionTabla2(lineas);
      validacionTipoRuta();
      valicionEstadoDocumento();
      validacionTransportista();
      validacionPlaca();
      validacionCopiloto("1");
      validacionCopiloto("2");
      validacionCopiloto("3");
      validacionCopiloto("4");
      validacionOrigen();
      validacionDestino();
      validacionPropietario();
      validacionSerieDocumento();
      validacionNroDocumento();   
      validacionObservaciones();
      detallesIni(lineas);
      validacionSocio(lineas);
});

function validacionTotalCajas(orden,lineas)
{   
        $('#contenido-nroCajas'+orden).on('change',function(){      
          var cajas = 0;
            for(i=1;i<=lineas;i++)
            {
                lineaCaja = $('#contenido-nroCajas'+i+' input').val()*1;
                if(lineaCaja<0)
                {
                    alert("No puedes ingresar cajas negativas");
                    lineaCaja=0;
                    $('#contenido-nroCajas'+i+' input').val("");
                }
                    cajas +=lineaCaja;
            }
            $('#contenido-totalCajas').html("<strong>"+cajas+"</strong>");
        });
}
//usado por otra funcion
function validacionTotalCajasSinEvento(lineas) {
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
   
}
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
function validacionTipoRuta()
{
          $.ajax('/Rutas/infoTipoRuta',
                {
                //data : parametros,
                dataType :'html',
                cache : false,
                type : 'post'
                })
          .done(function(response)
                {
                       $('#contenido-tipoRuta').html(response);
                       $('#contenido-FechaCont').css('display','none');
                });
}
function valicionEstadoDocumento()
{
           $.ajax('/Rutas/infoEstadoDocumento',
                {
                //data : parametros,
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
function validacionTransportista()
{       
         $.ajax('/Rutas/infoTransportista',
                {
                //data : parametros,
                dataType :'html',
                cache : false,
                type : 'post'
                })
          .done(function(response)
                {
                       $('#contenido-transportista').html(response);
                });        
}
function validacionPlaca()
{
          $.ajax('/Rutas/infoPlaca',
                {
                //data : parametros,
                dataType :'html',
                cache : false,
                type : 'post'
                })
          .done(function(response)
                {
                       $('#contenido-placa').html(response);
                });   
}
function validacionCopiloto(nro)
{
    var nroId = "";
    if (nro != "1") { nroId = nro; }
    var parametros = { "nro": nroId };
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
function validacionOrigen()
{
        $.ajax('/Rutas/infoOrigen',
                {
                //data : parametros,
                dataType :'html',
                cache : false,
                type : 'post'
                })
          .done(function(response)
                {
                       $('#contenido-origen').html(response);          
                });   
}
function validacionDestino()
{
        $.ajax('/Rutas/infoDestino',
                {
                //data : parametros,
                dataType :'html',
                cache : false,
                type : 'post'
                })
          .done(function(response)
                {
                       $('#contenido-destino').html(response);       
                });        
}
function validacionPropietario()
{
    $.ajax('/Rutas/infoPropietario',
        {
            //data : parametros,
            dataType: 'html',
            cache: false,
            type: 'post'
        })
        .done(function (response) {
            $('#contenido-propietario').html(response);
        });
}
function validacionSerieDocumento()
{
    $.ajax('/Rutas/infoSerieDocumento',
                {
                //data : parametros,
                dataType :'html',
                cache : false,
                type : 'post'
                })
          .done(function(response)
                {
                       $('#contenido-serieDocumento').html(response); 
                });   
}
function validacionNroDocumento()
{
    $.ajax('/Rutas/infoNroDocumento',
                {
                //data : parametros,
                dataType :'html',
                cache : false,
                type : 'post'
                })
          .done(function(response)
                {
                       $('#contenido-nroDocumento').html(response);
                });   
}
function validacionObservaciones()
{
    $.ajax('/Rutas/infoObservaciones',
                {
                //data : parametros,
                dataType :'html',
                cache : false,
                type : 'post'
                })
          .done(function(response)
                {
                       $('#contenido-observaciones').html(response);
               
                });   
}
//detalles de registro
function detalles(lineas)
{
    var parametros = {
        "FechaCont": $('#contenido-FechaCont input').val(),
        "tipoRuta": $('#contenido-tipoRuta select').val()
    };
    $.ajax('/Rutas/infoDetalles',
        {
            data: parametros,
            dataType: 'json',
            cache: false,
            type: 'post'
        })
        .done(function (response) {
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
                $('#contenido-productos' + h).html("");
            }
            validacionTotalCajasSinEvento(lineas);
        });
}
function detallesIni(lineas)
{
    $('#contenido-tipoRuta').on('change', function () {
        detalles(lineas);
    });
}
function validacionSocio(lineas)
{
    $('#contenido-FechaCont').on('change',function(){           
        detalles(lineas);
        var parametros ={
            "FechaCont": $('#contenido-FechaCont input').val()
            ,"TipoRuta": $('#contenido-tipoRuta select').val()
        };
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
                       validacionRuc(h, lineas);
                       validaciondirDest(h);
                       validacionMontoTotalVenta(h);
                       validacionGuiasTraslado(h,lineas);
                       validacionTotalCajas(h,lineas);
                   } 
                });
    });
}
function validacionRuc(orden, lineas)
{
    $('#contenido-SocioNegocio' + orden).on('change', function () {
        $('#contenido-listaVentas' + orden).html("<p></p>");
        $('#contenido-listaGuias' + orden).html("<p></p>");
        $('#contenido-monto' + orden).html("<p></p>");
        $('#contenido-vendedor' + orden).html("<p></p>");
        $('#contenido-productos' + orden).html("");
        $('#contenido-nroCajas' + orden + ' input').val("");
        validacionTotalCajasSinEvento(lineas)
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
                    "orden": orden,
                    "TipoRuta": $('#contenido-tipoRuta select').val()
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
/***transferencias de stock INICIO**/
function validacionGuiasTraslado(orden, BaseLineas) {
    $('#contenido-SocioNegocio' + orden + '').on('change', function () {
        var TipoRuta = $('#contenido-tipoRuta select').val()
        var CardCode = $("#contenido-SocioNegocio" + orden + " #ListaSocios option[value='" + $("#contenido-SocioNegocio" + orden + " input").val() + "']").attr("SocioCod");
        if (!CardCode) { CardCode = "" };
        var parametros = {
            "CardCode": CardCode,
            "FechaCont": $('#contenido-FechaCont input').val(),
            "orden": orden,
            "TipoRuta": TipoRuta
        };
        if (TipoRuta == "TA")
        {
            $.ajax('/Rutas/infoGuiasTraslado',
                {
                    data: parametros,
                    dataType: 'html',
                    cache: false,
                    type: 'post',
                })
                .done(function (response) {
                    $('#contenido-listaGuias' + orden).html(response);
                    validacionDatosTraslado(orden, BaseLineas);
                });
        }
    });
}
function validacionDatosTraslado(orden, BaseLineas) {
    $('#contenido-listaGuias' + orden).on('change', function () {
        $('#contenido-productos'+orden).html('<img src="/imagenes/index/ReportesDigemid/cargando.gif" />');
        var parametros = {
            "Guia": $('#contenido-listaGuias' + orden + ' select').val()
            ,"orden":orden
        }; 
            $.ajax('/Rutas/infoDatosTraslado',
                {
                    data: parametros,
                    dataType: 'json',
                    cache: false,
                    type: 'post',
                })
                .done(function (response) {
                    $('#contenido-listaVentas' + orden).html(response.InfoContenidoListaVentas);
                    $('#contenido-vendedor' + orden).html(response.InfoContenidoVendedor);
                    $('#contenido-productos' + orden).html(response.InfoContenidoDetalleProductos);
                    for (var h=1;h<=$('#rru12-subLineas'+orden).attr('subLineas');h++) {
                        validacionSubtotalCajasMaster(orden, h, $('#rru12-subLineas' + orden).attr('subLineas'), BaseLineas);
                    }

                });
    });
}
function validacionSubtotalCajasMaster(BaseLinea, Linea, subLineas,BaseLineas) {    
       $('#contenido-productos' + BaseLinea + ' #rru12-Cajas' + Linea).on('change', function () {
           var cajasMaster = 0;
           for (i = 1; i <= subLineas; i++) {
               lineaCaja = $('#contenido-productos' + BaseLinea + ' #rru12-Cajas' + i + ' input').val() * 1;
                if (lineaCaja < 0) {
                    alert("No puedes ingresar cajas negativas");
                    lineaCaja = 0;
                    $('#contenido-productos' + BaseLinea + ' #rru12-Cajas' + i + ' input').val("");
                }
               cajasMaster += lineaCaja;
            }
            //$('#contenido-totalCajas').html("<strong>" + cajas + "</strong>");
            //alert($('#contenido-productos' + BaseLinea + ' #rru12-Cajas' + Linea+' input').val());
           $('#contenido-nroCajas' + BaseLinea + ' input').val(cajasMaster);
           validacionTotalCajasSinEvento(BaseLineas);
        }); 
}
/***transferencias de stock FIN **/
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




