using Capa_Datos;
using Capa_Entidad.ComprobantesContables_ENT;
using Capa_Entidad.Ventas_ENT.TablasSql;
using Capa_Negocio.Ventas_NEG.Tablas;
using Capa_Negocio.Ventas_NEG.TablasSql;
using iTextSharp.text.pdf;
using iTextSharp.text;
using Rotativa;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using Capa_Entidad.Ventas_ENT.Tablas;
using Capa_Negocio.ComprobantesContables_NEG;
using Capa_Entidad.ReportesDigemid_ENT.Reportes;
namespace Capa_Usuario.Controllers
{
    public class ComprobantesContablesController : Controller
    {
        Utilitarios uti = new Utilitarios(); 
        ORTV_N ortvN = new ORTV_N(); 
        OINV_N oinvN = new OINV_N();
        Comprobante_N compN= new Comprobante_N();
        private List<Comprobante_E> ObtenerEncabezados(List<int> listDocEntrySap, ORTV_E obj, string Tipo)
        {
            Comprobante_N compN = new Comprobante_N();
            List<Comprobante_E> documentos = new List<Comprobante_E>();
            // Obtener los documentos basados en el tipo proporcionado
            switch (Tipo)
            {
                case "F":
                    foreach (var docEntryOrden in listDocEntrySap)
                    {
                        documentos.AddRange(compN.ObtenerEncabezadoFacturas(docEntryOrden, obj.LugarDestino));
                    }
                    break;
                case "G":
                    if (obj.LugarDestino.Equals("Domicilio") || obj.LugarDestino.Equals("Agencia"))
                    {
                        documentos.AddRange(compN.ObtenerEncabezadoGuiasPorEntrega(listDocEntrySap));
                    }
                    else
                    {
                        documentos.AddRange(compN.ObtenerEncabezadoGuiasTransferencia(obj));
                    }
                    break;
                case "NC":
                    List<Comprobante_E> Facturas = new List<Comprobante_E>();
                    foreach (var docEntryOrden in listDocEntrySap)
                    {
                        Facturas = compN.ObtenerEncabezadoFacturas(docEntryOrden, obj.LugarDestino);
                    }
                    string FacturasConcatenadas = string.Join(", ", Facturas.Select(x => $"{x.U_SYP_MDTD}-{x.U_SYP_MDSD}-{x.U_SYP_MDCD}"));
                    documentos.AddRange(compN.ObtenerEncabezadoNotaCredito(obj.Det4, FacturasConcatenadas));
                    break;
                case "ND":
                    List<Comprobante_E> FacturasParaNotaDébito = new List<Comprobante_E>();
                    foreach (var docEntryOrden in listDocEntrySap)
                    {
                        FacturasParaNotaDébito = compN.ObtenerEncabezadoFacturas(docEntryOrden,obj.LugarDestino);
                    }
                    string FacturasConcatenadasParaNotaDébito = string.Join(", ", FacturasParaNotaDébito.Select(x => $"{x.U_SYP_MDTD}-{x.U_SYP_MDSD}-{x.U_SYP_MDCD}"));
                    documentos.AddRange(compN.ObtenerEncabezadoNotaDebito(FacturasConcatenadasParaNotaDébito));
                    break;
            }
            // Filtrar documentos con U_SYP_MDCD no vacío y eliminar duplicados
            var documentosFiltrados = documentos
                .Where(d => !string.IsNullOrWhiteSpace(d.U_SYP_MDCD))
                .GroupBy(d => d.U_SYP_MDCD)
                .Select(g => g.First())
                .ToList();
            return documentosFiltrados;
        }
        public JsonResult CrearYObtenerDocumento(int DocEntry, string Tipo) // Metodo principal, ajax desde ListadoTicketsAlmacen (picking packing)
        {
            ORTV_N ortvN = new ORTV_N();
            OINV_N oinvN = new OINV_N();
            Comprobante_N compN = new Comprobante_N();
            string fileUrl = string.Empty;
            string fileName = string.Empty;
            ORTV_E ortvE = ortvN.ObtenerDatosTicketParaDocumentos(DocEntry);
            List<int> listDocEntryOrdenesVenta = compN.ObtenerDocEntryOV(ortvE.Det2,false);
            List<Comprobante_E> documentos = new List<Comprobante_E>();
            //Primeras validaciones
            if (ortvE.Estado.Equals("ANULADO") || ortvE.Estado.Equals("CANCELADO"))
            {
                return Json(new { success = false, message = "Ticket esta anulado o cancelado" }, JsonRequestBehavior.AllowGet);
            }
            else if (ortvE.Det2 == null || ortvE.Det2.Count == 0 || listDocEntryOrdenesVenta == null || listDocEntryOrdenesVenta.Count == 0) //Existencia de ordenes de venta validas
            {
                return Json(new { success = false, message = "No se encontraron órdenes SAP vigentes, revise el ticket." }, JsonRequestBehavior.AllowGet);
            }
            //Hallamos documentos en relacion a las ordenes de venta y tipo
            documentos = ObtenerEncabezados(listDocEntryOrdenesVenta, ortvE, Tipo);
            //solo llega un tipo de documento a la vez
            if (documentos != null && documentos.Count > 0)
            {
                switch (Tipo)
                {
                case "F":
                    fileName = $"Facturas_{ortvE.DocNum}.pdf";
                        decimal MontoFinalFacturas = documentos.Sum(f => f.DocTotal) + documentos.Sum(f => f.AnticipoBruto);
                        if (ortvE.MontoTotal != MontoFinalFacturas)
                        { return Json(new { success = false, message = "Los montos de facturas no coinciden con lo emitido." }, JsonRequestBehavior.AllowGet); }
                        break;
                case "ND":
                    fileName = $"NotasDebito_{ortvE.DocNum}.pdf";
                    break;
                case "NC":
                    fileName = $"NotasCredito_{ortvE.DocNum}.pdf";
                        decimal MontoFinalNotasCredito = documentos.Sum(f => f.DocTotal);
                        if (ortvE.DescuentoNC > MontoFinalNotasCredito)
                        { return Json(new { success = false, message = "Los montos de notas crédito no superan lo emitido." }, JsonRequestBehavior.AllowGet); }
                     break;
                case "G":
                    fileName = $"Guias_{ortvE.DocNum}.pdf";
                    break;
                default:
                    return Json(new { success = false, message = "Tipo del documento no reconocido." }, JsonRequestBehavior.AllowGet);
                }
                GeneracionDocumentoPDF(documentos, ortvE.DocNum, Tipo, fileName); 
                string filePath = Path.Combine(uti.directorioFileServer, "Comprobantes", fileName);
                fileUrl = Url.Action("DocumentoElectronico", "ComprobantesContables", new { fileName = fileName }, Request.Url.Scheme);
                return Json(new { success = true, fileUrl = fileUrl }, JsonRequestBehavior.AllowGet);
            }
            else { return Json(new { success = false, message = "No hay documentos encontrados." }, JsonRequestBehavior.AllowGet); }
        }
        //private void GeneracionDocumentoPDF(List<Comprobante_E> documentosDistinct, int DocNum, string Tipo, string fileName)
        //{
        //    Utilitarios uti = new Utilitarios();
        //    // Agrupa todos los documentos del mismo tipo en un solo PDF
        //    string filePath = Path.Combine(uti.directorioFileServer, "Comprobantes", fileName);
        //    using (MemoryStream combinedPdfStream = new MemoryStream())
        //    {
        //        using (Document document = new Document())
        //        {
        //            PdfCopy copy = null;
        //            try
        //            {
        //                document.Open();
        //                copy = new PdfCopy(document, combinedPdfStream);
        //                foreach (var f in documentosDistinct)
        //                {
        //                    AgruparPdfSegunTipo(f, DocNum, copy, Tipo);
        //                }
        //            }
        //            finally
        //            {
        //                document.Close();
        //                copy?.Close();
        //            }
        //        }
        //        System.IO.File.WriteAllBytes(filePath, combinedPdfStream.ToArray());
        //    }
        //}
        private void GeneracionDocumentoPDF(List<Comprobante_E> documentosDistinct, int DocNum, string Tipo, string fileName)
        {
            Utilitarios uti = new Utilitarios();
            //agrupa todos los documentos del mismo tipo en un solo pdf
            string filePath = Path.Combine(uti.directorioFileServer, "Comprobantes", fileName);
            using (MemoryStream combinedPdfStream = new MemoryStream())
            {
                using (Document document = new Document())
                {
                    PdfCopy copy = new PdfCopy(document, combinedPdfStream);
                    document.Open();
                    foreach (var f in documentosDistinct)
                    {
                        AgruparPdfSegunTipo(f, DocNum, copy, Tipo);
                    }
                    document.Close();
                }
                System.IO.File.WriteAllBytes(filePath, combinedPdfStream.ToArray());
            }
        }
        private void AgruparPdfSegunTipo(Comprobante_E documento, int docNum, PdfCopy copy,string Tipo)
        {
            var pdfResult = new ActionAsPdf(null);
            string NumAtCard = $"{documento.U_SYP_MDTD}-{documento.U_SYP_MDSD}-{documento.U_SYP_MDCD}";
            string fileName = $"{documento.U_SYP_MDTD}_{documento.U_SYP_MDSD}_{documento.U_SYP_MDCD}.pdf";
            //Contemplar un caso de layout por cada tipo de documentos:
            //Factura,
            //Boleta,
            //Guia,
            //Nota credito,
            //Nota debito 
            switch (Tipo)
            {
                case "F":
                    var parametrosFactura = new
                    {
                        NumAtCard = NumAtCard,
                        Tipo = documento.U_SYP_MDTD.Equals("01") ? "F" : "B",
                        DocNumTicket = docNum
                    };
                    string _headerUrlFactura = Url.Action("LayoutFactura_header", "ComprobantesContables", parametrosFactura, "http");
                    pdfResult = new ActionAsPdf("LayoutFactura", new { NumAtCard = parametrosFactura.NumAtCard })
                    {
                        FileName = fileName,
                        PageOrientation = Rotativa.Options.Orientation.Portrait,
                        CustomSwitches = "--header-html " + _headerUrlFactura + " --header-spacing 0 ",
                        PageSize = Rotativa.Options.Size.A4,
                        PageMargins = new Rotativa.Options.Margins(65, 10, 20, 10)
                    };
                    break;
                case "ND":
                case "NC":
                    var parametrosNotaCredito = new
                    {
                        NumAtCard = NumAtCard,
                        DocNumTicket = docNum
                    };
                    string _headerUrlNotaCredito = Url.Action("LayoutNotaCreditoDebito_header", "ComprobantesContables", parametrosNotaCredito, "http");
                    pdfResult = new ActionAsPdf("LayoutNotaCreditoDebito", new { NumAtCard = NumAtCard})
                    {
                        FileName = fileName,
                        PageOrientation = Rotativa.Options.Orientation.Portrait,
                        CustomSwitches = "--header-html " + _headerUrlNotaCredito + " --header-spacing 0 ",
                        PageSize = Rotativa.Options.Size.A4,
                        PageMargins = new Rotativa.Options.Margins(70, 10, 20, 10)
                    };
                    break;
                case "G":
                    var parametrosGuia = new
                    {
                        NumAtCard = NumAtCard,
                        DocNumTicket = docNum,
                        Tabla = documento.TablaSAP
                    };
                    string _headerUrlGuia = Url.Action("LayoutGuia_header", "ComprobantesContables", parametrosGuia, "http");
                    pdfResult = new ActionAsPdf("LayoutGuia", parametrosGuia)
                    {
                        FileName = fileName,
                        PageOrientation = Rotativa.Options.Orientation.Portrait,
                        CustomSwitches = "--header-html " + _headerUrlGuia + " --header-spacing 0 ",
                        PageSize = Rotativa.Options.Size.A4,
                        PageMargins = new Rotativa.Options.Margins(65, 10, 20, 10)
                    };
                    break;
            }
            var pdfBytes = pdfResult.BuildFile(ControllerContext);
            using (var pdfStream = new MemoryStream(pdfBytes))
            {
                using (var pdfReader = new PdfReader(pdfStream))
                {
                    // Aplicar paginación al PDF antes de agregarlo al documento combinado
                    using (MemoryStream paginatedPdfStream = new MemoryStream())
                    {
                        using (PdfStamper stamper = new PdfStamper(pdfReader, paginatedPdfStream))
                        {
                            int totalPages = pdfReader.NumberOfPages;
                            for (int i = 1; i <= totalPages; i++)
                            {
                                PdfContentByte content = stamper.GetUnderContent(i);
                                iTextSharp.text.Font font = FontFactory.GetFont("Helvetica", BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 8);
                                Phrase phrase = new Phrase($"Página {i} de {totalPages}", font);
                                ColumnText.ShowTextAligned(content, Element.ALIGN_RIGHT, phrase, 570, 30, 0);
                            }
                        }
                        using (var paginatedPdfReader = new PdfReader(paginatedPdfStream.ToArray()))
                        {
                            // Agregar el PDF paginado al documento combinado
                            copy.AddDocument(paginatedPdfReader);
                        }
                    }
                }
            }
        }
        // Acción para crear y guardar el documento, y luego devolver una URL para acceder al archivo en forma content desde el navegador
        public ActionResult DocumentoElectronico(string fileName)
        {
            string basePath = uti.directorioFileServer;
            string pathComplete = Path.Combine(basePath, "Comprobantes", fileName);
            if (!System.IO.File.Exists(pathComplete))
            {
                return HttpNotFound();
            }
            Response.AppendHeader("Content-Disposition", "inline; filename=" + fileName);
            return File(pathComplete, "application/pdf");
        }
        //Metodos para obtener cabecera o detalle segun sea el caso.
        private List<Guia_Remision_E> ObtenerGuia(string numAtCard, string Tabla, string tipo)
        {
            if (tipo == "Cabecera")
            {
                return compN.ObtenerCabeceraGuia(numAtCard, Tabla);
            }
            else { return compN.ObtenerDetalleGuia(numAtCard, Tabla); }
        }
        private List<ComprobanteDePago_E> ObtenerFactura(string numAtCard, string tipo)
        {
            if (tipo == "Cabecera") { return compN.ObtenerCabeceraFactura(numAtCard); }
            else { return compN.ObtenerDetalleFactura(numAtCard); }
        }
        private (List<NotaCreditoDebito_E> nota, string tipoDocumento, string subTipo) ObtenerNotaCreditoDebito(string numAtCard, string tipo)
        {
            var tipoDocumento = "";
            var subTipo = "";
            List<NotaCreditoDebito_E> nota = new List<NotaCreditoDebito_E>();
            if (numAtCard.Contains("FN") || numAtCard.Contains("BN")) // Nota de crédito ORIN
            {
                tipoDocumento = "NC";
                // Nota Crédito puede ser Artículo y Servicio
                var orinN = new ORIN_N();
                // Hallar el subtipo 
                var cabeceraNota = orinN.ObtenerCabecera(0, numAtCard);
                subTipo = cabeceraNota.DocType;
                if (tipo == "Cabecera")
                {
                    //Si solo busca cabecera trae los datos necesarios
                    nota.Add(new NotaCreditoDebito_E
                    {
                        SerieDoc = cabeceraNota.SerieDoc,
                        CorreDoc = cabeceraNota.CorreDoc,
                        NombreSocio = cabeceraNota.CardName,
                        DirPagar = cabeceraNota.DirPagar,
                        Ruc = cabeceraNota.Ruc,
                        DocDate = cabeceraNota.DocDate,
                        MonedaLetras = cabeceraNota.MonedaLetras
                    });
                }
                //Solo si estamos buscando el detalle de la nota se debe ingresar a otros metodos:
                else if (tipo == "Cuerpo")
                {
                    if (subTipo.Equals("I"))
                    {
                        nota = orinN.ObtenerDetalleNotaCreditoArticulo(numAtCard);
                    }
                    else if (subTipo.Equals("S"))
                    {
                        nota = orinN.ObtenerDetalleNotaCreditoServicio(numAtCard);
                    }
                }
            }
            else if (numAtCard.Contains("FD") || numAtCard.Contains("BD")) // Nota de débito OINV
            {
                tipoDocumento = "ND";
                var oinvN = new OINV_N();
                // Nota de Débito solo puede ser Servicio
                if (tipo == "Cuerpo")
                {
                    nota = oinvN.ObtenerDetalleNotaDebito(numAtCard);
                }
                else { nota.Add(oinvN.ObtenerCabeceraNotaDebito(numAtCard)); }
            }
            return (nota, tipoDocumento, subTipo);
        }
        public ActionResult LayoutGuia_header(string NumAtCard, string DocNumTicket, string Tabla)
        {
            var guia = ObtenerGuia(NumAtCard, Tabla, "Cabecera");
            ViewBag.DocNumTicket = DocNumTicket;
            ViewBag.Tipo = Tabla.Equals("OWTR") ? "T" : "E";
            return View(guia);
        }
        public ActionResult LayoutGuia(string NumAtCard, string Tabla, int DocNumTicket)
        {
            var guia = ObtenerGuia(NumAtCard, Tabla, "Cuerpo");
            ORTV_N ortvN = new ORTV_N();
            ViewBag.PersonaRecojo = ortvN.obtenerPersonaRecojoParaGuia(DocNumTicket);
            ViewBag.Tipo = Tabla.Equals("OWTR") ? "T" : "E";
            return View(guia);
        }
        public ActionResult LayoutFactura_header(string NumAtCard, string Tipo, int DocNumTicket)
        {
            var factura = ObtenerFactura(NumAtCard, "Cabecera");
            ViewBag.Tipo = Tipo; 
            ViewBag.DocNumTicket = DocNumTicket;
            return View(factura);
        }
        public ActionResult LayoutFactura(string NumAtCard)
        {
            var factura = ObtenerFactura(NumAtCard,"Cuerpo");
            return View(factura);
        }
        public ActionResult LayoutNotaCreditoDebito_header(string numAtCard, string DocNumTicket)
        {
            var (nota, tipo, subTipo) = ObtenerNotaCreditoDebito(numAtCard,"Cabecera");
            ViewBag.DocNumTicket = DocNumTicket;
            ViewBag.Tipo = tipo;
            return View(nota);
        }
        public ActionResult LayoutNotaCreditoDebito(string numAtCard)
        {
            var (nota, tipo, subTipo) = ObtenerNotaCreditoDebito(numAtCard,"Cuerpo");
            ViewBag.Tipo = tipo;
            ViewBag.SubTipo = subTipo;
            return View(nota);
        }
        public JsonResult ExistenciaDeDocumentos(int DocEntry) // Metodo secundario para consulta existencia antes de habilitar botones en modal de impresion de documentos 
        {
            Comprobante_N compN = new Comprobante_N();
            string existeNC = string.Empty,
                existeND = string.Empty,
                existeF= string.Empty,
                existeG = string.Empty;
            var ortvE = new Capa_Negocio.Ventas_NEG.TablasSql.ORTV_N().ObtenerDatosTicketParaDocumentos(DocEntry);
            List<int> listDocEntryOrdenesVenta = compN.ObtenerDocEntryOV(ortvE.Det2, false);
            //Primeras validaciones
            if (ortvE.Estado.Equals("ANULADO") || ortvE.Estado.Equals("CANCELADO"))
            {
                return Json(new { success = false, message = "Ticket esta anulado o cancelado" }, JsonRequestBehavior.AllowGet);
            }
            else if (ortvE.Det2 == null || ortvE.Det2.Count == 0 || listDocEntryOrdenesVenta == null || listDocEntryOrdenesVenta.Count == 0) //Existencia de ordenes de venta validas
            {
                return Json(new { success = false, message = "No se encontraron órdenes SAP vigentes, revise el ticket." }, JsonRequestBehavior.AllowGet);
            }
            //Hallamos documentos en relacion a las ordenes de venta y tipo
            var notaDebito = ObtenerEncabezados(listDocEntryOrdenesVenta, ortvE, "ND");
            var notaCredito = ObtenerEncabezados(listDocEntryOrdenesVenta, ortvE, "NC");
            var factura = ObtenerEncabezados(listDocEntryOrdenesVenta, ortvE, "F");
            var guia = ObtenerEncabezados(listDocEntryOrdenesVenta, ortvE, "G");
            if (notaCredito.Any()) { existeNC = "Y"; }
            if (notaDebito.Any()) { existeND = "Y"; }
            if (factura.Any()) { existeF = "Y"; }
            if (guia.Any()) { existeG = "Y"; }
            return Json(new 
            { 
                existeNC = existeNC,
                existeND = existeND,
                existeF = existeF,
                existeG = existeG,
            }, 
                JsonRequestBehavior.AllowGet);
            }
        }
}