using Capa_Entidad.Almacen_ENT.Tablas;
﻿using Capa_Datos;
using Capa_Entidad.Compras_ENT.Tablas;
using Capa_Entidad.DireccionTecnica_ENT.TablasSql;
using Capa_Entidad.General_ENT.Tablas;
using Capa_Entidad.General_ENT.TablasSql;
using Capa_Entidad.ReportesDigemid_ENT;
using Capa_Entidad.ReportesDigemid_ENT.Formularios;
using Capa_Entidad.Seguridad_ENT;
using Capa_Entidad.Ventas_ENT.Tablas;
using Capa_Entidad.Ventas_ENT.TablasSql;
using Capa_Negocio.Almacen_NEG.Tablas;
using Capa_Negocio.DireccionTecnica_NEG.TablasSql;
using Capa_Negocio.General_NEG.Tablas;
using Capa_Negocio.General_NEG.TablasSql;
using Capa_Negocio.ReportesDigemid_NEG;
using Capa_Negocio.Seguridad_NEG;
using Capa_Negocio.Ventas_NEG.Tablas;
using Capa_Negocio.Ventas_NEG.TablasSql;
using DocumentFormat.OpenXml.Math;
using Rotativa;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Services.Description;
using Capa_Negocio.DireccionTecnica_NEG.TablasHANA;

namespace Capa_Usuario.Controllers
{
    public class DireccionTecnicaController : Controller
    {
        Rol1_N rol1 = new Rol1_N(); int modulo = 3;
        DocumentosDig_N dgN = new DocumentosDig_N();
        COB_LUG_ENTREGA_N lugarEntN = new COB_LUG_ENTREGA_N();
        /** 
         * Método para buscar el responsable de almacén de las actas de recepción y despacho 
         * @param {String} tipoFirma - Para saber que tipo de firma estamos buscando 
         * @param {String} almacen
         * @returns {Dictionary} result
         */
        protected Dictionary<string, string> BuscarFirmas(string tipoFirma, string almacen)
        {
            Dictionary<string, string> lista = new Dictionary<string, string>();
            Dictionary<string, string> result = new Dictionary<string, string>();

            if (!string.IsNullOrEmpty(almacen))
            {
                // ResponsableALMActas: ActaRecepcionEm, ActaRecepcionTs
                if (!string.IsNullOrEmpty(tipoFirma) && tipoFirma.Equals("ResponsableALMActas"))
                {
                    lista.Add("00", "");                     // ---- aquí el problema 
                    lista.Add("01", "186");                     // DESPACHO25 - Mireya Roman Silva
                    lista.Add("ALM01", "186");                     // DESPACHO25 - Mireya Roman Silva
                    lista.Add("02", "834");                     // DESPACHO86 - Hemerson Richard Laura Paucar 
                    lista.Add("ALM02", "834");                     // DESPACHO86  - Hemerson Richard Laura Paucar 
                    lista.Add("03", "185");                     // DESPACHO23 - Julio Roman Silva
                    lista.Add("ALM03", "185");                     // DESPACHO23 - Julio Roman Silva
                    lista.Add("05", "697");                     // SALM30 - Jesus Angel Nunahuanca Cordova
                    lista.Add("ALM05", "697");                     // SALM30 - Jesus Angel Nunahuanca Cordova
                    lista.Add("DEV05", "697");              // SALM30 - Jesus Angel Nunahuanca Cordova
                    lista.Add("06", "182");                     // DESPACHO20 - Yasmani Huarachi Mamani
                    lista.Add("ALM06", "182");                     // DESPACHO20 - Yasmani Huarachi Mamani
                    lista.Add("09", "182");                     // DESPACHO20 - Yasmani Huarachi Mamani
                    lista.Add("ALM09", "182");                     // DESPACHO20 - Yasmani Huarachi Mamani
                    lista.Add("ALM07", "161");              // SALM1 - Carmen Condori Saravia
                    lista.Add("CUAR07", "161");             // SALM1 - Carmen Condori Saravia
                    lista.Add("DEV07", "161");              // SALM1 - Carmen Condori Saravia
                }
                else if (!string.IsNullOrEmpty(tipoFirma) && tipoFirma.Equals("QuimicoFarmaceutico"))
                {
                    lista.Add("00", "206");                     // ALM Revalorización - Diana Quiquia Urribarre (DT22)
                    lista.Add("01", "416");                     // Maryori Córdova García (SDT4)
                    lista.Add("ALM01", "416");              // Maryori Córdova García (SDT4)
                    lista.Add("02", "416");                     // Maryori Córdova García (SDT4)
                    lista.Add("ALM02", "416");              // Maryori Córdova García (SDT4)
                    lista.Add("03", "206");                     // María Aguirre Reyes (DT20)
                    lista.Add("ALM03", "206");              // María Aguirre Reyes (DT20)
                    lista.Add("ALM06", "206");              // María Aguirre Reyes (DT20) --- EQUIVALENTE A ALM03
                    lista.Add("04", "208");                     // Diana Quiquia Urribarre (DT22)
                    lista.Add("ALM04", "208");              // Diana Quiquia Urribarre (DT22)
                    lista.Add("05", "197");                     // Evelin Mamani Delgado (DT11)
                    lista.Add("ALM05", "197");              // Evelin Mamani Delgado (DT11)
                    lista.Add("09", "208");                     // 09 es equivalente a ALM06 - Diana Quiquia Urribarre (DT22)
                    lista.Add("ALM09", "208");              // 09 es equivalente a ALM06 - Diana Quiquia Urribarre (DT22)
                    lista.Add("07", "339");                     // Roly Gonzales Romero (DT05)
                    lista.Add("ALM07", "339");              // Roly Gonzales Romero (DT05)
                    lista.Add("CUAR07", "339");         // Roly Gonzales Romero (DT05)
                    lista.Add("DEV07", "339");              // Roly Gonzales Romero (DT05)
                    lista.Add("08", "208");                    // Diana Quiquia Urribarre (DT22)
                    lista.Add("ALM08", "208");             // Diana Quiquia Urribarre (DT22)
                }

                string docEntryUsuario = lista[almacen];

                if (!string.IsNullOrEmpty(docEntryUsuario))
                {
                    string FilePath;
                    Firmas_N firN = new Firmas_N();
                    Firmas_E firE = new Firmas_E()
                    {
                        DocEntryUsuario = Convert.ToInt32(docEntryUsuario)
                    };

                    var firma = firN.ListarFirmas(firE);

                    if (firma != null && firma.Count >= 1)
                    {
                        FilePath = firN.ListarFirmas(firE)[0].RutaFirma;
                        result.Add("NombApe", (firma != null && firma.Count >= 1) ? $"{firma[0].Nombres} {firma[0].Apellidos}" : "");

                        byte[] archivo = System.IO.File.ReadAllBytes(FilePath);
                        var base64 = Convert.ToBase64String(archivo);                                               //La propiedad de tu modelo que es byte[]
                        result.Add("Firma", String.Format("data:image/gif;base64,{0}", base64));       // Damos formato para indicar que se trata de una cadena base64
                    }
                }
            }

            return result;
        }

        public ActionResult LayoutOrdenDeVenta(int DocNum, int idOperation = 1702)
        {
            if (verificacionAccesos(idOperation) == "C_Access")
            {
                return RedirectToAction("OrdenDeVenta", "Ventas", new { DocNum = DocNum });
            }
            else if (verificacionAccesos(idOperation) == "E_Login")
            { return RedirectToAction("Index", "Index"); }
            else
            { return RedirectToAction("Error", "Index"); }
        }
        public ActionResult ListadoFacturasDeVenta(OINV_E fil, int idOperation = 1801)
        {
            COB_LUG_ENTREGA_N cN = new COB_LUG_ENTREGA_N();
            OINV_N oinvN = new OINV_N();
            if (verificacionAccesos(idOperation) == "C_Access")
            {
                if (fil != null) { ViewBag.Oinv = fil; } else { ViewBag.Oinv = new OINV_E(); }
                ViewBag.ListaLugarEntregas = cN.listadoLugaresDeEntrega();
                return View(oinvN.listadoFacturasDeVenta(fil));
            }
            else if (verificacionAccesos(idOperation) == "E_Login")
            { return RedirectToAction("Index", "Index"); }
            else
            { return RedirectToAction("Error", "Index"); }
        }
        public ActionResult ActaRecepcionVt(int DocEntry, int idOperation = 1802)
        {
            if (verificacionAccesos(idOperation) == "C_Access")
            {
                var result = dgN.ConsultarActaRecepcionVt(DocEntry);

                if (result != null && result[0].Almacen != null)
                {
                    var datosFirma = BuscarFirmas("ResponsableALMActas", result[0].Almacen);

                    if (datosFirma != null && datosFirma.Count >= 1)
                    {
                        ViewBag.DatosResponsable = datosFirma["NombApe"];
                        ViewBag.FirmaResponsable = datosFirma["Firma"];
                    }
                }

                return View(result);
            }
            else if (verificacionAccesos(idOperation) == "E_Login")
            { return RedirectToAction("Index", "Index"); }
            else
            { return RedirectToAction("Error", "Index"); }
        }
        public ActionResult ActaDespachoVt(int DocEntry, int idOperation = 1803)
        {
            if (verificacionAccesos(idOperation) == "C_Access")
            {
                var result = dgN.ConsultarActaDespachoVt(DocEntry);

                if (result != null && result[0].T1_WhsCode != null)
                {
                    var datosFirma = BuscarFirmas("ResponsableALMActas", result[0].T1_WhsCode);

                    if (datosFirma != null && datosFirma.Count >= 1)
                    {
                        ViewBag.DatosResponsable = datosFirma["NombApe"];
                        ViewBag.FirmaResponsable = datosFirma["Firma"];
                    }
                }

                return View(result);
            }
            else if (verificacionAccesos(idOperation) == "E_Login")
            { return RedirectToAction("Index", "Index"); }
            else
            { return RedirectToAction("Error", "Index"); }
        }
        public ActionResult OrganolepticoVt(int DocEntry, int idOperation = 1804)
        {
            if (verificacionAccesos(idOperation) == "C_Access")
            {
                var orgVT = dgN.ConsultarOrganolepticoVt(DocEntry);
                if (orgVT != null && orgVT.Count() >= 1)
                {
                    var result = BuscarFirmas("QuimicoFarmaceutico", orgVT[0].Almacen);

                    if (result != null && result.Count >= 1)
                    {
                        ViewBag.QuimicoFarmaceuticoAsistente = result["NombApe"];
                        ViewBag.Firma = result["Firma"];
                    }
                }

                string FilePathDT = "D:\\COBEFARWEBFILES\\Firmas\\FirmaPamelaCollahuaSenosain.png";
                byte[] archivoDT = System.IO.File.ReadAllBytes(FilePathDT);
                var base64DT = Convert.ToBase64String(archivoDT); //La propiedad de tu modelo que es byte[]
                ViewBag.FirmaDT = String.Format("data:image/gif;base64,{0}", base64DT); // Damos formato para indicar que se trata de una cadena base64

                return View(orgVT);
            }
            else if (verificacionAccesos(idOperation) == "E_Login")
            { return RedirectToAction("Index", "Index"); }
            else
            { return RedirectToAction("Error", "Index"); }
        }

        public ActionResult CalcularPdfsDescarga(int idOperation = 1805)
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
            List<(string,int)> resultado = new List<(string, int)>() ; 
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
            if (verificacionAccesos(idOperation) == "C_Access")
            {
                ViewBag.Tipo = Tipo;
                return View(dgN.ConsultarComprobanteDePago(DocEntry));
            }
            else if (verificacionAccesos(idOperation) == "E_Login")
            { return RedirectToAction("Index", "Index"); }
            else
            { return RedirectToAction("Error", "Index"); }
        }
        public ActionResult ListadoBoletasDeVenta(OINV_E fil, int idOperation = 1901)
        {
            Capa_Negocio.General_NEG.Tablas.COB_LUG_ENTREGA_N cN = new Capa_Negocio.General_NEG.Tablas.COB_LUG_ENTREGA_N();
            Capa_Negocio.Ventas_NEG.Tablas.OINV_N oinvN = new Capa_Negocio.Ventas_NEG.Tablas.OINV_N();
            if (verificacionAccesos(idOperation) == "C_Access")
            {
                if (fil != null) { ViewBag.Oinv = fil; } else { ViewBag.Oinv = new OINV_E(); }
                ViewBag.ListaLugarEntregas = cN.listadoLugaresDeEntrega();
                return View(oinvN.listadoBoletasDeVenta(fil));
            }
            else if (verificacionAccesos(idOperation) == "E_Login")
            { return RedirectToAction("Index", "Index"); }
            else
            { return RedirectToAction("Error", "Index"); }
        }
        public ActionResult ListadoEntregasDeVenta(ODLN_E fil, int idOperation = 2001)
        {
            ODLN_N odlnN = new ODLN_N();
            COB_LUG_ENTREGA_N cN = new COB_LUG_ENTREGA_N();
            if (verificacionAccesos(idOperation) == "C_Access")
            {
                if (fil != null) { ViewBag.Odln = fil; } else { ViewBag.Odln = new ODLN_E(); }
                ViewBag.ListaLugarEntregas = cN.listadoLugaresDeEntrega();
                return View(odlnN.listarEntregasVenta(fil));
            }
            else if (verificacionAccesos(idOperation) == "E_Login")
            { return RedirectToAction("Index", "Index"); }
            else
            { return RedirectToAction("Error", "Index"); }
        }
        public ActionResult ListadoNotasDeCreditoVenta(ORIN_E fil, int idOperation = 2101)
        {
            ORIN_N orinN = new ORIN_N();
            if (verificacionAccesos(idOperation) == "C_Access")
            {
                if (fil != null) { ViewBag.Orin = fil; } else { ViewBag.Orin = new ORIN_E(); }
                return View(orinN.listarNotasDeCredito(fil));
            }
            else if (verificacionAccesos(idOperation) == "E_Login")
            { return RedirectToAction("Index", "Index"); }
            else
            { return RedirectToAction("Error", "Index"); }
        }
        public ActionResult NotaDeCreditoVentaArticulos(int DocEntry, int idOperation = 2102)
        {
            if (verificacionAccesos(idOperation) == "C_Access")
            {
                return View(dgN.ConsultarNotaCreditoVentaArticulos(DocEntry));
            }
            else if (verificacionAccesos(idOperation) == "E_Login")
            { return RedirectToAction("Index", "Index"); }
            else
            { return RedirectToAction("Error", "Index"); }
        }
        public ActionResult ListadoTransferenciasDeStock(OWTR_E fil, int idOperation = 2201)
        {
            Capa_Negocio.Almacen_NEG.Tablas.OWTR_N owN = new Capa_Negocio.Almacen_NEG.Tablas.OWTR_N();
            Capa_Negocio.General_NEG.Tablas.OSLP_N osN = new Capa_Negocio.General_NEG.Tablas.OSLP_N();
            Capa_Negocio.General_NEG.Tablas.OWHS_N owhN = new Capa_Negocio.General_NEG.Tablas.OWHS_N();
            if (verificacionAccesos(idOperation) == "C_Access")
            {
                if (fil != null) { ViewBag.Owtr = fil; } else { ViewBag.Owtr = new OWTR_E(); }
                ViewBag.Almacenes = owhN.ListarAlmacenes();
                ViewBag.ListaOslp = osN.listadoOslp("ALM");
                return View(owN.listadoTransferenciasStock(fil));
            }
            else if (verificacionAccesos(idOperation) == "E_Login")
            { return RedirectToAction("Index", "Index"); }
            else
            { return RedirectToAction("Error", "Index"); }
        }
        public JsonResult CambiarEstadoOWTR(SQL_OWTR_E datos)
        {
            verificacionAccesos(0);         // Validar sesion logueada, solo para ajax
            if (datos.DocNumSAP >= 1 && !string.IsNullOrEmpty(datos.Estado))
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
            if (verificacionAccesos(idOperation) == "C_Access")
            {
                var result = dgN.ConsultarActaRecepcionTs(DocEntry);

                if (result != null && result[0].AlmacenDestino != null)
                {
                    var datosFirma = BuscarFirmas("ResponsableALMActas", result[0].AlmacenDestino);

                    if (datosFirma != null && datosFirma.Count >= 1)
                    {
                        ViewBag.DatosResponsable = datosFirma["NombApe"];
                        ViewBag.FirmaResponsable = datosFirma["Firma"];
                    }
                }

                return View(result);
            }
            else if (verificacionAccesos(idOperation) == "E_Login")
            { return RedirectToAction("Index", "Index"); }
            else
            { return RedirectToAction("Error", "Index"); }
        }
        public ActionResult ActaDespachoTs(int DocEntry, int idOperation = 2203)
        {
            if (verificacionAccesos(idOperation) == "C_Access")
            {
                var result = dgN.ConsultarActaDespachoTs(DocEntry);

                if (result != null && result[0].AlmDestino != null)
                {
                    var datosFirma = BuscarFirmas("ResponsableALMActas", result[0].AlmOrigen);

                    if (datosFirma != null && datosFirma.Count >= 1)
                    {
                        ViewBag.DatosResponsable = datosFirma["NombApe"];
                        ViewBag.FirmaResponsable = datosFirma["Firma"];
                    }
                }

                return View(result);
            }
            else if (verificacionAccesos(idOperation) == "E_Login")
            { return RedirectToAction("Index", "Index"); }
            else
            { return RedirectToAction("Error", "Index"); }
        }
        public ActionResult OrganolepticoTs(int DocEntry, int idOperation = 2204)
        {
            if (verificacionAccesos(idOperation) == "C_Access")
            {
                var orgTS = dgN.ConsultarOrganolepticoTs(DocEntry);

                if (orgTS != null && orgTS.Count() >= 1)
                {
                    var result = BuscarFirmas("QuimicoFarmaceutico", orgTS[0].AlmacenDestino);

                    if (result != null && result.Count >= 1)
                    {
                        ViewBag.QuimicoFarmaceuticoAsistente = result["NombApe"];
                        ViewBag.Firma = result["Firma"];
                    }
                }

                string FilePathDT = "D:\\COBEFARWEBFILES\\Firmas\\FirmaPamelaCollahuaSenosain.png";
                byte[] archivoDT = System.IO.File.ReadAllBytes(FilePathDT);
                var base64DT = Convert.ToBase64String(archivoDT); //La propiedad de tu modelo que es byte[]
                ViewBag.FirmaDT = String.Format("data:image/gif;base64,{0}", base64DT); // Damos formato para indicar que se trata de una cadena base64

                return View(orgTS);
            }
            else if (verificacionAccesos(idOperation) == "E_Login")
            { return RedirectToAction("Index", "Index"); }
            else
            { return RedirectToAction("Error", "Index"); }
        }
        public ActionResult ListadoEntradasDeMercancias(Capa_Entidad.Compras_ENT.Tablas.OPDN_E fil, int idOperation = 2301)
        {
            Capa_Negocio.General_NEG.Tablas.OWHS_N owhN = new Capa_Negocio.General_NEG.Tablas.OWHS_N();
            Capa_Negocio.Compras_NEG.Tablas.OPDN_N opdnN = new Capa_Negocio.Compras_NEG.Tablas.OPDN_N();
            if (verificacionAccesos(idOperation) == "C_Access")
            {
                ViewBag.Almacenes = owhN.ListarAlmacenes("todos");
                if (fil != null) { ViewBag.Opdn = fil; } else { ViewBag.Opdn = new Capa_Entidad.Compras_ENT.Tablas.OPDN_E(); }
                return View(opdnN.listadoEntradaMercancias(fil));
            }
            else if (verificacionAccesos(idOperation) == "E_Login")
            { return RedirectToAction("Index", "Index"); }
            else
            { return RedirectToAction("Error", "Index"); }
        }

        public ActionResult ActaRecepcionEm(int DocEntry, string Almacen, int idOperation = 2302)
        {
            if (verificacionAccesos(idOperation) == "C_Access")
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
            else if (verificacionAccesos(idOperation) == "E_Login")
            { return RedirectToAction("Index", "Index"); }
            else
            { return RedirectToAction("Error", "Index"); }
        }
        public ActionResult OrganolepticoEm(int DocEntry, string Almacen, int idOperation = 2303)
        {
            if (verificacionAccesos(idOperation) == "C_Access")
            {
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

                string FilePathDT = "D:\\COBEFARWEBFILES\\Firmas\\FirmaPamelaCollahuaSenosain.png";
                byte[] archivoDT = System.IO.File.ReadAllBytes(FilePathDT);
                var base64DT = Convert.ToBase64String(archivoDT); //La propiedad de tu modelo que es byte[]
                ViewBag.FirmaDT = String.Format("data:image/gif;base64,{0}", base64DT); // Damos formato para indicar que se trata de una cadena base64

                return View(orgEM);
            }
            else if (verificacionAccesos(idOperation) == "E_Login")
            { return RedirectToAction("Index", "Index"); }
            else
            { return RedirectToAction("Error", "Index"); }
        }
        public ActionResult RealizarEntradaDeMercancias(int DocEntry, int idOperation = 2304)
        {
            Capa_Negocio.Compras_NEG.Tablas.OPDN_N opdnN = new Capa_Negocio.Compras_NEG.Tablas.OPDN_N();
            if (verificacionAccesos(idOperation) == "C_Access")
            {
                return View(opdnN.buscarEntradaMercancias(DocEntry));
            }
            else if (verificacionAccesos(idOperation) == "E_Login")
            { return RedirectToAction("Index", "Index"); }
            else
            { return RedirectToAction("Error", "Index"); }
        }
        [HttpPost]
        public ActionResult RealizarEntradaDeMercancias(SQL_OPDN_E s, int idOperation = 2304)
        {
            Capa_Negocio.Compras_NEG.Tablas.SQL_OPDN_N SqlOpdnN = new Capa_Negocio.Compras_NEG.Tablas.SQL_OPDN_N();
            Capa_Negocio.Compras_NEG.Tablas.OPDN_N opdnN = new Capa_Negocio.Compras_NEG.Tablas.OPDN_N();
            if (verificacionAccesos(idOperation) == "C_Access")
            {
                Usuario_E user = (Usuario_E)Session["UsuarioId"];
                s.OpRealizacion = $"{user.Nombres} {user.Apellidos}";
                s.ObjType = 20;
                try
                {
                    SqlOpdnN.realizarSqlEntradaDeMercancias(s);
                    ViewBag.Mensaje = s.DocEntry;
                    return RedirectToAction("ListadoEntradasDeMercancias");
                }
                catch (Exception e) { ViewBag.Mensaje = e.Message; return View(opdnN.buscarEntradaMercancias(s.DocEntry)); }
            }
            else if (verificacionAccesos(idOperation) == "E_Login")
            { return RedirectToAction("Index", "Index"); }
            else
            { return RedirectToAction("Error", "Index"); }
        }
        public ActionResult AnularRealizarEntradaDeMercancias(int DocEntry, int idOperation = 2305)
        {
            Capa_Negocio.Compras_NEG.Tablas.OPDN_N opdnN = new Capa_Negocio.Compras_NEG.Tablas.OPDN_N();
            if (verificacionAccesos(idOperation) == "C_Access")
            {
                return View(opdnN.buscarEntradaMercancias(DocEntry));
            }
            else if (verificacionAccesos(idOperation) == "E_Login")
            { return RedirectToAction("Index", "Index"); }
            else
            { return RedirectToAction("Error", "Index"); }
        }
        [ActionName("AnularRealizarEntradaDeMercancias")]
        [HttpPost]
        public ActionResult AnularRealizarEntradaDeMercanciasPost(int DocEntry, int idOperation = 2305)
        {
            Capa_Negocio.Compras_NEG.Tablas.SQL_OPDN_N SqlOpdnN = new Capa_Negocio.Compras_NEG.Tablas.SQL_OPDN_N();
            Capa_Negocio.Compras_NEG.Tablas.OPDN_N opdnN = new Capa_Negocio.Compras_NEG.Tablas.OPDN_N();
            if (verificacionAccesos(idOperation) == "C_Access")
            {
                try
                {
                    SqlOpdnN.eliminarSqlEntradaDeMercancias(DocEntry);
                    ViewBag.Mensaje = DocEntry;
                    return RedirectToAction("ListadoEntradasDeMercancias");
                }
                catch (Exception e) { ViewBag.Mensaje = e.Message; return View(opdnN.buscarEntradaMercancias(DocEntry)); }
            }
            else if (verificacionAccesos(idOperation) == "E_Login")
            { return RedirectToAction("Index", "Index"); }
            else
            { return RedirectToAction("Error", "Index"); }
        }
        public ActionResult ListadoFacturasDeProveedores(OPCH_E fil, int idOperation = 2401)
        {
            Capa_Negocio.Compras_NEG.Tablas.OPCH_N opchN = new Capa_Negocio.Compras_NEG.Tablas.OPCH_N();
            if (verificacionAccesos(idOperation) == "C_Access")
            {
                if (fil != null) { ViewBag.Opch = fil; } else { ViewBag.Opch = new OPCH_E(); }
                return View(opchN.listadoFacturasProveedores(fil));
            }
            else if (verificacionAccesos(idOperation) == "E_Login")
            { return RedirectToAction("Index", "Index"); }
            else
            { return RedirectToAction("Error", "Index"); }
        }
        public ActionResult FacturaDeProveedor(int DocEntry, int idOperation = 2402)
        {
            Capa_Negocio.Compras_NEG.Tablas.OPCH_N opchN = new Capa_Negocio.Compras_NEG.Tablas.OPCH_N();
            if (verificacionAccesos(idOperation) == "C_Access")
            {
                return View(opchN.buscarFacturaProveedor(DocEntry));
            }
            else if (verificacionAccesos(idOperation) == "E_Login")
            { return RedirectToAction("Index", "Index"); }
            else
            { return RedirectToAction("Error", "Index"); }
        }
        public ActionResult ListadoMaestroDeArticulos(OITM_E fil, int idOperation = 2501)
        {
            Capa_Negocio.Almacen_NEG.Tablas.OITM_N oitmN = new Capa_Negocio.Almacen_NEG.Tablas.OITM_N();
            if (verificacionAccesos(idOperation) == "C_Access")
            {
                if (fil != null) { ViewBag.fil = fil; } else { ViewBag.fil = new OITM_E(); }
                return View(oitmN.Listar(fil));
            }
            else if (verificacionAccesos(idOperation) == "E_Login")
            { return RedirectToAction("Index", "Index"); }
            else
            { return RedirectToAction("Error", "Index"); }
        }
        public ActionResult DetallesArticulo(string ItemCode, int idOperation = 2502)
        {
            Capa_Negocio.Almacen_NEG.Tablas.OITM_N oitmN = new Capa_Negocio.Almacen_NEG.Tablas.OITM_N();
            Capa_Negocio.Almacen_NEG.Tablas.OITW_N oitwN = new Capa_Negocio.Almacen_NEG.Tablas.OITW_N();
            Capa_Negocio.General_NEG.Tablas.OWHS_N owhsN = new Capa_Negocio.General_NEG.Tablas.OWHS_N();
            if (verificacionAccesos(idOperation) == "C_Access")
            {
                ViewBag.Owhs = owhsN;
                ViewBag.listaOitw = oitwN.listarDetArticulosInv(ItemCode);
                return View(oitmN.buscarArticulo(ItemCode));
            }
            else if (verificacionAccesos(idOperation) == "E_Login")
            { return RedirectToAction("Index", "Index"); }
            else
            { return RedirectToAction("Error", "Index"); }
        }
        public ActionResult Reportes(int idOperation = 2601)
        {
            if (verificacionAccesos(idOperation) == "C_Access")
            {
                OMRC_N omrcN = new OMRC_N();
                ViewBag.Laboratorios = omrcN.listarFabricantes();

                return View();
            }
            else if (verificacionAccesos(idOperation) == "E_Login")
            { return RedirectToAction("Index", "Index"); }
            else
            { return RedirectToAction("Error", "Index"); }
        }
        //aciones dentRo de reportes
        public ActionResult ListadoSaldosAnteriores(COB_SALDO_E fil, string Mensaje = "", int idOperation = 2602)
        {
            Capa_Negocio.General_NEG.Tablas.COB_SALDO_N cobN = new Capa_Negocio.General_NEG.Tablas.COB_SALDO_N();
            if (verificacionAccesos(idOperation) == "C_Access")
            {
                if (fil == null) { ViewBag.fil = new COB_SALDO_E(); } else { ViewBag.fil = fil; }
                ViewBag.Mensaje = Mensaje;
                return View(cobN.listarSaldosAnteriores(fil));
            }
            else if (verificacionAccesos(idOperation) == "E_Login")
            { return RedirectToAction("Index", "Index"); }
            else
            { return RedirectToAction("Error", "Index"); }
        }
        public ActionResult AgregarSaldoAnterior(int idOperation = 2603)
        {
            Capa_Negocio.Almacen_NEG.Tablas.OITM_N oitmN = new Capa_Negocio.Almacen_NEG.Tablas.OITM_N();
            if (verificacionAccesos(idOperation) == "C_Access")
            {
                OITM_E o = new OITM_E() { ItmsGrpCod = 103 };
                ViewBag.ListaArticulos = oitmN.Listar(o);
                ViewBag.Mensaje = "";
                return View(new COB_SALDO_E());
            }
            else if (verificacionAccesos(idOperation) == "E_Login")
            { return RedirectToAction("Index", "Index"); }
            else
            { return RedirectToAction("Error", "Index"); }
        }
        [HttpPost]
        public ActionResult AgregarSaldoAnterior(COB_SALDO_E c, int idOperation = 2603)
        {
            Capa_Negocio.Almacen_NEG.Tablas.OITM_N oitmN = new Capa_Negocio.Almacen_NEG.Tablas.OITM_N();
            Capa_Negocio.General_NEG.Tablas.COB_SALDO_N cobN = new Capa_Negocio.General_NEG.Tablas.COB_SALDO_N();
            if (verificacionAccesos(idOperation) == "C_Access")
            {
                try
                {
                    cobN.agregarSaldoAnterior(c);
                    return RedirectToAction("ListadoSaldosAnteriores", new { Mensaje = c.Name + " Agregado correctamente" });
                }
                catch (Exception e)
                {
                    ViewBag.Mensaje = e.Message;
                    OITM_E o = new OITM_E() { ItmsGrpCod = 103 };
                    ViewBag.ListaArticulos = oitmN.Listar(o);
                    return View(c);
                }
            }
            else if (verificacionAccesos(idOperation) == "E_Login")
            { return RedirectToAction("Index", "Index"); }
            else
            { return RedirectToAction("Error", "Index"); }
        }

        public ActionResult EditarSaldoAnterior(string Code, int idOperation = 2604)
        {
            Capa_Negocio.Almacen_NEG.Tablas.OITM_N oitmN = new Capa_Negocio.Almacen_NEG.Tablas.OITM_N();
            Capa_Negocio.General_NEG.Tablas.COB_SALDO_N cobN = new Capa_Negocio.General_NEG.Tablas.COB_SALDO_N();
            if (verificacionAccesos(idOperation) == "C_Access")
            {
                OITM_E o = new OITM_E() { ItmsGrpCod = 103 };
                ViewBag.ListaArticulos = oitmN.Listar(o);
                ViewBag.Mensaje = "";
                return View(cobN.buscarSaldoAnterior(Code));
            }
            else if (verificacionAccesos(idOperation) == "E_Login")
            { return RedirectToAction("Index", "Index"); }
            else
            { return RedirectToAction("Error", "Index"); }
        }
        [HttpPost]
        public ActionResult EditarSaldoAnterior(COB_SALDO_E c, int idOperation = 2604)
        {
            Capa_Negocio.Almacen_NEG.Tablas.OITM_N oitmN = new Capa_Negocio.Almacen_NEG.Tablas.OITM_N();
            Capa_Negocio.General_NEG.Tablas.COB_SALDO_N cobN = new Capa_Negocio.General_NEG.Tablas.COB_SALDO_N();
            if (verificacionAccesos(idOperation) == "C_Access")
            {
                try
                {
                    cobN.editarSaldoAnterior(c);
                    return RedirectToAction("ListadoSaldosAnteriores", new { Mensaje = c.Name + " Editado correctamente" });
                }
                catch (Exception e)
                {
                    OITM_E o = new OITM_E() { ItmsGrpCod = 103 };
                    ViewBag.ListaArticulos = oitmN.Listar(o);
                    ViewBag.Mensaje = e.Message;
                    return View(c);
                }
            }
            else if (verificacionAccesos(idOperation) == "E_Login")
            { return RedirectToAction("Index", "Index"); }
            else
            { return RedirectToAction("Error", "Index"); }
        }
        public ActionResult EliminarSaldoAnterior(string Code, int idOperation = 2605)
        {
            Capa_Negocio.General_NEG.Tablas.COB_SALDO_N cobN = new Capa_Negocio.General_NEG.Tablas.COB_SALDO_N();
            if (verificacionAccesos(idOperation) == "C_Access")
            {
                return View(cobN.buscarSaldoAnterior(Code));
            }
            else if (verificacionAccesos(idOperation) == "E_Login")
            { return RedirectToAction("Index", "Index"); }
            else
            { return RedirectToAction("Error", "Index"); }
        }
        [ActionName("EliminarSaldoAnterior")]
        [HttpPost]
        public ActionResult EliminarSaldoAnteriorPost(string Code, int idOperation = 2605)
        {
            Capa_Negocio.General_NEG.Tablas.COB_SALDO_N cobN = new Capa_Negocio.General_NEG.Tablas.COB_SALDO_N();
            if (verificacionAccesos(idOperation) == "C_Access")
            {
                try
                {
                    cobN.eliminarSaldoAnterior(Code);
                    return RedirectToAction("ListadoSaldosAnteriores", new { Mensaje = "Eliminado correctamente" });
                }
                catch (Exception e) { ViewBag.Mensaje = e.Message; return View(cobN.buscarSaldoAnterior(Code)); }
            }
            else if (verificacionAccesos(idOperation) == "E_Login")
            { return RedirectToAction("Index", "Index"); }
            else
            { return RedirectToAction("Error", "Index"); }
        }

        /********************* R E G I S T R O S   S A N I T A R I O S *********************/
        public ActionResult RegistrosSanitarios(int idOperation = 2900)
        {
            if (verificacionAccesos(idOperation) == "C_Access")
            { return View(); }
            else if (verificacionAccesos(idOperation) == "E_Login")
            { return RedirectToAction("Index", "Index"); }
            else
            { return RedirectToAction("Error", "Index"); }
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
            verificacionAccesos(0);     // Validar sesion logueada, solo para ajax
            Usuario_E usu = (Usuario_E)Session["UsuarioId"];
            rs.RegistradoPor = $"{usu.Nombres} {usu.Apellidos}";

            OORS_N orsN = new OORS_N();
            var result = orsN.RegistrarObservacion(rs);

            return Json(new { Mensaje = result });
        }

        public JsonResult ConsultarRegistrosSanitariosExpirados()
        {
            verificacionAccesos(0);         // Validar sesion logueada, solo para ajax

            OORS_N orsN = new OORS_N();
            var result = orsN.ConsultarRegistrosSanitariosExpirados();

            return Json(new { Datos = result });
        }
        /************************  Ó R D E N E S   D E   V E N T A ************************/
        public ActionResult ListadoOrdenesDeVenta(Capa_Entidad.Ventas_ENT.Tablas.ORDR_E filtros, int idOperation = 1701)
        {
            switch (verificacionAccesos(idOperation))
            {
                case "C_Access":
                    var usuario = Session["UsuarioId"] as Usuario_E;
                    int idRol = usuario?.IdRol ?? 0;

                    // VENTAS o SVENTAS
                    bool versionVentas = new List<int> { 6, 7 }.Contains(idRol);
                    ViewBag.CargarLista = versionVentas == true ? "ListadoOrdenesVenta_VT" : "ListadoOrdenesVenta_DT";
                    ViewBag.SlpCode = filtros?.SlpCode ?? 0;

                    return View();

                case "E_Login":
                    return RedirectToAction("Index", "Index");

                default:
                    return RedirectToAction("Error", "Index");
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
            var lugaresEntregas = new Capa_Negocio.General_NEG.TablasSql.OWHS_N().listarAlmacenes(new[] { "01", "03", "09", "ALM07","07"});
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
        /************************************************************************************/
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
                    Capa_Negocio.Utilitarios_N utiN = new Capa_Negocio.Utilitarios_N();
                    utiN.registrarLog(user.Prefijo + user.Id, "intento de " + nombreOperacion, ope, Request.UserHostAddress, Request.UserHostName);
                    return "C_Access";
                }
                else
                { return "E_Access"; }
            }
        }
    }
}