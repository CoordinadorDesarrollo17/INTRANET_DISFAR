using Capa_Entidad.Almacen_ENT.Tablas;
using Capa_Entidad.DireccionTecnica_ENT.Reportes.BalanceControlados;
using Capa_Entidad.ReportesDigemid_ENT.Formularios;
using Capa_Entidad.ReportesDigemid_ENT.Reportes;
using Capa_Entidad.Seguridad_ENT;
using Capa_Negocio.DireccionTecnica_NEG.Reportes;
using Capa_Negocio.ReportesDigemid_NEG;
using Capa_Usuario.Helpers;
using Microsoft.Reporting.WebForms;
using OfficeOpenXml;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using OfficeOpenXml.Style;
using OfficeOpenXml.Table;
using Rotativa;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
namespace Capa_Usuario.Controllers
{
    public class ReportesDigemidController : Controller
    {
        DocumentosDig_N dN = new DocumentosDig_N();
        /************************* C O N F I G U R A C I Ó N *************************/
        private ActionResult VerificarPermiso(int idOperation)
        {
            var accesoHelper = new Capa_Entidad.AccessoHelper_E
            {
                OpeID = idOperation,
                usuario = (Usuario_E)Session["UsuarioId"],
                controllerDestino = this.ControllerContext.RouteData.Values["controller"].ToString(),
                action = this.ControllerContext.RouteData.Values["action"].ToString(),
                userHostAddress = Request.UserHostAddress,
                userHostName = Request.UserHostName
            };
            return AccesoHelper.GestionarAccesoController(this, accesoHelper);
        }
        /********************************************************************/
        public ActionResult infoKardex()
        {
            Capa_Negocio.General_NEG.Tablas.OWHS_N owhsN = new Capa_Negocio.General_NEG.Tablas.OWHS_N();
            Capa_Negocio.Almacen_NEG.Tablas.OMRC_N omrcN = new Capa_Negocio.Almacen_NEG.Tablas.OMRC_N();
            ViewBag.ListaAlmacenes = owhsN.ListarAlmacenes();
            ViewBag.ListaLaboratorios = omrcN.listarFabricantes();
            return View();
        }
        public ActionResult infoBalanceControlados()
        {
            return View();
        }
        public ActionResult infoAuditoriaStocks()
        {
            Capa_Negocio.Almacen_NEG.Tablas.OITB_N oitbN = new Capa_Negocio.Almacen_NEG.Tablas.OITB_N();
            Capa_Negocio.General_NEG.Tablas.OWHS_N owhsN = new Capa_Negocio.General_NEG.Tablas.OWHS_N();
            ViewBag.ListaAlmacenes = owhsN.ListarAlmacenes("todos");
            ViewBag.ListaGrupoArticulos = oitbN.listarGrupoArticulos();
            return View();
        }
        public ActionResult infoOperacionesLotes()
        {
            Capa_Negocio.Almacen_NEG.Tablas.OITB_N oitbN = new Capa_Negocio.Almacen_NEG.Tablas.OITB_N();
            Capa_Negocio.General_NEG.Tablas.OWHS_N owhsN = new Capa_Negocio.General_NEG.Tablas.OWHS_N();
            ViewBag.ListaAlmacenes = owhsN.ListarAlmacenes();
            ViewBag.ListaGrupoArticulos = oitbN.listarGrupoArticulos();
            return View();
        }
        public ActionResult infoPreciosOpm()
        {
            return View();
        }
        public ActionResult infoVentasArtLote()
        {
            Capa_Negocio.General_NEG.Tablas.OWHS_N owhsN = new Capa_Negocio.General_NEG.Tablas.OWHS_N();
            Capa_Negocio.Almacen_NEG.Tablas.OITM_N oitmN = new Capa_Negocio.Almacen_NEG.Tablas.OITM_N();
            Capa_Negocio.Almacen_NEG.Tablas.OMRC_N omrcN = new Capa_Negocio.Almacen_NEG.Tablas.OMRC_N();
            ViewBag.ListaAlmacenes = new SelectList(owhsN.ListarAlmacenes(), "WhsCode", "WhsName");
            ViewBag.ListaLaboratorios = new SelectList(omrcN.listarFabricantes(), "FirmCode", "U_SYP_DESC");
            return View();
        }
        public ActionResult ListaProductosAuditoria(OITM_E o)
        {
            Capa_Negocio.Almacen_NEG.Tablas.OITM_N oitmN = new Capa_Negocio.Almacen_NEG.Tablas.OITM_N();
            return Content(oitmN.datalistArticulos(o));
        }
        public ActionResult ListaProductosHtml(OITM_E o)
        {
            Capa_Negocio.Almacen_NEG.Tablas.OITM_N oitmN = new Capa_Negocio.Almacen_NEG.Tablas.OITM_N();
            return Content(oitmN.datalistArticulosLabo(o));
        }
        public ActionResult tbReportePreciosOpm(string FecIni, string FecFin)
        {
            ReportViewer rp = new ReportViewer();
            try
            {
                rp.ProcessingMode = ProcessingMode.Local;
                rp.SizeToReportContent = true;
                rp.LocalReport.ReportPath =
                    Request.MapPath(Request.ApplicationPath) + @"Reportes\RptDireccionTecnica\RptPreciosOpm.rdlc";
                rp.LocalReport.DataSources.Add(new ReportDataSource("DS_PreciosOpm", dN.tbReportePreciosOpm(FecIni, FecFin)));
                ViewBag.REPORTE = rp;
            }
            catch { }
            return View("reporteViewer");
        }
        public ActionResult tbReporteOperacionesLotes(FrmOperacionesLotes_E f)
        {
            ViewBag.Frm = f;
            try
            {
                return View(dN.ReporteOperacionesLotes(f, "Si"));
            }
            catch (Exception e) { ViewBag.Mensaje = e.Message; return View(new List<OperacionesLotes_E>()); }
        }
        public JsonResult tbReporteOperacionesLotesDet(FrmOperacionesLotes_E f)
        {
            try
            {
                return Json(dN.ReporteOperacionesLotes(f, "No"));
            }
            catch (Exception e) { ViewBag.Mensaje = e.Message; return Json(new List<OperacionesLotes_E>()); }
        }
        public ActionResult tbReporteAuditoriaStocks(FrmAuditoriaStocks_E f)
        {
            List<string> CodArticulos = new List<string>();
            ViewBag.CodArticulos = CodArticulos;
            try
            {
                List<AuditoriaStocks_E> lista = dN.ReporteAuditoriaStocks(f);
                foreach (AuditoriaStocks_E a in lista)
                {
                    if (CodArticulos.FirstOrDefault(x => x.Equals(a.ItemCode)) == null)
                    {
                        CodArticulos.Add(a.ItemCode);
                    }
                }
                ViewBag.CodArticulos = CodArticulos;
                return View(lista);
            }
            catch (Exception e) { ViewBag.Mensaje = e.Message; return View(new List<AuditoriaStocks_E>()); }
        }
        /******************************* K Á R D E X   C O N T R O L   D E   E X I S T E N C I A S *******************************/
        public ActionResult RptKardexAlmacenes(FrmKardex_E filtros)
        {
            filtros.DetKardexAlmacenes = new Capa_Negocio.DireccionTecnica_NEG.Reportes.ReportesDigemid_N().ReporteKardexAlmacenes(filtros);
            return View(filtros);
        }
        public ActionResult RptExcelKardexAlmacenes(FrmKardex_E filtros, int idOperation = 1327)
        {
            string acceso = AccesoHelper.VerificarAccesos(idOperation, (Usuario_E)Session["UsuarioId"], this.ControllerContext.RouteData.Values["action"].ToString(), Request.UserHostAddress, Request.UserHostName);
            if (acceso == "C_Access")
            {
                ReportesDigemid_N digN = new ReportesDigemid_N();
                string nombreArchivo = $"RptKardexAlmacenes.xlsx";
                string excelContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                var datos = digN.ReporteKardexAlmacenes(filtros);
                using (var libro = new ExcelPackage())
                {
                    var worksheet = libro.Workbook.Worksheets.Add("RptExcelKardexAlmacenes");
                    worksheet.Cells["A1"].LoadFromCollection(datos, PrintHeaders: true);
                    if (datos != null)
                    {
                        if (datos.Count >= 1)
                        {
                            for (var col = 1; col <= 18; col++)
                            {
                                worksheet.Column(col).AutoFit();
                            }
                            var tabla = worksheet.Tables.Add(new ExcelAddressBase(fromRow: 1, fromCol: 1, toRow: datos.Count + 1, toColumn: 18), "RptExcelKardexAlmacenes");
                            tabla.ShowHeader = true;
                            tabla.TableStyle = TableStyles.Medium2;
                        }
                    }
                    return File(libro.GetAsByteArray(), excelContentType, nombreArchivo);
                }
            }
            else
            {
                return null;
            }
        }
        /*****************************************************************************************/
        /********************** B A L A N C E   C O N T R O L A D O S **********************/
        public ActionResult ExcelBalanceControlados_Ingreso(IEnumerable<dynamic> lista)
        {
            using (var package = new ExcelPackage())
            {
                var ws = package.Workbook.Worksheets.Add("Ingresos");
                int row = 1;
                ws.Cells[row, 1].Value = "CÓDIGO";
                ws.Cells[row, 2].Value = "DESCRIPCIÓN DEL PRODUCTO";
                ws.Cells[row, 4].Value = "N° REGISTRO SANITARIO";
                ws.Cells[row, 5].Value = "CONCENTRACIÓN";
                ws.Cells[row, 6].Value = "FORMA PRESENTACIÓN";
                ws.Cells[row, 7].Value = "F.F";
                ws.Cells[row, 8].Value = "LOTE";
                ws.Cells[row, 9].Value = "CANTIDAD";
                ws.Cells[row, 10].Value = "PROVEEDOR";
                ws.Cells[row, 11].Value = "R.U.C.";
                ws.Cells[row, 12].Value = "DIRECCIÓN";
                ws.Cells[row, 16].Value = "N° FACTURA";
                ws.Cells[row, 17].Value = "FECHA";

                ws.Cells[row, 2, row, 3].Merge = true;
                ws.Cells[row, 12, row, 15].Merge = true;

                row++;

                ws.Cells[row, 2].Value = "NOMBRE GENÉRICO";
                ws.Cells[row, 3].Value = "NOMBRE COMERCIAL";
                ws.Cells[row, 12].Value = "CALLE / JR / AV";
                ws.Cells[row, 13].Value = "DISTRITO";
                ws.Cells[row, 14].Value = "PROVINCIA";
                ws.Cells[row, 15].Value = "DEPARTAMENTO";

                ws.Cells[1, 1, 2, 1].Merge = true;
                ws.Cells[1, 4, 2, 4].Merge = true;
                ws.Cells[1, 5, 2, 5].Merge = true;
                ws.Cells[1, 6, 2, 6].Merge = true;
                ws.Cells[1, 7, 2, 7].Merge = true;
                ws.Cells[1, 8, 2, 8].Merge = true;
                ws.Cells[1, 9, 2, 9].Merge = true;
                ws.Cells[1, 10, 2, 10].Merge = true;
                ws.Cells[1, 11, 2, 11].Merge = true;
                ws.Cells[1, 16, 2, 16].Merge = true;
                ws.Cells[1, 17, 2, 17].Merge = true;

                using (var range = ws.Cells[1, 1, 2, 17])
                {
                    range.Style.Font.Bold = true;
                    range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    range.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    range.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                }

                row++;

                string codActual = "";
                decimal total = 0;

                foreach (var det in lista)
                {
                    if (codActual != "" && codActual != det.CodProducto)
                    {
                        ws.Cells[row, 7].Value = "TOTAL";
                        ws.Cells[row, 7, row, 8].Merge = true;
                        ws.Cells[row, 9].Value = total;
                        ws.Cells[row, 7, row, 9].Style.Font.Bold = true;

                        row++;
                        total = 0;
                    }
                    ws.Cells[row, 1].Value = det.CodProducto;
                    ws.Cells[row, 2].Value = det.NombreGenerico;
                    ws.Cells[row, 3].Value = det.NombreComercial;
                    ws.Cells[row, 4].Value = det.RegSanitario;
                    ws.Cells[row, 5].Value = det.Concentracion;
                    ws.Cells[row, 6].Value = det.FormaPresentacion;
                    ws.Cells[row, 7].Value = det.FormaFamaceutica;
                    ws.Cells[row, 8].Value = det.NroLote;
                    ws.Cells[row, 9].Value = det.CantLote;
                    ws.Cells[row, 10].Value = det.Proveedor;
                    ws.Cells[row, 11].Value = det.RucProveedor;
                    ws.Cells[row, 12].Value = det.CalleJrAvN;
                    ws.Cells[row, 13].Value = det.Distrito;
                    ws.Cells[row, 14].Value = det.Provincia;
                    ws.Cells[row, 15].Value = det.Departamento;
                    ws.Cells[row, 16].Value = det.NroFacturaNcredito;
                    ws.Cells[row, 17].Value = det.Fecha;

                    total += det.CantLote;
                    codActual = det.CodProducto;
                    row++;
                }
                ws.Cells[row, 7].Value = "TOTAL";
                ws.Cells[row, 7, row, 8].Merge = true;
                ws.Cells[row, 9].Value = total;
                ws.Cells[row, 7, row, 9].Style.Font.Bold = true;

                ws.Cells[1, 1, row, 17].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                ws.Cells.AutoFitColumns();
                ws.Column(4).Width = 22;

                return File(
                    package.GetAsByteArray(),
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    "BalanceControlados_Ingresos.xlsx"
                );
            }
        }
        public ActionResult ExcelBalanceControlados_Egreso(IEnumerable<dynamic> lista)
        {
            using (var package = new ExcelPackage())
            {
                var ws = package.Workbook.Worksheets.Add("Egresos");
                int row = 1;

                // =========================
                // CABECERA
                // =========================
                ws.Cells[row, 1].Value = "CÓDIGO";
                ws.Cells[row, 2].Value = "DESCRIPCIÓN DEL PRODUCTO";
                ws.Cells[row, 4].Value = "N° REGISTRO SANITARIO";
                ws.Cells[row, 5].Value = "CONCENTRACIÓN";
                ws.Cells[row, 6].Value = "FORMA PRESENTACIÓN";
                ws.Cells[row, 7].Value = "F.F";
                ws.Cells[row, 8].Value = "LOTE";
                ws.Cells[row, 9].Value = "CANTIDAD";
                ws.Cells[row, 10].Value = "ESTABLECIMIENTO ATENDIDO";
                ws.Cells[row, 11].Value = "R.U.C.";
                ws.Cells[row, 12].Value = "DIRECCIÓN";
                ws.Cells[row, 16].Value = "N° FACTURA / BOLETA";
                ws.Cells[row, 17].Value = "FECHA";

                ws.Cells[row, 2, row, 3].Merge = true;
                ws.Cells[row, 12, row, 15].Merge = true;

                row++;

                ws.Cells[row, 2].Value = "NOMBRE GENÉRICO";
                ws.Cells[row, 3].Value = "NOMBRE COMERCIAL";
                ws.Cells[row, 12].Value = "CALLE / JR / AV";
                ws.Cells[row, 13].Value = "DISTRITO";
                ws.Cells[row, 14].Value = "PROVINCIA";
                ws.Cells[row, 15].Value = "DEPARTAMENTO";

                ws.Cells[1, 1, 2, 1].Merge = true;
                ws.Cells[1, 4, 2, 4].Merge = true;
                ws.Cells[1, 5, 2, 5].Merge = true;
                ws.Cells[1, 6, 2, 6].Merge = true;
                ws.Cells[1, 7, 2, 7].Merge = true;
                ws.Cells[1, 8, 2, 8].Merge = true;
                ws.Cells[1, 9, 2, 9].Merge = true;
                ws.Cells[1, 10, 2, 10].Merge = true;
                ws.Cells[1, 11, 2, 11].Merge = true;
                ws.Cells[1, 16, 2, 16].Merge = true;
                ws.Cells[1, 17, 2, 17].Merge = true;

                using (var range = ws.Cells[1, 1, 2, 17])
                {
                    range.Style.Font.Bold = true;
                    range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    range.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    range.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                }

                row++;

                // =========================
                // CUERPO
                // =========================
                string codActual = "";
                decimal total = 0;

                foreach (var det in lista)
                {
                    if (codActual != "" && codActual != det.CodProducto)
                    {
                        ws.Cells[row, 7].Value = "TOTAL";
                        ws.Cells[row, 7, row, 8].Merge = true;
                        ws.Cells[row, 9].Value = total;
                        ws.Cells[row, 7, row, 9].Style.Font.Bold = true;

                        row++;
                        total = 0;
                    }

                    ws.Cells[row, 1].Value = det.CodProducto;
                    ws.Cells[row, 2].Value = det.NombreGenerico;
                    ws.Cells[row, 3].Value = det.NombreComercial;
                    ws.Cells[row, 4].Value = det.RegSanitario;
                    ws.Cells[row, 5].Value = det.Concentracion;
                    ws.Cells[row, 6].Value = det.FormaPresentacion;
                    ws.Cells[row, 7].Value = det.FormaFamaceutica;
                    ws.Cells[row, 8].Value = det.NroLote;
                    ws.Cells[row, 9].Value = det.CantLote;
                    ws.Cells[row, 10].Value = det.Establecimiento;
                    ws.Cells[row, 11].Value = det.RucEstab;
                    ws.Cells[row, 12].Value = det.CalleJrAvN;
                    ws.Cells[row, 13].Value = det.Distrito;
                    ws.Cells[row, 14].Value = det.Provincia;
                    ws.Cells[row, 15].Value = det.Departamento;
                    ws.Cells[row, 16].Value = det.NroFactura;
                    ws.Cells[row, 17].Value = det.Fecha;

                    total += det.CantLote;
                    codActual = det.CodProducto;
                    row++;
                }

                // TOTAL FINAL
                ws.Cells[row, 7].Value = "TOTAL";
                ws.Cells[row, 7, row, 8].Merge = true;
                ws.Cells[row, 9].Value = total;
                ws.Cells[row, 7, row, 9].Style.Font.Bold = true;

                ws.Cells[1, 1, row, 17].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                ws.Cells.AutoFitColumns();
                ws.Column(4).Width = 22;
                ws.Column(16).Width = 30;

                return File(
                    package.GetAsByteArray(),
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    "BalanceControlados_Egresos.xlsx"
                );
            }
        }
        public ActionResult ExcelBalanceControlados_Consolidado(IEnumerable<RptBalanceControladosConsolidado_E> lista)
        {
            using (var package = new ExcelPackage())
            {
                var ws = package.Workbook.Worksheets.Add("Consolidado");
                int row = 1;

                ws.Column(1).Width = 20;
                ws.Column(2).Width = 60;
                ws.Column(3).Width = 60;
                ws.Column(4).Width = 25;
                ws.Column(5).Width = 15;
                ws.Column(6).Width = 15;
                ws.Column(7).Width = 15;
                ws.Column(8).Width = 15;
                ws.Column(9).Width = 15;
                ws.Column(10).Width = 15;
                ws.Column(11).Width = 20;
                // =========================
                // CABECERA
                // =========================
                ws.Cells[row, 1].Value = "CÓDIGO";
                ws.Cells[row, 2].Value = "DESCRIPCIÓN DEL PRODUCTO";
                ws.Cells[row, 4].Value = "CONCENTRACIÓN";
                ws.Cells[row, 5].Value = "F.F.";
                ws.Cells[row, 6].Value = "SALDO ANTERIOR";
                ws.Cells[row, 7].Value = "INGRESOS";
                ws.Cells[row, 9].Value = "EGRESOS";
                ws.Cells[row, 11].Value = "SALDO ACTUAL";

                ws.Cells[row, 2, row, 3].Merge = true;
                ws.Cells[row, 7, row, 8].Merge = true;
                ws.Cells[row, 9, row, 10].Merge = true;

                row++;

                ws.Cells[row, 2].Value = "NOMBRE GENÉRICO";
                ws.Cells[row, 3].Value = "NOMBRE COMERCIAL";
                ws.Cells[row, 7].Value = "COMPRA";
                ws.Cells[row, 8].Value = "OTROS";
                ws.Cells[row, 9].Value = "VENTA";
                ws.Cells[row, 10].Value = "OTROS";

                ws.Cells[1, 1, 2, 1].Merge = true;
                ws.Cells[1, 4, 2, 4].Merge = true;
                ws.Cells[1, 5, 2, 5].Merge = true;
                ws.Cells[1, 6, 2, 6].Merge = true;
                ws.Cells[1, 11, 2, 11].Merge = true;

                using (var range = ws.Cells[1, 1, 2, 11])
                {
                    range.Style.Font.Bold = true;
                    range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    range.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    range.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                }

                row++;

                // =========================
                // CUERPO
                // =========================
                string codActual = "";
                decimal sumCompra = 0;
                decimal sumOtrosIng = 0;
                decimal sumVenta = 0;
                decimal sumOtrosEgr = 0;
                decimal saldoActual = 0;

                RptBalanceControladosConsolidado_E baseRow = null;

                foreach (var det in lista)
                {
                    if (codActual != "" && codActual != det.CodProducto)
                    {
                        ws.Cells[row, 1].Value = baseRow.CodProducto;
                        ws.Cells[row, 2].Value = baseRow.NombreGenerico;
                        ws.Cells[row, 3].Value = baseRow.NombreComercial;
                        ws.Cells[row, 4].Value = baseRow.Concentracion;
                        ws.Cells[row, 5].Value = baseRow.FormaFamaceutica;
                        ws.Cells[row, 6].Value = baseRow.SaldoAnterior;
                        ws.Cells[row, 7].Value = sumCompra;
                        ws.Cells[row, 8].Value = sumOtrosIng;
                        ws.Cells[row, 9].Value = sumVenta;
                        ws.Cells[row, 10].Value = sumOtrosEgr;
                        ws.Cells[row, 11].Value = saldoActual + baseRow.SaldoAnterior;

                        row++;

                        sumCompra = 0;
                        sumOtrosIng = 0;
                        sumVenta = 0;
                        sumOtrosEgr = 0;
                    }

                    baseRow = det;

                    sumCompra += det.Compra;
                    sumOtrosIng += det.OtrosIngresosNC;
                    sumVenta += det.Venta;
                    sumOtrosEgr += det.OtrosEgresosDEV;

                    saldoActual = sumCompra + sumOtrosIng - sumVenta - sumOtrosEgr;
                    codActual = det.CodProducto;
                }

                // ÚLTIMO REGISTRO
                if (baseRow != null)
                {
                    ws.Cells[row, 1].Value = baseRow.CodProducto;
                    ws.Cells[row, 2].Value = baseRow.NombreGenerico;
                    ws.Cells[row, 3].Value = baseRow.NombreComercial;
                    ws.Cells[row, 4].Value = baseRow.Concentracion;
                    ws.Cells[row, 5].Value = baseRow.FormaFamaceutica;
                    ws.Cells[row, 6].Value = baseRow.SaldoAnterior;
                    ws.Cells[row, 7].Value = sumCompra;
                    ws.Cells[row, 8].Value = sumOtrosIng;
                    ws.Cells[row, 9].Value = sumVenta;
                    ws.Cells[row, 10].Value = sumOtrosEgr;
                    ws.Cells[row, 11].Value = saldoActual + baseRow.SaldoAnterior;
                }

                ws.Cells[1, 1, row, 11].Style.Border.BorderAround(ExcelBorderStyle.Thin);
        
                return File(
                    package.GetAsByteArray(),
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    "BalanceControlados_Consolidado.xlsx"
                );
            }
        }

        public ActionResult ExcelBalanceControlados(FrmBalanceControlados_E filtros)
        {
            ReportesDigemid_N digN = new ReportesDigemid_N();

            if (filtros.Informe.Equals("Ingresos"))
            {
                var lista = digN.ReporteBalanceControladosIngreso(filtros);
                return ExcelBalanceControlados_Ingreso(lista);
            }
            else if (filtros.Informe.Equals("Egresos"))
            {
                var lista = digN.ReporteBalanceControladosEgreso(filtros);
                return ExcelBalanceControlados_Egreso(lista);
            }
            else if (filtros.Informe.Equals("Consolidado"))
            {
                var lista = digN.ReporteBalanceControladosConsolidado(filtros);
                return ExcelBalanceControlados_Consolidado(lista);
            }
        
            return Content("No hay datos");
   
        }



        public ActionResult PdfRptBalanceControlados(FrmBalanceControlados_E filtros, string impresion)
        {
            return new ActionAsPdf("RptBalanceControlados", new { Informe = filtros.Informe, FecIni = filtros.FecIni, FecFin = filtros.FecFin, TipoControlado = filtros.TipoControlado, Impresion = impresion }) { FileName = $"ReporteBalanceControlados{filtros.Informe}.pdf", PageOrientation = Rotativa.Options.Orientation.Landscape, PageSize = Rotativa.Options.Size.A4 };
        }
        public ActionResult RptBalanceControlados(FrmBalanceControlados_E filtros)
        {
            ReportesDigemid_N digN = new ReportesDigemid_N();
            ReportViewer rp = new ReportViewer();
            Dictionary<string, string> tipos = new Dictionary<string, string>
            {
                {"P", "PRECURSORES" },
                {"S", "PSICOTROPICOS" },
                {"E", "ESTUPEFACIENTES" },
            };
            if (!string.IsNullOrWhiteSpace(filtros.TipoControlado))
            {
                filtros.DescTipoControlado = tipos[filtros.TipoControlado];
            }
            try
            {
                rp.ProcessingMode = ProcessingMode.Local;
                rp.SizeToReportContent = true;
                if (filtros.Informe.Equals("Ingresos"))
                {
                    filtros.DetBalanceControladosIngreso = digN.ReporteBalanceControladosIngreso(filtros);
                    return View("BalanceControlados/RptBalanceControladosIngreso", filtros);
                }
                else if (filtros.Informe.Equals("Egresos"))
                {
                    filtros.DetBalanceControladosEgreso = digN.ReporteBalanceControladosEgreso(filtros);
                    return View("BalanceControlados/RptBalanceControladosEgreso", filtros);
                }
                else if (filtros.Informe.Equals("Consolidado"))
                {
                    filtros.DetBalanceControladosConsolidado = digN.ReporteBalanceControladosConsolidado(filtros);
                    return View("BalanceControlados/RptBalanceControladosConsolidado", filtros);
                }
                else if (filtros.Informe.Equals("Libro Controlados"))
                {
                    filtros.DetBalanceControladosLibroControlados = digN.ReporteBalanceControladosLibroControlados(filtros);
                    return View("BalanceControlados/RptBalanceControladosLibroControlados", filtros);
                }
            }
            catch { }
            return View(filtros);
        }
        /*****************************************************************************************/
        /********************** O P E R A C I O N E S   D E   N °   L O T E **********************/
        public ActionResult RptOperacionesLotes(FrmOperacionesLotes_E f)
        {
            ViewBag.Frm = f;
            try
            {
                return View(dN.ReporteOperacionesLotes(f, "Si"));
            }
            catch (Exception e) { ViewBag.Mensaje = e.Message; return View(new List<OperacionesLotes_E>()); }
        }
        public JsonResult RptOperacionesLotesDet(FrmOperacionesLotes_E f)
        {
            try
            {
                return Json(dN.ReporteOperacionesLotes(f, "No"));
            }
            catch (Exception e) { ViewBag.Mensaje = e.Message; return Json(new List<OperacionesLotes_E>()); }
        }
        /*****************************************************************************************/
        /********************** R E G I S T R O   S A N I T A R I O **********************/
        public ActionResult InfoRegistroSanitario()
        {
            Capa_Negocio.Almacen_NEG.Tablas.OMRC_N omrcN = new Capa_Negocio.Almacen_NEG.Tablas.OMRC_N();
            ViewBag.ListaLaboratorios = new SelectList(omrcN.listarFabricantes(), "FirmCode", "U_SYP_DESC");
            return View();
        }
        public ActionResult RptExcelRegistroSanitario(string codArticulo, string firmCode, int idOperation = 1327)
        {
            string acceso = AccesoHelper.VerificarAccesos(idOperation, (Usuario_E)Session["UsuarioId"], this.ControllerContext.RouteData.Values["action"].ToString(), Request.UserHostAddress, Request.UserHostName);
            if (acceso == "C_Access")
            {
                ReportesDigemid_N digN = new ReportesDigemid_N();
                string nombreArchivo = $"RptExcelRegistroSanitario.xlsx";
                string excelContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                var datos = digN.ReporteRegistroSanitario(codArticulo, firmCode);
                using (var libro = new ExcelPackage())
                {
                    var worksheet = libro.Workbook.Worksheets.Add("RptExcelRegistroSanitario");
                    worksheet.Cells["A1"].LoadFromCollection(datos, PrintHeaders: true);
                    if (datos != null)
                    {
                        if (datos.Count >= 1)
                        {
                            for (var col = 1; col <= 10; col++)
                            {
                                worksheet.Column(col).AutoFit();
                            }
                            var tabla = worksheet.Tables.Add(new ExcelAddressBase(fromRow: 1, fromCol: 1, toRow: datos.Count + 1, toColumn: 10), "RptExcelRegistroSanitario");
                            tabla.ShowHeader = true;
                            tabla.TableStyle = TableStyles.Medium2;
                        }
                    }
                    return File(libro.GetAsByteArray(), excelContentType, nombreArchivo);
                }
            }
            else
            {
                return null;
            }
        }
        /*******************************************************************************/
        public ActionResult tbVentasArtLote(FrmKardex_E f)
        {
            //verificacionAccesos(0);
            ReportViewer rp = new ReportViewer();
            try
            {
                rp.ProcessingMode = ProcessingMode.Local;
                rp.SizeToReportContent = true;
                rp.LocalReport.ReportPath =
                    Request.MapPath(Request.ApplicationPath) + @"Reportes\RptDireccionTecnica\RptVentasArtLote.rdlc";
                rp.LocalReport.DataSources.Add(new ReportDataSource("DS_VentasArtLote", dN.ListaVentasArtLote(f)));
            }
            catch (Exception e) { ViewBag.Mensaje = e.Message; }
            ViewBag.REPORTE = rp;
            return View("reporteViewer");
        }
        public ActionResult reporteViewer()
        {
            return View();
        }        
    }
}