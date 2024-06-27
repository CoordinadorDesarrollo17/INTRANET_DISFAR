using Capa_Datos;
using Capa_Entidad.Almacen_ENT.Tablas;
using Capa_Entidad.General_ENT.TablasSql;
using Capa_Entidad.Rutas_ENT.ReportesSql;
using Capa_Entidad.Rutas_ENT.TablasSql;
using Capa_Entidad.Seguridad_ENT;
using Capa_Entidad.SocioNegocios_ENT.Tablas;
using Capa_Entidad.Ventas_ENT.Tablas;
using Capa_Entidad.Ventas_ENT.TablasSql;
using Capa_Negocio;
using Capa_Negocio.Almacen_NEG.Tablas;
using Capa_Negocio.General_NEG.TablasSql;
using Capa_Negocio.Rutas_NEG.TablasSql;
using Capa_Negocio.Seguridad_NEG;
using Capa_Negocio.SocioNegocios_NEG.Tablas;
using Capa_Negocio.Ventas_NEG.Tablas;
using Capa_Negocio.Ventas_NEG.TablasSql;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.Reporting.WebForms;
using OfficeOpenXml;
using OfficeOpenXml.Table;
using PdfiumViewer;
using Rotativa;
using Sap.Data.Hana;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Printing;
using System.Linq;
using System.Web.Mvc;

namespace Capa_Usuario.Controllers
{
    public class RutasController : Controller
    {
        // ope - 201-299
        int modulo = 2; Rol1_N rol1 = new Rol1_N();
        OVEH_N ovehN = new OVEH_N();
        ORRU_N orruN = new ORRU_N(); Usuario_N ousrN = new Usuario_N(); OWHS_N owhsN = new OWHS_N();
        protected List<Firmas_E> BuscarFirmas(List<int> listaUsuarios)
        {
            List<Firmas_E> result = new List<Firmas_E>();

            if (listaUsuarios != null && listaUsuarios.Count >= 1)
            {
                string FilePath;
                Firmas_N firN = new Firmas_N();
                Firmas_E firE = new Firmas_E();

                firE.ListaDocEntryUsuario = listaUsuarios;
                var firma = firN.ListarFirmas(firE);

                if (firma != null && firma.Count >= 1)
                {
                    foreach (var f in firma)
                    {
                        Firmas_E datos = new Firmas_E();
                        FilePath = f.RutaFirma;
                        datos.Nombres = f.Nombres;
                        datos.Apellidos = f.Apellidos;
                        datos.IdRolUsuario = f.IdRolUsuario;
                        datos.DocEntryUsuario = f.DocEntryUsuario;

                        if (!string.IsNullOrEmpty(FilePath))
                        {
                            byte[] archivo = System.IO.File.ReadAllBytes(FilePath);
                            var base64 = Convert.ToBase64String(archivo);                                               //La propiedad de tu modelo que es byte[]
                            datos.RutaFirma = String.Format("data:image/gif;base64,{0}", base64);       // Damos formato para indicar que se trata de una cadena base64
                        }


                        result.Add(datos);
                    }

                }
            }

            return result;
        }
        public ActionResult ListadoRutas(int DocNum = 0, ORRU_E o = null, int idOperation = 201)
        {
            if (verificacionAccesos(idOperation) == "C_Access")
            {
                OCHO_N ochoN = new OCHO_N();
                ViewBag.DocNum = DocNum;
                ViewBag.ListaTransportistas = ochoN.listaChoferes(0, null);
                ViewBag.Mensaje = "";
                ViewBag.Orru = o;
                ViewBag.listaVeh = ovehN.listaVeh(0, null);
                return View(orruN.Listar(o));
            }
            else if (verificacionAccesos(idOperation) == "E_Login")
            { return RedirectToAction("Index", "Index"); }
            else
            { return RedirectToAction("Error", "Index"); }
        }
        public ActionResult CrearOrdenDeRuta(string TipoRep, int idOperation = 202)
        {
            if (verificacionAccesos(idOperation) == "C_Access")
            {
                OCHO_N ochoN = new OCHO_N();
                ViewBag.Mensaje = string.Empty;
                ViewBag.TipoRep = TipoRep;
                ViewBag.ListaChoferes = ochoN.listaChoferes(0, null);
                ViewBag.ListaVehiculos = ovehN.listaVeh(0, null);
                ViewBag.ListaCopilotos = ousrN.ListaUsuarios(new Usuario_E { IdRol = 55 });
                ViewBag.ListaOrigenesDestinos = owhsN.listarAlmacenes();
                return View(new ORRU_E());
            }
            else if (verificacionAccesos(idOperation) == "E_Login")
            { return RedirectToAction("Index", "Index"); }
            else
            { return RedirectToAction("Error", "Index"); }
        }
        [HttpPost]
        public ActionResult CrearOrdenDeRuta(string TipoRep, ORRU_E o, int idOperation = 202)
        {
            if (verificacionAccesos(idOperation) == "C_Access")
            {
                try
                {
                    Usuario_E user = (Usuario_E)Session["UsuarioId"];
                    o.Propietario = $"{user.Nombres} {user.Apellidos}";
                    int DocNum = orruN.CrearOrdenDeRuta(o);
                    //ELIMINO LOS COMPROBANTES QUE SE RELACIONAN CON LOS TICKETS DE LA RUTA DESDE LA TABLA AL.TEMP_RRU01
                    if (DocNum > 0)
                    {
                        TEMP_RRU01_N tempNeg = new TEMP_RRU01_N();
                        foreach (var tk in o.DetRRU0)
                        {
                            tempNeg.Eliminar(tk.DocEntryTicket);
                        }
                    }
                    if (TipoRep == "Re")
                    {
                        return RedirectToAction("ListadoRepartos", new { DocNum = DocNum });
                    }
                    else
                    {
                        return RedirectToAction("ListadoRutas", new { DocNum = DocNum });
                    }
                }
                catch (Exception e)
                {
                    OCHO_N ochoN = new OCHO_N();
                    ViewBag.Mensaje = e.Message;
                    ViewBag.ListaChoferes = ochoN.listaChoferes(0, null);
                    ViewBag.ListaVehiculos = ovehN.listaVeh(0, null);
                    ViewBag.ListaCopilotos = ousrN.ListaUsuarios(new Usuario_E { IdRol = 4 });
                    ViewBag.ListaOrigenesDestinos = owhsN.listarAlmacenes();
                    return View(new { TipoRep = TipoRep });
                }
            }
            else if (verificacionAccesos(idOperation) == "E_Login")
            { return RedirectToAction("Index", "Index"); }
            else
            { return RedirectToAction("Error", "Index"); }
        }

        public ActionResult EditarOrdenDeRuta(int DocEntry, string TipoRep, int idOperation = 203)
        {
            switch (verificacionAccesos(idOperation))
            {
                case "C_Access":
                    var user = (Usuario_E)Session["UsuarioId"];
                    var datosOrdenRuta = orruN.obtenerOrdenDeRuta(DocEntry);
                    var filtrosAlm = Array.Empty<string>();
                    var nombreVista = "EditarOrdenDeRuta";

                    if (datosOrdenRuta?.TipoRuta == "TA")
                    {
                        nombreVista = "EditarOrdenDeRuta_TA";
                        filtrosAlm = new string[] { "01", "02", "03", "04", "09", "ALM07", "ALM08","CUAR07" };
                    }

                    ViewBag.UsuarioSesion = $"{user.Nombres} {user.Apellidos}";
                    ViewBag.Mensaje = string.Empty;
                    ViewBag.TipoRep = TipoRep;
                    ViewBag.ListaChoferes = new OCHO_N().listaChoferes(0, null);
                    ViewBag.ListaVehiculos = ovehN.listaVeh(0, null);
                    ViewBag.ListaCopilotos = ousrN.ListaUsuarios(new Usuario_E { IdRol = 4 });
                    ViewBag.Agencias = new COUR_N().Listar();
                    ViewBag.ListaOrigenesDestinos = owhsN.listarAlmacenes(filtrosAlm);

                    return View(nombreVista, datosOrdenRuta);

                case "E_Login":
                    return RedirectToAction("Index", "Index");

                default:
                    return RedirectToAction("Error", "Index");
            }
        }

        [HttpPost]
        public ActionResult EditarOrdenDeRuta(ORRU_E o, string TipoRep, int idOperation = 203)
        {
            if (verificacionAccesos(idOperation) == "C_Access")
            {
                try
                {
                    ViewBag.Mensaje = "";
                    ViewBag.TipoRep = TipoRep;
                    Usuario_E user = (Usuario_E)Session["UsuarioId"];
                    o.Propietario = $"{user.Nombres} {user.Apellidos}";
                    int DocNum = orruN.EditarEncabezadoOrdenDeRuta(o);
                    if (TipoRep == "Re")
                    {
                        return RedirectToAction("ListadoRepartos", new { DocNum = DocNum });
                    }
                    else
                    {
                        return RedirectToAction("ListadoRutas", new { DocNum = DocNum });
                    }
                }
                catch (Exception e)
                {
                    OCHO_N ochoN = new OCHO_N(); COUR_N courN = new COUR_N();
                    ViewBag.Mensaje = e.Message;
                    ViewBag.ListaChoferes = ochoN.listaChoferes(0, null);
                    ViewBag.ListaVehiculos = ovehN.listaVeh(0, null);
                    ViewBag.ListaCopilotos = ousrN.ListaUsuarios(new Usuario_E { IdRol = 4 });
                    ViewBag.ListaOrigenesDestinos = owhsN.listarAlmacenes();
                    ViewBag.Agencias = courN.Listar();
                    return View(orruN.obtenerOrdenDeRuta(o.DocEntry));
                }
            }
            else if (verificacionAccesos(idOperation) == "E_Login")
            { return RedirectToAction("Index", "Index"); }
            else
            { return RedirectToAction("Error", "Index"); }
        }
        public ActionResult agregarRRU0(RRU0_E o, int idOperation = 203)
        {
            if (verificacionAccesos(idOperation) == "C_Access")
            {
                RRU0_N rru0N = new RRU0_N(); TEMP_RRU01_N tempNeg = new TEMP_RRU01_N(); RRU01_N rru01N = new RRU01_N(); ORRU_N orruN = new ORRU_N();
                try
                {
                    Usuario_E user = (Usuario_E)Session["UsuarioId"];
                    o.Operario = $"{user.Nombres} {user.Apellidos}";
                    //var r = orruN.obtenerOrdenDeRuta(o.DocEntry);
                    //if (r.TipoRuta == "VD" || r.TipoRuta == "VA" || r.TipoRuta == "VC" || r.TipoRuta == "VG")
                    //{
                    //    //obtiene del temp los documentos comprobantes nuevos que han sido agregados
                    //    List<TEMP_RRU01_E> listaTemp = tempNeg.Obtener(o.DocEntryTicket);
                    //    List<string> list = new List<string>(); List<TEMP_RRU01_E> tempPri = new List<TEMP_RRU01_E>();
                    //    if (listaTemp.Count > 0)
                    //    {
                    //        foreach (var e in listaTemp)
                    //        {
                    //            if (e.Impreso == 0)
                    //            {
                    //                //lista de correlativos que no se imprimieron segun tabla temporal
                    //                list.Add(e.U_SYP_MDTD + "-" + e.U_SYP_MDSD + "-" + e.U_SYP_MDCD);
                    //            }
                    //            else { tempPri.Add(e); }
                    //        }
                    //    }
                    //    if (list.Count > 0)
                    //    {
                    //        for (int i = 0; i < list.Count; i++)
                    //        {
                    //            //busca el elemento en la tabla top 1 para saber si fue impreso antes
                    //            RRU01_E rru01E = rru01N.BuscarCorrelativo(list[i]);
                    //            if (rru01E.Id == 0)
                    //            {
                    //                //return Content("El ticket en linea " + o.Linea + " tiene documentos que no se han impreso. No puede agregar sin antes imprimir.");
                    //            }

                    //        }
                    //    }
                    //    if (tempPri != null && tempPri.Count > 0)
                    //    {
                    //        rru0N.agregarRRU0(o);
                    //        foreach (var i in tempPri)
                    //        {
                    //            RRU01_E x = new RRU01_E { DocEntryORRU = o.DocEntry, DocEntryTicket = o.DocEntryTicket, TablaSAP = i.TablaSAP, Identificador = i.Identificador, U_BPP_FECINITRA = i.U_BPP_FECINITRA, U_SYP_MDTD = i.U_SYP_MDTD, U_SYP_MDSD = i.U_SYP_MDSD, U_SYP_MDCD = i.U_SYP_MDCD, DocDate = i.DocDate, Impreso = 1, Estado = "VIGENTE", Operario = o.Operario, FechaOperación = DateTime.Now.ToString("yyyy-MM-dd"), HoraOperación = DateTime.Now.ToString("HH:mm:ss") };
                    //            rru01N.Agregar(x);
                    //        }
                    //        //eliminar documentos del temporal
                    //        TEMP_RRU01_N temprru01N = new TEMP_RRU01_N();
                    //        List<TEMP_RRU01_E> tempPriDistinct = tempPri
                    //        .GroupBy(e => e.DocEntryTicket)
                    //        .Select(group => group.First())
                    //        .ToList();
                    //        foreach (var j in tempPriDistinct)
                    //        {
                    //            temprru01N.Eliminar(j.DocEntryTicket);
                    //        }
                    //    }
                    //    else
                    //    {
                    //        return Content("Error en grabacion de ticket: " + o.DocNumTicket + " dentro de ruta");
                    //    }
                    //}
                    //else
                    //{
                    rru0N.agregarRRU0(o);
                    //}
                }
                catch (Exception e) { return Content(e.Message); }
                return Content("ok");
            }
            else
            { return Content("Sin permisos ni accesos"); }
        }
        public ActionResult AnularOrdenDeRuta(int DocEntry, string TipoRep, int idOperation = 204) // falta agregar operacion a bd
        {
            if (verificacionAccesos(idOperation) == "C_Access")
            {
                ViewBag.TipoRep = TipoRep;
                ViewBag.Mensaje = string.Empty;
                return View(orruN.obtenerOrdenDeRuta(DocEntry));
            }
            else if (verificacionAccesos(idOperation) == "E_Login")
            { return RedirectToAction("Index", "Index"); }
            else
            { return RedirectToAction("Error", "Index"); }
        }
        [HttpPost]
        public ActionResult AnularOrdenDeRuta(int DocEntry, ORRU_E o, string TipoRep, int idOperation = 204)
        {
            if (verificacionAccesos(idOperation) == "C_Access")
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
            else if (verificacionAccesos(idOperation) == "E_Login")
            { return RedirectToAction("Index", "Index"); }
            else
            { return RedirectToAction("Error", "Index"); }
        }
        public ActionResult CrearOrdenDeRutaAlm(string TipoRep, int idOperation = 205)
        {
            if (verificacionAccesos(idOperation) == "C_Access")
            {
                OCHO_N ochoN = new OCHO_N();
                ViewBag.Mensaje = string.Empty;
                ViewBag.TipoRep = TipoRep;
                ViewBag.ListaChoferes = ochoN.listaChoferes(0, null);
                ViewBag.ListaVehiculos = ovehN.listaVeh(0, null);
                ViewBag.ListaCopilotos = ousrN.ListaUsuarios(new Usuario_E { IdRol = 4 });
                ViewBag.ListaOrigenesDestinos = owhsN.listarAlmacenes();
                ViewBag.Filas = 30;
                return View(new ORRU_E());
            }
            else if (verificacionAccesos(idOperation) == "E_Login")
            { return RedirectToAction("Index", "Index"); }
            else
            { return RedirectToAction("Error", "Index"); }
        }
        [HttpPost]
        [ActionName("CrearOrdenDeRutaAlm")]
        public ActionResult CrearOrdenDeRutaAlmPost(string TipoRep, ORRU_E o, int idOperation = 205)
        {
            if (verificacionAccesos(idOperation) == "C_Access")
            {
                try
                {
                    Usuario_E user = (Usuario_E)Session["UsuarioId"];
                    o.Propietario = $"{user.Nombres} {user.Apellidos}";
                    int DocNum = orruN.CrearOrdenDeRuta(o);
                    ViewBag.Mensaje = "Creado correctamente";
                    if (TipoRep == "Re")
                    {
                        return RedirectToAction("ListadoRepartos", new { DocNum = DocNum });
                    }
                    else
                    {
                        return RedirectToAction("ListadoRutas", new { DocNum = DocNum });
                    }
                }
                catch
                {
                    OCHO_N ochoN = new OCHO_N();
                    ViewBag.Mensaje = string.Empty;
                    ViewBag.ListaChoferes = ochoN.listaChoferes(0, null);
                    ViewBag.ListaVehiculos = ovehN.listaVeh(0, null);
                    ViewBag.ListaCopilotos = ousrN.ListaUsuarios(new Usuario_E { IdRol = 4 });
                    ViewBag.ListaOrigenesDestinos = owhsN.listarAlmacenes();
                    ViewBag.Filas = 30;
                    return View(o);
                }
            }
            else if (verificacionAccesos(idOperation) == "E_Login")
            { return RedirectToAction("Index", "Index"); }
            else
            { return RedirectToAction("Error", "Index"); }
        }
        public ActionResult ReportesRutas(int idOperation = 206)
        {
            if (verificacionAccesos(idOperation) == "C_Access")
            {
                return View();
            }
            else if (verificacionAccesos(idOperation) == "E_Login")
            { return RedirectToAction("Index", "Index"); }
            else
            { return RedirectToAction("Error", "Index"); }
        }
        public ActionResult infoHojasRuta(int idOperation = 207)
        {
            if (verificacionAccesos(idOperation) == "C_Access")
            {
                OCHO_N ochoN = new OCHO_N(); OCRD_N ocrdN = new OCRD_N();
                ViewBag.Almacenes = owhsN.listarAlmacenes();
                ViewBag.Clientes = ocrdN.listarSociosDeNegocios(new OCRD_E { CardType = "C" });
                ViewBag.Transportistas = ochoN.listaChoferes(0, null);
                ViewBag.ListaVehiculos = ovehN.listaVeh(0, null);
                return View();
            }
            else if (verificacionAccesos(idOperation) == "E_Login")
            { return RedirectToAction("Index", "Index"); }
            else
            { return RedirectToAction("Error", "Index"); }
        }
        public ActionResult RptHojasRuta(ORRU_E o, int idOperation = 208)
        {
            if (string.IsNullOrEmpty(o.FecConIni) || string.IsNullOrEmpty(o.FecConFin))
            {
                return null;
            }

            if (verificacionAccesos(idOperation) == "C_Access")
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
                            for (var col = 1; col <= 36; col++)
                            {
                                worksheet.Column(col).AutoFit();
                            }

                            var tabla = worksheet.Tables.Add(new ExcelAddressBase(fromRow: 1, fromCol: 1, toRow: reporte.Count + 1, toColumn: 36), "ReporteHojasRuta");

                            tabla.ShowHeader = true;
                            tabla.TableStyle = TableStyles.Medium2;
                        }

                    }

                    return File(libro.GetAsByteArray(), excelContentType, "AnalisisHojasRuta.xlsx");
                }
            }
            else if (verificacionAccesos(idOperation) == "E_Login") { return null; }
            else { return null; }
        }
        public ActionResult ListadoRepartos(ORRU_E o, int DocNum = 0, string msj = "", int idOperation = 211)
        {
            if (verificacionAccesos(idOperation) == "C_Access")
            {
                OCHO_N ochoN = new OCHO_N();
                ViewBag.DocNum = DocNum;
                ViewBag.ListaTransportistas = ochoN.listaChoferes(0, null);
                ViewBag.Mensaje = msj;
                ViewBag.Orru = o;
                return View(orruN.Listar(o));
            }
            else if (verificacionAccesos(idOperation) == "E_Login")
            { return RedirectToAction("Index", "Index"); }
            else
            { return RedirectToAction("Error", "Index"); }
        }
        public ActionResult SeguimientoRepartoRutas(int DocEntry, string Vista, int idOperation = 212)
        {
            if (verificacionAccesos(idOperation) == "C_Access")
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
            else if (verificacionAccesos(idOperation) == "E_Login")
            { return RedirectToAction("Index", "Index"); }
            else
            { return RedirectToAction("Error", "Index"); }
        }
        public ActionResult GestionarChoferes(OCHO_E o, string TipoRep = null, string res = null, int idOperation = 213)
        {
            if (verificacionAccesos(idOperation) == "C_Access")
            {
                OCHO_N ochoN = new OCHO_N();
                ViewBag.Mensaje = res;
                ViewBag.Ocho = o;
                ViewBag.TipoRep = TipoRep;
                ViewBag.ListaOpRepartos = ousrN.ListaUsuarios(new Usuario_E { IdRol = 55 });
                return View(ochoN.listaChoferes(0, o));
            }
            else if (verificacionAccesos(idOperation) == "E_Login")
            { return RedirectToAction("Index", "Index"); }
            else
            { return RedirectToAction("Error", "Index"); }
        }
        public ActionResult RegistrarChofer(OCHO_E o, string TipoRep, int idOperation = 213)
        {
            if (verificacionAccesos(idOperation) == "C_Access")
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
            else if (verificacionAccesos(idOperation) == "E_Login")
            { return RedirectToAction("Index", "Index"); }
            else
            { return RedirectToAction("Error", "Index"); }
        } //no vista
        public ActionResult EliminarChofer(string DocEntry, string TipoRep, int idOperation = 213)
        {
            if (verificacionAccesos(idOperation) == "C_Access")
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
            else if (verificacionAccesos(idOperation) == "E_Login")
            { return RedirectToAction("Index", "Index"); }
            else
            { return RedirectToAction("Error", "Index"); }
        }//no vista
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
            if (verificacionAccesos(idOperation) == "C_Access")
            {
                ViewBag.Oveh = o;
                ViewBag.Mensaje = res;
                ViewBag.TipoRep = TipoRep;
                return View(ovehN.listaVeh(0, o));
            }
            else if (verificacionAccesos(idOperation) == "E_Login")
            { return RedirectToAction("Index", "Index"); }
            else
            { return RedirectToAction("Error", "Index"); }
        }
        public ActionResult RegistrarVehiculo(OVEH_E o, string TipoRep, int idOperation = 214)
        {
            if (verificacionAccesos(idOperation) == "C_Access")
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
            else if (verificacionAccesos(idOperation) == "E_Login")
            { return RedirectToAction("Index", "Index"); }
            else
            { return RedirectToAction("Error", "Index"); }
        }//no vista
        public ActionResult EliminarVehiculo(string DocEntry, string TipoRep, int idOperation = 214)
        {
            if (verificacionAccesos(idOperation) == "C_Access")
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
            else if (verificacionAccesos(idOperation) == "E_Login")
            { return RedirectToAction("Index", "Index"); }
            else
            { return RedirectToAction("Error", "Index"); }
        }//no vista
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
            if (verificacionAccesos(idOperation) == "C_Access")
            {
                var lista = orruN.obtenerOrdenDeRuta(DocEntry);

                ViewBag.TipoRep = TipoRep;
                ViewBag.Mensaje = msj;

                return View(lista);
            }
            else if (verificacionAccesos(idOperation) == "E_Login")
            { return RedirectToAction("Index", "Index"); }
            else
            { return RedirectToAction("Error", "Index"); }
        }
        public ActionResult EntregarDetReparto(int DocEntry, int Linea, string TipoRep, string tipoVenta, int idOperation = 216)
        {
            if (verificacionAccesos(idOperation) == "C_Access")
            {
                RRU0_N rru0N = new RRU0_N(); RRU1_N rru1N = new RRU1_N();
                ORRU_E orru = orruN.obtenerOrdenDeRuta(DocEntry);

                ViewBag.TipoRuta = orru.TipoRuta;
                ViewBag.TipoRep = TipoRep;
                ViewBag.DocEntry = DocEntry;
                ViewBag.Linea = Linea;
                ViewBag.RRU0 = rru0N.buscarRRU0(DocEntry, Linea);
                ViewBag.RRU1 = rru1N.buscarRRU1(DocEntry, Linea);
                ViewBag.TipoVenta = tipoVenta;

                return View();
            }
            else if (verificacionAccesos(idOperation) == "E_Login")
            { return RedirectToAction("Index", "Index"); }
            else
            { return RedirectToAction("Error", "Index"); }
        }
        [HttpPost]
        public ActionResult EntregarDetReparto(RRU0_E r0, RRU1_E r1, string TipoRep, int idOperation = 216)
        {
            if (verificacionAccesos(idOperation) == "C_Access")
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

                        rru0N.entregarRRU0(r0);
                    }
                    return RedirectToAction("ListadoEntregaRepartos", new { DocEntry = r0.DocEntry, Linea = r0.Linea, TipoRep = TipoRep });
                }
                catch { return View(); }
            }
            else if (verificacionAccesos(idOperation) == "E_Login")
            { return RedirectToAction("Index", "Index"); }
            else
            { return RedirectToAction("Error", "Index"); }
        }
        public ActionResult IniciarReparto(RRU0_E o, int idOperation = 217)
        {
            if (verificacionAccesos(idOperation) == "C_Access")
            {
                ORRU_E obj = orruN.obtenerOrdenDeRuta(o.DocEntry);
                try
                {
                    Usuario_E user = (Usuario_E)Session["UsuarioId"];
                    foreach (RRU1_E dt in obj.DetRRU1)
                    { dt.TempI1 = o.TempI1; dt.TempI2 = o.TempI2; dt.HumedI1 = o.HumedI1; dt.HumedI2 = o.HumedI2; }
                    foreach (RRU0_E dt in obj.DetRRU0)
                    { dt.TempI1 = o.TempI1; dt.TempI2 = o.TempI2; dt.HumedI1 = o.HumedI1; dt.HumedI2 = o.HumedI2; }
                    obj.Operario = $"{user.Nombres} {user.Apellidos}";
                    orruN.IniciarReparto(obj);
                    return RedirectToAction("ListadoRepartos", new { DocNum = obj.DocNum, msj = "Ruta iniciada exitosamente" });
                }
                catch (Exception e)
                {
                    return RedirectToAction("ListadoRepartos", new { DocNum = obj.DocNum, msj = e.Message });
                }
            }
            else if (verificacionAccesos(idOperation) == "E_Login")
            { return RedirectToAction("Index", "Index"); }
            else
            { return RedirectToAction("Error", "Index"); }
        }
        public ActionResult TerminarReparto(RRU0_E o, int idOperation = 218)
        {
            if (verificacionAccesos(idOperation) == "C_Access")
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
            else if (verificacionAccesos(idOperation) == "E_Login")
            { return RedirectToAction("Index", "Index"); }
            else
            { return RedirectToAction("Error", "Index"); }
        }

        // Botón se encuentra oculto, es para Agencia Courier - ACTUALMENTE NO SE USA
        public ActionResult TerminarHojaCargo(RRU0_E o, int idOperation = 218)
        {
            if (verificacionAccesos(idOperation) == "C_Access")
            {
                ORRU_E obj = orruN.obtenerOrdenDeRuta(o.DocEntry);
                try
                {
                    Usuario_E user = (Usuario_E)Session["UsuarioId"]; RRU0_N rru0N = new RRU0_N();
                    obj.Operario = $"{user.Nombres} {user.Apellidos}";

                    orruN.IniciarReparto(obj);

                    foreach (var i in obj.DetRRU0)
                    {
                        if (i.Estado != "LIBERADO")
                        {
                            // Seteamos el usuario en sesión que está ejecutando esta acción
                            i.OpEntrega = $"{user.Nombres} {user.Apellidos}";
                            rru0N.entregarRRU0(i);
                        }
                    }
                    obj = orruN.obtenerOrdenDeRuta(o.DocEntry);
                    obj.Operario = $"{user.Nombres} {user.Apellidos}";
                    orruN.TerminarReparto(obj);
                    return RedirectToAction("ListadoRepartos", new { DocNum = obj.DocNum, msj = "Ruta terminada correctamente" });
                }
                catch (Exception e)
                {
                    return RedirectToAction("ListadoRepartos", new { DocNum = obj.DocNum, msj = e.Message });
                }
            }
            else if (verificacionAccesos(idOperation) == "E_Login")
            { return RedirectToAction("Index", "Index"); }
            else
            { return RedirectToAction("Error", "Index"); }
        }
        public ActionResult RptControlTemperaturaHumedad(Rpt_TempHumed_E datos, string FechaTerEn, int idOperation = 219)
        {
            if (verificacionAccesos(idOperation) == "C_Access")
            {
                ViewBag.Placa = datos.Placa;
                ViewBag.Serie = datos.Serie;
                var resultTempHum = orruN.RptTempHumed(datos.Placa, FechaTerEn, datos.Serie);
                if (resultTempHum != null && resultTempHum.Count >= 1)
                {
                    Usuario_N usuN = new Usuario_N();

                    var datosUsuario = usuN.BuscarDocEntryUsuario(resultTempHum[0].TransCod);
                    var firmas = BuscarFirmas(new List<int> { datosUsuario.DocEntry, 414 });       // docEntry ejm

                    if (firmas != null && firmas.Count >= 1)
                    {
                        foreach (var f in firmas)
                        {
                            if (f.IdRolUsuario.Equals(55))      // solo para pruebas
                                                                //if (f.IdRolUsuario.Equals(55))		// ROL "REPA"
                            {
                                ViewBag.FirmaTransportista = f.RutaFirma;
                            }
                            else if (f.IdRolUsuario.Equals(2))  // solo para pruebas
                                                                //}else if(f.IdRolUsuario.Equals(3))		// ROL: "DT"
                            {
                                ViewBag.FirmaDT = f.RutaFirma;
                            }
                        }
                    }
                }

                return View("Reportes/RptControlTemperaturaHumedad", resultTempHum);
            }
            else if (verificacionAccesos(idOperation) == "E_Login")
            { return RedirectToAction("Index", "Index"); }
            else
            { return RedirectToAction("Error", "Index"); }
        }
        public ActionResult RptTempHumed(string Placa, string FechaTerEn, string Serie, int idOperation = 219)
        {
            if (verificacionAccesos(idOperation) == "C_Access")
            {
                ReportViewer rp = new ReportViewer();
                try
                {
                    rp.ProcessingMode = ProcessingMode.Local;
                    rp.SizeToReportContent = true;
                    rp.LocalReport.ReportPath =
                        Request.MapPath(Request.ApplicationPath) + @"Reportes\RptRutas\RptTempHumed.rdlc";
                    rp.LocalReport.DataSources.Add(new ReportDataSource("DS_TempHumed", orruN.RptTempHumed(Placa, FechaTerEn, Serie)));
                    ViewBag.REPORTE = rp;
                }
                catch (Exception e)
                {
                    ViewBag.Mensaje = e.Message;
                }
                return View("reporteViewer");
            }
            else if (verificacionAccesos(idOperation) == "E_Login")
            { return RedirectToAction("Index", "Index"); }
            else
            { return RedirectToAction("Error", "Index"); }

        }
        public FileResult EvidenciaReparto(int DocEntry, string TipoRuta, int idOperation = 220)
        {
            if (verificacionAccesos(idOperation) == "C_Access")
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
            if (verificacionAccesos(idOperation) == "C_Access")
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
            else if (verificacionAccesos(idOperation) == "E_Login")
            { return RedirectToAction("Index", "Index"); }
            else
            { return RedirectToAction("Error", "Index"); }
        }
        public ActionResult EntregaMasiva(RRU0_E o, int idOperation = 222)
        {
            if (verificacionAccesos(idOperation) == "C_Access")
            {
                Usuario_E user = (Usuario_E)Session["UsuarioId"];
                o.OpEntrega = $"{user.Nombres} {user.Apellidos}";
                RRU0_N rru0N = new RRU0_N();
                try
                {
                    rru0N.entregaMasivaDetRep(o);
                    return RedirectToAction("ListadoEntregaRepartos", new { TipoRep = "Re", DocEntry = o.DocEntry, msj = "Se entrego masivamente" });
                }
                catch (Exception e)
                { return RedirectToAction("ListadoEntregaRepartos", new { TipoRep = "Re", DocEntry = o.DocEntry, msj = e.Message }); }
            }
            else if (verificacionAccesos(idOperation) == "E_Login")
            { return RedirectToAction("Index", "Index"); }
            else
            { return RedirectToAction("Error", "Index"); }
        }
        public ActionResult GestionarOficinas(OUR1_E o, string msj, int idOperation = 223)
        {
            if (verificacionAccesos(idOperation) == "C_Access")
            {
                OUR1_N oofiN = new OUR1_N(); UBIG_N ubigN = new UBIG_N(); COUR_N courN = new COUR_N();
                ViewBag.Ubigeos = ubigN.Listar();
                ViewBag.Couriers = courN.Listar();
                ViewBag.Mensaje = msj;
                return View(oofiN.Listar());
            }
            else if (verificacionAccesos(idOperation) == "E_Login")
            { return RedirectToAction("Index", "Index"); }
            else
            { return RedirectToAction("Error", "Index"); }
        }
        public ActionResult RegistrarOficina(OUR1_E o, int idOperation = 224)
        {
            if (verificacionAccesos(idOperation) == "C_Access")
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
            else if (verificacionAccesos(idOperation) == "E_Login")
            { return RedirectToAction("Index", "Index"); }
            else
            { return RedirectToAction("Error", "Index"); }
        }//no vista
        public ActionResult EliminarOficina(int Id, int idOperation = 225)
        {
            if (verificacionAccesos(idOperation) == "C_Access")
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
            else if (verificacionAccesos(idOperation) == "E_Login")
            { return RedirectToAction("Index", "Index"); }
            else
            { return RedirectToAction("Error", "Index"); }
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
        public ActionResult CrearHojaDeCargo(string TipoRep, int idOperation = 226)
        {
            if (verificacionAccesos(idOperation) == "C_Access")
            {
                COUR_N courN = new COUR_N();
                ViewBag.Mensaje = "";
                ViewBag.TipoRep = TipoRep;
                ViewBag.ListaOrigenesDestinos = owhsN.listarAlmacenes();
                ViewBag.Agencias = courN.Listar();
                return View(new ORRU_E());
            }
            else if (verificacionAccesos(idOperation) == "E_Login")
            { return RedirectToAction("Index", "Index"); }
            else
            { return RedirectToAction("Error", "Index"); }
        }
        [HttpPost]
        public ActionResult CrearHojaDeCargo(ORRU_E o, int idOperation = 226)
        {
            if (verificacionAccesos(idOperation) == "C_Access")
            {
                try
                {
                    Usuario_E user = (Usuario_E)Session["UsuarioId"];
                    o.Propietario = $"{user.Nombres} {user.Apellidos}";
                    int DocNum = orruN.CrearOrdenDeRuta(o);
                    ViewBag.Mensaje = "Creado correctamente";
                    ViewBag.TipoRep = "Re";

                    return RedirectToAction("ListadoRepartos", new { DocNum = DocNum });
                }
                catch (Exception ex)
                {
                    OCHO_N ochoN = new OCHO_N(); COUR_N courN = new COUR_N();
                    ViewBag.Mensaje = ex.Message;
                    ViewBag.ListaChoferes = ochoN.listaChoferes(0, null);
                    ViewBag.ListaVehiculos = ovehN.listaVeh(0, null);
                    ViewBag.ListaCopilotos = ousrN.ListaUsuarios(new Usuario_E { IdRol = 4 });
                    ViewBag.ListaOrigenesDestinos = owhsN.listarAlmacenes();
                    ViewBag.Filas = 30;
                    ViewBag.Agencias = courN.Listar();
                    return View(o);
                }
            }
            else if (verificacionAccesos(idOperation) == "E_Login")
            { return RedirectToAction("Index", "Index"); }
            else
            { return RedirectToAction("Error", "Index"); }
        }
        public ActionResult AnularHojaCargo(int DocEntry, string TipoRep, string msj, int idOperation = 227)
        {
            if (verificacionAccesos(idOperation) == "C_Access")
            {
                ORRU_N obj = new ORRU_N(); ORTV_N ortvN = new ORTV_N();
                ViewBag.TipoRep = TipoRep;
                ViewBag.Mensaje = msj;
                var o = obj.obtenerOrdenDeRuta(DocEntry);
                if (o.DetRRU0 != null)
                {
                    foreach (RRU0_E r0 in o.DetRRU0)
                    {
                        r0.Ticket = ortvN.obtenerTicket(r0.DocEntryTicket);
                    }
                }
                return View(o);
            }
            else if (verificacionAccesos(idOperation) == "E_Login")
            { return RedirectToAction("Index", "Index"); }
            else
            { return RedirectToAction("Error", "Index"); }
        }
        public JsonResult RegistrarAgencia(COUR_E o, int idOperation = 228)
        {
            if (verificacionAccesos(idOperation) == "C_Access")
            {
                COUR_N oN = new COUR_N();
                return Json(oN.Registrar(o));
            }
            else
            { return null; }
        }
        public ActionResult GestionarTarifarios(string msj, int idOperation = 229)
        {
            if (verificacionAccesos(idOperation) == "C_Access")
            {
                UBIG_N ubigN = new UBIG_N(); COUR_N courN = new COUR_N();
                OUR2_N our2N = new OUR2_N();
                ViewBag.Ubigeos = ubigN.Listar();
                ViewBag.Agencias = courN.Listar();
                ViewBag.Mensaje = msj;
                return View(our2N.Listar(""));
            }
            else if (verificacionAccesos(idOperation) == "E_Login")
            { return RedirectToAction("Index", "Index"); }
            else
            { return RedirectToAction("Error", "Index"); }
        }
        public JsonResult PrevisualizarExcel(string filename, int idOperation = 230)
        {
            if (verificacionAccesos(idOperation) == "C_Access")
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
            if (verificacionAccesos(idOperation) == "C_Access")
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
            else if (verificacionAccesos(idOperation) == "E_Login")
            { return RedirectToAction("Index", "Index"); }
            else
            { return RedirectToAction("Error", "Index"); }
        }
        public void CalcularPrecioEnv(int DocEntry, int idOperation = 232)
        {
            if (verificacionAccesos(idOperation) == "C_Access")
            {
                string Destino = string.Empty;
                ORTV_N ortvN = new ORTV_N(); COUR_N courN = new COUR_N(); OUR2_N oN = new OUR2_N();
                RTV6_N rtv6N = new RTV6_N();
                ORTV_E obj = ortvN.obtenerTicket(DocEntry);
                var Courier = courN.Obtener(obj.Agencia);
                if (obj.LugarDestino == "Agencia Courier")
                {
                    if (obj.EnvioAgencia == "Oficina de agencia")
                    {
                        Destino = obj.Det3[1].Distrito.ToUpper();
                    }
                    else if (obj.EnvioAgencia == "Domicilio de cliente")
                    {
                        if (!string.IsNullOrEmpty(obj.Det3[0].Distrito))
                        {
                            Destino = obj.Det3[0].Distrito.ToUpper();
                        }
                        else if (!string.IsNullOrEmpty(obj.Det3[1].Distrito))
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
            if (verificacionAccesos(idOperation) == "C_Access")
            {
                string msj;
                ORTV_N ortvN = new ORTV_N(); RRU0_N rru0N = new RRU0_N();
                var tc = ortvN.obtenerTicket(DocEntryTicket);

                RRU0_E rru0E = rru0N.buscarRRU0(DocEntry, Linea);
                if (TipoRuta == "AC" /*&& tc.IdReg == 0 */)
                {
                    int i = ortvN.editarSeguimientoTicket("ANULARENTREGADO", DocEntryTicket, tc);
                    if (i > 0) { anularRRU0(rru0E); }
                    msj = "Correcta anulacion de entrega";
                }
                else { msj = "Ticket tiene regalo, no se puede anular entrega"; }
                return RedirectToAction("AnularHojaCargo", "Rutas", new { DocEntry = DocEntry, TipoRep = "Re", msj = msj });
            }
            else if (verificacionAccesos(idOperation) == "E_Login")
            { return RedirectToAction("Index", "Index"); }
            else
            { return RedirectToAction("Error", "Index"); }
        }
        public string anularRRU0(RRU0_E o, int idOperation = 233)
        {
            string res = string.Empty;
            if (verificacionAccesos(idOperation) == "C_Access")
            {
                RRU0_N rru0N = new RRU0_N();
                try
                {
                    Usuario_E user = (Usuario_E)Session["UsuarioId"];
                    o.Operario = $"{user.Nombres} {user.Apellidos}";
                    rru0N.anularRRU0(o);
                }
                catch (Exception e) { res = e.Message; return res; }
                return res;
            }
            else
            { return res; }
        }
        public ActionResult infoHojasCargo(int idOperation = 234)
        {
            if (verificacionAccesos(idOperation) == "C_Access")
            {
                OCRD_N ocrdN = new OCRD_N(); OCHO_N ochoN = new OCHO_N();
                ViewBag.Almacenes = owhsN.listarAlmacenes();
                ViewBag.Clientes = ocrdN.listarSociosDeNegocios(new OCRD_E { CardType = "C" });
                ViewBag.Transportistas = ochoN.listaChoferes(0, null);
                ViewBag.ListaVehiculos = ovehN.listaVeh(0, null);
                return View();
            }
            else if (verificacionAccesos(idOperation) == "E_Login")
            { return RedirectToAction("Index", "Index"); }
            else
            { return RedirectToAction("Error", "Index"); }
        }
        public ActionResult infoHojasCargoDetallado(int idOperation = 234)
        {
            if (verificacionAccesos(idOperation) == "C_Access")
            {
                OCRD_N ocrdN = new OCRD_N(); OCHO_N ochoN = new OCHO_N();
                ViewBag.Almacenes = owhsN.listarAlmacenes();
                ViewBag.Clientes = ocrdN.listarSociosDeNegocios(new OCRD_E { CardType = "C" });
                ViewBag.Transportistas = ochoN.listaChoferes(0, null);
                ViewBag.ListaVehiculos = ovehN.listaVeh(0, null);
                return View();
            }
            else if (verificacionAccesos(idOperation) == "E_Login")
            { return RedirectToAction("Index", "Index"); }
            else
            { return RedirectToAction("Error", "Index"); }
        }
        public ActionResult RptHojasCargo(ORRU_E o, int idOperation = 235)
        {
            if (string.IsNullOrEmpty(o.FecConIni) || string.IsNullOrEmpty(o.FecConFin))
            {
                return null;
            }

            if (verificacionAccesos(idOperation) == "C_Access")
            {

                ORRU_N orruN = new ORRU_N();
                string excelContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                var reporte = orruN.ReporteHojasCargo(o);
                using (var libro = new ExcelPackage())
                {
                    var worksheet = libro.Workbook.Worksheets.Add("ReporteHojasCargo");
                    worksheet.Cells["A1"].LoadFromCollection(reporte, PrintHeaders: true);

                    if (reporte != null)
                    {
                        if (reporte.Count >= 1)
                        {
                            for (var col = 1; col <= 36; col++)
                            {
                                worksheet.Column(col).AutoFit();
                            }

                            var tabla = worksheet.Tables.Add(new ExcelAddressBase(fromRow: 1, fromCol: 1, toRow: reporte.Count + 1, toColumn: 36), "ReporteHojasCargo");

                            tabla.ShowHeader = true;
                            tabla.TableStyle = TableStyles.Medium2;
                        }

                    }

                    return File(libro.GetAsByteArray(), excelContentType, "AnalisisHojasCargo.xlsx");
                }
            }
            else if (verificacionAccesos(idOperation) == "E_Login") { return null; }
            else { return null; }

        }
        public ActionResult RptHojasCargoDetallado(ORRU_E o, int idOperation = 236)
        {
            if (string.IsNullOrEmpty(o.FecConIni) || string.IsNullOrEmpty(o.FecConFin))
            {
                return null;
            }

            if (verificacionAccesos(idOperation) == "C_Access")
            {

                ORRU_N orruN = new ORRU_N();
                string excelContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                var reporte = orruN.ReporteHojasCargoDet(o);
                using (var libro = new ExcelPackage())
                {
                    var worksheet = libro.Workbook.Worksheets.Add("ReporteHojasCargoDet");
                    worksheet.Cells["A1"].LoadFromCollection(reporte, PrintHeaders: true);

                    if (reporte != null)
                    {
                        if (reporte.Count >= 1)
                        {
                            for (var col = 1; col <= 36; col++)
                            {
                                worksheet.Column(col).AutoFit();
                            }
                            var tabla = worksheet.Tables.Add(new ExcelAddressBase(fromRow: 1, fromCol: 1, toRow: reporte.Count + 1, toColumn: 40), "ReporteHojasCargoDet");

                            tabla.ShowHeader = true;
                            tabla.TableStyle = TableStyles.Medium2;
                        }
                    }

                    return File(libro.GetAsByteArray(), excelContentType, "AnalisisHojasCargoPesaje.xlsx");
                }
            }
            else if (verificacionAccesos(idOperation) == "E_Login") { return null; }
            else { return null; }

        }
        public ActionResult SeguimientoDeTicket(int DocEntry)
        {
            return RedirectToAction("SeguimientoDeTicket", "Ventas", new { DocEntry = DocEntry });
        }
        public ActionResult validarRuta(ORRU_E o)
        {
            try
            {
                orruN.validarDatosOrdenDeRuta(o);
            }
            catch (Exception e) { return Content(e.Message); }
            return Content("OK");
        }
        public ActionResult validarEditarRuta(ORRU_E o)
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
                    rru0N.validarEntDetReparto(r0);
                }

            }
            catch (Exception e) { return Content(e.Message); }
            return Content("ok");
        }
        public JsonResult infoTicketsReparto(string FechaSapTicket, string TipoRuta, string Zona, string AlmOrigenCod)
        {
            string[] estados = { "", "" };
            ORTV_N ortvN = new ORTV_N();
            ORTV_E ortvE = new ORTV_E { FechaSapTicket = FechaSapTicket, EstadoFacturacion = "FACTURADO", Zona = Zona };
            if (TipoRuta == "VD")
            {
                ortvE.LugarDestino = "Domicilio";
                estados[0] = "EMPACADO";
                ortvE.LugEntrega = AlmOrigenCod;
            }
            else if (TipoRuta == "VG")
            {
                ortvE.LugarDestino = "Agencia";
                estados[0] = "EMPACADO";
                estados[1] = "PESADO";
                ortvE.LugEntrega = " ";
            }
            else if (TipoRuta == "AC")
            {
                ortvE.LugarDestino = "Agencia Courier";
                estados[0] = "PESADO";
                ortvE.LugEntrega = AlmOrigenCod;
            }
            else if (TipoRuta == "VC")
            {
                ortvE.LugarDestino = "Centro";
                estados[0] = "EMPACADO";
                ortvE.LugEntrega = AlmOrigenCod;
            }
            else if (TipoRuta == "VA")
            {
                ortvE.LugarDestino = "Arriola";
                estados[0] = "EMPACADO";
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
            string[] estados = { "", "" };
            ORTV_N ortvN = new ORTV_N();
            ORTV_E ortvE = new ORTV_E { FechaSapTicket = FechaSapTicket, Zona = Zona };
            if (TipoRuta == "VD") { ortvE.LugarDestino = "Domicilio"; estados[0] = "EMPACADO"; ortvE.LugEntrega = AlmOrigenCod; }
            else if (TipoRuta == "VG")
            {
                ortvE.LugarDestino = "Agencia";
                estados[0] = "EMPACADO";
                estados[1] = "PESADO"; ortvE.LugEntrega = "";
            }
            else if (TipoRuta == "AC")
            {
                ortvE.LugarDestino = "Agencia Courier";
                estados[0] = "PESADO"; ortvE.LugEntrega = AlmOrigenCod;
            }
            else if (TipoRuta == "VC")
            {
                ortvE.LugarDestino = "Centro";
                estados[0] = "EMPACADO"; ortvE.LugEntrega = AlmOrigenCod;
            }
            else if (TipoRuta == "VA")
            {
                ortvE.LugarDestino = "Arriola";
                estados[0] = "EMPACADO"; ortvE.LugEntrega = AlmOrigenCod;
            }
            var resultado = ortvN.listarTicketsRepartosNoEnviados(ortvE, estados);
            return Json(resultado);
        }
        //public ActionResult grabarTempDocumentosRuta(int DocEntryTicket, int DocNumTicket)
        //{
        //    //Buscar documentos en Guias,Facturas Boletas NC ND en Tablas Hana
        //    Utilitarios uti = new Utilitarios();
        //    ORTV_N negTicket = new ORTV_N();
        //    OWTR_N owtrN = new OWTR_N();
        //    OINV_N oinvN = new OINV_N();
        //    ORIN_N orinN = new ORIN_N();
        //    TEMP_RRU01_N negTemp = new TEMP_RRU01_N();
        //    ORTV_E ticket = negTicket.obtenerTicket(DocEntryTicket); List<TEMP_RRU01_E> Documentos = new List<TEMP_RRU01_E>();
        //    /********************************************************GUIAS REMISION ************************************************/
        //    List<TEMP_RRU01_E> GuiasRemision = new List<TEMP_RRU01_E>();
        //    if (ticket.LugarDestino.Equals("Domicilio") || ticket.LugarDestino.Equals("Agencia"))
        //    {
        //        GuiasRemision = negTicket.GuiasRemisionSap(DocEntryTicket);
        //    }
        //    else
        //    {
        //        string WhsCode = string.Empty;
        //        if (ticket.LugarDestino.Equals("Centro")) { WhsCode = "01"; }
        //        else if (ticket.LugarDestino.Equals("Arriola")) { WhsCode = "09"; }
        //        GuiasRemision = owtrN.GuiasRemisionSap(DocNumTicket, WhsCode);
        //    }
        //    if (GuiasRemision != null & GuiasRemision.Count > 0)
        //    {
        //        foreach (var G in GuiasRemision) { Documentos.Add(G); }
        //    }
        //    /*******************************************************FACTURAS O BOLETAS ********************************************/
        //    List<TEMP_RRU01_E> FactBoleta = new List<TEMP_RRU01_E>();
        //    if (ticket.EstadoFacturacion.Equals("FACTURADO") && !ticket.Estado.Equals("ANULADO") && !ticket.Estado.Equals("CANCELADO") && ticket.Det2 != null && ticket.Det2.Count > 0)
        //    {
        //        List<int> OrdenesSap = new List<int>();
        //        foreach (var ordr in ticket.Det2)
        //        {
        //            string query = $"SELECT \"DocEntry\" FROM {uti.schemaHana}ordr WHERE \"DocNum\" = '{ordr.NroSap}' AND \"CANCELED\" = 'N'";
        //            HanaConnection cn = new HanaConnection(uti.cadHana);
        //            try
        //            {
        //                cn.Open();
        //                HanaCommand cmd = new HanaCommand(query, cn);
        //                cmd.CommandType = System.Data.CommandType.Text;
        //                HanaDataReader dr = cmd.ExecuteReader();
        //                dr.Read();
        //                if (!dr.IsDBNull(0)) { OrdenesSap.Add(dr.GetInt32(0)); }
        //                dr.Close();
        //                cn.Close();
        //            }
        //            catch { cn.Close(); }
        //        }

        //        foreach (var docEntryORDR in OrdenesSap)
        //        {
        //            List<TEMP_RRU01_E> FBxORDR = oinvN.FactBoletaSap(docEntryORDR);
        //            foreach (var f in FBxORDR)
        //            {
        //                if (!string.IsNullOrEmpty(f.U_SYP_MDCD))
        //                {
        //                    FactBoleta.Add(f);
        //                }
        //            }
        //        }
        //        if (FactBoleta != null & FactBoleta.Count > 0)
        //        {
        //            foreach (var FB in FactBoleta)
        //            {
        //                Documentos.Add(FB);
        //            }
        //        }
        //    }
        //    /*******************************************************NOTAS DE CREDITO***********************************************/
        //    if (ticket.DescuentoNC > 0 && ticket.Det4 != null && ticket.Det4.Count > 0)
        //    {
        //        List<TEMP_RRU01_E> NotasCredito = new List<TEMP_RRU01_E>(); TEMP_RRU01_E objNc;
        //        foreach (var obj in ticket.Det4)
        //        {
        //            objNc = orinN.NotaCreditoSap(obj.Nc.DocNum);
        //            NotasCredito.Add(objNc);
        //        }
        //        if (NotasCredito != null & NotasCredito.Count > 0)
        //        {
        //            foreach (var NC in NotasCredito) { Documentos.Add(NC); }
        //        }
        //    }
        //    /********************************************************NOTAS DE DEBITO************************************************/
        //    if ((ticket.Flete > 0 || ticket.GastoEnvio > 0) && FactBoleta.Count > 0)
        //    {
        //        List<TEMP_RRU01_E> NotasDebito = new List<TEMP_RRU01_E>();
        //        string FBConcatenadas = string.Join(", ", FactBoleta.Select(obj => $"{obj.U_SYP_MDTD}-{obj.U_SYP_MDSD}-{obj.U_SYP_MDCD}"));

        //        NotasDebito = oinvN.NotasDebitoSap(FBConcatenadas);
        //        if (NotasDebito != null & NotasDebito.Count > 0)
        //        {
        //            foreach (var ND in NotasDebito) { Documentos.Add(ND); }
        //        }
        //    }
        //    //
        //    //
        //    //
        //    // grabar en la tabla temporal todos los documentos hallados almacenado en la variable Documentos
        //    //
        //    //
        //    Usuario_E user = (Usuario_E)Session["UsuarioId"];
        //    string OperarioLogueado = $"{user.Nombres} {user.Apellidos}";
        //    Documentos.ForEach(d =>
        //    {
        //        d.DocEntryTicket = DocEntryTicket;
        //        d.Operario = OperarioLogueado;
        //    });
        //    string resultado = negTemp.Registrar(Documentos);
        //    return Content(resultado);
        //}
        public ActionResult buscarDocumentosRuta(int DocEntryTicket)
        {
            RRU01_N rru01 = new RRU01_N();
            string resultado = rru01.BuscarComprobantes(DocEntryTicket);
            return Content(resultado);
        }
        public ActionResult LayoutFacturaBoletaSap(string NumAtCard, string DocNum)
        {
            OINV_N oinvN = new OINV_N(); ViewBag.DocNumTicket = DocNum;
            return View(oinvN.buscarFacturaBoletaSap(NumAtCard));
        }
        public ActionResult LayoutFacturaBoletaSap_header(string NumAtCard, string Tipo)
        {
            OINV_N oinvN = new OINV_N(); ViewBag.Tipo = Tipo;
            return View(oinvN.buscarFacturaBoletaSap(NumAtCard));
        }
        public ActionResult LayoutGuiaRemisionSap(string NumAtCard, string DocNumTicket)
        {
            string Tipo = "E"; //Entrega
            ODLN_N odlN = new ODLN_N(); OWTR_N owtrN = new OWTR_N(); ViewBag.DocNumTicket = DocNumTicket;
            List<Guia_Remision_E> guia = odlN.buscarGuiaRemisionSap(NumAtCard);
            if (guia == null || guia.Count == 0)
            {
                Tipo = "T"; //transferencia
                guia = owtrN.buscarGuiaRemisionSap(NumAtCard);
            }
            ViewBag.Tipo = Tipo;
            return View(guia);
        }
        public ActionResult LayoutGuiaRemisionSap_header(string NumAtCard)
        {
            string Tipo = "E"; //Entrega
            ODLN_N odlN = new ODLN_N(); OWTR_N owtrN = new OWTR_N();
            List<Guia_Remision_E> guia = odlN.buscarGuiaRemisionSap(NumAtCard);
            if (guia == null || guia.Count == 0)
            {
                Tipo = "T"; //transferencia
                guia = owtrN.buscarGuiaRemisionSap(NumAtCard);
            }
            ViewBag.Tipo = Tipo;
            return View(guia);
        }
        public ActionResult LayoutNotaCreditoDebitoSap(string NumAtCard)
        {
            OINV_N oinvN = new OINV_N(); ORIN_N orinN = new ORIN_N();
            string Tipo = "", SubTipo = "";
            List<NotaCreditoDebito_E> nota = new List<NotaCreditoDebito_E>();
            if (NumAtCard.Contains("FN") || NumAtCard.Contains("BN"))//nota de credito ORIN
            {
                Tipo = "NC"; //Nota Credito de Articulo y Servicio
                             //buscar DocType
                List<ORIN_E> listOrin = orinN.listarNotasDeCredito(new ORIN_E { NumAtCard = NumAtCard });
                if (listOrin != null && listOrin.Count > 0) { SubTipo = listOrin[0].DocType; }
                if (SubTipo.Equals("I"))
                {
                    nota = orinN.buscarNotaCreditoSapArticulo(NumAtCard);
                }
                else if (SubTipo.Equals("S"))
                {
                    nota = orinN.buscarNotaCreditoSapServicio(NumAtCard);
                }
            }
            else if (NumAtCard.Contains("FD") || NumAtCard.Contains("BD"))//nota de debito OINV
            {
                Tipo = "ND"; //Nota de Debito solo Servicio
                nota = oinvN.buscarNotaDebitoSap(NumAtCard);
            }
            ViewBag.Tipo = Tipo;
            ViewBag.SubTipo = SubTipo;
            return View(nota);
        }
        public ActionResult LayoutNotaCreditoDebitoSap_header(string NumAtCard)
        {
            OINV_N oinvN = new OINV_N(); ORIN_N orinN = new ORIN_N();
            string Tipo = "", SubTipo = "";
            List<NotaCreditoDebito_E> nota = new List<NotaCreditoDebito_E>();
            if (NumAtCard.Contains("FN") || NumAtCard.Contains("BN"))//nota de credito ORIN
            {
                Tipo = "NC"; //Nota Credito de Articulo y Servicio
                             //buscar DocType
                List<ORIN_E> listOrin = orinN.listarNotasDeCredito(new ORIN_E { NumAtCard = NumAtCard });
                if (listOrin != null && listOrin.Count > 0) { SubTipo = listOrin[0].DocType; }
                if (SubTipo.Equals("I"))
                {
                    nota = orinN.buscarNotaCreditoSapArticulo(NumAtCard);
                }
                else if (SubTipo.Equals("S"))
                {
                    nota = orinN.buscarNotaCreditoSapServicio(NumAtCard);
                }
            }
            else if (NumAtCard.Contains("FD") || NumAtCard.Contains("BD"))//nota de debito OINV
            {
                Tipo = "ND"; //Nota de Debito solo Servicio
                nota = oinvN.buscarNotaDebitoSap(NumAtCard);
            }
            ViewBag.Tipo = Tipo;
            ViewBag.SubTipo = SubTipo;
            return View(nota);
        }
        [Obsolete]
        public JsonResult PdfComprobanteSap(string NumAtCard, int DocNumTicket, string Impresora)
        {
            TEMP_RRU01_N negTemp = new TEMP_RRU01_N();
            bool exito = false; string Mensaje = "";
            string NewNumAtCard = NumAtCard.Replace("-", "_");
            //buscar si el correlativo ya esta siendo impreso por otro usuario
            if (!string.IsNullOrEmpty(negTemp.ConsultarImpreso(NumAtCard)))
            {
                Mensaje += "Aviso: " + negTemp.ConsultarImpreso(NumAtCard) + ",";
            }

            var pdfResult = new ActionAsPdf(null);
            if (NumAtCard.Contains("F0"))
            {
                var parametros = new
                {
                    NumAtCard = NumAtCard,
                    Tipo = "F",
                };
                // Define la URL de la Cabecera 
                string _headerUrl = Url.Action("LayoutFacturaBoletaSap_header", "Rutas", parametros, "http");

                pdfResult = new ActionAsPdf("LayoutFacturaBoletaSap", new { NumAtCard = NumAtCard, DocNum = DocNumTicket })
                {
                    FileName = "Layout" + NewNumAtCard + ".pdf",
                    PageOrientation = Rotativa.Options.Orientation.Portrait,
                    CustomSwitches = "--header-html " + _headerUrl + " --header-spacing 0 ",
                    PageSize = Rotativa.Options.Size.A4,
                    PageMargins = new Rotativa.Options.Margins(65, 10, 20, 10)
                };
            }
            else if (NumAtCard.Contains("B0"))
            {
                var parametros = new
                {
                    NumAtCard = NumAtCard,
                    Tipo = "B",
                };
                string _headerUrl = Url.Action("LayoutFacturaBoletaSap_header", "Rutas", parametros, "http");
                pdfResult = new ActionAsPdf("LayoutFacturaBoletaSap", new { NumAtCard = NumAtCard, DocNum = DocNumTicket })
                {
                    FileName = "Layout" + NewNumAtCard + ".pdf",
                    PageOrientation = Rotativa.Options.Orientation.Portrait,
                    CustomSwitches = "--header-html " + _headerUrl + " --header-spacing 0 ",
                    PageSize = Rotativa.Options.Size.A4,
                    PageMargins = new Rotativa.Options.Margins(65, 10, 20, 10)
                };
            }
            else if (NumAtCard.Contains("T0"))
            {
                var parametros = new
                {
                    NumAtCard = NumAtCard
                };
                string _headerUrl = Url.Action("LayoutGuiaRemisionSap_header", "Rutas", parametros, "http");
                pdfResult = new ActionAsPdf("LayoutGuiaRemisionSap", new { NumAtCard = NumAtCard, DocNumTicket = DocNumTicket })
                {
                    FileName = "Layout" + NewNumAtCard + ".pdf",
                    PageOrientation = Rotativa.Options.Orientation.Portrait,
                    CustomSwitches = "--header-html " + _headerUrl + " --header-spacing 0 ",
                    PageSize = Rotativa.Options.Size.A4,
                    PageMargins = new Rotativa.Options.Margins(65, 10, 20, 10)
                };
            }
            else if (NumAtCard.Contains("FN") || NumAtCard.Contains("BN"))//nota de credito
            {
                var parametros = new
                {
                    NumAtCard = NumAtCard
                };
                string _headerUrl = Url.Action("LayoutNotaCreditoDebitoSap_header", "Rutas", parametros, "http");
                pdfResult = new ActionAsPdf("LayoutNotaCreditoDebitoSap", new { NumAtCard = NumAtCard })
                {
                    FileName = "Layout" + NewNumAtCard + ".pdf",
                    PageOrientation = Rotativa.Options.Orientation.Portrait,
                    CustomSwitches = "--header-html " + _headerUrl + " --header-spacing 0 ",
                    PageSize = Rotativa.Options.Size.A4,
                    PageMargins = new Rotativa.Options.Margins(70, 10, 20, 10)
                };
            }
            else if (NumAtCard.Contains("FD") || NumAtCard.Contains("BD"))//nota de debito
            {
                var parametros = new
                {
                    NumAtCard = NumAtCard
                };
                string _headerUrl = Url.Action("LayoutNotaCreditoDebitoSap_header", "Rutas", parametros, "http");
                pdfResult = new ActionAsPdf("LayoutNotaCreditoDebitoSap", new { NumAtCard = NumAtCard })
                {
                    FileName = "Layout" + NewNumAtCard + ".pdf",
                    PageOrientation = Rotativa.Options.Orientation.Portrait,
                    CustomSwitches = "--header-html " + _headerUrl + " --header-spacing 0 ",
                    PageSize = Rotativa.Options.Size.A4,
                    PageMargins = new Rotativa.Options.Margins(70, 10, 20, 10),

                };
            }
            //proceso igual para todos los comprobantes
            byte[] pdfContent = pdfResult.BuildPdf(ControllerContext);
            string filePath = Server.MapPath("~/ComprobantesRepartos/Layout" + NewNumAtCard + ".pdf");
            System.IO.File.WriteAllBytes(filePath, pdfContent); string pdfPath = filePath;
            //paginacion
            iTextSharp.text.pdf.PdfReader reader = new iTextSharp.text.pdf.PdfReader(pdfPath);
            int startIndex = pdfPath.IndexOf("Layout"); startIndex += "Layout".Length;
            string nombreasignar = pdfPath.Substring(startIndex);
            string nuevaruta = Server.MapPath("~/ComprobantesRepartos/" + nombreasignar);
            // Crea un objeto PdfStamper para modificar el PDF ahora con la paginacion
            iTextSharp.text.pdf.PdfStamper stamper = new iTextSharp.text.pdf.PdfStamper(reader, new System.IO.FileStream(nuevaruta, System.IO.FileMode.Create));
            int totalPages = reader.NumberOfPages;

            for (int i = 1; i <= totalPages; i++)
            {
                PdfContentByte content = stamper.GetUnderContent(i);
                //agrega la paginacion
                iTextSharp.text.Font font = FontFactory.GetFont(FontFactory.HELVETICA, 8);
                Phrase phrase = new Phrase("Página " + i + " de " + totalPages, font);
                Phrase phrase2 = new Phrase(DocNumTicket.ToString(), font);
                ColumnText.ShowTextAligned(content, Element.ALIGN_LEFT, phrase2, 30, 35, 0);
                ColumnText.ShowTextAligned(content, Element.ALIGN_CENTER, phrase, 300, 35, 0);
            }
            stamper.Close();
            reader.Close();
            System.IO.File.Delete(filePath); ImprimirPDF(nuevaruta, Impresora);
            //exito = ImprimirPDF(nuevaruta,Impresora);
            //si hubo exito en la impresion debera hacerse un update al documento en la tabla al.TEMP_RRU01, columna Impreso pasa a 1
            /* if (exito)
             {*/
            int DocEntryTicket = DocNumTicket - 2000000000;
            negTemp.EditarImpreso(NumAtCard, DocEntryTicket);
            // }

            return Json(new { Mensaje = Mensaje });
        }
        public bool ImprimirPDF(string pdfPath, string Impresora)
        {
            if (!System.IO.File.Exists(pdfPath))
            {
                Console.WriteLine("El archivo PDF no existe en la ruta especificada.");
                return false;
            }
            using (PdfiumViewer.PdfDocument pdfDocument = PdfiumViewer.PdfDocument.Load(pdfPath))
            {
                try
                {
                    PrintDocument printDoc = new PrintDocument();
                    printDoc.PrinterSettings.PrinterName = Impresora;
                    //validamos si la impresora predeterminada existe en el sistema
                    if (!PrinterSettings.InstalledPrinters.Cast<string>().Contains(printDoc.PrinterSettings.PrinterName))
                    {
                        Console.WriteLine("La impresora especificada no está instalada en el sistema.");
                        return false;
                    }
                    // Contador para el número de página actual
                    int pageNumber = 0;

                    printDoc.PrintPage += (sender, e) =>
                    {
                        // Verificar si hay más páginas para imprimir
                        if (pageNumber < pdfDocument.PageCount)
                        {
                            // Convertir la página del PDF a una imagen
                            using (Bitmap bmp = (Bitmap)pdfDocument.Render(pageNumber, (int)e.PageBounds.Width, (int)e.PageBounds.Height, 300, 300, PdfRenderFlags.Annotations))
                            {
                                using (Graphics graphics = Graphics.FromImage(bmp))
                                {
                                    graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                                    graphics.SmoothingMode = SmoothingMode.HighQuality;
                                    graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                                    graphics.CompositingQuality = CompositingQuality.HighQuality;

                                    // Dibujar la imagen en la página de impresión
                                    e.Graphics.DrawImage(bmp, e.PageBounds);
                                }
                                pageNumber++;

                                // Indicar si hay más páginas para imprimir
                                e.HasMorePages = pageNumber < pdfDocument.PageCount;
                            }
                        }
                    };

                    // Imprimir el documento que ahora es una imagen y retorna true porque fue exitoso
                    printDoc.Print();
                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Se produjo un error al imprimir el documento: " + ex.Message);
                    return false;
                }
            }
        }
        public JsonResult buscarComprobantesTicket(int DocEntryTicket)
        {
            verificacionAccesos(0);
            TEMP_RRU01_N negTemp = new TEMP_RRU01_N(); RRU01_N negRRU01 = new RRU01_N();
            List<string> listaComprobantes = negTemp.Listar(DocEntryTicket);
            if (listaComprobantes == null || listaComprobantes.Count == 0)
            {
                List<RRU01_E> detRRU01 = negRRU01.Listar(DocEntryTicket);
                if (detRRU01 != null && detRRU01.Count > 0)
                {
                    foreach (var cmprb in detRRU01) { listaComprobantes.Add(cmprb.U_SYP_MDTD + "-" + cmprb.U_SYP_MDSD + "-" + cmprb.U_SYP_MDCD); }
                }
            }
            return Json(listaComprobantes);
        }
        public ActionResult infoGuiasTicketsVenta(int DocEntry, int idOperation = 202)
        {
            if (verificacionAccesos(idOperation) == "C_Access")
            {
                ORTV_N ortvN = new ORTV_N();
                return Content(ortvN.GuiasTicket(DocEntry));
            }
            else
            { return Content(""); }
        }
        public ActionResult liberarRRU0(RRU0_E o, int idOperation = 202)
        {
            if (verificacionAccesos(idOperation) == "C_Access")
            {
                RRU0_N rru0N = new RRU0_N();
                try
                {
                    Usuario_E user = (Usuario_E)Session["UsuarioId"];
                    o.Operario = $"{user.Nombres} {user.Apellidos}";
                    rru0N.liberarRRU0(o);
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
            verificacionAccesos(0);
            OUR2_N our2N = new OUR2_N();
            string mensajeResult = our2N.Registrar(datos);
            string lista = ListarTarifarios("registrar");

            return Json(new { mensaje = mensajeResult, listaActualizada = lista });
        }
        [HttpPost]
        public JsonResult BuscarTarifario(OUR2_E datos)
        {
            verificacionAccesos(0);
            OUR2_N our2N = new OUR2_N();
            return Json(our2N.Buscar(datos));
        }
        [HttpPost]
        public JsonResult EditarTarifario(OUR2_E datos)
        {
            verificacionAccesos(0);
            OUR2_N our2N = new OUR2_N();
            var mensaje = our2N.Editar(datos);
            string lista = ListarTarifarios("editar");
            return Json(new { mensaje = mensaje, listaActualizada = lista });
        }
        [HttpPost]
        public JsonResult EliminarTarifario(int Id)
        {
            verificacionAccesos(0);
            OUR2_N our2N = new OUR2_N();
            our2N.Eliminar(Id);
            string lista = ListarTarifarios("eliminar");

            return Json(new { mensaje = "Tarifario eliminado satisfactoriamente.", listaActualizada = lista });
        }
        public ActionResult DocumentoRutasTransferenciaAlm(int DocEntry)
        {
            verificacionAccesos(0);
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
            verificacionAccesos(0);
            ORRU_E o = new ORRU_E();
            ORTV_N ortvN = new ORTV_N();
            try
            {
                o = orruN.obtenerOrdenDeRuta(DocEntry);
                if (o.DetRRU0 != null)
                {
                    foreach (RRU0_E r0 in o.DetRRU0)
                    {
                        r0.Ticket = ortvN.obtenerTicket(r0.DocEntryTicket);
                    }
                }
            }
            catch { }
            ViewBag.Letra = 2;
            return View(o);
        }
        public ActionResult DocumentoRutasDomicilio(int DocEntry)
        {
            verificacionAccesos(0);
            ORRU_E o = new ORRU_E(); ORTV_N ortvN = new ORTV_N();
            try
            {
                o = orruN.obtenerOrdenDeRuta(DocEntry);
                if (o.DetRRU0 != null)
                {
                    foreach (RRU0_E r0 in o.DetRRU0)
                    {
                        r0.Ticket = ortvN.obtenerTicket(r0.DocEntryTicket);
                    }
                }
            }
            catch { }
            ViewBag.Letra = 2;
            return View(o);
        }
        public ActionResult DocumentoRepartoAg(int DocEntry)
        {
            verificacionAccesos(0);
            ORRU_E o = new ORRU_E(); ORTV_N ortvN = new ORTV_N();
            try
            {
                o = orruN.obtenerOrdenDeRuta(DocEntry);
                if (o.DetRRU0 != null)
                {
                    foreach (RRU0_E r0 in o.DetRRU0)
                    {
                        r0.Ticket = ortvN.obtenerTicket(r0.DocEntryTicket);
                    }
                }
            }
            catch { }
            ViewBag.Letra = 2;
            return View(o);
        }
        public ActionResult ManifiestoCourier(int DocEntry)
        {
            verificacionAccesos(0);
            ORRU_E o = new ORRU_E(); ORTV_N ortvN = new ORTV_N();
            try
            {
                o = orruN.obtenerOrdenDeRuta(DocEntry);
                if (o.DetRRU0 != null)
                {
                    foreach (RRU0_E r0 in o.DetRRU0)
                    {
                        r0.Ticket = ortvN.obtenerTicket(r0.DocEntryTicket);
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
            verificacionAccesos(0);
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
            verificacionAccesos(0);

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
            verificacionAccesos(0);
            return new ActionAsPdf("DocumentoRutas", new { DocNum = DocNum }) { FileName = "rutas.pdf", PageOrientation = Rotativa.Options.Orientation.Landscape, PageSize = Rotativa.Options.Size.A4 };
        }
        public ActionResult infoTicketsPesaje(int idOperation = 237)
        {
            if (verificacionAccesos(idOperation) == "C_Access")
            {
                return View();
            }
            else if (verificacionAccesos(idOperation) == "E_Login")
            { return RedirectToAction("Index", "Index"); }
            else
            { return RedirectToAction("Error", "Index"); }
        }
        public ActionResult RptPesaje(FiltroRptPesaje filtros, int idOperation = 237)
        {
            if (verificacionAccesos(idOperation) == "C_Access")
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
            else if (verificacionAccesos(idOperation) == "E_Login")
            { return RedirectToAction("Index", "Index"); }
            else
            { return RedirectToAction("Error", "Index"); }
        }
        /*public ActionResult infoGuiasTransferenciaTicketsVenta(int DocNum, string TipoRuta, int idOperation = 202)
        {
            if (verificacionAccesos(idOperation) == "C_Access")
            {
                Capa_Negocio.Almacen_NEG.Tablas.OWTR_N owtrN = new Capa_Negocio.Almacen_NEG.Tablas.OWTR_N();
                if (TipoRuta == "VC") { TipoRuta = "01"; }
                else if (TipoRuta == "VA") { TipoRuta = "09"; }

                return Content(owtrN.GuiasTicketTransferencia(DocNum, TipoRuta));
            }
            else
            { return Content(""); }
        }*/
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
        private string verificacionAccesos(int ope)
        {
            string nombreOperacion = this.ControllerContext.RouteData.Values["action"].ToString();
            Usuario_E user = (Usuario_E)Session["UsuarioId"];
            if (user == null)
            { return "E_Login"; }
            else
            {
                if ((rol1.verificarAccesoOperacion(user.IdRol, ope, nombreOperacion, modulo) == 1) || (user.IdRol == 1))
                {
                    Utilitarios_N utiN = new Utilitarios_N();
                    utiN.registrarLog($"{user.Prefijo}{user.Id}", "intento de " + nombreOperacion, ope, Request.UserHostAddress, Request.UserHostName);
                    return "C_Access";
                }
                else
                { return "E_Access"; }
            }
        }

    }
}