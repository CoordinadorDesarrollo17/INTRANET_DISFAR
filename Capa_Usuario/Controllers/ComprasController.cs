using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Capa_Negocio.Compras_NEG;
using Capa_Entidad.Seguridad_ENT;
using Capa_Entidad.Compras_ENT;
using Capa_Entidad.Compras_ENT.Reportes;
using Capa_Negocio.Seguridad_NEG;
using Capa_Negocio.SocioNegocios_NEG.Tablas;
using Capa_Negocio.Compras_NEG.TablasSql;
namespace Capa_Usuario.Controllers
{
    public class ComprasController : Controller
    {
        Rol1_N rol1 = new Rol1_N(); int modulo = 5;
        LineaProduccion_N lpN = new LineaProduccion_N();
        ContratoRebate_N crN = new ContratoRebate_N();
        //******acciones*****
        //------acciones lineas de produccion
        public ActionResult ListadoLineasProduccion(LineaProduccion_E lf = null, int idOperation = 1101, string Mensaje = "")
        {
            if (VerificacionAccesos(idOperation) == "C_Access")
            {
                ViewBag.Mensaje = Mensaje;
                ViewBag.Linea = lf;
                return View(lpN.listarLineasProduccion(lf));
            }
            else if (VerificacionAccesos(idOperation) == "E_Login")
            { return RedirectToAction("Index", "Index"); }
            else
            { return RedirectToAction("Error", "Index"); }
        }
        public ActionResult CrearLineaProduccion(int idOperation = 1102)
        {
            if (VerificacionAccesos(idOperation) == "C_Access")
            {
                ViewBag.ListaProveedores = lpN.listarProveedores();
                ViewBag.ListaFabricantes = lpN.listarFabricantes();
                ViewBag.MensajeProd = "Ningun producto encontrado";
                ViewBag.Mensaje = "";
                return View(new LineaProduccion_E());
            }
            else if (VerificacionAccesos(idOperation) == "E_Login")
            { return RedirectToAction("Index", "Index"); }
            else
            { return RedirectToAction("Error", "Index"); }
        }
        [HttpPost]
        public ActionResult CrearLineaProduccion(LineaProduccion_E l, int idOperation = 1102)
        {
            if (VerificacionAccesos(idOperation) == "C_Access")
            {
                try
                {
                    if (ModelState.IsValid)
                    {
                        lpN.CrearLineaProduccion(l);
                        return RedirectToAction("ListadoLineasProduccion");
                    }
                    ViewBag.ListaProveedores = lpN.listarProveedores();
                    ViewBag.ListaFabricantes = lpN.listarFabricantes();
                    ViewBag.Mensaje = "";
                    return View(l);
                }
                catch (Exception e) { ViewBag.Mensaje = e.Message; ViewBag.ListaFabricantes = lpN.listarFabricantes();
                    ViewBag.ListaProveedores = lpN.listarProveedores(); return View(); }
            }
            else if (VerificacionAccesos(idOperation) == "E_Login")
            { return RedirectToAction("Index", "Index"); }
            else
            { return RedirectToAction("Error", "Index"); }
        }
        public ActionResult EditarLineaProduccion(int id, int idOperation = 1103)
        {
            if (VerificacionAccesos(idOperation) == "C_Access")
            {
                ViewBag.ListaProveedores = lpN.listarProveedores();
                ViewBag.ListaFabricantes = lpN.listarFabricantes();
                ViewBag.MensajeProd = "Ningun producto encontrado";
                ViewBag.Mensaje = "";
                return View(lpN.obtenerLineaProduccion(id));
            }
            else if (VerificacionAccesos(idOperation) == "E_Login")
            { return RedirectToAction("Index", "Index"); }
            else
            { return RedirectToAction("Error", "Index"); }
        }
        [HttpPost]
        public ActionResult EditarLineaProduccion(LineaProduccion_E l, int idOperation = 1103)
        {
            if (VerificacionAccesos(idOperation) == "C_Access")
            {
                try
                {
                    int id = lpN.EditarLineaProduccion(l);
                    return RedirectToAction("ListadoLineasProduccion");
                }
                catch (Exception e) {
                    ViewBag.ListaProveedores = lpN.listarProveedores();
                    ViewBag.ListaFabricantes = lpN.listarFabricantes();
                    ViewBag.MensajeProd = "Ningun producto encontrado";
                    ViewBag.Mensaje = e.Message;
                    return View(l); }
            }
            else if ( VerificacionAccesos(idOperation) == "E_Login")
            { return RedirectToAction("Index", "Index"); }
            else
            { return RedirectToAction("Error", "Index"); }
        }
        public ActionResult EliminarLineaProduccion(int id, int idOperation = 1104)
        {
            if (VerificacionAccesos(idOperation) == "C_Access")
            {
                return View(lpN.obtenerLineaProduccion(id));
            }
            else if (VerificacionAccesos(idOperation) == "E_Login")
            { return RedirectToAction("Index", "Index"); }
            else
            { return RedirectToAction("Error", "Index"); }
        }
        [HttpPost]
        [ActionName("EliminarLineaProduccion")]
        public ActionResult EliminarLineaProduccionPost(int id, int idOperation = 1104)
        {
            if (VerificacionAccesos(idOperation) == "C_Access")
            {
                try
                {
                    lpN.EliminarLineaProduccion(id);
                    ViewBag.Mensaje = "Se ha elimindao correctamente";
                    return RedirectToAction("ListadoLineasProduccion", new { Mensaje = ViewBag.Mensaje });
                }
                catch
                {
                    ViewBag.Mensaje = " No pudo Eliminar porque un contrato esta usandolo";
                    return RedirectToAction("ListadoLineasProduccion", new { Mensaje = ViewBag.Mensaje });
                }
            }
            else if (VerificacionAccesos(idOperation) == "E_Login")
            { return RedirectToAction("Index", "Index"); }
            else
            { return RedirectToAction("Error", "Index"); }
        }
        //-------acciones contratos rebate
        public ActionResult ListadoContratosRebate(ContratoRebate_E c = null, int idOperation = 1201, string Mensaje = "")
        {
            if (VerificacionAccesos(idOperation) == "C_Access")
            {
                ViewBag.Mensaje = Mensaje;
                ViewBag.Contrato = c;
                return View(crN.listarContratosRebate(c));
            }
            else if (VerificacionAccesos(idOperation) == "E_Login")
            { return RedirectToAction("Index", "Index"); }
            else
            { return RedirectToAction("Error", "Index"); }
        }
        public ActionResult CrearContratoRebate(int idOperation = 1202)
        {
            if (VerificacionAccesos(idOperation) == "C_Access")
            {
                ViewBag.Mensaje = "";
                return View(new ContratoRebate_E());
            }
            else if (VerificacionAccesos(idOperation) == "E_Login")
            { return RedirectToAction("Index", "Index"); }
            else
            { return RedirectToAction("Error", "Index"); }
        }
        [HttpPost]
        public ActionResult CrearContratoRebate(ContratoRebate_E c, int idOperation = 1202)
        {
            if (VerificacionAccesos(idOperation) == "C_Access")
            {
                try
                {
                    int id = crN.CrearContratoRebate(c);
                    return RedirectToAction("ListadoContratosRebate");
                }
                catch (Exception e) { ViewBag.Mensaje = e.Message; return View(c); }
            }
            else if (VerificacionAccesos(idOperation) == "E_Login")
            { return RedirectToAction("Index", "Index"); }
            else
            { return RedirectToAction("Error", "Index"); }
        }
        public ActionResult GenerarCuadreContrato(int id, int idOperation = 1203)
        {
            if (VerificacionAccesos(idOperation) == "C_Access")
            {
                return View(crN.GenerarCuadreContrato(id));
            }
            else if (VerificacionAccesos(idOperation) == "E_Login")
            { return RedirectToAction("Index", "Index"); }
            else
            { return RedirectToAction("Error", "Index"); }
        }
        public ActionResult DetallesContratoRebate(int id, int idOperation = 1204)
        {
            if (VerificacionAccesos(idOperation) == "C_Access")
            {
                return View(crN.obtenerContratoRebate(id));
            }
            else if (VerificacionAccesos(idOperation) == "E_Login")
            { return RedirectToAction("Index", "Index"); }
            else
            { return RedirectToAction("Error", "Index"); }
        }
        public ActionResult EliminarContratoRebate(int id, int idOperation = 1205)
        {
            if (VerificacionAccesos(idOperation) == "C_Access")
            {
                return View(crN.obtenerContratoRebate(id));
            }
            else if (VerificacionAccesos(idOperation) == "E_Login")
            { return RedirectToAction("Index", "Index"); }
            else
            { return RedirectToAction("Error", "Index"); }
        }
        [HttpPost]
        [ActionName("EliminarContratoRebate")]
        public ActionResult EliminarContratoRebatePost(int id, int idOperation = 1205)
        {
            if (VerificacionAccesos(idOperation) == "C_Access")
            {
                try
                {
                    crN.EliminarContratoRebate(id);
                    return RedirectToAction("ListadoContratosRebate", new { Mensaje = "Eliminado Correctamente" });
                }
                catch (Exception e)
                {
                    return RedirectToAction("ListadoContratosRebate", new { Mensaje = "No se pudo eliminar" + e.Message });
                }
            }
            else if (VerificacionAccesos(idOperation) == "E_Login")
            { return RedirectToAction("Index", "Index"); }
            else
            { return RedirectToAction("Error", "Index"); }
        }
        public ActionResult AgregarProveedorListaNegra(string Mensaje = null, int idOperation = 1206)
        {
            if (VerificacionAccesos(idOperation) == "C_Access")
            {
                OCRD_N ocrdN = new OCRD_N();
                LNPV_N lnpvN = new LNPV_N();
                ViewBag.Mensaje = Mensaje;
                ViewBag.ListaProveedores = 
                        ocrdN.listarSociosDeNegocios(new Capa_Entidad.SocioNegocios_ENT.Tablas.OCRD_E { CardType="S"});
                return View(lnpvN.listarProveedoresListaNegra());
            }
            else if (VerificacionAccesos(idOperation) == "E_Login")
            { return RedirectToAction("Index", "Index"); }
            else
            { return RedirectToAction("Error", "Index"); }
        }
        [HttpPost]
        public ActionResult AgregarProveedorListaNegra(ProveedorListaNegra_E p, int idOperation = 1206)
        {
            if (VerificacionAccesos(idOperation) == "C_Access")
            {
                LNPV_N lnpvN = new LNPV_N();
                OCRD_N ocrdN = new OCRD_N();
                try
                {
                    lnpvN.agregarProveedorListaNegra(p);
                    ViewBag.Mensaje = "Agregado Correctamente";
                } catch (Exception e)
                {
                    ViewBag.Mensaje = "Error: " + e.Message;
                }
                ViewBag.ListaProveedores = 
                        ocrdN.listarSociosDeNegocios(new Capa_Entidad.SocioNegocios_ENT.Tablas.OCRD_E { CardType="S"});
                return View(lnpvN.listarProveedoresListaNegra());
            }
            else if (VerificacionAccesos(idOperation) == "E_Login")
            { return RedirectToAction("Index", "Index"); }
            else
            { return RedirectToAction("Error", "Index"); }
        }
        public ActionResult RetirarProveedorListaNegra(string CardCode, int idOperation = 1207)
        {
            if (VerificacionAccesos(idOperation) == "C_Access")
            {
                LNPV_N lnpvN = new LNPV_N();
                try
                {
                    lnpvN.retirarProveedorListaNegra(CardCode);
                    return RedirectToAction("AgregarProveedorListaNegra", new { Mensaje = "Retirado Correctamente" });
                } catch (Exception e)
                {
                    return RedirectToAction("AgregarProveedorListaNegra", new { Mensaje = "Error: " + e.Message });
                }
            }
            else if (VerificacionAccesos(idOperation) == "E_Login")
            { return RedirectToAction("Index", "Index"); }
            else
            { return RedirectToAction("Error", "Index"); }
        }
        public ActionResult ConfirmarContratoRebate(int id,int idOperation=1208)
        {
            if (VerificacionAccesos(idOperation) == "C_Access")
            {
                return View(crN.obtenerContratoRebate(id));   
            }
            else if (VerificacionAccesos(idOperation) == "E_Login")
            { return RedirectToAction("Index", "Index"); }
            else
            { return RedirectToAction("Error", "Index"); }
        }
        [HttpPost]
        public ActionResult ConfirmarContratoRebate(ContratoRebate_E c,int idOperation = 1208)
        {
            if (VerificacionAccesos(idOperation) == "C_Access")
            {
                try
                {
                    crN.EditarContratoRebate("UC", c);
                    ViewBag.Mensaje = "Confirmado Correctamente";
                    return RedirectToAction("ListadoContratosRebate");
                }catch(Exception e) { ViewBag.Mensaje=e.Message; return View(c); }
            }
            else if (VerificacionAccesos(idOperation) == "E_Login")
            { return RedirectToAction("Index", "Index"); }
            else
            { return RedirectToAction("Error", "Index"); }
        }
        public ActionResult ResumenRebate(DetCuadreContrato_E filtro=null,int año=0,int idOperation=1401)
        {
            if (VerificacionAccesos(idOperation) == "C_Access")
            {
                if (año == 0) { año = DateTime.Now.Year; }
                ViewBag.año = año;
                if(filtro!=null)
                {
                    ViewBag.Filtro = filtro;
                }
                else { ViewBag.Filtro = new DetCuadreContrato_E(); }
                List<ResumenRebate_E> lista = crN.listarResumenCuadreContrato(filtro,año);
                return View(lista);
            }
            else if (VerificacionAccesos(idOperation) == "E_Login")
            { return RedirectToAction("Index", "Index"); }
            else
            { return RedirectToAction("Error", "Index"); }
        }
        //***********infos**** sin operation 
        //--- contrato rebate info
        public ActionResult infoListarSocios(string Tipo)
        {
            return Content(crN.infoListarSocios(Tipo));
        }
        public ActionResult infoListarDetallesContratoRebate(ContratoRebate_E c)
        {
            return Content(crN.infoListarDetallesContratoRebate(c));
        }
        public ActionResult infoListarEspDetCR2(string PeriodoRebate,int Linea,string SubTipo ,string CardCode,int idLp,string FecIni,string FecFin)
        {
            return Content(crN.infoListarEspDetCR2(PeriodoRebate,Linea,SubTipo, CardCode,idLp, FecIni, FecFin));
        }
            //---lineas de produccion info
        public JsonResult ListarArticulos(int idLabVal, string idTipoVal)
        {
            return Json(lpN.listarArticulos(idLabVal, idTipoVal));
        }
        public ActionResult infoListarArticulos(int idLabVal, string idTipoVal)
        {
            return Content(lpN.infoListarArticulos(lpN.listarArticulos(idLabVal, "Todos")));
        }
        public ActionResult infoListarArticulos2(int idLabVal, string idTipoVal)
        {
            return Content(lpN.infoListarArticulos2(lpN.listarArticulos(idLabVal, "Todos")));
        }
        public ActionResult infoListarArticulosManual(int idLabVal, string idTipoVal)
        {
            return Content(lpN.infoListarArticulosManual(lpN.listarArticulos(idLabVal, idTipoVal)));
        }
        //---------calculos y validaciones -----------
        public ActionResult ValidarDatosContrato(ContratoRebate_E c)
        {
            VerificacionAccesos(0);
            string status = "true";
            try
            {
                crN.validarContrato(c);
                return Content(status);
            }
            catch (Exception e) { return Content(e.Message); }
        }
        /************************************/
        private string VerificacionAccesos(int ope)
        {
            string nombreOperacion = this.ControllerContext.RouteData.Values["action"].ToString();
            Usuario_E user = (Usuario_E)Session["UsuarioId"];
            if (user == null)
            { return "E_Login"; }
            else
            {
                if ((rol1.VerificarAccesoOperacion(user.IdRol, ope, nombreOperacion, modulo) == 1) || (user.IdRol == 1))
                {
                    Capa_Negocio.Utilitarios_N utiN = new Capa_Negocio.Utilitarios_N();
                    //utiN.registrarLog(user.id, "intento de " + nombreOperacion, ope, Request.UserHostAddress, Request.UserHostName);
                    return "C_Access";
                }
                else
                { return "E_Access"; }
            }
        }
        /******-documentos imprimibles*************/
        public ActionResult FileComprobanteProveedor(string CardCode,string NumAtCard)
        {
            VerificacionAccesos(0);
            try
            {
                byte[] fileBytes = System.IO.File.ReadAllBytes(@"E:\COBEFAR\REBATE\PDFREBATE\" + CardCode + @"\" + NumAtCard + ".pdf");
                return File(@"E:\COBEFAR\REBATE\PDFREBATE\" + CardCode + @"\" + NumAtCard + ".pdf", "application/pdf");
            }
            catch
            {
                return File(@"E:\COBEFAR\REBATE\PDFREBATE\PRUEBA\ERROR.pdf", "application/pdf");
            }
        }
        public ActionResult ComprobanteProveedor2(string CardCode = "PRUEBA", string NumAtCard = "ERROR")
        {
            VerificacionAccesos(0);
            ViewBag.CardCode = CardCode;
            ViewBag.NumAtCard = NumAtCard;
            return View();
        }
    }
}