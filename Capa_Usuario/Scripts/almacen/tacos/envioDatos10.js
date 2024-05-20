$(document).ready(function(){
	$('#contenido-validar').on('click',function(){
		if(confirm("¿Esta seguro/a de Validar Documento?"))
		{
			var parametros={
				"TipoTaco": $('#contenido-tipoTaco select').val()
				,"FechaVenta" : $('#contenido-fechaVenta input').val()
				,"CardCode" : ($("#contenido-razonSocial #ListaSocios option[value='"+$("#contenido-razonSocial input").val()+"']").attr("SocioCod"))
								?
								$("#contenido-razonSocial #ListaSocios option[value='"+$("#contenido-razonSocial input").val()+"']").attr("SocioCod")
								:"NULL"
				, "RazonDesc": encodeURIComponent($("#contenido-razonSocial input").val()?$("#contenido-razonSocial input").val():"NULL")
				,"MontoTotal" : (($("#contenido-montoTotalVenta select").val()!="0")&&($("#contenido-montoTotalVenta select").val())) 
								   ? 
								   $("#contenido-montoTotalVenta select").val()
								   :"NULL"
				,"Referencia" : $("#contenido-referencia input").val()
				,"Guias" : ($("#contenido-guias p").text())? $("#contenido-guias p").text():" "
				,"TipoEmbalaje" : ($("#contenido-tipoEmbalaje input:radio[name=Embalaje]:checked").val())
								   ?$("#contenido-tipoEmbalaje input:radio[name=Embalaje]:checked").val():"NULL"
				,"TipoAgregado": $('#contenido-tipoAgregado select').val()				  
				,"TipoTransferencia" : ($("#contenido-tipoTransferencias input:radio[name=Transferencias]:checked").val())
								   ?$("#contenido-tipoTransferencias input:radio[name=Transferencias]:checked").val():"NULL"
				,"TipoEntregas" : ($("#contenido-tipoEntregas input:radio[name=Entregas]:checked").val())
								   ?$("#contenido-tipoEntregas input:radio[name=Entregas]:checked").val():"NULL"
				, "VerificadorCod": ($("#contenido-OperarioVerificador #ListaPropietarios option[value='" + $("#contenido-OperarioVerificador input").val() + "']").attr("PropietarioCod"))
									?$("#contenido-OperarioVerificador #ListaPropietarios option[value='"+$("#contenido-OperarioVerificador input").val()+"']").attr("PropietarioCod")
									:"0"
				, "VerificadorDesc": ($("#contenido-OperarioVerificador input").val()) ? $("#contenido-OperarioVerificador input").val() :"NULL"
				, "Verificador2Cod": ($("#contenido-OperarioVerificador2 #ListaPropietarios option[value='"+$("#contenido-OperarioVerificador2 input").val()+"']").attr("PropietarioCod"))
									?$("#contenido-OperarioVerificador2 #ListaPropietarios option[value='"+$("#contenido-OperarioVerificador2 input").val()+"']").attr("PropietarioCod")
									:"0"
				, "Verificador2Desc": ($("#contenido-OperarioVerificador2 input").val()) ? $("#contenido-OperarioVerificador2 input").val() :" "
				
				, "EncajadorCod": ($("#contenido-OperarioEncajador #ListaPropietarios option[value='"+$("#contenido-OperarioEncajador input").val()+"']").attr("PropietarioCod"))
								?$("#contenido-OperarioEncajador #ListaPropietarios option[value='"+$("#contenido-OperarioEncajador input").val()+"']").attr("PropietarioCod")
								:"0"
				, "EncajadorDesc": ($("#contenido-OperarioEncajador input").val()) ? $("#contenido-OperarioEncajador input").val() :"NULL"
				, "Encajador2Cod": ($("#contenido-OperarioEncajador2 #ListaPropietarios option[value='"+$("#contenido-OperarioEncajador2 input").val()+"']").attr("PropietarioCod"))
								?$("#contenido-OperarioEncajador2 #ListaPropietarios option[value='"+$("#contenido-OperarioEncajador2 input").val()+"']").attr("PropietarioCod")
								:"0"
				,"Encajador2Desc" : ($("#contenido-OperarioEncajador2 input").val())?$("#contenido-OperarioEncajador2 input").val():" "
				,"VendedorCod" : ($("#contenido-OperarioVentas p").attr("VendedorCod"))? $("#contenido-OperarioVentas p").attr("VendedorCod"):"0"
				,"VendedorDesc" : (($("#contenido-OperarioVentas p").text()!="")&&($("#contenido-OperarioVentas p").text()))
									 ?
									 $("#contenido-OperarioVentas p").text()
									 :"NULL"
				,"Hora" : ($('#contenido-hora input').val())? $('#contenido-hora input').val():"NULL"
				,"Cajas" : ($("#contenido-cajas input").val())? $("#contenido-cajas input").val():"0"
				
			};
			
			$.ajax('/Tacos/ValidaRegistroTacos',
				{
					data : parametros,
        			dataType :'json',
        			cache : false,
        			type : 'post'
				})
			.done(function(response)
				{
					if(response.Mensaje==""){generarPdfTaco(response)}
					else { alert(response.Mensaje);}
				});
		}
	});
});
function generarPdfTaco(json)
{
	window.location.href = "/Tacos/pdfTacos?TipoTaco=" + json.TipoTaco + "&FechaVenta=" + json.FechaVenta
		+ "&RazonDesc=" + json.RazonDesc + "&MontoTotal=" + json.MontoTotal + "&Referencia=" + json.Referencia
		+ "&Guias=" + json.Guias + "&TipoEmbalaje=" + json.TipoEmbalaje + "&TipoAgregado=" + json.TipoAgregado
		+ "&TipoTransferencia=" + json.TipoTransferencia + "&TipoEntregas=" + json.TipoEntregas
		+ "&VerificadorDesc=" + json.VerificadorDesc + "&Verificador2Desc=" + json.Verificador2Desc
		+ "&EncajadorDesc=" + json.EncajadorDesc + "&Encajador2Desc=" + json.Encajador2Desc + "&VendedorDesc=" + json.VendedorDesc
		+ "&Hora=" + json.Hora + "&Cajas=" + json.Cajas;
}
