using Capa_Entidad.Almacen_ENT.Tablas;
using Capa_Entidad.Compras_ENT.Tablas;
using Capa_Entidad.DireccionTecnica_ENT.TablasSql;
using Capa_Entidad.General_ENT.Tablas;
using Capa_Entidad.General_ENT.TablasSql;
using Capa_Entidad.ReportesDigemid_ENT;
using Capa_Entidad.Seguridad_ENT;
using Capa_Entidad.Ventas_ENT.Tablas;
using Capa_Negocio.Almacen_NEG.Tablas;
using Capa_Negocio.DireccionTecnica_NEG.TablasSql;
using Capa_Negocio.General_NEG.Tablas;
using Capa_Negocio.General_NEG.TablasSql;
using Capa_Negocio.ReportesDigemid_NEG;
using Capa_Negocio.Ventas_NEG.Tablas;
using Capa_Negocio.Ventas_NEG.TablasSql;
using Rotativa;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Capa_Negocio.DireccionTecnica_NEG.TablasHANA;
using Capa_Usuario.Helpers;
using Capa_Negocio;
namespace Capa_Usuario.Controllers
{
    public class DireccionTecnicaController : Controller
    {
        DocumentosDig_N dgN = new DocumentosDig_N();
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
        /** 
         * Método para buscar el responsable de almacén de las actas de recepción y despacho 
         * @param {String} tipoFirma - Para saber que tipo de firma estamos buscando 
         * @param {String} almacen
         * @returns {Dictionary} result
         */
        protected Dictionary<string, string> BuscarFirmas(string tipoFirma, string almacen)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            if (!string.IsNullOrWhiteSpace(tipoFirma) && !string.IsNullOrWhiteSpace(almacen))
            {
                // tipoFirma = 'ResponsableALMActas' abarca -> ActaRecepcionEm, ActaRecepcionTs
                var firmas = new Firmas_N().ListarFirmas(new Firmas_E() { TipoFirma = tipoFirma, CodigoAlmacen = almacen });

                if (firmas != null && firmas.Any())
                {
                    string FilePath;
                    var firma = firmas.First();

                    FilePath = firma.RutaFirma;
                    result.Add("NombApe", $"{firma.Nombres} {firma.Apellidos}");
                    byte[] archivo = System.IO.File.ReadAllBytes(FilePath);
                    var base64 = Convert.ToBase64String(archivo);                                   //La propiedad de tu modelo que es byte[]
                    result.Add("Firma", String.Format("data:image/gif;base64,{0}", base64));       // Damos formato para indicar que se trata de una cadena base64
                }
            }
            return result;
        }
        public ActionResult LayoutOrdenDeVenta(int DocNum, int idOperation = 1702)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                return RedirectToAction("OrdenDeVenta", "Ventas", new { DocNum = DocNum });
            }
            else
            {
                return resultadoAcceso;
            }
        }
        public ActionResult ListadoFacturasDeVenta(OINV_E fil, int idOperation = 1801)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                if (fil != null) { ViewBag.Oinv = fil; } else { ViewBag.Oinv = new OINV_E(); }
                ViewBag.ListaLugarEntregas = new COB_LUG_ENTREGA_N().listadoLugaresDeEntrega();
                return View(new OINV_N().listadoFacturasDeVenta(fil));
            }
            else
            {
                return resultadoAcceso;
            }
        }
        public ActionResult ActaRecepcionVt(int DocEntry, int idOperation = 1802)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                var result = dgN.ConsultarActaRecepcionVt(DocEntry);
                if (result != null && result[0].Almacen != null)
                {
                    var datosFirma = BuscarFirmas("ResponsableALMActas", result[0].CodAlmacen);
                    if (datosFirma != null && datosFirma.Count >= 1)
                    {
                        ViewBag.DatosResponsable = datosFirma["NombApe"];
                        ViewBag.FirmaResponsable = datosFirma["Firma"];
                    }
                }
                return View(result);
            }
            else
            {
                return resultadoAcceso;
            }
        }
        public ActionResult ActaDespachoVt(int DocEntry, int idOperation = 1803)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                var result = dgN.ConsultarActaDespachoVt(DocEntry);
                if (result != null && result.Any() && result[0].T1_WhsCode != null)
                {
                    var datosFirma = BuscarFirmas("ResponsableALMActas", result[0].T1_WhsCode);
                    if (datosFirma != null && datosFirma.Any())
                    {
                        ViewBag.DatosResponsable = datosFirma["NombApe"];
                        ViewBag.FirmaResponsable = datosFirma["Firma"];
                    }
                }
                return View(result);
            }
            else
            {
                return resultadoAcceso;
            }
        }
        public ActionResult OrganolepticoVt(int DocEntry, int idOperation = 1804)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);

            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                var orgVT = dgN.ConsultarOrganolepticoVt(DocEntry);
                if (orgVT != null && orgVT.Any())
                {
                    var result = BuscarFirmas("QuimicoFarmaceutico", orgVT[0].CodAlmacen);
                    if (result != null && result.Any())
                    {
                        ViewBag.QuimicoFarmaceuticoAsistente = result["NombApe"];
                        ViewBag.Firma = result["Firma"];
                    }
                }

                var firmaResponsableDT = BuscarFirmas("ResponsableDT", "08");
                ViewBag.FirmaDT = firmaResponsableDT != null && firmaResponsableDT.Any() ? firmaResponsableDT["Firma"] : "";

                return View(orgVT);
            }
            else
            {
                return resultadoAcceso;
            }
        }
        public ActionResult CalcularPdfsDescarga(int idOperation = 1805)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                return View();
            }
            else
            {
                return resultadoAcceso;
            }
        }
        public JsonResult CalculadoraPdfXampp(int opcion, string Fecha, string U_SYP_STATUS, string U_COB_LUGAREN, string TipoComprobante)
        {
            string mensaje = string.Empty;
            OINV_N oinv = new OINV_N(); OWTR_N owtr = new OWTR_N(); ODLN_N odln = new ODLN_N();
            switch (opcion)
            {
                case 1:
                    mensaje = owtr.CalcularPdfsActaRecepcion(Fecha, U_SYP_STATUS, U_COB_LUGAREN);
                    break;
                case 2:
                    mensaje = oinv.CalcularPdfsActaDespachoOINV(Fecha, U_SYP_STATUS, U_COB_LUGAREN, TipoComprobante);
                    break;
                case 3:
                    mensaje = owtr.CalcularPdfsActaDespachoOWTR(Fecha, U_SYP_STATUS, U_COB_LUGAREN);
                    break;
                case 4:
                    mensaje = odln.CalcularPdfsActaDespachoODLN(Fecha, U_SYP_STATUS, U_COB_LUGAREN, TipoComprobante);
                    break;
            }
            return Json(mensaje);
        }
        public JsonResult DetalleCalculadoraPdf(int opcion, string Fecha, string U_SYP_STATUS, string U_COB_LUGAREN, string TipoComprobante)
        {
            List<(string, int)> resultado = new List<(string, int)>();
            OINV_N oinv = new OINV_N(); OWTR_N owtr = new OWTR_N(); ODLN_N odln = new ODLN_N();
            switch (opcion)
            {
                case 1:
                    resultado = owtr.DetalleCalculadoraPdf(Fecha, U_SYP_STATUS, U_COB_LUGAREN);
                    break;
                case 2:
                    resultado = oinv.DetalleCalculadoraPdfOINV(Fecha, U_SYP_STATUS, U_COB_LUGAREN, TipoComprobante);
                    break;
                case 3:
                    resultado = owtr.DetalleCalculadoraPdfOWTR(Fecha, U_SYP_STATUS, U_COB_LUGAREN);
                    break;
                case 4:
                    resultado = odln.DetalleCalculadoraPdf(Fecha, U_SYP_STATUS, U_COB_LUGAREN);
                    break;
            }
            return Json(resultado);
        }
        public ActionResult frmActaRecepcion()
        {
            return View();
        }
        public ActionResult frmActaDespachoOINV()
        {
            return View();
        }
        public ActionResult frmActaDespachoOWTR()
        {
            return View();
        }
        public ActionResult frmActaDespachoODLN()
        {
            return View();
        }
        public ActionResult ComprobanteDePago(int DocEntry, string Tipo, int idOperation = 1806)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                ViewBag.Tipo = Tipo;
                return View(dgN.ConsultarComprobanteDePago(DocEntry));
            }
            else
            {
                return resultadoAcceso;
            }
        }
        public ActionResult ListadoBoletasDeVenta(OINV_E fil, int idOperation = 1901)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                if (fil != null) { ViewBag.Oinv = fil; } else { ViewBag.Oinv = new OINV_E(); }
                ViewBag.ListaLugarEntregas = new Capa_Negocio.General_NEG.Tablas.COB_LUG_ENTREGA_N().listadoLugaresDeEntrega();
                return View(new Capa_Negocio.Ventas_NEG.Tablas.OINV_N().listadoBoletasDeVenta(fil));
            }
            else
            {
                return resultadoAcceso;
            }
        }
        public ActionResult ListadoEntregasDeVenta(ODLN_E fil, int idOperation = 2001)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                if (fil != null) { ViewBag.Odln = fil; } else { ViewBag.Odln = new ODLN_E(); }
                ViewBag.ListaLugarEntregas = new COB_LUG_ENTREGA_N().listadoLugaresDeEntrega();
                return View(new ODLN_N().listarEntregasVenta(fil));
            }
            else
            {
                return resultadoAcceso;
            }
        }
        public ActionResult ListadoNotasDeCreditoVenta(ORIN_E fil, int idOperation = 2101)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                if (fil != null) { ViewBag.Orin = fil; } else { ViewBag.Orin = new ORIN_E(); }
                return View(new ORIN_N().Listar(fil));
            }
            else
            {
                return resultadoAcceso;
            }
        }
        public ActionResult NotaDeCreditoVentaArticulos(int DocEntry, int idOperation = 2102)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                return View(dgN.ConsultarNotaCreditoVentaArticulos(DocEntry));
            }
            else
            {
                return resultadoAcceso;
            }
        }
        public ActionResult ListadoTransferenciasDeStock(OWTR_E fil, int idOperation = 2201)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                if (fil != null) { ViewBag.Owtr = fil; } else { ViewBag.Owtr = new OWTR_E(); }
                var almacenes = new Capa_Negocio.General_NEG.Tablas.OWHS_N().ListarAlmacenes();

                ViewBag.Almacenes = almacenes != null && almacenes.Any()
                    ? almacenes.OrderBy(a => a.WhsName).ToList()
                    : new List<Capa_Entidad.General_ENT.Tablas.OWHS_E>();

                ViewBag.DictionaryAlmacenes = almacenes != null && almacenes.Any()
                    ? almacenes
                        .GroupBy(a => a.WhsCode)
                        .Select(g => g.First())
                        .ToDictionary(a => a.WhsCode, a => a.WhsName)
                    : new Dictionary<string, string>();

                var oslps = new Capa_Negocio.General_NEG.Tablas.OSLP_N().listadoOslp("ALM");
                ViewBag.ListaOslp = oslps ?? new List<OSLP_E>();

                return View(new Capa_Negocio.Almacen_NEG.Tablas.OWTR_N().listadoTransferenciasStock(fil));
            }
            else
            {
                return resultadoAcceso;
            }
        }
        public JsonResult CambiarEstadoOWTR(SQL_OWTR_E datos)
        {
            //verificacionAccesos(0);         // Validar sesion logueada, solo para ajax
            if (datos.DocNumSAP >= 1 && !string.IsNullOrWhiteSpace(datos.Estado))
            {
                Usuario_E usu = (Usuario_E)Session["UsuarioId"];
                datos.OpRegistro = $"{usu.Nombres} {usu.Apellidos}";
                SQL_OWTR_N owrtN = new SQL_OWTR_N();
                var result1 = owrtN.ObtenerOWTR(datos.DocNumSAP);
                var result2 = owrtN.CambiarEstadoOWTR(datos, (result1 != null && result1.DocNumSAP >= 1) ? "ACT" : "INS");
                var msj = (result2.Equals(1) ? "Estado Actualizado" : "Error al actualizar estado");
                return Json(new { Mensaje = msj });
            }
            else
            {
                return null;
            }
        }
        public ActionResult ActaRecepcionTs(int DocEntry, int idOperation = 2202)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                var result = dgN.ConsultarActaRecepcionTs(DocEntry);
                if (result != null && result[0].AlmacenDestino != null)
                {
                    var datosFirma = BuscarFirmas("ResponsableALMActas", result[0].CodAlmacenDestino);
                    if (datosFirma != null && datosFirma.Count >= 1)
                    {
                        ViewBag.DatosResponsable = datosFirma["NombApe"];
                        ViewBag.FirmaResponsable = datosFirma["Firma"];
                    }

                    var firmaPersonaEntrega = BuscarFirmas("PersonaEntrega", result[0].CodAlmacenEnvio);
                    ViewBag.PersonaEntrega = firmaPersonaEntrega != null && firmaPersonaEntrega.Any() ? firmaPersonaEntrega["Firma"] : "";
                }

                return View(result);
            }
            else
            {
                return resultadoAcceso;
            }
        }
        public ActionResult ActaDespachoTs(int DocEntry, int idOperation = 2203)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                var result = dgN.ConsultarActaDespachoTs(DocEntry);
                if (result != null && result[0].CodAlmOrigen != null)
                {
                    var datosFirma = BuscarFirmas("ResponsableALMActas", result[0].CodAlmOrigen);
                    if (datosFirma != null && datosFirma.Count >= 1)
                    {
                        ViewBag.DatosResponsable = datosFirma["NombApe"];
                        ViewBag.FirmaResponsable = datosFirma["Firma"];
                    }
                }
                return View(result);
            }
            else
            {
                return resultadoAcceso;
            }
        }
        public ActionResult OrganolepticoTs(int DocEntry, int idOperation = 2204)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                Utilitarios_N utilitarios = new Utilitarios_N();
                var orgTS = dgN.ConsultarOrganolepticoTs(DocEntry);
                if (orgTS != null && orgTS.Any())
                {
                    var result = BuscarFirmas("QuimicoFarmaceutico", orgTS[0].CodAlmacenDestino);
                    if (result != null && result.Count >= 1)
                    {
                        ViewBag.QuimicoFarmaceuticoAsistente = result["NombApe"];
                        ViewBag.Firma = result["Firma"];
                    }
                }

                var firmaResponsableDT = BuscarFirmas("ResponsableDT", "08");
                ViewBag.FirmaDT = firmaResponsableDT != null && firmaResponsableDT.Any() ? firmaResponsableDT["Firma"] : "";

                return View(orgTS);
            }
            else
            {
                return resultadoAcceso;
            }
        }
        public ActionResult ListadoEntradasDeMercancias(Capa_Entidad.Compras_ENT.Tablas.OPDN_E fil, int idOperation = 2301)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                ViewBag.Almacenes = new Capa_Negocio.General_NEG.Tablas.OWHS_N().ListarAlmacenes("todos").OrderBy(a => a.WhsName);
                if (fil != null) { ViewBag.Opdn = fil; } else { ViewBag.Opdn = new Capa_Entidad.Compras_ENT.Tablas.OPDN_E(); }
                return View(new Capa_Negocio.Compras_NEG.Tablas.OPDN_N().listadoEntradaMercancias(fil));
            }
            else
            {
                return resultadoAcceso;
            }
        }
        public ActionResult ActaRecepcionEm(int DocEntry, string Almacen, int idOperation = 2302)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                var result = dgN.ConsultarActaRecepcionEm(DocEntry);
                if (result != null && result.Count() >= 1)
                {
                    var datosFirma = BuscarFirmas("ResponsableALMActas", Almacen);
                    if (datosFirma != null && datosFirma.Count >= 1)
                    {
                        ViewBag.DatosResponsable = datosFirma["NombApe"];
                        ViewBag.FirmaResponsable = datosFirma["Firma"];
                    }
                }
                return View(result);
            }
            else
            {
                return resultadoAcceso;
            }
        }
        public ActionResult OrganolepticoEm(int DocEntry, string Almacen, int idOperation = 2303)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                Utilitarios_N utilitarios = new Utilitarios_N();
                var orgEM = dgN.ConsultarOrganolepticoEm(DocEntry);
                if (orgEM != null && orgEM.Count() >= 1)
                {
                    var result = BuscarFirmas("QuimicoFarmaceutico", Almacen);
                    if (result != null && result.Count >= 1)
                    {
                        ViewBag.QuimicoFarmaceuticoAsistente = result["NombApe"];
                        ViewBag.Firma = result["Firma"];
                    }
                }

                var firmaResponsableDT = BuscarFirmas("ResponsableDT", "08");
                ViewBag.FirmaDT = firmaResponsableDT != null && firmaResponsableDT.Any() ? firmaResponsableDT["Firma"] : "";

                return View(orgEM);
            }
            else
            {
                return resultadoAcceso;
            }
        }
        public ActionResult RealizarEntradaDeMercancias(int DocEntry, int idOperation = 2304)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                return View(new Capa_Negocio.Compras_NEG.Tablas.OPDN_N().buscarEntradaMercancias(DocEntry));
            }
            else
            {
                return resultadoAcceso;
            }
        }
        [HttpPost]
        public ActionResult RealizarEntradaDeMercancias(SQL_OPDN_E s, int idOperation = 2304)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                Usuario_E user = (Usuario_E)Session["UsuarioId"];
                s.OpRealizacion = $"{user.Nombres} {user.Apellidos}";
                s.ObjType = 20;
                try
                {
                    new Capa_Negocio.Compras_NEG.Tablas.SQL_OPDN_N().realizarSqlEntradaDeMercancias(s);
                    ViewBag.Mensaje = s.DocEntry;
                    return RedirectToAction("ListadoEntradasDeMercancias");
                }
                catch (Exception e) { ViewBag.Mensaje = e.Message; return View(new Capa_Negocio.Compras_NEG.Tablas.OPDN_N().buscarEntradaMercancias(s.DocEntry)); }
            }
            else
            {
                return resultadoAcceso;
            }
        }
        public ActionResult AnularRealizarEntradaDeMercancias(int DocEntry, int idOperation = 2305)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                return View(new Capa_Negocio.Compras_NEG.Tablas.OPDN_N().buscarEntradaMercancias(DocEntry));
            }
            else
            {
                return resultadoAcceso;
            }
        }
        [ActionName("AnularRealizarEntradaDeMercancias")]
        [HttpPost]
        public ActionResult AnularRealizarEntradaDeMercanciasPost(int DocEntry, int idOperation = 2305)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                try
                {
                    new Capa_Negocio.Compras_NEG.Tablas.SQL_OPDN_N().eliminarSqlEntradaDeMercancias(DocEntry);
                    ViewBag.Mensaje = DocEntry;
                    return RedirectToAction("ListadoEntradasDeMercancias");
                }
                catch (Exception e) { ViewBag.Mensaje = e.Message; return View(new Capa_Negocio.Compras_NEG.Tablas.OPDN_N().buscarEntradaMercancias(DocEntry)); }
            }
            else
            {
                return resultadoAcceso;
            }
        }
        public ActionResult ListadoFacturasDeProveedores(OPCH_E fil, int idOperation = 2401)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                if (fil != null) { ViewBag.Opch = fil; } else { ViewBag.Opch = new OPCH_E(); }
                return View(new Capa_Negocio.Compras_NEG.Tablas.OPCH_N().listadoFacturasProveedores(fil));
            }
            else
            {
                return resultadoAcceso;
            }
        }
        public ActionResult FacturaDeProveedor(int DocEntry, int idOperation = 2402)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                return View(new Capa_Negocio.Compras_NEG.Tablas.OPCH_N().buscarFacturaProveedor(DocEntry));
            }
            else
            {
                return resultadoAcceso;
            }
        }
        public ActionResult ListadoMaestroDeArticulos(OITM_E fil, int idOperation = 2501)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                if (fil != null) { ViewBag.fil = fil; } else { ViewBag.fil = new OITM_E(); }
                return View(new Capa_Negocio.Almacen_NEG.Tablas.OITM_N().Listar(fil));
            }
            else
            {
                return resultadoAcceso;
            }
        }
        public ActionResult DetallesArticulo(string ItemCode, int idOperation = 2502)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                ViewBag.Owhs = new Capa_Negocio.General_NEG.Tablas.OWHS_N();
                ViewBag.listaOitw = new Capa_Negocio.Almacen_NEG.Tablas.OITW_N().listarDetArticulosInv(ItemCode);
                return View(new Capa_Negocio.Almacen_NEG.Tablas.OITM_N().buscarArticulo(ItemCode));
            }
            else
            {
                return resultadoAcceso;
            }
        }
        public ActionResult Reportes(int idOperation = 2601)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                OMRC_N omrcN = new OMRC_N();
                ViewBag.Laboratorios = omrcN.listarFabricantes();
                return View();
            }
            else
            {
                return resultadoAcceso;
            }
        }
        //aciones dentRo de reportes
        public ActionResult ListadoSaldosAnteriores(COB_SALDO_E fil, string Mensaje = "", int idOperation = 2602)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                if (fil == null) { ViewBag.fil = new COB_SALDO_E(); } else { ViewBag.fil = fil; }
                ViewBag.Mensaje = Mensaje;
                return View(new Capa_Negocio.General_NEG.Tablas.COB_SALDO_N().listarSaldosAnteriores(fil));
            }
            else
            {
                return resultadoAcceso;
            }
        }
        public ActionResult AgregarSaldoAnterior(int idOperation = 2603)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                OITM_E o = new OITM_E() { ItmsGrpCod = 103 };
                ViewBag.ListaArticulos = new Capa_Negocio.Almacen_NEG.Tablas.OITM_N().Listar(o);
                ViewBag.Mensaje = "";
                return View(new COB_SALDO_E());
            }
            else
            {
                return resultadoAcceso;
            }
        }
        [HttpPost]
        public ActionResult AgregarSaldoAnterior(COB_SALDO_E c, int idOperation = 2603)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                try
                {
                    new Capa_Negocio.General_NEG.Tablas.COB_SALDO_N().agregarSaldoAnterior(c);
                    return RedirectToAction("ListadoSaldosAnteriores", new { Mensaje = c.Name + " Agregado correctamente" });
                }
                catch (Exception e)
                {
                    ViewBag.Mensaje = e.Message;
                    OITM_E o = new OITM_E() { ItmsGrpCod = 103 };
                    ViewBag.ListaArticulos = new Capa_Negocio.Almacen_NEG.Tablas.OITM_N().Listar(o);
                    return View(c);
                }
            }
            else
            {
                return resultadoAcceso;
            }
        }
        public ActionResult EditarSaldoAnterior(string Code, int idOperation = 2604)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                OITM_E o = new OITM_E() { ItmsGrpCod = 103 };
                ViewBag.ListaArticulos = new Capa_Negocio.Almacen_NEG.Tablas.OITM_N().Listar(o);
                ViewBag.Mensaje = "";
                return View(new Capa_Negocio.General_NEG.Tablas.COB_SALDO_N().buscarSaldoAnterior(Code));
            }
            else
            {
                return resultadoAcceso;
            }
        }
        [HttpPost]
        public ActionResult EditarSaldoAnterior(COB_SALDO_E c, int idOperation = 2604)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                try
                {
                    new Capa_Negocio.General_NEG.Tablas.COB_SALDO_N().editarSaldoAnterior(c);
                    return RedirectToAction("ListadoSaldosAnteriores", new { Mensaje = c.Name + " Editado correctamente" });
                }
                catch (Exception e)
                {
                    OITM_E o = new OITM_E() { ItmsGrpCod = 103 };
                    ViewBag.ListaArticulos = new Capa_Negocio.Almacen_NEG.Tablas.OITM_N().Listar(o);
                    ViewBag.Mensaje = e.Message;
                    return View(c);
                }
            }
            else
            {
                return resultadoAcceso;
            }
        }
        public ActionResult EliminarSaldoAnterior(string Code, int idOperation = 2605)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                return View(new Capa_Negocio.General_NEG.Tablas.COB_SALDO_N().buscarSaldoAnterior(Code));
            }
            else
            {
                return resultadoAcceso;
            }
        }
        [ActionName("EliminarSaldoAnterior")]
        [HttpPost]
        public ActionResult EliminarSaldoAnteriorPost(string Code, int idOperation = 2605)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                Capa_Negocio.General_NEG.Tablas.COB_SALDO_N cobN = new Capa_Negocio.General_NEG.Tablas.COB_SALDO_N();
                try
                {
                    cobN.eliminarSaldoAnterior(Code);
                    return RedirectToAction("ListadoSaldosAnteriores", new { Mensaje = "Eliminado correctamente" });
                }
                catch (Exception e) { ViewBag.Mensaje = e.Message; return View(cobN.buscarSaldoAnterior(Code)); }
            }
            else
            {
                return resultadoAcceso;
            }
        }
        /********************* R E G I S T R O S   S A N I T A R I O S *********************/
        public ActionResult RegistrosSanitarios(int idOperation = 2900)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                return View();
            }
            else
            {
                return resultadoAcceso;
            }
        }
        // Listado interno dentro de RegistrosSanitarios para ser reutilizable
        public ActionResult ListarRegistrosSanitarios(OORS_E datos)
        {
            OORS_N rsN = new OORS_N();
            ViewBag.ORS = datos;
            ViewBag.ListaEstadosRS = new COB_ESTA_RS_N().ListarEstadoRegistrosSanitarios();
            return PartialView("DireccionTecnica/RegistrosSanitarios/ListadoRegistrosSanitarios", rsN.ListarRegistrosSanitarios(datos));
        }
        public ActionResult AgregarObservaciones(OORS_E datos)
        {
            return PartialView("DireccionTecnica/RegistrosSanitarios/AgregarObservaciones", datos);
        }
        public ActionResult VerSeguimientoObservaciones(OORS_E datos)
        {
            OORS_N rsN = new OORS_N();
            var result = rsN.ObtenerDatosObsRS(datos.RegistroSanitario, datos.CodArticulo, "");
            if (result != null && result.Count >= 1)
            {
                foreach (var res in result)
                {
                    res.DescArticulo = datos.DescArticulo;
                }
            }
            return PartialView("DireccionTecnica/RegistrosSanitarios/SeguimientoObservaciones", result);
        }
        public JsonResult AgregarObservacion(OORS_E rs)
        {
            //verificacionAccesos(0);     // Validar sesion logueada, solo para ajax
            Usuario_E usu = (Usuario_E)Session["UsuarioId"];
            rs.RegistradoPor = $"{usu.Nombres} {usu.Apellidos}";
            OORS_N orsN = new OORS_N();
            var result = orsN.RegistrarObservacion(rs);
            return Json(new { Mensaje = result });
        }
        public JsonResult ConsultarRegistrosSanitariosExpirados()
        {
            //verificacionAccesos(0);         // Validar sesion logueada, solo para ajax
            OORS_N orsN = new OORS_N();
            var result = orsN.ConsultarRegistrosSanitariosExpirados();
            return Json(new { Datos = result });
        }
        /************************  Ó R D E N E S   D E   V E N T A ************************/
        public ActionResult ListadoOrdenesDeVenta(Capa_Entidad.Ventas_ENT.Tablas.ORDR_E filtros, int idOperation = 1701)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                var usuario = Session["UsuarioId"] as Usuario_E;
                int idRol = usuario?.IdRol ?? 0;
                // VENTAS o SVENTAS
                bool versionVentas = new List<int> { 6, 7 }.Contains(idRol);
                ViewBag.CargarLista = versionVentas == true ? "ListadoOrdenesVenta_VT" : "ListadoOrdenesVenta_DT";
                ViewBag.SlpCode = filtros?.SlpCode ?? 0;
                return View();
            }
            else
            {
                return resultadoAcceso;
            }
        }
        [HttpGet]
        public ActionResult ListarOrdenesVenta(Capa_Entidad.Ventas_ENT.Tablas.ORDR_E filtros, string version, int idOperation = 1701)
        {
            var usuario = Session["UsuarioId"] as Usuario_E;
            int idRol = usuario?.IdRol ?? 0;
            ViewBag.IdRol = idRol;
            ViewBag.Ordr = filtros ?? new Capa_Entidad.Ventas_ENT.Tablas.ORDR_E();
            ViewBag.ListaOslp = new OSLP_N().listadoOslp("VENTA");
            var lugaresEntregas = new Capa_Negocio.General_NEG.TablasSql.OWHS_N().listarAlmacenes(new[] { "01", "03", "09", "ALM07", "07", "16", "15" });
            ViewBag.ListaLugarEntregas = lugaresEntregas;
            ViewBag.Almacenes = lugaresEntregas?.ToDictionary(item => item.WhsCode, item => item.WhsName) ?? new Dictionary<string, string>();
            // NO mostrar cuando sea VENTAS o SVENTAS
            // En caso de realizar una modificación, también realizarlo en su vista parcial
            bool mostrarCompVinculados = !new List<int> { 6, 7 }.Contains(idRol);
            var lista = new Capa_Negocio.Ventas_NEG.Tablas.ORDR_N().listadoOrdenesDeVenta(filtros, mostrarCompVinculados);
            return PartialView($"DireccionTecnica/{version}", lista);
        }
        public ActionResult ExportarPdfOrdenesVenta(Capa_Entidad.Ventas_ENT.Tablas.ORDR_E filtros)
        {
            return new ActionAsPdf("PDF_OrdenesDeVentas", new { DocNum = filtros.DocNum }) { FileName = $"{filtros.CardName}.pdf", PageOrientation = Rotativa.Options.Orientation.Portrait, PageSize = Rotativa.Options.Size.A4 };
        }
        public ActionResult PDF_OrdenesDeVentas(OrdenDeVenta_E filtros)
        {
            var lista = new ORTV_N().obtenerOrdenDeVenta(filtros.DocNum);
            return View("~/Views/Ventas/PDF/PDF_OrdenesDeVentas.cshtml", lista);
        }
    }
}