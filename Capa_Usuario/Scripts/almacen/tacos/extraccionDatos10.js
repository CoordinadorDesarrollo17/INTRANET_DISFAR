$(document).ready(function(){
    validaTipoTaco();
    mostrarContenido(); 
	validacionRazonSocial();
	validacionMontoTotalVenta();
    validacionGuias();
	validacionTipoEmbalaje();
    validaTipoAgregado();
    validacionTipoTransferencia();
	validacionTipoEntregas();
    validacionTiposGenerales('/Rutas/infoPropietario','#contenido-OperarioVerificador');
    validacionTiposGenerales('/Rutas/infoPropietario','#contenido-OperarioEncajador');
    validacionTiposGenerales('/Rutas/infoPropietario','#contenido-OperarioVerificador2');
    validacionTiposGenerales('/Rutas/infoPropietario','#contenido-OperarioEncajador2');
    validacionOperarioVentas();
	validacionTotalCajas();
});

function resetearValores()
{
    $('#contenido-razonSocial').html("");
    $('#contenido-montoTotalVenta').html("");
    $('#contenido-tipoEmbalaje').html("");
    $('#contenido-tipoEntregas').html("");
    $('#contenido-tipoTransferencias').html("");
    $('#contenido-OperarioVerificador').html("");
    $('#contenido-OperarioEncajador').html("");
    $('#contenido-OperarioVerificador2').html("");
    $('#contenido-OperarioEncajador2').html("");
    $('#contenido-OperarioVentas').html("");
    $('#contenido-hora').html('<input type="time" name="HoraVenta">');
    $('#contenido-cajas').html('<input type="number" style="width: 50px;">');
}
function validaTipoTaco()
{
    $.ajax('/Tacos/infoTipoTaco',
                        {
                            dataType : 'html',
                            cache : false,
                            type : 'post',
                        })
                .done(function(response)
                        {
                            $('#contenido-tipoTaco').html(response);
                            $('#contenido-fechaVenta').css('display','none');
                        });
}
function mostrarContenido()
{
    $('#contenido-tipoTaco').on('change',function(){
       var parametros = {"TipoTaco" : $('#contenido-tipoTaco select').val()};
        $.ajax('/Tacos/infoContenidos',
                {
                    data : parametros,
                    dataType :'json',
                    cache : false,
                    type : 'post'
                })
          .done(function(response)
          {    
              resetearValores();
              //alert(response.EstFechaVenta);
              $('#titulo-fechaVenta').css('display', response.EstFechaVenta); $('#contenido-fechaVenta').css('display', response.EstFechaVenta);
              $('#titulo-razonSocial').css('display', response.EstRazonSocial); $('#contenido-razonSocial').css('display', response.EstRazonSocial);
              $('#titulo-montoTotalVenta').css('display', response.EstMontoTotalVenta); $('#contenido-montoTotalVenta').css('display', response.EstMontoTotalVenta);
              $('#titulo-referencia').css('display', response.EstReferencia); $('#contenido-referencia').css('display', response.EstReferencia);
              $('#titulo-guias').css('display', response.EstGuias); $('#contenido-guias').css('display', response.EstGuias);
              $('#titulo-tipoEmbalaje').css('display', response.EstTipoEmbalaje); $('#contenido-tipoEmbalaje').css('display', response.EstTipoEmbalaje);
              $('#titulo-tipoTransferencias').css('display', response.EstTipoTransferencias); $('#contenido-tipoTransferencias').css('display', response.EstTipoTransferencias);
              $('#titulo-tipoEntregas').css('display', response.EstTipoEntregas); $('#contenido-tipoEntregas').css('display', response.EstTipoEntregas);
              $('#titulo-OperarioVerificador').css('display', response.EstOperarioVerificador); $('#contenido-OperarioVerificador').css('display', response.EstOperarioVerificador);
              $('#titulo-OperarioEncajador').css('display', response.EstOperarioEncajador); $('#contenido-OperarioEncajador').css('display', response.EstOperarioEncajador);
              $('#titulo-OperarioVentas').css('display', response.EstOperarioVentas); $('#contenido-OperarioVentas').css('display', response.EstOperarioVentas);
              $('#titulo-hora').css('display', response.EstHora); $('#contenido-hora').css('display', response.EstHora);
              $('#titulo-cajas').css('display', response.EstCajas); $('#contenido-cajas').css('display', response.EstCajas);
                        //$('#contenido-fechaVenta').css('display','none');
                        //resetearValores();
                    
                }); 
    });
}
function validacionRazonSocial()
{
    $('#contenido-fechaVenta').on('change', function () {
        if($('#contenido-fechaVenta input').val()==""){resetearValores();return;}
        var parametros = {
            "FechaCont": $('#contenido-fechaVenta input').val()
            , "TipoRuta": $('#contenido-tipoTaco select').val()
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
        	           $('#contenido-razonSocial').html(response);
                });
     
	}); 
} 
function validacionMontoTotalVenta()
{        
        $('#contenido-razonSocial').on('change',function(){
                var CardCode = $("#contenido-razonSocial #ListaSocios option[value='"+$("#contenido-razonSocial input").val()+"']").attr("SocioCod");
                if(!CardCode){CardCode=""};
                var parametros={"CardCode": CardCode,"FechaCont": $('#contenido-fechaVenta input').val()};

                $.ajax('/Rutas/infoMontoTotalVenta',
                        {
                            data : parametros,
                            dataType : 'html',
                            cache : false,
                            type : 'post',
                        })
                .done(function(response)
                        {
                            $('#contenido-montoTotalVenta').html(response);
                        });
        });
}
function validacionGuias()
{       
        $('#contenido-razonSocial').on('change',function(){$('#contenido-guias').html("<p></p>");});

        $('#contenido-montoTotalVenta').on('change',function(){
                MontoTotalVenta = $('#contenido-montoTotalVenta select').val();
                if(MontoTotalVenta=="0"){$('#contenido-guias').html("");return;}
                var CardCode = $("#contenido-razonSocial #ListaSocios option[value='" + $("#contenido-razonSocial input").val() + "']").attr("SocioCod");
                var parametros ={
                        "MontoTotalVenta" : MontoTotalVenta,
                        "FechaVenta" : $('option:selected', '#contenido-montoTotalVenta select').attr('fe'),
                        "CardCode": CardCode
                };
                $.ajax('/Rutas/infoListaVentas',
                        {
                                data : parametros,
                                dataType : 'json',
                                cache : false,
                                type : 'post'
                        })
                .done(function(response)
                        {
                                $('#contenido-guias').html(response.InfoContenidoListaGuias);
                        });                
        });
}
function validacionTipoEmbalaje()
{
	$('#contenido-fechaVenta').on('change',function(){	
		if($('#contenido-fechaVenta input').val()=="")
        {
            resetearValores();
            return;    
        }
        $.ajax('/Tacos/infoTipoEmbalaje',{})
		.done(function(response)
				{
					$('#contenido-tipoEmbalaje').html(response);
				});
    });
}
function validaTipoAgregado()
{
                $.ajax('/Tacos/infoTipoAgregado',
                        {
                            dataType : 'html',
                            cache : false,
                            type : 'post',
                        })
                .done(function(response)
                        {
                            $('#contenido-tipoAgregado').html(response);
                        });
}
function validacionTipoTransferencia()
{
    $('#contenido-fechaVenta').on('change',function(){        
        if($('#contenido-fechaVenta input').val()==""){resetearValores();return;}
        $.ajax('/Tacos/infoTipoTransferencia',{})
        .done(function(response)
                {
                    $('#contenido-tipoTransferencias').html(response);
                });
    });
}
function validacionTipoEntregas()
{
	$('#contenido-fechaVenta').on('change',function(){
    	
		if($('#contenido-fechaVenta input').val()==""){resetearValores();return;}
        $.ajax('/Tacos/infoTipoEntregas',{})
		.done(function(response)
				{
					$('#contenido-tipoEntregas').html(response);
				});
    });
}
function validacionTiposGenerales(ruta,contenedor)
{
	$('#contenido-fechaVenta').on('change',function(){
    	if($('#contenido-fechaVenta input').val()=="")
        {
            resetearValores();
            return;    
        }
        $.ajax(ruta, {
            dataType: 'html',
            cache: false,
            type: 'post'
        })
		.done(function(response)
				{
					$(contenedor).html(response);
				});
    });	
}
function validacionOperarioVentas()
{
        $('#contenido-razonSocial').on('change',function(){$('#contenido-OperarioVentas').html("");});
        $('#contenido-montoTotalVenta').on('change',function(){
                
                var MontoTotalVenta = $('#contenido-montoTotalVenta select').val();
                var CardCode = $("#contenido-razonSocial #ListaSocios option[value='" + $("#contenido-razonSocial input").val() + "']").attr("SocioCod");
                if(MontoTotalVenta=="--"){$('#contenido-OperarioVentas').html("");return;}
                var parametros ={
                        "MontoTotalVenta" : MontoTotalVenta,
                        "FechaVenta" : $('option:selected', '#contenido-montoTotalVenta select').attr('fe'),
                        "CardCode": CardCode
                };

                $.ajax('/Rutas/infoListaVentas',
                        {
                                data : parametros,
                                dataType : 'json',
                                cache : false,
                                type : 'post'
                        })
                .done(function(response)
                        {
                                $('#contenido-OperarioVentas').html(response.InfoContenidoVendedor);
                        });
                
        });       
}
function validacionTotalCajas()
{   
        $('#contenido-cajas').on('change',function(){
                lineaCaja = $('#contenido-cajas input').val()*1;
                if(lineaCaja<0)
                {
                    alert("No puedes ingresar cajas negativas");
                    lineaCaja=0;
                    $('#contenido-cajas input').val("");
                }
                    
        });
}