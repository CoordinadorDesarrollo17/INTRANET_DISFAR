using Capa_Datos;
using Capa_Entidad.ComprobantesContables_ENT;
using Capa_Entidad.Ventas_ENT.TablasSql;
using Capa_Negocio.Ventas_NEG.Tablas;
using Capa_Negocio.Ventas_NEG.TablasSql;
using iTextSharp.text.pdf;
using iTextSharp.text;
using Rotativa;
using Sap.Data.Hana;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Capa_Datos.Ventas_DAO.TablasSql;
using Capa_Entidad.Almacen_ENT.Tablas;
using Capa_Entidad.Ventas_ENT.Tablas;
using Capa_Negocio.Almacen_NEG.Tablas;
using Capa_Datos.Almacen_DAO.Tablas;
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
                .Where(d => !string.IsNullOrEmpty(d.U_SYP_MDCD))
                .GroupBy(d => d.U_SYP_MDCD)
                .Select(g => g.First())
                .ToList();

            return documentosFiltrados;
        }
        public JsonResult CrearYObtenerDocumento(int DocEntry, string Tipo) // Metodo principal, ajax desde ListadoTicketsAlmacen
        {
            ORTV_N ortvN = new ORTV_N();
            OINV_N oinvN = new OINV_N();
            Comprobante_N compN = new Comprobante_N();
            ORTV_E ortvE = ortvN.ObtenerDatosTicketParaDocumentos(DocEntry);
            List<int> listDocEntryOrdenesVenta = compN.ObtenerDocEntryOV(ortvE.Det2,false);
             if (ortvE.Estado.Equals("ANULADO") || ortvE.Estado.Equals("CANCELADO"))
            {
                return Json(new { success = false, message = "Ticket en un estado no valido para la descarga de documentos" }, JsonRequestBehavior.AllowGet);
            }
            else if (ortvE.Det2 == null || ortvE.Det2.Count == 0 || listDocEntryOrdenesVenta == null || listDocEntryOrdenesVenta.Count == 0)
            {
                return Json(new { success = false, message = "No se encontraron órdenes SAP activas, revise el estado del ticket y órdenes." }, JsonRequestBehavior.AllowGet);
            }
            string fileUrl = string.Empty; string fileName = string.Empty;
            
            List<Comprobante_E> documentos = new List<Comprobante_E>();

            documentos = ObtenerEncabezados(listDocEntryOrdenesVenta, ortvE, Tipo);
            if (documentos != null && documentos.Count>0)
            {
                switch (Tipo)
                {
                case "F":
                    fileName = $"Facturas_{ortvE.DocNum}.pdf";
                        decimal MontoFinalFacturas = documentos.Sum(f => f.DocTotal);
                        if (ortvE.MontoTotal != MontoFinalFacturas)
                        { return Json(new { success = false, message = "Los montos de facturas no coinciden con lo emitido." }, JsonRequestBehavior.AllowGet);}
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
            //contemplar un caso de layout por cada tipo de documentos:
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
                        PageMargins = new Rotativa.Options.Margins(75, 10, 20, 10)
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
                        PageMargins = new Rotativa.Options.Margins(65, 10, 20, 10)
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
                        PageMargins = new Rotativa.Options.Margins(70, 10, 20, 10)
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
                                ColumnText.ShowTextAligned(content, Element.ALIGN_CENTER, phrase, 300, 30, 0);
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
        //
        public ActionResult LayoutFactura_header(string NumAtCard, string Tipo, int DocNumTicket)
        {
            var factura = ObtenerDetalleFactura(NumAtCard);
            ViewBag.Tipo = Tipo; 
            ViewBag.DocNumTicket = DocNumTicket;
            return View(factura);
        }
        public ActionResult LayoutFactura(string NumAtCard)
        {
            var factura = ObtenerDetalleFactura(NumAtCard);
            return View(factura);
        }
        //
        public ActionResult LayoutGuia_header(string NumAtCard, string DocNumTicket, string Tabla)
        {
            var guia = ObtenerDetalleGuia(NumAtCard, Tabla);
            ViewBag.DocNumTicket = DocNumTicket;
            ViewBag.Tipo = Tabla.Equals("OWTR") ? "T" : "E";
            return View(guia);
        }
        public ActionResult LayoutGuia(string NumAtCard,string Tabla,int DocNumTicket)
        {
            var guia = ObtenerDetalleGuia(NumAtCard, Tabla);
            ORTV_N ortvN = new ORTV_N();
            ViewBag.PersonaRecojo = ortvN.obtenerPersonaRecojoParaGuia(DocNumTicket);
            ViewBag.Tipo = Tabla.Equals("OWTR") ?  "T" : "E";
            return View(guia);
        }
        private List<Guia_Remision_E> ObtenerDetalleGuia(string numAtCard, string Tabla)
        {
            List<Guia_Remision_E> guia = compN.ObtenerDetalleGuia(numAtCard, Tabla);
            return guia;
        }
        private List<ComprobanteDePago_E> ObtenerDetalleFactura(string numAtCard)
        {
            return compN.ObtenerDetalleFactura(numAtCard);
        }
        public ActionResult LayoutNotaCreditoDebito_header(string NumAtCard, string DocNumTicket)
        {
            var (nota, tipo, subTipo) = ObtenerNotaCreditoDebito(NumAtCard);
            ViewBag.DocNumTicket = DocNumTicket;
            ViewBag.Tipo = tipo;
            ViewBag.SubTipo = subTipo;
            return View(nota);
        }
        public ActionResult LayoutNotaCreditoDebito(string NumAtCard)
        {
            var (nota, tipo, subTipo) = ObtenerNotaCreditoDebito(NumAtCard);
            ViewBag.Tipo = tipo;
            ViewBag.SubTipo = subTipo;
            return View(nota);
        }
        private (List<NotaCreditoDebito_E> nota, string tipo, string subTipo) ObtenerNotaCreditoDebito(string numAtCard)
        {
            OINV_N oinvN = new OINV_N();
            ORIN_N orinN = new ORIN_N();
            string tipo = "";
            string subTipo = "";
            List<NotaCreditoDebito_E> nota = new List<NotaCreditoDebito_E>();

            if (numAtCard.Contains("FN") || numAtCard.Contains("BN")) // Nota de crédito ORIN
            {
                tipo = "NC"; // Nota Crédito de Artículo y Servicio
                List<ORIN_E> listOrin = orinN.listarNotasDeCredito(new ORIN_E { NumAtCard = numAtCard });
                if (listOrin != null && listOrin.Count > 0)
                {
                    subTipo = listOrin[0].DocType;
                }
                if (subTipo.Equals("I"))
                {
                    nota = orinN.buscarNotaCreditoSapArticulo(numAtCard);
                }
                else if (subTipo.Equals("S"))
                {
                    nota = orinN.buscarNotaCreditoSapServicio(numAtCard);
                }
            }
            else if (numAtCard.Contains("FD") || numAtCard.Contains("BD")) // Nota de débito OINV
            {
                tipo = "ND"; // Nota de Débito solo Servicio
                nota = oinvN.buscarNotaDebitoSap(numAtCard);
            }

            return (nota, tipo, subTipo);
        }

    }
}