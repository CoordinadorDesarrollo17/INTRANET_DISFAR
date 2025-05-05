using Capa_Entidad.General_ENT.TablasSql;
using Capa_Entidad.Rutas_ENT.ReportesSql;
using Capa_Entidad.Rutas_ENT.TablasSql;
using Capa_Entidad.Seguridad_ENT;
using Capa_Entidad.SocioNegocios_ENT.Tablas;
using Capa_Entidad.Ventas_ENT.TablasSql;
using Capa_Negocio;
using Capa_Negocio.General_NEG.TablasSql;
using Capa_Negocio.Rutas_NEG.TablasSql;
using Capa_Negocio.Seguridad_NEG;
using Capa_Negocio.SocioNegocios_NEG.TablasExternas;
using Capa_Negocio.Ventas_NEG.TablasSql;
using Capa_Usuario.Helpers;
using OfficeOpenXml;
using OfficeOpenXml.Table;
using Rotativa;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Capa_Usuario.Controllers
{
    public class RutasController : Controller
    {
        // ope - 201-299
        private readonly OVEH_N ovehN = new OVEH_N();
        private readonly ORRU_N orruN = new ORRU_N();
        private readonly Usuario_N ousrN = new Usuario_N();
        private readonly Capa_Negocio.General_NEG.TablasSql.OWHS_N owhsN = new Capa_Negocio.General_NEG.TablasSql.OWHS_N();
        private readonly Capa_Negocio.General_NEG.Tablas.OWHS_N _owhsSapN = new Capa_Negocio.General_NEG.Tablas.OWHS_N();
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
        protected Dictionary<string, string> BuscarFirmas(string tipoFirma, int docEntryUsuario = 0)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            if (!string.IsNullOrWhiteSpace(tipoFirma))
            {
                // tipoFirma = 'ResponsableALMActas' abarca -> ActaRecepcionEm, ActaRecepcionTs
                var filtros = new Firmas_E() { TipoFirma = tipoFirma };

                if (docEntryUsuario > 0)
                    filtros.DocEntryUsuario = docEntryUsuario;

                var firmas = new Firmas_N().ListarFirmas(filtros);

                if (firmas != null && firmas.Any())
                {
                    string FilePath;
                    var firma = firmas.First();

                    FilePath = firma.RutaFirma;
                    result.Add("NombApe", $"{firma.Nombres} {firma.Apellidos}");
                    result.Add("IdRolUsuario", firma.IdRolUsuario.ToString());
                    byte[] archivo = System.IO.File.ReadAllBytes(FilePath);
                    var base64 = Convert.ToBase64String(archivo);                                   //La propiedad de tu modelo que es byte[]
                    result.Add("Firma", String.Format("data:image/gif;base64,{0}", base64));       // Damos formato para indicar que se trata de una cadena base64
                }
            }

            return result;
        }

        public ActionResult ListadoRutas(int DocNum = 0, ORRU_E o = null, int idOperation = 201)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                OCHO_N ochoN = new OCHO_N();
                ViewBag.DocNum = DocNum;
                ViewBag.Conductores = new Capa_Negocio.Repartos_NEG.TablasHana.SYP_CONDUC_N().listar();
                ViewBag.Mensaje = "";
                ViewBag.Orru = o;
                ViewBag.listaVeh = ovehN.listaVeh(0, null);
                return View(orruN.Listar(o));
            }
            else
            {
                return resultadoAcceso;
            }
        }
        private void CapturarViewBag(string tipoRep, string tipoRuta = "", string[] filtrosAlm = null, string mensaje = "", int filas = 30)
        {
            ViewBag.Mensaje = mensaje;
            ViewBag.TipoRep = tipoRep;
            if (tipoRuta == "TA")
            {
                var listaUsuariosCopilotos = ousrN.ListaUsuarios(new Usuario_E { IdRol = 4 });
                var copilotos = new List<string>();
                copilotos = listaUsuariosCopilotos.Select(x => $"{x.Nombres} {x.Apellidos}").ToList();
                ViewBag.ListaCopilotos = copilotos;
            }
            else
            {
                ViewBag.ListaCopilotos = ousrN.listaCopilotos();
            }
            ViewBag.Conductores = new Capa_Negocio.Repartos_NEG.TablasHana.SYP_CONDUC_N().listar();
            ViewBag.ListaOrigenesDestinos = owhsN.listarAlmacenes(filtrosAlm ?? Array.Empty<string>());
            ViewBag.ListaVehiculos = new Capa_Negocio.Repartos_NEG.TablasHana.SYP_VEHICU_N().listar();
            ViewBag.Filas = filas;
        }
        public ActionResult NuevaTransferenciaEntreAlmacenes(string TipoRep, int idOperation = 205)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                CapturarViewBag(TipoRep, "TA");
                return View(new ORRU_E());
            }
            else
            {
                return resultadoAcceso;
            }
        }
        [HttpPost]
        public ActionResult NuevaTransferenciaEntreAlmacenes(ORRU_E o, string TipoRep, int idOperation = 205)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                try
                {
                    Usuario_E user = (Usuario_E)Session["UsuarioId"];
                    o.Propietario = $"{user.Nombres} {user.Apellidos}";
                    int DocNum = orruN.NuevaHojaDeReparto(o);
                    return TipoRep == "Re"
                        ? RedirectToAction("ListadoRepartos", new { DocNum })
                        : RedirectToAction("ListadoRutas", new { DocNum });
                }
                catch
                {
                    CapturarViewBag(TipoRep, "TA");
                    return View(o);
                }
            }
            else
            {
                return resultadoAcceso;
            }
        }
        public ActionResult NuevaHojaDeReparto(string TipoRep, int idOperation = 202)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                CapturarViewBag(TipoRep);
                return View(new ORRU_E());
            }
            else
            {
                return resultadoAcceso;
            }
        }
        [HttpPost]
        public ActionResult NuevaHojaDeReparto(ORRU_E o, string TipoRep, int idOperation = 202)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                try
                {
                    Usuario_E user = (Usuario_E)Session["UsuarioId"];
                    o.Propietario = $"{user.Nombres} {user.Apellidos}";
                    int DocNum = orruN.NuevaHojaDeReparto(o);
                    return TipoRep == "Re"
                        ? RedirectToAction("ListadoRepartos", new { DocNum })
                        : RedirectToAction("ListadoRutas", new { DocNum });
                }
                catch (Exception e)
                {
                    CapturarViewBag(TipoRep, mensaje: e.Message);
                    return View(new { TipoRep });
                }
            }
            else
            {
                return resultadoAcceso;
            }
        }
        public ActionResult EditarHojaDeReparto(int DocEntry, string TipoRep, int idOperation = 203)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                var user = (Usuario_E)Session["UsuarioId"];
                var datosOrdenRuta = orruN.obtenerOrdenDeRuta(DocEntry);
                var filtrosAlm = Array.Empty<string>();
                var nombreVista = "EditarHojaDeReparto";
                if (datosOrdenRuta?.TipoRuta == "TA")
                {
                    nombreVista = "EditarTransferenciaEntreAlmacenes";
                }
                CapturarViewBag(TipoRep, null, mensaje: string.Empty);
                ViewBag.UsuarioSesion = $"{user.Nombres} {user.Apellidos}";
                return View(nombreVista, datosOrdenRuta);
            }
            else
            {
                return resultadoAcceso;
            }
        }
        [HttpPost]
        public ActionResult EditarHojaDeReparto(ORRU_E o, string TipoRep, int idOperation = 203)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                try
                {
                    Usuario_E user = (Usuario_E)Session["UsuarioId"];
                    o.Propietario = $"{user.Nombres} {user.Apellidos}";
                    int DocNum = orruN.EditarHojaDeReparto(o);
                    return TipoRep == "Re"
                        ? RedirectToAction("ListadoRepartos", new { DocNum })
                        : RedirectToAction("ListadoRutas", new { DocNum });
                }
                catch (Exception e)
                {
                    CapturarViewBag(TipoRep, mensaje: e.Message);
                    ViewBag.Agencias = new COUR_N().Listar();
                    return View(orruN.obtenerOrdenDeRuta(o.DocEntry));
                }
            }
            else
            {
                return resultadoAcceso;
            }
        }
        public ActionResult AgregarRRU0(RRU0_E o, int idOperation = 203)
        {
            string acceso = AccesoHelper.VerificarAccesos(idOperation, (Usuario_E)Session["UsuarioId"], this.ControllerContext.RouteData.Values["action"].ToString(), Request.UserHostAddress, Request.UserHostName);
            if (acceso == "C_Access")
            {
                RRU0_N rru0N = new RRU0_N();
                try
                {
                    Usuario_E user = (Usuario_E)Session["UsuarioId"];
                    o.Operario = $"{user.Nombres} {user.Apellidos}";
                    rru0N.AgregarRRU0(o);
                }
                catch (Exception e) { return Content(e.Message); }
                return Content("ok");
            }
            else
            {
                return Content("Sin permisos ni accesos");
            }
        }
        public ActionResult AnularOrdenDeRuta(int DocEntry, string TipoRep, int idOperation = 204)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                ViewBag.TipoRep = TipoRep;
                ViewBag.Mensaje = string.Empty;
                return View(orruN.obtenerOrdenDeRuta(DocEntry));
            }
            else
            {
                return resultadoAcceso;
            }
        }
        [HttpPost]
        public ActionResult AnularOrdenDeRuta(int DocEntry, ORRU_E o, string TipoRep, int idOperation = 204)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                try
                {
                    Usuario_E user = (Usuario_E)Session["UsuarioId"];
                    string OpRegistro = $"{user.Nombres} {user.Apellidos}";
                    ViewBag.Mensaje = "";
                    ViewBag.TipoRep = TipoRep;
                    int DocNum = orruN.AnularOrdenDeRuta(DocEntry, OpRegistro);
                    if (TipoRep == "Re")
                    {
                        return RedirectToAction("ListadoRepartos", new { DocNum = DocNum });
                    }
                    else
                    {
                        return RedirectToAction("ListadoRutas", new { DocNum = DocNum });
                    }
                }
                catch (Exception e) { ViewBag.Mensaje = e.Message; return View(orruN.obtenerOrdenDeRuta(DocEntry)); }
            }
            else
            {
                return resultadoAcceso;
            }
        }
        public ActionResult ReportesRutas(int idOperation = 206)
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
        public ActionResult infoHojasRuta(int idOperation = 207)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                OCHO_N ochoN = new OCHO_N(); OCRD_N ocrdN = new OCRD_N();
                ViewBag.Almacenes = _owhsSapN.ListarAlmacenes("todos");
                ViewBag.Clientes = ocrdN.listarSociosDeNegocios(new OCRD_E { CardType = "C" });
                ViewBag.Transportistas = ochoN.listaChoferes(0, null);
                ViewBag.ListaVehiculos = ovehN.listaVeh(0, null);
                return View();
            }
            else
            {
                return resultadoAcceso;
            }
        }
        public ActionResult RptHojasRuta(ORRU_E o, int idOperation = 208)
        {
            if (string.IsNullOrWhiteSpace(o.FechaRegistroDesde) || string.IsNullOrWhiteSpace(o.FechaRegistroHasta))
                return null;

            string acceso = AccesoHelper.VerificarAccesos(idOperation, (Usuario_E)Session["UsuarioId"], this.ControllerContext.RouteData.Values["action"].ToString(), Request.UserHostAddress, Request.UserHostName);
            if (acceso == "C_Access")
            {
                ORRU_N orruN = new ORRU_N();
                string excelContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                var reporte = orruN.ReporteHojasRuta(o);
                using (var libro = new ExcelPackage())
                {
                    var worksheet = libro.Workbook.Worksheets.Add("ReporteHojasRuta");
                    worksheet.Cells["A1"].LoadFromCollection(reporte, PrintHeaders: true);
                    if (reporte != null)
                    {
                        if (reporte.Count >= 1)
                        {
                            for (var col = 1; col <= 37; col++)
                            {
                                worksheet.Column(col).AutoFit();
                            }
                            var tabla = worksheet.Tables.Add(new ExcelAddressBase(fromRow: 1, fromCol: 1, toRow: reporte.Count + 1, toColumn: 37), "ReporteHojasRuta");
                            tabla.ShowHeader = true;
                            tabla.TableStyle = TableStyles.Medium2;
                        }
                    }
                    return File(libro.GetAsByteArray(), excelContentType, "AnalisisHojasRuta.xlsx");
                }
            }
            else
            {
                return null;
            }
        }
        public ActionResult ListadoRepartos(ORRU_E o, int DocNum = 0, string msj = "", int idOperation = 211)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                Capa_Negocio.Repartos_NEG.TablasHana.SYP_CONDUC_N condN = new Capa_Negocio.Repartos_NEG.TablasHana.SYP_CONDUC_N();
                ViewBag.DocNum = DocNum;
                ViewBag.Conductores = condN.listar();
                ViewBag.Mensaje = msj;
                ViewBag.Orru = o;
                return View(orruN.Listar(o));
            }
            else
            {
                return resultadoAcceso;
            }
        }
        public ActionResult SeguimientoRepartoRutas(int DocEntry, string Vista, int idOperation = 212)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                try
                {
                    ORRU_E orruE = orruN.obtenerOrdenDeRuta(DocEntry);
                    ViewBag.Mensaje = "";
                    ViewBag.Vista = Vista;
                    return View(orruE);
                }
                catch
                {
                    return RedirectToAction("ListadoRepartos");
                }
            }
            else
            {
                return resultadoAcceso;
            }
        }
        public ActionResult GestionarChoferes(OCHO_E o, string TipoRep = null, string res = null, int idOperation = 213)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                OCHO_N ochoN = new OCHO_N();
                ViewBag.Mensaje = res;
                ViewBag.Ocho = o;
                ViewBag.TipoRep = TipoRep;
                ViewBag.ListaOpRepartos = ousrN.ListaUsuarios(new Usuario_E { IdRol = 55 });
                return View(ochoN.listaChoferes(0, o));
            }
            else
            {
                return resultadoAcceso;
            }
        }
        public ActionResult RegistrarChofer(OCHO_E o, string TipoRep, int idOperation = 213)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                try
                {
                    OCHO_N ochoN = new OCHO_N();
                    ochoN.registrarChofer(o);
                    return RedirectToAction("GestionarChoferes", new { o = new OCHO_E() { Code = o.Code }, TipoRep = TipoRep, res = "Chofer registrado" });
                }
                catch (Exception e)
                {
                    return RedirectToAction("GestionarChoferes", new { TipoRep = TipoRep, res = e.Message });
                }
            }
            else
            {
                return resultadoAcceso;
            }
        }
        public ActionResult EliminarChofer(string DocEntry, string TipoRep, int idOperation = 213)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                try
                {
                    OCHO_N ochoN = new OCHO_N();
                    ochoN.eliminarChofer(DocEntry);
                    return RedirectToAction("GestionarChoferes", new { TipoRep = TipoRep, res = "Chofer eliminado exitosamente" });
                }
                catch (Exception e)
                {
                    return RedirectToAction("GestionarChoferes", new { TipoRep = TipoRep, res = e.Message });
                }
            }
            else
            {
                return resultadoAcceso;
            }
        }
        public ActionResult validarNuevoChofer(OCHO_E o)
        {
            string status = "true";
            try
            {
                OCHO_N ochoN = new OCHO_N();
                ochoN.validarChofer(o);
                return Content(status);
            }
            catch (Exception e) { return Content(e.Message); }
        }//no vista
        public ActionResult GestionarVehiculos(OVEH_E o, string TipoRep = null, string res = null, int idOperation = 214)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                ViewBag.Oveh = o;
                ViewBag.Mensaje = res;
                ViewBag.TipoRep = TipoRep;
                return View(ovehN.listaVeh(0, o));
            }
            else
            {
                return resultadoAcceso;
            }
        }
        public ActionResult RegistrarVehiculo(OVEH_E o, string TipoRep, int idOperation = 214)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                try
                {
                    ovehN.registrarVeh(o);
                    return RedirectToAction("GestionarVehiculos", new { o = new OVEH_E() { Code = o.Code }, TipoRep = TipoRep, res = "Vehiculo registrado" });
                }
                catch (Exception e)
                {
                    return RedirectToAction("GestionarVehiculos", new { TipoRep = TipoRep, res = e.Message });
                }
            }
            else
            {
                return resultadoAcceso;
            }
        }
        public ActionResult EliminarVehiculo(string DocEntry, string TipoRep, int idOperation = 214)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                try
                {
                    ovehN.eliminarVeh(DocEntry);
                    return RedirectToAction("GestionarVehiculos", new { TipoRep = TipoRep, res = "Vehiculo eliminado" });
                }
                catch (Exception e)
                {
                    return RedirectToAction("GestionarVehiculos", new { TipoRep = TipoRep, res = e.Message });
                }
            }
            else
            {
                return resultadoAcceso;
            }
        }
        public ActionResult validarNuevoVehiculo(OVEH_E o)
        {
            string status = "true";
            try
            {
                ovehN.validarVehiculo(o);
                return Content(status);
            }
            catch (Exception e) { return Content(e.Message); }
        }//no vista
        public ActionResult ListadoEntregaRepartos(string TipoRep, int DocEntry, string msj = "", int idOperation = 215)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                var lista = orruN.obtenerOrdenDeRuta(DocEntry);
                ViewBag.TipoRep = TipoRep;
                ViewBag.Mensaje = msj;
                return View(lista);
            }
            else
            {
                return resultadoAcceso;
            }
        }
        public ActionResult EntregarDetReparto(int DocEntry, int Linea, string TipoRep, string tipoVenta, int idOperation = 216)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                RRU0_N rru0N = new RRU0_N(); RRU1_N rru1N = new RRU1_N();
                ORRU_E orru = orruN.obtenerOrdenDeRuta(DocEntry);
                ViewBag.TipoRuta = orru.TipoRuta;
                ViewBag.TipoRep = TipoRep;
                ViewBag.DocEntry = DocEntry;
                ViewBag.Linea = Linea;
                ViewBag.RRU0 = rru0N.BuscarRRU0(DocEntry, Linea);
                ViewBag.RRU1 = rru1N.buscarRRU1(DocEntry, Linea);
                ViewBag.TipoVenta = tipoVenta;
                return View();
            }
            else
            {
                return resultadoAcceso;
            }
        }
        [HttpPost]
        public ActionResult EntregarDetReparto(RRU0_E r0, RRU1_E r1, string TipoRep, int idOperation = 216)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                ORRU_E o = orruN.obtenerOrdenDeRuta(r0.DocEntry);
                Usuario_E user = (Usuario_E)Session["UsuarioId"];
                try
                {
                    if (o.TipoRuta == "TA")
                    {
                        RRU1_N rru1N = new RRU1_N();
                        r1.OpEntrega = $"{user.Nombres} {user.Apellidos}";
                        rru1N.entregarRRU1(r1);
                    }
                    else
                    {
                        RRU0_N rru0N = new RRU0_N();
                        r0.OpEntrega = $"{user.Nombres} {user.Apellidos}";
                        rru0N.EntregarRRU0(r0);
                    }
                    return RedirectToAction("ListadoEntregaRepartos", new { DocEntry = r0.DocEntry, Linea = r0.Linea, TipoRep = TipoRep });
                }
                catch { return View(); }
            }
            else
            {
                return resultadoAcceso;
            }
        }
        public ActionResult IniciarReparto(RRU0_E o, int idOperation = 217)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                ORRU_E obj = orruN.obtenerOrdenDeRuta(o.DocEntry);
                try
                {
                    Usuario_E user = (Usuario_E)Session["UsuarioId"];
                    foreach (RRU1_E dt in obj.DetRRU1)
                    { dt.TempI1 = o.TempI1; dt.TempI2 = o.TempI2; }
                    foreach (RRU0_E dt in obj.DetRRU0)
                    { dt.TempI1 = o.TempI1; dt.TempI2 = o.TempI2; }
                    obj.Operario = $"{user.Nombres} {user.Apellidos}";
                    orruN.IniciarReparto(obj);
                    return RedirectToAction("ListadoRepartos", new { DocNum = obj.DocNum, msj = "Ruta iniciada exitosamente" });
                }
                catch (Exception e)
                {
                    return RedirectToAction("ListadoRepartos", new { DocNum = obj.DocNum, msj = e.Message });
                }
            }
            else
            {
                return resultadoAcceso;
            }
        }
        public ActionResult TerminarReparto(RRU0_E o, int idOperation = 218)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                ORRU_E obj = orruN.obtenerOrdenDeRuta(o.DocEntry);
                try
                {
                    Usuario_E user = (Usuario_E)Session["UsuarioId"];
                    obj.Operario = $"{user.Nombres} {user.Apellidos}";
                    orruN.TerminarReparto(obj);
                    return RedirectToAction("ListadoRepartos", new { DocNum = obj.DocNum, msj = "Ruta terminada exitosamente" });
                }
                catch (Exception e)
                {
                    return RedirectToAction("ListadoRepartos", new { DocNum = obj.DocNum, msj = e.Message });
                }
            }
            else
            {
                return resultadoAcceso;
            }
        }
        public ActionResult RptControlTemperaturaHumedad(Rpt_TempHumed_E datos, string FechaTerEn, int idOperation = 219)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                int docEntryUsuarioTransportista = 0;

                ViewBag.Placa = datos.Placa;
                ViewBag.Serie = datos.Serie;
                ViewBag.Anio = Convert.ToDateTime(FechaTerEn).ToString("yyyy");
                ViewBag.Mes = Convert.ToDateTime(FechaTerEn).ToString("MMMM");

                var resultTempHum = orruN.RptTempHumed(datos.Placa, FechaTerEn, datos.Serie);

                if (resultTempHum != null && resultTempHum.Any())
                {
                    var datosUsuario = new Usuario_N().BuscarDocEntryPorNombreCompleto(resultTempHum[0].Encargado);
                    docEntryUsuarioTransportista = datosUsuario.DocEntry;
                }

                var firmaTransportista = BuscarFirmas("Transportista", docEntryUsuarioTransportista);
                ViewBag.FirmaTransportista = firmaTransportista != null && firmaTransportista.Any() && firmaTransportista["IdRolUsuario"] == "55" ? firmaTransportista["Firma"] : "";

                var firmaResponsableQF = BuscarFirmas("ResponsableQF");
                ViewBag.FirmaQF = firmaResponsableQF != null && firmaResponsableQF.Any() ? firmaResponsableQF["Firma"] : "";

                return View("Reportes/RptControlTemperaturaHumedad", resultTempHum);
            }
            else
            {
                return resultadoAcceso;
            }
        }
        public FileResult EvidenciaReparto(int DocEntry, string TipoRuta, int idOperation = 220)
        {
            string acceso = AccesoHelper.VerificarAccesos(idOperation, (Usuario_E)Session["UsuarioId"], this.ControllerContext.RouteData.Values["action"].ToString(), Request.UserHostAddress, Request.UserHostName);
            if (acceso == "C_Access")
            {
                Utilitarios_N utiN = new Utilitarios_N();
                string rutajpg = utiN.directorioFileServer + @"Repartos\Evidencias\" + TipoRuta + @"\" + DocEntry + ".jpg";
                string rutajpeg = utiN.directorioFileServer + @"Repartos\Evidencias\" + TipoRuta + @"\" + DocEntry + ".jpeg";
                string rutapng = utiN.directorioFileServer + @"Repartos\Evidencias\" + TipoRuta + @"\" + DocEntry + ".png";
                try
                {
                    if (System.IO.File.Exists(rutajpg))
                    {
                        return File(rutajpg, "image/jpg", DocEntry + ".jpg");
                    }
                    else if (System.IO.File.Exists(rutajpeg))
                    {
                        return File(rutajpeg, "image/png", DocEntry + ".jpeg");
                    }
                    else if (System.IO.File.Exists(rutapng))
                    {
                        return File(rutapng, "image/png", DocEntry + ".png");
                    }
                    else
                    {
                        return null;
                    }
                }
                catch { return null; }
            }
            else { return null; }
        }
        public ActionResult DocumentoRutas(int DocEntry, string TipoRuta, int idOperation = 221)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                if (TipoRuta.Equals("VC") || TipoRuta.Equals("VA"))
                {
                    return RedirectToAction("DocumentoRutasTransferencia", new { DocEntry = DocEntry });
                }
                else if (TipoRuta.Equals("VD"))
                {
                    return RedirectToAction("DocumentoRutasDomicilio", new { DocEntry = DocEntry });
                }
                else if (TipoRuta.Equals("TA"))
                {
                    return RedirectToAction("DocumentoRutasTransferenciaAlm", new { DocEntry = DocEntry });
                }
                else if (TipoRuta.Equals("VG") || TipoRuta.Equals("AC"))
                {
                    return RedirectToAction("DocumentoRepartoAg", new { DocEntry = DocEntry });
                }
                else
                { return View(); }
            }
            else
            {
                return resultadoAcceso;
            }
        }
        public ActionResult EntregaMasiva(RRU0_E o, int idOperation = 222)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                Usuario_E user = (Usuario_E)Session["UsuarioId"];
                o.OpEntrega = $"{user.Nombres} {user.Apellidos}";
                RRU0_N rru0N = new RRU0_N();
                try
                {
                    rru0N.EntregaMasivaDetRep(o);
                    return RedirectToAction("ListadoEntregaRepartos", new { TipoRep = "Re", DocEntry = o.DocEntry, msj = "Se entrego masivamente" });
                }
                catch (Exception e)
                { return RedirectToAction("ListadoEntregaRepartos", new { TipoRep = "Re", DocEntry = o.DocEntry, msj = e.Message }); }
            }
            else
            {
                return resultadoAcceso;
            }
        }
        public ActionResult GestionarOficinas(OUR1_E o, string msj, int idOperation = 223)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                OUR1_N oofiN = new OUR1_N(); UBIG_N ubigN = new UBIG_N(); COUR_N courN = new COUR_N();
                ViewBag.Ubigeos = ubigN.Listar(null);
                ViewBag.Couriers = courN.Listar();
                ViewBag.Mensaje = msj;
                return View(oofiN.Listar());
            }
            else
            {
                return resultadoAcceso;
            }
        }
        public ActionResult RegistrarOficina(OUR1_E o, int idOperation = 224)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                try
                {
                    OUR1_N oofiN = new OUR1_N();
                    oofiN.Registrar(o);
                    return RedirectToAction("GestionarOficinas", new
                    {
                        msj = "Oficina registrada correctamente"
                    });
                }
                catch (Exception e)
                {
                    ViewBag.Mensaje = e.Message;
                    return RedirectToAction("GestionarOficinas");
                }
            }
            else
            {
                return resultadoAcceso;
            }
        }//no vista
        public ActionResult EliminarOficina(int Id, int idOperation = 225)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                OUR1_N ofiN = new OUR1_N();
                try
                {
                    ofiN.Eliminar(Id);
                    return RedirectToAction("GestionarOficinas");
                }
                catch (Exception e)
                {
                    ViewBag.Mensaje = e.Message;
                    return RedirectToAction("GestionarOficinas");
                }
            }
            else
            {
                return resultadoAcceso;
            }
        }//no vista 
        public ActionResult validarNuevaOficina(OUR1_E o)
        {
            string status = "true";
            OUR1_N ofiN = new OUR1_N();
            try
            {
                ofiN.Validar(o);
                return Content(status);
            }
            catch (Exception e) { return Content(e.Message); }
        }
        public JsonResult RegistrarAgencia(COUR_E o, int idOperation = 228)
        {
            string acceso = AccesoHelper.VerificarAccesos(idOperation, (Usuario_E)Session["UsuarioId"], this.ControllerContext.RouteData.Values["action"].ToString(), Request.UserHostAddress, Request.UserHostName);
            if (acceso == "C_Access")
            {
                COUR_N oN = new COUR_N();
                return Json(oN.Registrar(o));
            }
            else
            { return null; }
        }
        public ActionResult GestionarTarifarios(string msj, int idOperation = 229)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                UBIG_N ubigN = new UBIG_N(); COUR_N courN = new COUR_N();
                OUR2_N our2N = new OUR2_N();
                ViewBag.Ubigeos = ubigN.Listar(null);
                ViewBag.Agencias = courN.Listar();
                ViewBag.Mensaje = msj;
                return View(our2N.Listar(""));
            }
            else
            {
                return resultadoAcceso;
            }
        }
        public JsonResult PrevisualizarExcel(string filename, int idOperation = 230)
        {
            string acceso = AccesoHelper.VerificarAccesos(idOperation, (Usuario_E)Session["UsuarioId"], this.ControllerContext.RouteData.Values["action"].ToString(), Request.UserHostAddress, Request.UserHostName);
            if (acceso == "C_Access")
            {
                OUR2_N oN = new OUR2_N();
                var list = oN.Visualizar(filename);
                return Json(list);
            }
            else
            { return null; }
        }
        public ActionResult ImportarExcel(string filename, int IdCourier, int idOperation = 231)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                OUR2_N our2N = new OUR2_N();
                var res = string.Empty;
                var list = our2N.Visualizar(filename);
                foreach (var o in list)
                {
                    o.IdCourier = IdCourier;
                    res = our2N.Registrar(o);
                }
                return RedirectToAction("GestionarTarifarios", "Rutas", new { msj = res });
            }
            else
            {
                return resultadoAcceso;
            }
        }
        public void CalcularPrecioEnv(int DocEntry, int idOperation = 232)
        {
            string acceso = AccesoHelper.VerificarAccesos(idOperation, (Usuario_E)Session["UsuarioId"], this.ControllerContext.RouteData.Values["action"].ToString(), Request.UserHostAddress, Request.UserHostName);
            if (acceso == "C_Access")
            {
                string Destino = string.Empty;
                ORTV_N ortvN = new ORTV_N(); COUR_N courN = new COUR_N(); OUR2_N oN = new OUR2_N();
                RTV6_N rtv6N = new RTV6_N();
                ORTV_E obj = ortvN.ObtenerDatosCompletosTicket(DocEntry);
                var Courier = courN.Obtener(obj.Agencia);
                if (obj.LugarDestino == "Agencia Courier")
                {
                    if (obj.EnvioAgencia == "Oficina de agencia")
                    {
                        Destino = obj.Det3[1].Distrito.ToUpper();
                    }
                    else if (obj.EnvioAgencia == "Domicilio de cliente")
                    {
                        if (!string.IsNullOrWhiteSpace(obj.Det3[0].Distrito))
                        {
                            Destino = obj.Det3[0].Distrito.ToUpper();
                        }
                        else if (!string.IsNullOrWhiteSpace(obj.Det3[1].Distrito))
                        {
                            Destino = obj.Det3[1].Distrito.ToUpper();
                        }
                    }
                }
                OUR2_E Tarifario = oN.Obtener(Courier.Id, Destino);
                decimal Precio;
                switch (obj.EnvioAgencia)
                {
                    case "Oficina de agencia":
                        foreach (var i in obj.Det6)
                        {
                            Precio = ((Tarifario.TarifaKg * i.Peso) + Tarifario.PrecioBase) * 118 / 100;
                            if (Precio < Courier.MinAgencia)
                            {
                                Precio = Courier.MinAgencia * 118 / 100;
                            }
                            rtv6N.AsignarPrecio(i.DocEntry, i.Linea, Decimal.Round(Precio, 2));
                        }
                        break;
                    case "Domicilio de cliente":
                        foreach (var i in obj.Det6)
                        {
                            Precio = ((Tarifario.TarifaKg * i.Peso) + Tarifario.PrecioBase) * 118 / 100;
                            if (Precio < Courier.MinDomicilio)
                            {
                                Precio = Courier.MinDomicilio * 118 / 100;
                            }
                            rtv6N.AsignarPrecio(i.DocEntry, i.Linea, Decimal.Round(Precio, 2));
                        }
                        break;
                    default:
                        break;
                }
            }
        }
        public ActionResult AnularEntregaTicket(int DocEntryTicket, int DocEntry, int Linea, string TipoRuta, int idOperation = 233)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                string msj;
                ORTV_N ortvN = new ORTV_N(); RRU0_N rru0N = new RRU0_N();
                var tc = ortvN.ObtenerDatosCompletosTicket(DocEntryTicket);
                RRU0_E rru0E = rru0N.BuscarRRU0(DocEntry, Linea);
                if (TipoRuta == "AC" /*&& tc.IdReg == 0 */)
                {
                    int i = ortvN.editarSeguimientoTicket("ANULARENTREGADO", DocEntryTicket, tc);
                    if (i > 0) { anularRRU0(rru0E); }
                    msj = "Correcta anulacion de entrega";
                }
                else { msj = "Ticket tiene regalo, no se puede anular entrega"; }
                return RedirectToAction("AnularHojaCargo", "Rutas", new { DocEntry = DocEntry, TipoRep = "Re", msj = msj });
            }
            else
            {
                return resultadoAcceso;
            }
        }
        public string anularRRU0(RRU0_E o, int idOperation = 233)
        {
            string res = string.Empty;
            string acceso = AccesoHelper.VerificarAccesos(idOperation, (Usuario_E)Session["UsuarioId"], this.ControllerContext.RouteData.Values["action"].ToString(), Request.UserHostAddress, Request.UserHostName);
            if (acceso == "C_Access")
            {
                RRU0_N rru0N = new RRU0_N();
                try
                {
                    Usuario_E user = (Usuario_E)Session["UsuarioId"];
                    o.Operario = $"{user.Nombres} {user.Apellidos}";
                    rru0N.AnularRRU0(o);
                }
                catch (Exception e) { res = e.Message; return res; }
                return res;
            }
            else
            { return res; }
        }
        public ActionResult SeguimientoDeTicket(int DocEntry)
        {
            return RedirectToAction("SeguimientoDeTicket", "Ventas", new { DocEntry = DocEntry });
        }
        public ActionResult validarNuevaHojaDeRepartoOTransferencia(ORRU_E o)
        {
            try
            {
                orruN.validarNuevaHojaDeRepartoOTransferencia(o);
            }
            catch (Exception e) { return Content(e.Message); }
            return Content("OK");
        }
        public ActionResult validarDatosEncabezadoRuta(ORRU_E o)
        {
            try
            {
                orruN.validarDatosEncabezadoRuta(o);
            }
            catch (Exception e) { return Content(e.Message); }
            return Content("OK");
        }
        [HttpPost]
        public ActionResult validarEntDetReparto(RRU0_E r0, RRU1_E r1)
        {
            try
            {
                ORRU_E o = orruN.obtenerOrdenDeRuta(r0.DocEntry);
                if (o.TipoRuta == "TA")
                {
                    RRU1_N rru1N = new RRU1_N();
                    rru1N.validarEntDetReparto(r1);
                }
                else
                {
                    RRU0_N rru0N = new RRU0_N();
                    rru0N.ValidarEntDetReparto(r0);
                }
            }
            catch (Exception e) { return Content(e.Message); }
            return Content("ok");
        }
        public JsonResult infoTicketsReparto(string FechaSapTicket, string TipoRuta, string Zona, string AlmOrigenCod)
        {
            string[] estados = { "EMPACADO", "PESADO" };
            ORTV_N ortvN = new ORTV_N();
            ORTV_E ortvE = new ORTV_E { FechaSapTicket = FechaSapTicket, EstadoFacturacion = "FACTURADO", Zona = Zona };
            if (TipoRuta == "VD")
            {
                ortvE.LugarDestino = "Domicilio";
                ortvE.LugEntrega = AlmOrigenCod;
            }
            else if (TipoRuta == "VG")
            {
                ortvE.LugarDestino = "Agencia";
                ortvE.LugEntrega = " ";
            }
            else if (TipoRuta == "VC")
            {
                ortvE.LugarDestino = "Centro";
                ortvE.LugEntrega = AlmOrigenCod;
            }
            else if (TipoRuta == "VA")
            {
                ortvE.LugarDestino = "Arriola";
                ortvE.LugEntrega = AlmOrigenCod;
            }
            int cantidadTicketsNoEnviados;
            var resultado = ortvN.listarTicketsParaRepartos(ortvE, estados, out cantidadTicketsNoEnviados);
            var response = new
            {
                Resultado = resultado,
                CantidadTicketsNoEnviados = cantidadTicketsNoEnviados
            };
            return Json(response);
        }
        public JsonResult listarTicketsRepartosNoEnviados(string FechaSapTicket, string TipoRuta, string Zona, string AlmOrigenCod)
        {
            string[] estados = { "EMPACADO", "PESADO" };
            ORTV_N ortvN = new ORTV_N();
            ORTV_E ortvE = new ORTV_E { FechaSapTicket = FechaSapTicket, Zona = Zona };
            if (TipoRuta == "VD") { ortvE.LugarDestino = "Domicilio"; ortvE.LugEntrega = AlmOrigenCod; }
            else if (TipoRuta == "VG")
            {
                ortvE.LugarDestino = "Agencia";
                ortvE.LugEntrega = "";
            }
            else if (TipoRuta == "AC")
            {
                ortvE.LugarDestino = "Agencia Courier";
                ortvE.LugEntrega = AlmOrigenCod;
            }
            else if (TipoRuta == "VC")
            {
                ortvE.LugarDestino = "Centro";
                ortvE.LugEntrega = AlmOrigenCod;
            }
            else if (TipoRuta == "VA")
            {
                ortvE.LugarDestino = "Arriola";
                ortvE.LugEntrega = AlmOrigenCod;
            }
            var resultado = ortvN.listarTicketsRepartosNoEnviados(ortvE, estados);
            return Json(resultado);
        }
        public ActionResult infoGuiasTicketsVenta(int DocEntry, int idOperation = 202)
        {
            string acceso = AccesoHelper.VerificarAccesos(idOperation, (Usuario_E)Session["UsuarioId"], this.ControllerContext.RouteData.Values["action"].ToString(), Request.UserHostAddress, Request.UserHostName);
            if (acceso == "C_Access")
            {
                ORTV_N ortvN = new ORTV_N();
                return Content(ortvN.GuiasTicket(DocEntry));
            }
            else
            { return Content(""); }
        }
        public ActionResult liberarRRU0(RRU0_E o, int idOperation = 202)
        {
            string acceso = AccesoHelper.VerificarAccesos(idOperation, (Usuario_E)Session["UsuarioId"], this.ControllerContext.RouteData.Values["action"].ToString(), Request.UserHostAddress, Request.UserHostName);
            if (acceso == "C_Access")
            {
                RRU0_N rru0N = new RRU0_N();
                try
                {
                    Usuario_E user = (Usuario_E)Session["UsuarioId"];
                    o.Operario = $"{user.Nombres} {user.Apellidos}";
                    rru0N.LiberarRRU0(o);
                }
                catch (Exception e) { return Content(e.Message); }
                return Content("ok");
            }
            else
            { return Content(""); }
        }
        public string ListarTarifarios(string accion)
        {
            OUR2_N our2N = new OUR2_N();
            var tarifarios = our2N.Listar(accion);
            string filaTabla = "";
            int num = 1;
            foreach (var tarif in tarifarios)
            {
                filaTabla += "<tr>" +
                                $"<td>{num}</td>" +
                                $"<td>{tarif.Destino}</td>" +
                                $"<td class='text-center'>{tarif.TarifaKg}</td>" +
                                $"<td class='text-center'>{tarif.PrecioBase}</td>" +
                                $"<td class='text-center'>{tarif.NombreAgencia}</td>" +
                                "<td class='text-center'>" +
                                $"<button class='btn btn-primary mr-lg-2 mr-md-0 mr-sm-1' onclick=\"buscarTarifario({tarif.Id})\"><i class='icon-pencil'></i></a>" +
                                $"<button class='btn btn-danger ml-sm-1' onclick=\"mostrarMensaje('eliminar', 'EliminarTarifario', {tarif.Id})\"><i class='icon-bin'></i></button>" +
                            "</td>" +
                        "</tr>";
                ++@num;
            }
            return filaTabla;
        }
        [HttpPost]
        public JsonResult RegistrarTarifario(OUR2_E datos)
        {
            //verificacionAccesos(0);
            OUR2_N our2N = new OUR2_N();
            string mensajeResult = our2N.Registrar(datos);
            string lista = ListarTarifarios("registrar");
            return Json(new { mensaje = mensajeResult, listaActualizada = lista });
        }
        [HttpPost]
        public JsonResult BuscarTarifario(OUR2_E datos)
        {
            //verificacionAccesos(0);
            OUR2_N our2N = new OUR2_N();
            return Json(our2N.Buscar(datos));
        }
        [HttpPost]
        public JsonResult EditarTarifario(OUR2_E datos)
        {
            //verificacionAccesos(0);
            OUR2_N our2N = new OUR2_N();
            var mensaje = our2N.Editar(datos);
            string lista = ListarTarifarios("editar");
            return Json(new { mensaje = mensaje, listaActualizada = lista });
        }
        [HttpPost]
        public JsonResult EliminarTarifario(int Id)
        {
            //verificacionAccesos(0);
            OUR2_N our2N = new OUR2_N();
            our2N.Eliminar(Id);
            string lista = ListarTarifarios("eliminar");
            return Json(new { mensaje = "Tarifario eliminado satisfactoriamente.", listaActualizada = lista });
        }
        public ActionResult DocumentoRutasTransferenciaAlm(int DocEntry)
        {
            //verificacionAccesos(0);
            ORRU_E o = new ORRU_E();
            try
            {
                ViewBag.Tipo = "PZA";
                o = orruN.obtenerOrdenDeRuta(DocEntry);
                foreach (var obj in o.DetRRU1)
                {
                    foreach (var a in obj.ListaRRU11)
                    {
                        if (!a.UnitMed.Equals("PZA"))
                        {
                            ViewBag.Tipo = "CAJAS";
                            break;
                        }
                    }
                }
            }
            catch { }
            ViewBag.Letra = 2;
            return View(o);
        }
        public ActionResult DocumentoRutasTransferencia(int DocEntry)
        {
            //verificacionAccesos(0);
            ORRU_E o = new ORRU_E();
            ORTV_N ortvN = new ORTV_N();
            try
            {
                o = orruN.obtenerOrdenDeRuta(DocEntry);
                if (o.DetRRU0 != null)
                {
                    foreach (RRU0_E r0 in o.DetRRU0)
                    {
                        r0.Ticket = ortvN.ObtenerDatosCompletosTicket(r0.DocEntryTicket);
                    }
                }
            }
            catch { }
            ViewBag.Letra = 2;
            return View(o);
        }
        public ActionResult DocumentoRutasDomicilio(int DocEntry)
        {
            //verificacionAccesos(0);
            ORRU_E o = new ORRU_E(); ORTV_N ortvN = new ORTV_N();
            try
            {
                o = orruN.obtenerOrdenDeRuta(DocEntry);
                if (o.DetRRU0 != null)
                {
                    foreach (RRU0_E r0 in o.DetRRU0)
                    {
                        r0.Ticket = ortvN.ObtenerDatosCompletosTicket(r0.DocEntryTicket);
                    }
                }
            }
            catch { }
            ViewBag.Letra = 2;
            return View(o);
        }
        public ActionResult DocumentoRepartoAg(int DocEntry)
        {
            //verificacionAccesos(0);
            ORRU_E o = new ORRU_E(); ORTV_N ortvN = new ORTV_N();
            try
            {
                o = orruN.obtenerOrdenDeRuta(DocEntry);
                if (o.DetRRU0 != null)
                {
                    foreach (RRU0_E r0 in o.DetRRU0)
                    {
                        r0.Ticket = ortvN.ObtenerDatosCompletosTicket(r0.DocEntryTicket);
                    }
                }
            }
            catch { }
            ViewBag.Letra = 2;
            return View(o);
        }
        public ActionResult ManifiestoCourier(int DocEntry)
        {
            //verificacionAccesos(0);
            ORRU_E o = new ORRU_E(); ORTV_N ortvN = new ORTV_N();
            try
            {
                o = orruN.obtenerOrdenDeRuta(DocEntry);
                if (o.DetRRU0 != null)
                {
                    foreach (RRU0_E r0 in o.DetRRU0)
                    {
                        r0.Ticket = ortvN.ObtenerDatosCompletosTicket(r0.DocEntryTicket);
                    }
                }
            }
            catch { }
            ViewBag.Letra = 2;
            return View(o);
        }
        [HttpPost]
        public JsonResult EditarDetalleOrdenRuta(int BaseEntry, int BaseLinea, int[] DetRRU11, int idOperation = 203)
        {
            bool nrocajas = false;
            string mensaje;
            if (Array.IndexOf(DetRRU11, 0) == -1)
            {
                RRU11_N rru11N = new RRU11_N();
                RRU1_N rru1N = new RRU1_N();
                rru11N.EditarDetalleOrdenRuta(BaseEntry, BaseLinea, DetRRU11);
                rru1N.ActualizarNroCajas(BaseEntry, BaseLinea);
                mensaje = "N° de cajas actualizado correctamente";
                nrocajas = true;
            }
            else
            {
                mensaje = "El n° de cajas debe ser mayor a 0";
            }
            return Json(new { mensaje = mensaje, nrocajas = nrocajas });
        }
        [HttpPost]
        public JsonResult ObtenerDetalleOrdenRuta(int BaseEntry, int BaseLinea, int HabilitarBotonEditar, int idOperation = 203)
        {
            string lista = string.Empty, mensaje = "No existe detalle sobre la guía seleccionada";
            bool existeDatos = false;
            RRU11_N rru11N = new RRU11_N();
            List<RRU11_E> datos = rru11N.BuscarRRU11(BaseEntry, BaseLinea);
            if (datos.Count >= 1)
            {
                mensaje = "Detalle de guía cargado correctamente";
                existeDatos = true;
                foreach (var det in datos)
                {
                    lista += $"<tr><td class='text-center'>{det.BaseLinea}</td>" +
                                $"<td class='text-center'>{det.Linea}</td>" +
                                $"<td class='text-center'>{det.ItemName}</td>" +
                                $"<td class='text-center'>{det.Lote}</td>" +
                                $"<td class='text-center'>{Math.Round(det.CantidadL, 0)}</td>" +
                                $"<td class='text-center'>{det.LaboDesc}</td>" +
                                $"<td class='text-center'>{det.UnitMed}</td>" +
                                "<td>" +
                                "<div class='d-flex justify-content-center '>";
                    if (HabilitarBotonEditar == 1)
                    {
                        lista += $"<input name = 'DetRRU11[]' id='cajas_{det.Linea}' type='number' class='form-control text-center border-success' style='width:90px;' onchange='actualizarCajas();' value='{det.Cajas}'>";
                    }
                    else
                    {
                        lista += det.Cajas;
                    }
                    lista += "</div></td></tr>";
                }
            }
            return Json(new
            {
                mensaje = mensaje,
                lista = lista,
                existeDatos = existeDatos,
            });
        }
        public ActionResult pdfRutas(string DocNum = "0")
        {
            //verificacionAccesos(0);
            return new ActionAsPdf("DocumentoRutas", new { DocNum = DocNum }) { FileName = "rutas.pdf", PageOrientation = Rotativa.Options.Orientation.Landscape, PageSize = Rotativa.Options.Size.A4 };
        }
        public ActionResult infoTicketsPesaje(int idOperation = 237)
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
        public ActionResult RptPesaje(FiltroRptPesaje filtros, int idOperation = 237)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                string excelContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                var analisisPesaje = orruN.ListarRptPesaje(filtros);
                using (var libro = new ExcelPackage())
                {
                    var worksheet = libro.Workbook.Worksheets.Add("AnalisisPesaje");
                    worksheet.Cells["A1"].LoadFromCollection(analisisPesaje, PrintHeaders: true);
                    if (analisisPesaje != null)
                    {
                        if (analisisPesaje.Count >= 1)
                        {
                            for (var col = 1; col <= 47; col++)
                            {
                                worksheet.Column(col).AutoFit();
                            }
                            var tabla = worksheet.Tables.Add(new ExcelAddressBase(fromRow: 1, fromCol: 1, toRow: analisisPesaje.Count + 1, toColumn: 47), "AnalisisPesaje"); tabla.ShowHeader = true;
                            tabla.TableStyle = TableStyles.Medium2;
                        }
                    }
                    return File(libro.GetAsByteArray(), excelContentType, "ReportePesaje.xlsx");
                }
            }
            else
            {
                return resultadoAcceso;
            }
        }
        public ActionResult infoDatosTraslado(string guia, int orden, string Origen)
        {
            return Content(orruN.infoListaProductosOWTQ(guia, orden, Origen));
        }
        //Trae guias en la generacion de Orden de Ruta entre almacenes
        public JsonResult infoGuiasTransferencia(string Origen)
        {
            ORRU_N orruN = new ORRU_N();
            return Json(orruN.listarGuiasTraslado(Origen));
        }
    }
}