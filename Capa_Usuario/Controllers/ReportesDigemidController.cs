using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Capa_Entidad.Seguridad_ENT;
using Capa_Entidad.Almacen_ENT.Tablas;
using Capa_Entidad.ReportesDigemid_ENT.Formularios;
using Capa_Entidad.ReportesDigemid_ENT.Reportes;
using Capa_Negocio.ReportesDigemid_NEG;
using Microsoft.Reporting.WebForms;
using Rotativa;
using Capa_Negocio.DireccionTecnica_NEG.Reportes;
using OfficeOpenXml;
using OfficeOpenXml.Table;
using Capa_Usuario.Helpers;

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
            // ViewBag.ListaProductos = oitmN.listarArticulos(null);
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
            //verificacionAccesos(0);
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
            //verificacionAccesos(0);
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
            //verificacionAccesos(0);
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
        public ActionResult PdfRptBalanceControlados(FrmBalanceControlados_E filtros, string impresion)
        {
            return new ActionAsPdf("RptBalanceControlados", new { Informe = filtros.Informe, FecIni = filtros.FecIni, FecFin = filtros.FecFin, TipoControlado = filtros.TipoControlado, Impresion = impresion }) { FileName = $"ReporteBalanceControlados{filtros.Informe}.pdf", PageOrientation = Rotativa.Options.Orientation.Landscape, PageSize = Rotativa.Options.Size.A4 };
        }

        public ActionResult RptBalanceControlados(FrmBalanceControlados_E filtros)
        {
            //verificacionAccesos(0);
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
            //verificacionAccesos(0);
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