using Capa_Datos;
using Capa_Entidad.Almacen_ENT.Tablas;
using Capa_Entidad.Almacen_ENT.TablasSql;
using Capa_Entidad.General_ENT.TablasSql;
using Capa_Entidad.Seguridad_ENT;
using Capa_Entidad.SocioNegocios_ENT.Tablas;
using Capa_Negocio.Almacen_NEG.Tablas;
using Capa_Negocio.Almacen_NEG.TablasSql;
using Capa_Negocio.General_NEG.TablasSql;
using Capa_Negocio.Seguridad_NEG;
using Capa_Negocio.SocioNegocios_NEG.Tablas;
using Capa_Usuario.Helpers;
using Microsoft.Reporting.WebForms;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using Rotativa;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web.Mvc;
using TableStyles = OfficeOpenXml.Table.TableStyles;

namespace Capa_Usuario.Controllers
{
    public class AlmacenController : Controller
    {
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

        /********************************* D E V O L U C I O N E S ********************************/
        // OPERACIONES DE 100 A 199 DISPONIBLES PARA DEVOLUCIONES
        protected string CargarListaDevoluciones(Capa_Entidad.Almacen_ENT.TablasSql.ORPD_E devolucion)
        {
            Capa_Negocio.Almacen_NEG.TablasSql.ORPD_N orpdN = new Capa_Negocio.Almacen_NEG.TablasSql.ORPD_N();
            var listaDevoluciones = orpdN.ListarDevoluciones(devolucion);
            string lista = string.Empty;

            foreach (var dev in listaDevoluciones)
            {
                string fila = "<tr>" +
                                $"<td class=\"text-center\">{dev.DocNum}</td>" +
                                $"<td class=\"text-center\">{dev.WhsCode}</td>" +
                                 $"<td class=\"text-center\">{dev.FechaOperacion}</td>" +
                                 $"<td class=\"text-center\">{dev.Correlativo}</td>" +
                                 $"<td class=\"text-center\">{dev.FechaDevolucion}</td>" +
                                $"<td class=\"text-center\">{dev.CardName}</td>" +
                                $"<td class=\"text-center\">{dev.Operario}</td>" +
                                $"<td class=\"text-center\">{dev.Estado}</td>" +
                                "<td class=\"text-center\">";
                if (dev.Estado.Equals("PENDIENTE DE RECOJO"))
                {
                    fila += $"<button type=\"button\" class=\"btn btn-sm btn-danger\" onclick=\"validarCambioEstadoDevolucion({dev.DocEntry}, {dev.DocNum}, '{dev.WhsCode}', 'AA')\" ><i title=\"Anular\" class=\"icon-bin\"></i></button>";
                }
                // Solo para Estado RECOGIDO y guardará "0" en el campo "SinNC" siempre y cuando no encuentre un NULL en NotaCredito línea x línea en RPD1
                /*else if (dev.Estado.Equals("RECOGIDO") && dev.SinNC.Equals(0))
                {
                    fila += $"<button type=\"button\" class=\"btn btn-sm btn-success\" onclick=\"validarCambioEstadoDevolucion({dev.DocEntry}, {dev.DocNum}, '{dev.WhsCode}', 'NC')\" >NC Aplicada</button>";
                }*/
                fila += "</td>" +
                    "<td class=\"text-center\">";
                if (dev.Estado.Equals("RECOGIDO"))
                {
                    fila += $"<button type=\"button\" class=\"btn btn-sm btn-orange\" onclick=\"validarCambioEstadoDevolucion({dev.DocEntry}, {dev.DocNum}, '{dev.WhsCode}', 'RR')\" >Rv. Recojo</button>";
                }
                if (dev.Estado.Equals("PENDIENTE DE RECOJO"))
                {
                    fila += $"<button type=\"button\" class=\"btn btn-sm btn-blue\" onclick=\"validarCambioEstadoDevolucion({dev.DocEntry}, {dev.DocNum}, '{dev.WhsCode}', 'R')\" >Recogido</button>";
                }
                fila += "</td>" +
                "<td class=\"text-center\">";

                if (dev.Estado.Equals("PENDIENTE DE RECOJO"))
                {
                    if (dev.WhsCode.Equals("DEV07"))
                    {
                        fila += $"<a href=\"/Almacen/EditarDevolucion?DocEntry={dev.DocEntry}&Almacen=DEV07\" class=\"btn btn-sm btn-dark\"><i title=\"Editar Devolución - ALM DEV07\" class=\"icon-pencil\"></i></a>";
                    }
                    else
                    {
                        fila += $"<a href=\"/Almacen/EditarDevolucion?DocEntry={dev.DocEntry}\" class=\"btn btn-sm btn-dark\"><i title=\"Editar Devolución\" class=\"icon-pencil\"></i></a>";
                    }
                }

                fila += "</td>" +
                                $"<td class=\"text-center\"><a href=\"/Almacen/SeguimientoDevolucion?DocEntry={dev.DocEntry}\" class=\"btn btn-sm btn-warning\"><i title=\"Ver Devolución\" class=\"icon-search\"></i></a></td>" +
                "</tr>";

                lista += fila;
            }

            return lista;
        }
        [HttpGet]
        public ActionResult DevolucionMercancias(Capa_Entidad.Almacen_ENT.TablasSql.ORPD_E devolucion = null, int idOperation = 101)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);

            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                Capa_Negocio.General_NEG.TablasSql.OWHS_N owhsN = new Capa_Negocio.General_NEG.TablasSql.OWHS_N();
                Capa_Negocio.Almacen_NEG.TablasSql.ORPD_N orpdN = new Capa_Negocio.Almacen_NEG.TablasSql.ORPD_N(); Capa_Negocio.Almacen_NEG.Tablas.OMRC_N omrcN = new Capa_Negocio.Almacen_NEG.Tablas.OMRC_N();
                Capa_Negocio.Almacen_NEG.TablasSql.MotivosDevoluciones_N mdN = new Capa_Negocio.Almacen_NEG.TablasSql.MotivosDevoluciones_N(); Capa_Negocio.Almacen_NEG.TablasSql.SubmotivosDevoluciones_N subN = new Capa_Negocio.Almacen_NEG.TablasSql.SubmotivosDevoluciones_N();

                var result = orpdN.ListarDevoluciones(devolucion);
                ViewBag.Devolucion = devolucion;
                string[] arrWhsCode = { "03", "05", "06", "CUAR07", "DEV07" };
                ViewBag.Almacenes = owhsN.listarAlmacenes(arrWhsCode);
                ViewBag.Laboratorios = omrcN.listarFabricantes();
                ViewBag.Motivos = mdN.ListarMotivosDevoluciones(null);          // Solo para el modal Motivos
                ViewBag.ListaParaSubmotivos = mdN.ListarMotivosDevoluciones(new MotivosDevoluciones_E { Estado = "1" });        // Esta lista es para la lista desplegable en el modal Observaciones
                ViewBag.Submotivos = subN.ListarSubmotivosDevoluciones(null);

                return View(result);
            }
            else
            {
                return resultadoAcceso;
            }
        }

        [HttpGet]
        public ActionResult NuevaDevolucion(Capa_Entidad.Almacen_ENT.TablasSql.ORPD_E Devolucion, string Almacen, int idOperation = 102)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);

            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                Capa_Negocio.General_NEG.TablasSql.OWHS_N owhsN = new Capa_Negocio.General_NEG.TablasSql.OWHS_N();
                Capa_Negocio.Almacen_NEG.TablasSql.MotivosDevoluciones_N mdN = new Capa_Negocio.Almacen_NEG.TablasSql.MotivosDevoluciones_N(); Capa_Negocio.Almacen_NEG.TablasSql.SubmotivosDevoluciones_N subN = new Capa_Negocio.Almacen_NEG.TablasSql.SubmotivosDevoluciones_N();
                Capa_Negocio.Almacen_NEG.Tablas.OMRC_N omrcN = new Capa_Negocio.Almacen_NEG.Tablas.OMRC_N();
                OCRD_N ocrdN = new OCRD_N();
                Usuario_E usu = (Usuario_E)Session["UsuarioId"];

                string nombreVista = "NuevaDevolucion";
                ViewBag.RolUsuario = usu.IdRol;
                ViewBag.Laboratorios = omrcN.listarFabricantes();
                ViewBag.MotivosDevoluciones = mdN.ListarMotivosDevoluciones(new MotivosDevoluciones_E { Estado = "1" });
                ViewBag.SubmotivosDevoluciones = subN.ListarSubmotivosDevoluciones(new SubmotivosDevoluciones_E { Estado = "1" });
                ViewBag.ListaProveedores = ocrdN.listarSociosDeNegocios(new OCRD_E { CardType = "S" });     // Solo socios Proveedores

                if (Almacen != null && Almacen.Equals("DEV07"))
                {
                    string[] arrWhsCode = { "DEV07" };
                    nombreVista = "NuevaDevolucionAlmDEV07";
                    ViewBag.Almacenes = owhsN.listarAlmacenes(arrWhsCode);
                }
                else
                {
                    string[] arrWhsCode = { "03", "05", "06", "CUAR07" };
                    ViewBag.Almacenes = owhsN.listarAlmacenes(arrWhsCode);
                }

                return View(nombreVista);
            }
            else
            {
                return resultadoAcceso;
            }
        }

        [HttpPost]
        public ActionResult NuevaDevolucion(Capa_Entidad.Almacen_ENT.TablasSql.ORPD_E Devolucion, List<Capa_Entidad.Almacen_ENT.TablasSql.RPD1_E> DetalleDevolucion, int idOperation = 102)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);

            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                Capa_Negocio.Almacen_NEG.TablasSql.ORPD_N orpdN = new Capa_Negocio.Almacen_NEG.TablasSql.ORPD_N();
                Capa_Negocio.General_NEG.TablasSql.OWHS_N owhsN = new Capa_Negocio.General_NEG.TablasSql.OWHS_N();
                Capa_Negocio.Almacen_NEG.TablasSql.MotivosDevoluciones_N mdN = new Capa_Negocio.Almacen_NEG.TablasSql.MotivosDevoluciones_N(); Capa_Negocio.Almacen_NEG.TablasSql.SubmotivosDevoluciones_N subN = new Capa_Negocio.Almacen_NEG.TablasSql.SubmotivosDevoluciones_N();
                Capa_Negocio.Almacen_NEG.Tablas.OMRC_N omrcN = new Capa_Negocio.Almacen_NEG.Tablas.OMRC_N();
                OCRD_N ocrdN = new OCRD_N();
                Usuario_E usu = (Usuario_E)Session["UsuarioId"];
                try
                {
                    Devolucion.Operario = $"{usu.Nombres} {usu.Apellidos}";
                    orpdN.RegistrarDevolucion(Devolucion, DetalleDevolucion);
                    return RedirectToAction("DevolucionMercancias");
                }
                catch (Exception e)
                {
                    ViewBag.Mensaje = e.Message;
                    ViewBag.RolUsuario = usu.IdRol;
                    ViewBag.Laboratorios = omrcN.listarFabricantes();
                    ViewBag.MotivosDevoluciones = mdN.ListarMotivosDevoluciones(new MotivosDevoluciones_E { Estado = "1" });
                    ViewBag.SubmotivosDevoluciones = subN.ListarSubmotivosDevoluciones(new SubmotivosDevoluciones_E { Estado = "1" });
                    ViewBag.ListaProveedores = ocrdN.listarSociosDeNegocios(new OCRD_E { CardType = "S" });     // Solo socios Proveedores
                    return View(Devolucion);
                }
            }
            else
            {
                return resultadoAcceso;
            }
        }

        [HttpGet]
        public ActionResult EditarDevolucion(int DocEntry, string Almacen, int idOperation = 103)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);

            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                Capa_Negocio.General_NEG.TablasSql.OWHS_N owhsN = new Capa_Negocio.General_NEG.TablasSql.OWHS_N();
                Capa_Negocio.Almacen_NEG.TablasSql.MotivosDevoluciones_N mdN = new Capa_Negocio.Almacen_NEG.TablasSql.MotivosDevoluciones_N();
                Capa_Negocio.Almacen_NEG.TablasSql.ORPD_N orpdN = new Capa_Negocio.Almacen_NEG.TablasSql.ORPD_N();
                Capa_Negocio.Almacen_NEG.Tablas.OMRC_N omrcN = new Capa_Negocio.Almacen_NEG.Tablas.OMRC_N();
                Capa_Negocio.Almacen_NEG.TablasSql.SubmotivosDevoluciones_N subN = new Capa_Negocio.Almacen_NEG.TablasSql.SubmotivosDevoluciones_N();

                var datosDevolucion = orpdN.ObtenerDevolucion(DocEntry);
                string[] arrWhsCode = { datosDevolucion.WhsCode };
                ViewBag.Almacenes = owhsN.listarAlmacenes(arrWhsCode);
                ViewBag.Laboratorios = omrcN.listarFabricantes();
                ViewBag.MotivosDevoluciones = mdN.ListarMotivosDevoluciones(new MotivosDevoluciones_E { Estado = "1" });
                ViewBag.SubmotivosDevoluciones = subN.ListarSubmotivosDevoluciones(new SubmotivosDevoluciones_E { Estado = "1" });
                if (Almacen != null && Almacen.Equals("DEV07"))
                {
                    return View("EditarDevolucionAlmDEV07", datosDevolucion);
                }
                else
                {
                    return View(datosDevolucion);
                }
            }
            else
            {
                return resultadoAcceso;
            }
        }
        [HttpPost]
        public ActionResult EditarDevolucion(Capa_Entidad.Almacen_ENT.TablasSql.ORPD_E Devolucion, List<Capa_Entidad.Almacen_ENT.TablasSql.RPD1_E> DetalleDevolucion, int idOperation = 103)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);

            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                Capa_Negocio.Almacen_NEG.TablasSql.ORPD_N orpdN = new Capa_Negocio.Almacen_NEG.TablasSql.ORPD_N();
                Usuario_E usu = (Usuario_E)Session["UsuarioId"];

                try
                {
                    Devolucion.Operario = $"{usu.Nombres} {usu.Apellidos}";
                    orpdN.EditarDevolucion(Devolucion, DetalleDevolucion);
                    return RedirectToAction("DevolucionMercancias");
                }
                catch (Exception e)
                {
                    ViewBag.Mensaje = e.Message;
                    return RedirectToAction("EditarDevolucion", new { DocEntry = Devolucion.DocEntry, Almacen = Devolucion.WhsCode });
                }
            }
            else
            {
                return resultadoAcceso;
            }
        }
        public ActionResult ValidarDatosDevolucion(List<Capa_Entidad.Almacen_ENT.TablasSql.RPD1_E> DetalleDevolucion)
        {
            string status = "true";
            try
            {
                Capa_Negocio.Almacen_NEG.TablasSql.ORPD_N orpdN = new Capa_Negocio.Almacen_NEG.TablasSql.ORPD_N();
                orpdN.validarCantidadDetalleDevolucion(DetalleDevolucion);
                return Content(status);
            }
            catch (Exception e) { return Content(e.Message); }

        }
        [HttpGet]
        public ActionResult SeguimientoDevolucion(int DocEntry, int idOperation = 104)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);

            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                Capa_Negocio.Almacen_NEG.TablasSql.ORPD_N orpdN = new Capa_Negocio.Almacen_NEG.TablasSql.ORPD_N();
                var datosDevolucion = orpdN.ObtenerDevolucion(DocEntry);
                return View(datosDevolucion);
            }
            else
            {
                return resultadoAcceso;
            }
        }
        public JsonResult BuscarProducto(OITM_E datos)
        {
            if (datos.FirmCode >= 1)
            {
                OITM_N oitmN = new OITM_N();
                var datalist = "<datalist id='ListaProductos'>";
                var listaProductos = oitmN.Listar(new OITM_E { FirmCode = datos.FirmCode});

                if (listaProductos != null && listaProductos.Count >= 1)
                {
                    foreach (var p in listaProductos)
                    {
                        datalist += $"<option ItemCode='{p.ItemCode}' FirmCode='{p.FirmCode}' FirmName='{p.U_SYP_FABRICANTE}' BuyUnitMsr='{p.BuyUnitMsr}' NumInBuy='{p.NumInBuy}' value='{p.ItemName}'></option>";
                    }
                }
                datalist += "</datalist>";
                return Json(datalist);
            }
            else
            {
                return null;
            }
        }
        public JsonResult BuscarLotesProducto(Capa_Entidad.Almacen_ENT.Tablas.OIBT_E filtros)
        {
            //verificacionAccesos(0);         // Validar sesion logueada, solo para ajax
            if (!string.IsNullOrEmpty(filtros.ItemCode))
            {
                Capa_Negocio.Almacen_NEG.Tablas.OIBT_N oibtN = new Capa_Negocio.Almacen_NEG.Tablas.OIBT_N();
                var datalist = "<datalist id='ListaLotes'>";
                var listaLotes = oibtN.listarArticulosLotes(filtros, false);

                if (listaLotes != null && listaLotes.Count >= 1)
                {
                    foreach (var l in listaLotes)
                    {
                        datalist += $"<option value='{l.BatchNum}'></option>";
                    }
                }

                datalist += "</datalist>";

                return Json(datalist);
            }
            else
            {
                return null;
            }
        }
        public JsonResult FiltrarArticulo(OPDN_E datos)
        {
            //verificacionAccesos(0);         // Validar sesion logueada, solo para ajax
            if (!string.IsNullOrEmpty(datos.DocDate) || datos.DocNum >= 1 || !string.IsNullOrEmpty(datos.ItemCode) || !string.IsNullOrEmpty(datos.BatchNum) || !string.IsNullOrEmpty(datos.NumAtCard))
            {
                OPDN_N opdnN = new OPDN_N();
                Capa_Negocio.Almacen_NEG.Tablas.OIBT_N oibtN = new Capa_Negocio.Almacen_NEG.Tablas.OIBT_N();
                var tbody = string.Empty;
                string producto;

                if (datos.SinEM)
                {

                    var listaArticulos1 = oibtN.listarArticulosLotes(new Capa_Entidad.Almacen_ENT.Tablas.OIBT_E { ItemCode = datos.ItemCode, BatchNum = datos.BatchNum, WhsCode = datos.U_COB_LUGAREN, Quantity = 1 }, true);

                    foreach (var art in listaArticulos1)
                    {
                        decimal QuantityConv = art.Quantity / art.NumInBuy;
                        producto = art.ItemName.Replace("\x022", "&quot;");
                        tbody += "<tr>" +
                                        $"<td><span class='text-danger font-weight-bold'>{art.ItemCode}</span></br>{art.ItemName}</td>" +
                                        $"<td class='text-center'>{art.BatchNum}</td>" +
                                        $"<td class='text-center'>{art.ExpDate}</td>" +
                                        $"<td class='text-center'>{art.BuyUnitMsr}</td>" +
                                        $"<td class='text-center'></td>" +
                                        $"<td class='text-center'><button type=\"button\" class=\"btn btn-warning\" onclick=\"agregarDetalleDevolucion('{art.ItemCode}', '{producto}', '{art.FirmCode}', '{art.FirmName}', '{art.BatchNum}', '{art.ExpDate}', '{art.BuyUnitMsr}', '', '{art.NumInBuy}', {art.Quantity}, 'BD','','',{QuantityConv});\"><i class='icon-plus'></i> </button></td>" +
                                        "<tr>";
                    }
                }
                else if (!string.IsNullOrEmpty(datos.U_COB_LUGAREN) && datos.U_COB_LUGAREN.Equals("DEV07"))
                {
                    if (!string.IsNullOrEmpty(datos.ItemCode) && !string.IsNullOrEmpty(datos.BatchNum))
                    {
                        List<Capa_Entidad.Almacen_ENT.Tablas.OIBT_E> listaOIBT = oibtN.listarArticulosLotes(new Capa_Entidad.Almacen_ENT.Tablas.OIBT_E { ItemCode = datos.ItemCode, BatchNum = datos.BatchNum, WhsCode = datos.U_COB_LUGAREN, Quantity = 1 }, true);
                        if (listaOIBT.Count() > 0)
                        {
                            Capa_Entidad.Almacen_ENT.Tablas.OIBT_E artOIBT = listaOIBT.FirstOrDefault();

                            //se envia la cantidad exacta que figura en OIBT, para buscar facturas que cubran esa cantidad
                            datos.Quantity = artOIBT.Quantity;
                        }

                        var listaArticulos2 = opdnN.Listar(datos);

                        foreach (var art in listaArticulos2)
                        {

                            decimal QuantityConv = datos.Quantity / art.NumInBuy;
                            producto = art.Dscription.Replace("\x022", "&quot;");
                            tbody += "<tr>" +
                                            $"<td><span class='text-danger font-weight-bold'>{art.ItemCode}</span></br>{art.Dscription}</td>" +
                                            $"<td class='text-center'>{art.BatchNum}</td>" +
                                            $"<td class='text-center'>{art.ExpDate}</td>" +
                                            $"<td class='text-center'>{art.BuyUnitMsr}</td>" +
                                            $"<td class='text-center'>{art.NumAtCard}</td>" +
                                            $"<td class='text-center'><button type=\"button\" class=\"btn btn-warning\" onclick=\"agregarDetalleDevolucion('{art.ItemCode}', '{producto}', '{art.FirmCode}', '{art.FirmName}', '{art.BatchNum}', '{art.ExpDate}', '{art.BuyUnitMsr}', '{art.NumAtCard}', '{art.NumInBuy}', {art.Quantity}, 'BD','{art.CardCode}','{art.CardName}',{Math.Round(QuantityConv, 2)});\"><i class='icon-plus'></i> </button></td>" +
                                            "<tr>";
                        }
                    }
                    else
                    {
                        var listaArticulos2 = opdnN.Listar(datos);

                        foreach (var art in listaArticulos2)
                        {
                            decimal QuantityConv = art.QuantityOIBT / art.NumInBuy;
                            producto = art.Dscription.Replace("\x022", "&quot;");
                            tbody += "<tr>" +
                                            $"<td><span class='text-danger font-weight-bold'>{art.ItemCode}</span></br>{art.Dscription}</td>" +
                                            $"<td class='text-center'>{art.BatchNum}</td>" +
                                            $"<td class='text-center'>{art.ExpDate}</td>" +
                                            $"<td class='text-center'>{art.BuyUnitMsr}</td>" +
                                            $"<td class='text-center'>{art.NumAtCard}</td>" +
                                            $"<td class='text-center'><button type=\"button\" class=\"btn btn-warning\" onclick=\"agregarDetalleDevolucion('{art.ItemCode}', '{producto}', '{art.FirmCode}', '{art.FirmName}', '{art.BatchNum}', '{art.ExpDate}', '{art.BuyUnitMsr}', '{art.NumAtCard}', '{art.NumInBuy}', {art.Quantity}, 'BD','{art.CardCode}','{art.CardName}',{QuantityConv});\"><i class='icon-plus'></i> </button></td>" +
                                            "<tr>";
                        }
                    }
                    return Json(tbody);

                }
                else if (!string.IsNullOrEmpty(datos.U_COB_LUGAREN) && datos.U_COB_LUGAREN.Equals("06"))
                {
                    //EXCLUSIVO PARA RETIRO DE MERCADO
                    if (!string.IsNullOrEmpty(datos.ItemCode) && !string.IsNullOrEmpty(datos.BatchNum))
                    {
                        List<Capa_Entidad.Almacen_ENT.Tablas.OIBT_E> listaOIBT = oibtN.listarArticulosLotes(new Capa_Entidad.Almacen_ENT.Tablas.OIBT_E { ItemCode = datos.ItemCode, BatchNum = datos.BatchNum, WhsCode = datos.U_COB_LUGAREN, Quantity = 1 }, true);
                        if (listaOIBT.Count() > 0)
                        {
                            Capa_Entidad.Almacen_ENT.Tablas.OIBT_E artOIBT = listaOIBT.FirstOrDefault();

                            //se envia la cantidad exacta que figura en OIBT, para buscar facturas que cubran esa cantidad
                            datos.Quantity = artOIBT.Quantity;
                        }

                        var listaArticulos2 = opdnN.Listar(datos);

                        foreach (var art in listaArticulos2)
                        {
                            //cantidad pasada a cajas desde piezas 
                            decimal QuantityConv = datos.Quantity / art.NumInBuy;
                            producto = art.Dscription.Replace("\x022", "&quot;");
                            tbody += "<tr>" +
                                            $"<td><span class='text-danger font-weight-bold'>{art.ItemCode}</span></br>{art.Dscription}</td>" +
                                            $"<td class='text-center'>{art.BatchNum}</td>" +
                                            $"<td class='text-center'>{art.ExpDate}</td>" +
                                            $"<td class='text-center'>{art.BuyUnitMsr}</td>" +
                                            $"<td class='text-center'>{art.NumAtCard}</td>" +
                                            $"<td class='text-center'><button type=\"button\" class=\"btn btn-warning\" onclick=\"agregarDetalleDevolucion('{art.ItemCode}', '{producto}', '{art.FirmCode}', '{art.FirmName}', '{art.BatchNum}', '{art.ExpDate}', '{art.BuyUnitMsr}', '{art.NumAtCard}', '{art.NumInBuy}', {art.Quantity}, 'BD','{art.CardCode}','{art.CardName}',{QuantityConv});\"><i class='icon-plus'></i> </button></td>" +
                                            "<tr>";
                        }
                    }
                    else
                    {
                        var listaArticulos2 = opdnN.Listar(datos);

                        foreach (var art in listaArticulos2)
                        {
                            //cantidad pasada a cajas desde piezas 
                            decimal QuantityConv = art.QuantityOIBT / art.NumInBuy;
                            producto = art.Dscription.Replace("\x022", "&quot;");
                            tbody += "<tr>" +
                                            $"<td><span class='text-danger font-weight-bold'>{art.ItemCode}</span></br>{art.Dscription}</td>" +
                                            $"<td class='text-center'>{art.BatchNum}</td>" +
                                            $"<td class='text-center'>{art.ExpDate}</td>" +
                                            $"<td class='text-center'>{art.BuyUnitMsr}</td>" +
                                            $"<td class='text-center'>{art.NumAtCard}</td>" +
                                            $"<td class='text-center'><button type=\"button\" class=\"btn btn-warning\" onclick=\"agregarDetalleDevolucion('{art.ItemCode}', '{producto}', '{art.FirmCode}', '{art.FirmName}', '{art.BatchNum}', '{art.ExpDate}', '{art.BuyUnitMsr}', '{art.NumAtCard}', '{art.NumInBuy}', {art.Quantity}, 'BD','{art.CardCode}','{art.CardName}',{QuantityConv});\"><i class='icon-plus'></i> </button></td>" +
                                            "<tr>";
                        }
                    }
                    return Json(tbody);

                }
                else
                {
                    var listaArticulos2 = opdnN.Listar(datos);

                    foreach (var art in listaArticulos2)
                    {
                        producto = art.Dscription.Replace("\x022", "&quot;");
                        tbody += "<tr>" +
                                        $"<td><span class='text-danger font-weight-bold'>{art.ItemCode}</span></br>{art.Dscription}</td>" +
                                        $"<td class='text-center'>{art.BatchNum}</td>" +
                                        $"<td class='text-center'>{art.ExpDate}</td>" +
                                        $"<td class='text-center'>{art.BuyUnitMsr}</td>" +
                                        $"<td class='text-center'>{art.NumAtCard}</td>" +
                                        $"<td class='text-center'><button type=\"button\" class=\"btn btn-warning\" onclick=\"agregarDetalleDevolucion('{art.ItemCode}', '{producto}', '{art.FirmCode}', '{art.FirmName}', '{art.BatchNum}', '{art.ExpDate}', '{art.BuyUnitMsr}', '{art.NumAtCard}', '{art.NumInBuy}', {art.Quantity}, 'BD','{art.CardCode}','{art.CardName}',0);\"><i class='icon-plus'></i> </button></td>" +
                                        "<tr>";
                    }
                }

                return Json(tbody);
            }
            else
            {
                return null;
            }
        }
        //para Retiro Mercado, se usa desde View NuevaDevolucion
        public JsonResult BuscarProductoRM(OITM_E datos)
        {
            //verificacionAccesos(0);         // Validar sesion logueada, solo para ajax
            if (datos.FirmCode >= 1)
            {
                OITM_N oitmN = new OITM_N();
                var datalist = "<datalist id='ListaProductosRM'>";
                var listaProductos = oitmN.Listar(new OITM_E { FirmCode = datos.FirmCode });

                if (listaProductos != null && listaProductos.Count >= 1)
                {
                    foreach (var p in listaProductos)
                    {
                        datalist += $"<option ItemCode='{p.ItemCode}' value='{p.ItemName}' ></option>";
                    }
                }

                datalist += "</datalist>";

                return Json(datalist);
            }
            else
            {
                return null;
            }
        }
        public JsonResult FiltrarArticuloOIBT(Capa_Entidad.Almacen_ENT.Tablas.OIBT_E datos)
        {
            //verificacionAccesos(0); // Validar sesion logueada, solo para ajax
            if (!string.IsNullOrEmpty(datos.ItemCode) || !string.IsNullOrEmpty(datos.BatchNum))
            {
                Capa_Negocio.Almacen_NEG.Tablas.OIBT_N oibtN = new Capa_Negocio.Almacen_NEG.Tablas.OIBT_N();
                var tbody = string.Empty;
                var listaArticulos = oibtN.listarArticulosLotes(datos);


                foreach (var art in listaArticulos)
                {
                    tbody += "<tr>" +
                                    $"<td><span class='text-danger font-weight-bold'>{art.ItemCode}</span></br>{art.ItemName}</td>" +
                                    $"<td class='text-center'>{art.BatchNum}</td>" +
                                    $"<td class='text-center'>{art.ExpDate}</td>" +
                                    $"<td class='text-center'>{art.Quantity}</td>" +
                                    $"<td class='text-center'><button type=\"button\" class=\"btn btn-warning\" onclick=\"buscarFacturas('{art.ItemCode}',  '{art.BatchNum}', {art.Quantity});\"><i class='icon-search'></i> </button></td>" +
                                    "<tr>";
                }

                return Json(tbody);
            }
            else
            {
                return null;
            }
        }
        public JsonResult BuscarFacturas(OPDN_E obj)
        {
            //verificacionAccesos(0);         // Validar sesion logueada, solo para ajax
            if (!string.IsNullOrEmpty(obj.ItemCode) || !string.IsNullOrEmpty(obj.BatchNum))
            {
                OPDN_N opdnN = new OPDN_N();
                var tbody = string.Empty;
                string producto;
                var listaArticulos = opdnN.Listar(obj);

                foreach (var art in listaArticulos)
                {
                    decimal CantidadCajas = obj.Quantity / art.NumInBuy;
                    producto = art.Dscription.Replace("\x022", "&quot;");
                    tbody += "<tr>" +
                                    $"<td><span class='text-danger font-weight-bold'>{art.ItemCode}</span></br>{art.Dscription}</br>{art.Quantity}</td>" +
                                    $"<td class='text-center'>{art.BatchNum}</td>" +
                                    $"<td class='text-center'>{art.ExpDate}</td>" +
                                    $"<td class='text-center'>{art.BuyUnitMsr}</td>" +
                                    $"<td><span class='text-danger font-weight-bold'>{art.CardName}</span></br>{art.NumAtCard} </br> {art.DocDate} </td>" +
                                    $"<td class='text-center'><button type=\"button\" class=\"btn btn-warning\" onclick=\"agregarDetalleDevolucion('{art.ItemCode}', '{producto}', '{art.FirmCode}', '{art.FirmName}', '{art.BatchNum}', '{art.ExpDate}', '{art.BuyUnitMsr}', '{art.NumAtCard}', '{art.NumInBuy}', {art.Quantity}, 'BD','{art.CardCode}','{art.CardName}',{CantidadCajas});\"><i class='icon-plus'></i> </button></td>" +
                                    "<tr>";
                }


                return Json(tbody);
            }
            else
            {
                return null;
            }
        }
        public JsonResult CambiarEstadoDevolucion(Capa_Entidad.Almacen_ENT.TablasSql.ORPD_E devolucion, string tipoMantenimiento)
        {
            //verificacionAccesos(0);         // Validar sesion logueada, solo para ajax
            if (devolucion.DocEntry >= 1 && devolucion.DocNum >= 1)
            {
                Usuario_E usu = (Usuario_E)Session["UsuarioId"];
                Capa_Negocio.Almacen_NEG.TablasSql.ORPD_N orpdN = new Capa_Negocio.Almacen_NEG.TablasSql.ORPD_N();
                devolucion.Operario = $"{usu.Nombres} {usu.Apellidos}";
                orpdN.CambiarEstadoDevolucion(devolucion, tipoMantenimiento);

                Dictionary<string, string> mensajes = new Dictionary<string, string>
                {
                    { "R", "Devolución recogida por el Proveedor" },
                    { "AA","Anulacion correcta" },
                    { "RR","Devolucion esta pendiente de recojo" },
                    { "NC","Nota de credito aplicada" },
                    { "EC","Correo enviado, revise su bandeja" }
                };

                return Json(new { Lista = CargarListaDevoluciones(devolucion), Mensaje = mensajes[tipoMantenimiento] });
            }
            else
            {
                return null;
            }
        }
        public JsonResult EnviarCorreo(int DocEntryDev, string Correo1, string Correo2, string WhsCode)
        {
            Usuario_D usuD = new Usuario_D(); Utilitarios uti = new Utilitarios();
            int DocEntryEncargado = 0;
            if (WhsCode.Equals("03")) { DocEntryEncargado = 185; }
            else if (WhsCode.Equals("05")) { DocEntryEncargado = 697; }
            else if (WhsCode.Equals("06")) { DocEntryEncargado = 202; }
            else if (WhsCode.Equals("CUAR07")) { DocEntryEncargado = 161; }
            else if (WhsCode.Equals("DEV07")) { DocEntryEncargado = 161; }
            if (DocEntryEncargado > 0)
            {
                var encargado = usuD.buscarUsuario(DocEntryEncargado);
                string destinatario = Correo1;
                string remitente = encargado.Email;
                string asunto = "COBEFAR SAC - DEVOLUCION DE MERCADERIA";
                string cuerpo = "<html><body><h3 style='color:green;'>DEVOLUCION DE MERCADERIA- COBEFAR SAC</h3><p style='font-size:16px;font-weight:bold'>Estimado proveedor,\n\n<br>Por medio de la presente adjuntamos el formato para devolucion de productos con su detallado, por favor sirvase a revisarlo y responder este correo en la brevedad para pactar una fecha de entrega.<br></p><span>Área Devoluciones - COBEFAR SAC\n</span></body></html>";
                MailMessage ms = new MailMessage(remitente, destinatario, asunto, cuerpo);
                ms.IsBodyHtml = true;  // Indicar que el cuerpo es HTML
                if (!string.IsNullOrEmpty(encargado.Email)) { ms.Bcc.Add(encargado.Email); }
                if (!string.IsNullOrEmpty(Correo2)) { ms.Bcc.Add(Correo2); }
                SmtpClient smtp = new SmtpClient(uti.Smtp, uti.CodigoSmtp);
                smtp.EnableSsl = true;
                smtp.Credentials = new NetworkCredential(remitente, encargado.ClaveEmail);

                //ADJUNTAR ARCHIVO PDF O EXCEL


                var root = Server.MapPath("~/PDF/");
                if (!System.IO.Directory.Exists(@root))
                {
                    System.IO.Directory.CreateDirectory(@root);
                }

                var pdfname = String.Format("FormatoDevolucion_" + DocEntryDev + ".pdf");
                var pathPdf = Path.Combine(root, pdfname);

                pathPdf = Path.GetFullPath(pathPdf);
                var something = new ActionAsPdf("FormatoDevolucionSimple", new { DocEntry = DocEntryDev }) { FileName = pdfname, PageOrientation = Rotativa.Options.Orientation.Landscape, PageSize = Rotativa.Options.Size.A4 };

                var binary = something.BuildPdf(this.ControllerContext);
                System.IO.File.Create(pathPdf).Close();
                System.IO.File.WriteAllBytes(@pathPdf, binary);
                var rootExcel = Server.MapPath("~/EXCEL/");
                if (!System.IO.Directory.Exists(@rootExcel))
                {
                    System.IO.Directory.CreateDirectory(@rootExcel);
                }

                var excelname = String.Format("FormatoDevolucion_" + DocEntryDev + ".xlsx");
                var pathExcel = Path.Combine(rootExcel, excelname);
                Capa_Negocio.Almacen_NEG.TablasSql.ORPD_N orpdN = new Capa_Negocio.Almacen_NEG.TablasSql.ORPD_N();
                var objdevolucion = orpdN.ObtenerDevolucion(DocEntryDev);
                //solo se envia formato excel si es de portugal no RM
                if (objdevolucion.RetiroMercado == false && objdevolucion.CardCode.Equals("P20100204330"))
                {
                    var dev = orpdN.RptCorreoDevolucion(DocEntryDev);
                    if (dev != null)
                    {
                        pathExcel = Path.GetFullPath(pathExcel);

                        using (var libro = new ExcelPackage())
                        {
                            ExcelWorksheet worksheet = libro.Workbook.Worksheets.Add("Devolucion");
                            worksheet.Cells["C2:I2"].Merge = true; // Combina las celdas de C2 a I2
                            worksheet.Cells["C2:I2"].Value = "DEVOLUCION AL LABORATORIO PORTUGAL";
                            // Establecer el formato de centrado
                            worksheet.Cells["C2:I2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            worksheet.Cells["C2:I2"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            worksheet.Cells["C2:I2"].Style.Font.Bold = true;
                            worksheet.Cells["C2:I2"].Style.Font.Size = 28;
                            worksheet.Cells["C3:I3"].Style.Font.Bold = true;
                            worksheet.Cells["C3:I3"].Style.Font.Size = 12;
                            worksheet.Cells["C3"].LoadFromCollection(dev, PrintHeaders: true);

                            // A continuación, establecemos el formato de texto para toda la columna F
                            worksheet.Cells["D:D"].Style.Numberformat.Format = "@";
                            worksheet.Cells["F:F"].Style.Numberformat.Format = "@";
                            if (dev != null && dev.Count >= 1)
                            {
                                // Establecer formato para todas las columnas
                                for (var col = 1; col <= 7; col++)
                                {
                                    worksheet.Column(col).AutoFit();
                                }

                                // Crear la tabla a partir de los datos en C3
                                var tabla = worksheet.Tables.Add(worksheet.Cells["C3:I" + (dev.Count + 2)], "Devolucion");
                                tabla.ShowHeader = true;
                                tabla.TableStyle = TableStyles.Medium7;

                            }
                            System.IO.File.Create(pathExcel).Close();
                            System.IO.File.WriteAllBytes(@pathExcel, libro.GetAsByteArray());
                            ms.Attachments.Add(new System.Net.Mail.Attachment(pathExcel));
                        }
                    }
                }
                ms.Attachments.Add(new System.Net.Mail.Attachment(pathPdf));
                try
                {
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                    smtp.Send(ms);
                    ms.Dispose();
                    //registrar el correo de proveedor en su devolucion TipoMantenimiento AC
                    CambiarEstadoDevolucion(new Capa_Entidad.Almacen_ENT.TablasSql.ORPD_E { DocEntry = DocEntryDev, Correo = Correo1, DocNum = 1 }, "EC");
                    System.IO.File.Delete(@pathPdf);
                    if (objdevolucion.RetiroMercado == false && objdevolucion.CardCode.Equals("P20100204330"))
                    { System.IO.File.Delete(@pathExcel); }

                    return Json("Envio de correo exitoso, revise su bandeja");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                    return Json("Error en envio de email");
                }
            }
            return null;
        }

        //Formato que se envia al proveedor por correo como referencia
        public ActionResult FormatoDevolucionSimple(int DocEntry)
        {
            Capa_Negocio.General_NEG.TablasSql.OWHS_N owhsN = new Capa_Negocio.General_NEG.TablasSql.OWHS_N();
            Capa_Negocio.Almacen_NEG.TablasSql.ORPD_N orpdN = new Capa_Negocio.Almacen_NEG.TablasSql.ORPD_N();
            Firmas_N firN = new Firmas_N();
            Firmas_E firE = new Firmas_E();
            Dictionary<string, int> listaEncargados = new Dictionary<string, int>
            {
                { "03", 185},					// Julio Roman Silva
				{ "06", 185},					// Julio Roman Silva
				{ "DEV07", 161},			    // Carmen Condori Saravia
				{ "CUAR07", 161},			    // Carmen Condori Saravia
			};

            var datosDevolucion = orpdN.ObtenerDevolucion(DocEntry);
            string[] arrWhsCode = { datosDevolucion.WhsCode };
            List<Capa_Entidad.General_ENT.TablasSql.OWHS_E> listaAlm = owhsN.listarAlmacenes(arrWhsCode);
            firE.DocEntryUsuario = listaEncargados[listaAlm[0].WhsCode];
            var firma = firN.ListarFirmas(firE);

            ViewBag.Almacenes = owhsN.listarAlmacenes(arrWhsCode);
            ViewBag.ResponsableAlmacen = (firma != null && firma.Count >= 1) ? $"{firma[0].Nombres} {firma[0].Apellidos}" : "";

            return View(datosDevolucion);
        }
        public ActionResult PdfFormatoDevolucionSimple(int DocEntry, string LevConformidad)
        {
            return new ActionAsPdf("FormatoDevolucionSimple", new { DocEntry = DocEntry, LevConformidad = LevConformidad }) { FileName = "FormatoDevolucion_" + DocEntry + ".pdf", PageOrientation = Rotativa.Options.Orientation.Landscape, PageSize = Rotativa.Options.Size.A4 };
        }
        //Formato que maneja internamente Direccion Tecnica
        public ActionResult FormatoDevolucionDT(int DocEntry)
        {
            Capa_Negocio.General_NEG.TablasSql.OWHS_N owhsN = new Capa_Negocio.General_NEG.TablasSql.OWHS_N();
            Usuario_E usu = (Usuario_E)Session["UsuarioId"];
            Capa_Negocio.Almacen_NEG.TablasSql.ORPD_N orpdN = new Capa_Negocio.Almacen_NEG.TablasSql.ORPD_N();
            Capa_Negocio.Almacen_NEG.TablasSql.CC_ORPD_N ccOrpdN = new Capa_Negocio.Almacen_NEG.TablasSql.CC_ORPD_N();
            Firmas_N firN = new Firmas_N();
            Firmas_E firE = new Firmas_E();

            Dictionary<string, int> listaEncargados = new Dictionary<string, int>
            {
                { "03", 185},					// Julio Roman Silva
				{ "05", 697},					// Jesus Angel Nunahuanca Cordova
				{ "06", 185},					// Julio Roman Silva
				{ "DEV07", 161},			// Carmen Condori Saravia
				{ "CUAR07", 161},			// Carmen Condori Saravia
			};

            var datosDevolucion = orpdN.ObtenerDevolucion(DocEntry);
            string[] arrWhsCode = { datosDevolucion.WhsCode };
            List<Capa_Entidad.General_ENT.TablasSql.OWHS_E> listaAlm = owhsN.listarAlmacenes(arrWhsCode);
            ViewBag.Almacenes = listaAlm;

            var ultimoEstado = ccOrpdN.UltimoEstadoCC_ORPD(DocEntry);

            // Descomentar cuando necesiten que la firma solo se muestre cuando haya culminado el proceso
            //if (ultimoEstado != null && listaAlm != null && (ultimoEstado.Equals("RECOGER") || ultimoEstado.Equals("TERMINAR")))
            //{
            string FilePath, FilePathDT;
            firE.DocEntryUsuario = listaEncargados[listaAlm[0].WhsCode];
            var firma = firN.ListarFirmas(firE);

            if (firma != null && firma.Count >= 1)
            {
                FilePath = firN.ListarFirmas(firE)[0].RutaFirma;
                ViewBag.ResponsableAlmacen = $"{firN.ListarFirmas(firE)[0].Nombres} {firN.ListarFirmas(firE)[0].Apellidos}";
                byte[] archivo = System.IO.File.ReadAllBytes(FilePath);

                var base64 = Convert.ToBase64String(archivo); //La propiedad de tu modelo que es byte[]
                ViewBag.Firmas = String.Format("data:image/gif;base64,{0}", base64); // Damos formato para indicar que se trata de una cadena base64
                                                                                     //}
            }

            FilePathDT = "D:\\COBEFARWEBFILES\\Firmas\\FirmaPamelaCollahuaSenosain.png";
            byte[] archivoDT = System.IO.File.ReadAllBytes(FilePathDT);
            var base64DT = Convert.ToBase64String(archivoDT); //La propiedad de tu modelo que es byte[]
            ViewBag.FirmaDT = String.Format("data:image/gif;base64,{0}", base64DT); // Damos formato para indicar que se trata de una cadena base64

            return View(datosDevolucion);
        }
        public ActionResult PdfFormatoDevolucionDT(int DocEntry, string LevConformidad)
        {
            return new ActionAsPdf("FormatoDevolucionDT", new { DocEntry = DocEntry, LevConformidad = LevConformidad }) { FileName = "FormatoDevolucionProveedor_" + DocEntry + ".pdf", PageOrientation = Rotativa.Options.Orientation.Landscape, PageSize = Rotativa.Options.Size.A4 };
        }

        public JsonResult VerificarExistenciaDevConFiltros(Capa_Entidad.Almacen_ENT.ReportesSql.RptFiltrosHistoricoDevoluciones_E devolucion)
        {
            //verificacionAccesos(0);         // Validar sesion logueada, solo para ajax

            Capa_Negocio.Almacen_NEG.TablasSql.ORPD_N orpdN = new Capa_Negocio.Almacen_NEG.TablasSql.ORPD_N();
            var result = orpdN.VerificarExistenciaDevolucion(devolucion);

            if (result)
            {
                return Json(new { Mensaje = "" });
            }
            else
            {
                return Json(new { Mensaje = "Sin Datos" });
            }
        }

        public ActionResult ExportarReporteHistoricoDevoluciones(Capa_Entidad.Almacen_ENT.ReportesSql.RptFiltrosHistoricoDevoluciones_E devolucion, int idOperation = 105)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);

            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                Capa_Negocio.Almacen_NEG.TablasSql.ORPD_N orpdN = new Capa_Negocio.Almacen_NEG.TablasSql.ORPD_N();
                string nombreArchivo = "ReporteHistoricoDevoluciones.xlsx";
                string excelContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                var devoluciones = orpdN.ExportarExcelDevoluciones(devolucion);

                if (devoluciones != null && devoluciones.Count >= 1)
                {
                    using (var libro = new ExcelPackage())
                    {
                        var worksheet = libro.Workbook.Worksheets.Add("ReporteHistoricoDevoluciones");
                        worksheet.Cells["A1"].LoadFromCollection(devoluciones, PrintHeaders: true);

                        if (devoluciones != null)
                        {
                            if (devoluciones.Count >= 1)
                            {
                                for (var col = 1; col <= 20; col++)
                                {
                                    worksheet.Column(col).AutoFit();
                                }

                                var tabla = worksheet.Tables.Add(new ExcelAddressBase(fromRow: 1, fromCol: 1, toRow: devoluciones.Count + 1, toColumn: 20), "ReporteHistoricoDevoluciones");
                                tabla.ShowHeader = true;
                                tabla.TableStyle = TableStyles.Medium2;
                            }
                        }

                        return File(libro.GetAsByteArray(), excelContentType, nombreArchivo);

                    }
                }
                else { return Content("No hay datos para exportar"); }
            }
            else
            {
                return resultadoAcceso;
            }
        }

        protected string CargarListaMotivosDevoluciones()
        {
            MotivosDevoluciones_N mdN = new MotivosDevoluciones_N();
            var motivos = mdN.ListarMotivosDevoluciones(null);
            string lista = string.Empty;
            string colorTexto = string.Empty;
            int num = 1;

            foreach (var md in motivos)
            {
                if (md.Estado.Equals("1"))
                {
                    colorTexto = "text-success";
                }
                else
                {
                    colorTexto = "text-danger";
                }

                string fila = "<tr>" +
                                $"<td class=\"text-center\">{num}</td>" +
                                $"<td class=\"text-center\">{md.Descripcion}</td>" +
                                $"<td class=\"text-center font-weight-bold {colorTexto}\">{md.DescEstado}</td>" +
                                $"<td class=\"text-center\"><button type=\"button\" class=\"btn btn-warning\" onclick=\"llenarCamposMotivo({md.IdMotivo}, '{md.Descripcion}')\"><i title=\"Editar Motivo\" class=\"icon-pencil\"></i></button></td>" +
                            "</tr>";

                lista += fila;
                ++num;
            }

            return lista;
        }

        public JsonResult NuevoMotivoDevolucion(MotivosDevoluciones_E motivo)
        {
            //verificacionAccesos(0);         // Validar sesion logueada, solo para ajax
            if (!string.IsNullOrEmpty(motivo.Descripcion))
            {
                Usuario_E usu = (Usuario_E)Session["UsuarioId"];
                MotivosDevoluciones_N mdN = new MotivosDevoluciones_N();

                motivo.Operario = $"{usu.Nombres} {usu.Apellidos}";
                var result = mdN.RegistrarMotivoDevolucion(motivo);
                var lista = mdN.ListarMotivosDevoluciones(new MotivosDevoluciones_E { Estado = "1" });

                var optionSelect = "<option value=\"\">Seleccione</option>";

                if (lista != null && lista.Count >= 1)
                {
                    foreach (var m in lista)
                    {
                        optionSelect += $"<option value=\"{m.IdMotivo}\">{m.Descripcion}</option>";
                    }
                }

                return Json(new { Lista = CargarListaMotivosDevoluciones(), ListaActualizadaParaObs = optionSelect, Mensaje = result });
            }
            else
            {
                return null;
            }
        }

        public JsonResult EditarMotivoDevolucion(MotivosDevoluciones_E motivo)
        {
            //verificacionAccesos(0);         // Validar sesion logueada, solo para ajax
            if (!string.IsNullOrEmpty(motivo.Descripcion))
            {
                Usuario_E usu = (Usuario_E)Session["UsuarioId"];
                MotivosDevoluciones_N mdN = new MotivosDevoluciones_N();

                motivo.Operario = $"{usu.Nombres} {usu.Apellidos}";
                var result = mdN.EditarMotivoDevolucion(motivo);

                return Json(new { Lista = CargarListaMotivosDevoluciones(), Mensaje = result });
            }
            else
            {
                return null;
            }
        }

        protected string CargarListaSubmotivosDevoluciones()
        {
            SubmotivosDevoluciones_N obsN = new SubmotivosDevoluciones_N();
            var submotivos = obsN.ListarSubmotivosDevoluciones(null);
            string lista = string.Empty;
            string colorTexto = string.Empty;
            int num = 1;

            foreach (var obs in submotivos)
            {
                if (obs.Estado.Equals("1"))
                {
                    colorTexto = "text-success";
                }
                else
                {
                    colorTexto = "text-danger";
                }

                string fila = "<tr>" +
                                $"<td class=\"text-center\">{num}</td>" +
                                $"<td class=\"text-center\">{obs.DescMotivo}</td>" +
                                $"<td class=\"text-center\">{obs.Descripcion}</td>" +
                                $"<td class=\"text-center font-weight-bold {colorTexto}\">{obs.DescEstado}</td>" +
                                $"<td class=\"text-center\"><button type=\"button\" class=\"btn btn-warning\" onclick=\"llenarCamposObservacion({obs.IdSubmotivo}, '{obs.Descripcion}', {obs.IdMotivo})\"><i title=\"Editar Motivo\" class=\"icon-pencil\"></i></button></td>" +
                            "</tr>";

                lista += fila;
                ++num;
            }

            return lista;
        }

        public JsonResult NuevoSubmotivoDevolucion(SubmotivosDevoluciones_E submotivo)
        {
            //verificacionAccesos(0);         // Validar sesion logueada, solo para ajax
            if (!string.IsNullOrEmpty(submotivo.Descripcion))
            {
                Usuario_E usu = (Usuario_E)Session["UsuarioId"];
                SubmotivosDevoluciones_N obsN = new SubmotivosDevoluciones_N();

                submotivo.Operario = $"{usu.Nombres} {usu.Apellidos}";
                var result = obsN.RegistrarSubmotivoDevolucion(submotivo);

                return Json(new { Lista = CargarListaSubmotivosDevoluciones(), Mensaje = result });
            }
            else
            {
                return null;
            }
        }

        public JsonResult EditarSubmotivoDevolucion(SubmotivosDevoluciones_E submotivo)
        {
            //verificacionAccesos(0);         // Validar sesion logueada, solo para ajax
            if (!string.IsNullOrEmpty(submotivo.Descripcion))
            {
                Usuario_E usu = (Usuario_E)Session["UsuarioId"];
                SubmotivosDevoluciones_N obsN = new SubmotivosDevoluciones_N();

                submotivo.Operario = $"{usu.Nombres} {usu.Apellidos}";
                var result = obsN.EditarSubmotivoDevolucion(submotivo);

                return Json(new { Lista = CargarListaSubmotivosDevoluciones(), Mensaje = result });
            }
            else
            {
                return null;
            }
        }

        /*************************************** I N V E N T A R I O ***************************************/
        public ActionResult ContabilizacionInventario(string msj = "", int idOperation = 1601)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);

            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                ViewBag.Mensaje = msj;
                ViewBag.Usuario = (Capa_Entidad.Seguridad_ENT.Usuario_E)Session["UsuarioId"];
                return View();
            }
            else
            {
                return resultadoAcceso;
            }
        }
        public ActionResult GestionPeriodos(Capa_Entidad.Almacen_ENT.TablasSql.OIPE_E filtro, int idOperation = 1602)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);

            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                ViewBag.Oipe = filtro;
                return View(new OIPE_N().listarPeriodosInventario(filtro));
            }
            else
            {
                return resultadoAcceso;
            }
        }
        public ActionResult NuevoPeriodo(int idOperation = 1603)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);

            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                ViewBag.Almacenes = new Capa_Negocio.General_NEG.Tablas.OWHS_N().ListarAlmacenes();
                return View();
            }
            else
            {
                return resultadoAcceso;
            }
        }
        [HttpPost]
        public ActionResult NuevoPeriodo(Capa_Entidad.Almacen_ENT.TablasSql.OIPE_E obj, int idOperation = 1603)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);

            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                try
                {
                    Usuario_E user = (Usuario_E)Session["UsuarioId"];
                    obj.Propietario = $"{user.Nombres} {user.Apellidos}";
                    new OIPE_N().Registrar(obj);
                    return RedirectToAction("GestionPeriodos");
                }
                catch (Exception e)
                { ViewBag.Mensaje = e.Message; ViewBag.Almacenes = new Capa_Negocio.General_NEG.Tablas.OWHS_N().ListarAlmacenes(); return View(obj); }
            }
            else
            {
                return resultadoAcceso;
            }
        }
        public ActionResult SeleccionarPeriodo(int id, int idOperation = 1604)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);

            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                return View(new OIPE_N().Buscar(id));
            }
            else
            {
                return resultadoAcceso;
            }

        }
        [ActionName("SeleccionarPeriodo")]
        [HttpPost]
        public ActionResult SeleccionarPeriodoPost(Capa_Entidad.Almacen_ENT.TablasSql.OIPE_E obj, int idOperation = 1604)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);

            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                ViewBag.Mensaje = "";
                OIPE_N oipeN = new OIPE_N();

                try
                {
                    Usuario_E user = (Usuario_E)Session["UsuarioId"];
                    obj.Operario = $"{user.Nombres} {user.Apellidos}";
                    ViewBag.Mensaje = obj.Operario + " ha seleccionado el periodo satisfactoriamente";
                    oipeN.Seleccionar(obj);
                    return View(oipeN.Buscar(obj.DocEntry));
                }
                catch (Exception e)
                {
                    ViewBag.Mensaje = "Error al seleccionar " + e.Message;
                    return View();
                }
            }
            else
            {
                return resultadoAcceso;
            }
        }
        public ActionResult EditarPeriodo(int id, int idOperation = 1605)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);

            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                ViewBag.Almacenes = new Capa_Negocio.General_NEG.Tablas.OWHS_N().ListarAlmacenes();
                return View(new OIPE_N().Buscar(id));
            }
            else
            {
                return resultadoAcceso;
            }
        }
        [HttpPost]
        public ActionResult EditarPeriodo(Capa_Entidad.Almacen_ENT.TablasSql.OIPE_E obj, int idOperation = 1605)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);

            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                Capa_Negocio.General_NEG.Tablas.OWHS_N owhsN = new Capa_Negocio.General_NEG.Tablas.OWHS_N(); OIPE_N oipeN = new OIPE_N();
                try
                {
                    Usuario_E user = (Usuario_E)Session["UsuarioId"];
                    obj.Operario = $"{user.Nombres} {user.Apellidos}";
                    oipeN.Editar(obj);
                    return RedirectToAction("GestionPeriodos", new { DocEntry = obj.DocEntry });
                }
                catch (Exception e)
                { ViewBag.Mensaje = e.Message; ViewBag.Almacenes = owhsN.ListarAlmacenes(); return View(obj); }
            }
            else
            {
                return resultadoAcceso;
            }
        }
        public ActionResult CerrarPeriodo(int id, string msj = "", int idOperation = 1606)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);

            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                ViewBag.Mensaje = msj;
                ViewBag.OIPECierre = new Capa_Negocio.Almacen_NEG.TablasSql.CC_OIPE_N().ListarCC_OIPE(id, "CERRAR");
                return View(new Capa_Negocio.Almacen_NEG.TablasSql.OIPE_N().Buscar(id));
            }
            else
            {
                return resultadoAcceso;
            }
        }
        public ActionResult CerrarPeriodoPost(Capa_Entidad.Almacen_ENT.TablasSql.OIPE_E obj, int idOperation = 1606)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);

            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                OIPE_N oipeN = new OIPE_N();
                try
                {
                    Usuario_E user = (Usuario_E)Session["UsuarioId"];
                    obj.Operario = $"{user.Nombres} {user.Apellidos}";
                    oipeN.Cerrar(obj);
                    return RedirectToAction("GestionPeriodos");
                }
                catch (Exception e)
                { return RedirectToAction("CerrarPeriodo", new { id = obj.DocEntry, msj = e.Message }); }
            }
            else
            {
                return resultadoAcceso;
            }
        }
        public ActionResult RevertirCierre(Capa_Entidad.Almacen_ENT.TablasSql.OIPE_E obj, int idOperation = 1607)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);

            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                try
                {
                    Usuario_E user = (Usuario_E)Session["UsuarioId"];
                    var Operario = $"{user.Nombres} {user.Apellidos}";
                    new OIPE_N().RevertirCerrar(obj.DocEntry, Operario);
                    return RedirectToAction("GestionPeriodos");
                }
                catch (Exception e)
                { return RedirectToAction("CerrarPeriodo", new { id = obj.DocEntry, msj = e.Message }); }
            }
            else
            {
                return resultadoAcceso;
            }
        }
        public ActionResult CargarDatosSap(int id, string Mensaje = "", int idOperation = 1608)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);

            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                OIPE_N oipeN = new OIPE_N();
                OIPE_E o = oipeN.Buscar(id, true);
                ViewBag.Oipe = o;
                ViewBag.Mensaje = Mensaje;
                return View(o.DetArticulos);
            }
            else
            {
                return resultadoAcceso;
            }
        }
        public ActionResult MigrarArticulos(Capa_Entidad.Almacen_ENT.TablasSql.OIPE_E obj, int idOperation = 1609)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);

            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                OIPE_N oipeN = new OIPE_N();
                try
                {
                    Usuario_E user = (Usuario_E)Session["UsuarioId"];
                    var Operario = $"{user.Nombres} {user.Apellidos}";
                    oipeN.MigrarArticulos(obj);
                    return RedirectToAction("CargarDatosSap", new { id = obj.DocEntry });
                }
                catch (Exception e)
                { return RedirectToAction("CargarDatosSap", new { id = obj.DocEntry, Mensaje = e.Message }); }
            }
            else
            {
                return resultadoAcceso;
            }
        }
        public ActionResult CargarArticulosMigrados(Capa_Entidad.Almacen_ENT.TablasSql.OIPE_E obj, int idOperation = 1610)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);

            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                OIPE_N oipeN = new OIPE_N();
                try
                {
                    Usuario_E user = (Usuario_E)Session["UsuarioId"];
                    obj.OpCarga = $"{user.Nombres} {user.Apellidos}";
                    oipeN.CargarArticulosMigrados(obj);
                    return RedirectToAction("CargarDatosSap", new { id = obj.DocEntry });
                }
                catch (Exception e)
                { return RedirectToAction("CargarDatosSap", new { id = obj.DocEntry, Mensaje = e.Message }); }
            }
            else
            {
                return resultadoAcceso;
            }
        }
        public ActionResult ExportarExcelArticulosCarga(int idOperation = 1611)
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

        /*********************************** E Q U I P O S   I N V E N T A R I O ***********************************/
        public ActionResult GestionEquipos(Capa_Entidad.Almacen_ENT.TablasSql.OIEQ_E filtro, int idOperation = 1612)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);

            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                try
                {
                    ViewBag.Oieq = filtro;
                    ViewBag.Almacenes = new Capa_Negocio.General_NEG.Tablas.OWHS_N().ListarAlmacenes();
                    return View(new OIEQ_N().listarEquipos(filtro));
                }
                catch (Exception e)
                {
                    return RedirectToAction("ContabilizacionInventario", new { msj = e.Message });
                }
            }
            else
            {
                return resultadoAcceso;
            }
        }
        public ActionResult NuevoEquipo(Capa_Entidad.Almacen_ENT.TablasSql.OIEQ_E obj, int idOperation = 1613)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);

            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                Capa_Negocio.General_NEG.Tablas.OWHS_N owhsN = new Capa_Negocio.General_NEG.Tablas.OWHS_N(); Capa_Negocio.Seguridad_NEG.Usuario_N ouN = new Capa_Negocio.Seguridad_NEG.Usuario_N(); OMRC_N omrcN = new OMRC_N();
                OIPE_N oipeN = new OIPE_N(); OIEQ_N oieqN = new OIEQ_N();

                ViewBag.Almacenes = owhsN.ListarAlmacenes();
                ViewBag.Miembros = ouN.ListaUsuarios(null);
                ViewBag.Periodos = oipeN.listarPeriodosInventario(null);
                ViewBag.Laboratorios = omrcN.listarFabricantes();
                obj.DocEntryPer = Capa_Entidad.Almacen_ENT.TablasSql.OIPE_E.PeriodoSeleccionado.DocEntry;
                Usuario_E user = (Usuario_E)Session["UsuarioId"];
                obj.Propietario = $"{user.Nombres} {user.Apellidos}";
                return View(oieqN.buscarEquipos(oieqN.separarEquipo(obj)));
            }
            else
            {
                return resultadoAcceso;
            }
        }
        [HttpPost]
        public ActionResult NuevoEquipo(Capa_Entidad.Almacen_ENT.TablasSql.OIEQ_E obj, int idOperation = 1613, int idRol = 0)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);

            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                Capa_Negocio.General_NEG.Tablas.OWHS_N owhsN = new Capa_Negocio.General_NEG.Tablas.OWHS_N();
                OIEQ_N oN = new OIEQ_N();
                Capa_Negocio.Seguridad_NEG.Usuario_N ouN = new Capa_Negocio.Seguridad_NEG.Usuario_N();
                OMRC_N omrcN = new OMRC_N();
                OIPE_N oipeN = new OIPE_N();
                try
                {
                    Usuario_E user = (Usuario_E)Session["UsuarioId"];
                    obj.Propietario = $"{user.Nombres} {user.Apellidos}";
                    oN.registrarNuevoEquipo(obj);
                    return RedirectToAction("GestionEquipos");
                }
                catch (Exception e)
                {
                    ViewBag.Mensaje = e.Message;
                    ViewBag.Almacenes = owhsN.ListarAlmacenes();
                    ViewBag.Miembros = ouN.ListaUsuarios(null);
                    ViewBag.Periodos = oipeN.listarPeriodosInventario(null);
                    ViewBag.Laboratorios = omrcN.listarFabricantes(); return View(obj);
                }
            }
            else
            {
                return resultadoAcceso;
            }
        }
        public ActionResult EditarEquipo(int id, int idOperation = 1614, int idRol = 0)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);

            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                Capa_Negocio.General_NEG.Tablas.OWHS_N owhsN = new Capa_Negocio.General_NEG.Tablas.OWHS_N();
                Capa_Negocio.Almacen_NEG.Tablas.OMRC_N l = new Capa_Negocio.Almacen_NEG.Tablas.OMRC_N();
                Capa_Negocio.Almacen_NEG.TablasSql.OIEQ_N oieqN = new Capa_Negocio.Almacen_NEG.TablasSql.OIEQ_N();
                Capa_Negocio.Almacen_NEG.TablasSql.OIPE_N oipeN = new Capa_Negocio.Almacen_NEG.TablasSql.OIPE_N();
                Capa_Negocio.Seguridad_NEG.Usuario_N ouN = new Capa_Negocio.Seguridad_NEG.Usuario_N();

                ViewBag.Almacenes = owhsN.ListarAlmacenes();
                ViewBag.Miembros = ouN.ListaUsuarios(new Capa_Entidad.Seguridad_ENT.Usuario_E { IdRol = idRol });
                ViewBag.Periodos = oipeN.listarPeriodosInventario(null);
                ViewBag.Laboratorios = l.listarFabricantes();
                return View(oieqN.buscarEquipos(id));
            }
            else
            {
                return resultadoAcceso;
            }
        }
        [HttpPost]
        public ActionResult EditarEquipo(Capa_Entidad.Almacen_ENT.TablasSql.OIEQ_E obj, int idOperation = 1614, int idRol = 0)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);

            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                Capa_Negocio.General_NEG.Tablas.OWHS_N owhsN = new Capa_Negocio.General_NEG.Tablas.OWHS_N();
                OMRC_N l = new OMRC_N();
                OIEQ_N oieqN = new OIEQ_N();
                OIPE_N oipeN = new OIPE_N();
                Usuario_N ouN = new Usuario_N();
                try
                {
                    Usuario_E user = (Usuario_E)Session["UsuarioId"];
                    obj.Propietario = $"{user.Nombres} {user.Apellidos}";
                    oieqN.editarEquipo(obj);
                    return RedirectToAction("GestionEquipos", new { DocEntry = obj.DocEntry });
                }
                catch (Exception e)
                {
                    ViewBag.Mensaje = e.Message;
                    ViewBag.Almacenes = owhsN.ListarAlmacenes();
                    ViewBag.Miembros = ouN.ListaUsuarios(new Usuario_E { IdRol = idRol });
                    ViewBag.Periodos = oipeN.listarPeriodosInventario(null);
                    ViewBag.Laboratorios = l.listarFabricantes(); return View(obj);
                }
            }
            else
            {
                return resultadoAcceso;
            }
        }
        public ActionResult EliminarEquipo(int id, int idOperation = 1615)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);

            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                ViewBag.Almacenes = new Capa_Negocio.General_NEG.Tablas.OWHS_N().ListarAlmacenes();
                ViewBag.Miembros = new Usuario_N().ListaUsuarios(null);
                ViewBag.Periodos = new OIPE_N().listarPeriodosInventario(null);
                ViewBag.Laboratorios = new OMRC_N().listarFabricantes();
                return View(new OIEQ_N().buscarEquipos(id));
            }
            else
            {
                return resultadoAcceso;
            }
        }
        [ActionName("EliminarEquipo")]
        [HttpPost]
        public ActionResult EliminarEquipoPost(int id, int idOperation = 1615)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);

            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                Capa_Negocio.General_NEG.Tablas.OWHS_N owhsN = new Capa_Negocio.General_NEG.Tablas.OWHS_N();
                OIEQ_N oieqN = new OIEQ_N();
                try
                {
                    oieqN.eliminarEquipo(id);
                    return RedirectToAction("GestionEquipos");
                }
                catch (Exception e)
                {
                    ViewBag.Mensaje = e.Message;
                    return View(oieqN.buscarEquipos(id));
                }
            }
            else
            {
                return resultadoAcceso;
            }
        }

        /*********************************** C O N T E O   I N V E N T A R I O ***********************************/
        public ActionResult GestionConteos(Capa_Entidad.Almacen_ENT.TablasSql.OIAR_E filtro, int idOperation = 1616, string tipo = "Conteo")
        {
            var resultadoAcceso = VerificarPermiso(idOperation);

            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                OIAR_N oiarN = new OIAR_N(); Capa_Negocio.General_NEG.Tablas.OWHS_N owhsN = new Capa_Negocio.General_NEG.Tablas.OWHS_N(); OIEQ_N oieqN = new OIEQ_N();
                filtro.Fase = 1;
                ViewBag.Oiar = filtro;
                Usuario_E user = (Usuario_E)Session["UsuarioId"];
                try
                {
                    List<Capa_Entidad.Almacen_ENT.TablasSql.OIEQ_E> equipoUsrPer2 = oieqN.buscarPertenenciaEquipo(user, Capa_Entidad.Almacen_ENT.TablasSql.OIPE_E.PeriodoSeleccionado, tipo);
                    ViewBag.Almacenes = owhsN.ListarAlmacenes();
                    return View(oiarN.Listar(filtro, user, tipo));
                }
                catch (Exception e)
                {
                    return RedirectToAction("ContabilizacionInventario", new { msj = e.Message });
                }
            }
            else
            {
                return resultadoAcceso;
            }
        }
        public ActionResult NuevoConteo(int idOperation = 1617)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);

            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                OIEQ_N oieqN = new OIEQ_N();
                Usuario_E user = (Usuario_E)Session["UsuarioId"];
                List<Capa_Entidad.Almacen_ENT.TablasSql.OIEQ_E> equipoUsrPer = oieqN.buscarEquipoUsrPer(user, Capa_Entidad.Almacen_ENT.TablasSql.OIPE_E.PeriodoSeleccionado);
                ViewBag.listaOieq = equipoUsrPer;
                return View();
            }
            else
            {
                return resultadoAcceso;
            }
        }

        [HttpPost]
        public ActionResult NuevoConteo(Capa_Entidad.Almacen_ENT.TablasSql.OIAR_E obj, int idOperation = 1617)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);

            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                OIAR_N oN = new OIAR_N(); OIEQ_N oieqN = new OIEQ_N();
                Usuario_E user = (Usuario_E)Session["UsuarioId"];
                try
                {
                    obj.Propietario = $"{user.Nombres} {user.Apellidos}";
                    oN.Registrar(obj);
                    return RedirectToAction("GestionConteos");
                }
                catch (Exception e)
                {
                    ViewBag.MensajePost = e.Message;
                    List<Capa_Entidad.Almacen_ENT.TablasSql.OIEQ_E> equipoUsrPer = oieqN.buscarEquipoUsrPer(user, Capa_Entidad.Almacen_ENT.TablasSql.OIPE_E.PeriodoSeleccionado);
                    ViewBag.listaOieq = equipoUsrPer;
                    return View(obj);
                }
            }
            else
            {
                return resultadoAcceso;
            }
        }
        public ActionResult IniciarConteo(int id, int idOperation = 1618)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);

            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                Usuario_N ouN = new Usuario_N(); OIAR_N oiarN = new OIAR_N();
                ViewBag.Miembros = ouN.ListaUsuarios(null);
                return View(oiarN.Buscar(id));
            }
            else
            {
                return resultadoAcceso;
            }
        }
        public ActionResult IniciarConteoPost(Capa_Entidad.Almacen_ENT.TablasSql.OIAR_E obj, int idOperation = 1618)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);

            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                OIAR_N oiarN = new OIAR_N();
                try
                {
                    Usuario_E user = (Usuario_E)Session["UsuarioId"];
                    obj.DetFases[0].Operario = $"{user.Nombres} {user.Apellidos}";
                    //obj.DetFases[0].TiempoFase = DateTime.Now;
                    oiarN.IniciarConteo(obj);
                    return RedirectToAction("GestionConteos", new { DocEntry = obj.DocEntry });
                }
                catch (Exception e)
                { return RedirectToAction("IniciarConteo", new { id = obj.DocEntry, msj = e.Message }); }
            }
            else
            {
                return resultadoAcceso;
            }
        }
        public ActionResult RevertirInicioConteo(Capa_Entidad.Almacen_ENT.TablasSql.OIAR_E obj, int idOperation = 1619)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);

            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                OIAR_N oiarN = new OIAR_N();
                try
                {
                    Usuario_E user = (Usuario_E)Session["UsuarioId"];
                    obj.DetFases[0].Operario = $"{user.Nombres} {user.Apellidos}";
                    oiarN.RevertirIniciarConteo(obj);
                    return RedirectToAction("GestionConteos", new { DocEntry = obj.DocEntry });
                }
                catch (Exception e)
                { return RedirectToAction("IniciarConteo", new { id = obj.DocEntry, msj = e.Message }); }
            }
            else
            {
                return resultadoAcceso;
            }
        }
        public ActionResult TerminarConteo(int id, int idOperation = 1620)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);

            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                Capa_Negocio.Almacen_NEG.TablasSql.OIAR_N oiarN = new Capa_Negocio.Almacen_NEG.TablasSql.OIAR_N(); Capa_Negocio.Almacen_NEG.TablasSql.IPE2_N ipe2N = new Capa_Negocio.Almacen_NEG.TablasSql.IPE2_N();
                OIAR_E obj = oiarN.Buscar(id);
                ViewBag.Lotes = ipe2N.listarArticulos(new IPE2_E { DocEntry = obj.DocEntryPer, ItemCode = obj.ItemCode, WhsCode = obj.WhsCode });
                return View(obj);
            }
            else
            {
                return resultadoAcceso;
            }
        }
        public ActionResult TerminarConteoPost(Capa_Entidad.Almacen_ENT.TablasSql.OIAR_E obj, int idOperation = 1620)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);

            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                OIAR_N oiarN = new OIAR_N();
                try
                {
                    Usuario_E user = (Usuario_E)Session["UsuarioId"];
                    obj.DetFases[0].Operario = $"{user.Nombres} {user.Apellidos}";

                    oiarN.TerminarConteo(obj);
                    return RedirectToAction("GestionConteos", new { DocEntry = obj.DocEntry });
                }
                catch (Exception e)
                {
                    return RedirectToAction("TerminarConteo", new { id = obj.DocEntry, msj = e.Message });
                }
            }
            else
            {
                return resultadoAcceso;
            }
        }
        public ActionResult RevertirTerminoConteo(Capa_Entidad.Almacen_ENT.TablasSql.OIAR_E obj, int idOperation = 1621)
        {
            //var resultadoAcceso = VerificarPermiso(idOperation);

            //if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)

            // {
            OIAR_N oiarN = new OIAR_N();
            try
            {
                Usuario_E user = (Usuario_E)Session["UsuarioId"];
                obj.DetFases[0].Operario = $"{user.Nombres} {user.Apellidos}";
                oiarN.RevertirTerminoConteo(obj);
                return RedirectToAction("GestionConteos", new { DocEntry = obj.DocEntry });
            }
            catch (Exception e)
            { return RedirectToAction("TerminarConteo", new { id = obj.DocEntry, msj = e.Message }); }
            //}
            //else
            //{
            // return resultadoAcceso;
            //}
        }

        /*********************************** R E C O N T E O   I N V E N T A R I O ***********************************/
        public ActionResult GestionReconteos(Capa_Entidad.Almacen_ENT.TablasSql.OIAR_E filtro, int idOperation = 1622, string tipo = "Reconteo")
        {
            var resultadoAcceso = VerificarPermiso(idOperation);

            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                OIEQ_N oieqN = new OIEQ_N(); OIAR_N oiarN = new OIAR_N(); Capa_Negocio.General_NEG.Tablas.OWHS_N owhsN = new Capa_Negocio.General_NEG.Tablas.OWHS_N();
                filtro.Fase = 3;
                ViewBag.Oiar = filtro;
                Usuario_E user = (Usuario_E)Session["UsuarioId"];
                try
                {
                    List<Capa_Entidad.Almacen_ENT.TablasSql.OIEQ_E> equipoUsrPer2 = oieqN.buscarPertenenciaEquipo(user, Capa_Entidad.Almacen_ENT.TablasSql.OIPE_E.PeriodoSeleccionado, tipo);
                    ViewBag.Almacenes = owhsN.ListarAlmacenes();
                    return View(oiarN.Listar(filtro, user, tipo));
                }
                catch (Exception e)
                {
                    return RedirectToAction("ContabilizacionInventario", new { msj = e.Message });
                }
            }
            else
            {
                return resultadoAcceso;
            }
        }
        public ActionResult IniciarReconteo(int id, int idOperation = 1623)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);

            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                Usuario_N ouN = new Usuario_N(); OIAR_N oiarN = new OIAR_N();
                ViewBag.Miembros = ouN.ListaUsuarios(null);
                return View(oiarN.Buscar(id));
            }
            else
            {
                return resultadoAcceso;
            }
        }
        public ActionResult IniciarReconteoPost(Capa_Entidad.Almacen_ENT.TablasSql.OIAR_E obj, int idOperation = 1623)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);

            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                OIAR_N oiarN = new OIAR_N();
                try
                {
                    Usuario_E user = (Usuario_E)Session["UsuarioId"];
                    obj.DetFases[0].Operario = $"{user.Nombres} {user.Apellidos}";
                    oiarN.IniciarReconteo(obj);
                    return RedirectToAction("GestionReconteos", new { DocEntry = obj.DocEntry });
                }
                catch (Exception e)
                { return RedirectToAction("IniciarReconteo", new { id = obj.DocEntry, msj = e.Message }); }
            }
            else
            {
                return resultadoAcceso;
            }
        }
        public ActionResult RevertirInicioReconteo(Capa_Entidad.Almacen_ENT.TablasSql.OIAR_E obj, int idOperation = 1624)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);

            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                OIAR_N oiarN = new OIAR_N();
                try
                {
                    Usuario_E user = (Usuario_E)Session["UsuarioId"];
                    obj.DetFases[0].Operario = $"{user.Nombres} {user.Apellidos}";
                    oiarN.RevertirIniciarReconteo(obj);
                    return RedirectToAction("GestionReconteos", new { DocEntry = obj.DocEntry });
                }
                catch (Exception e)
                { return RedirectToAction("IniciarReconteo", new { id = obj.DocEntry, msj = e.Message }); }
            }
            else
            {
                return resultadoAcceso;
            }
        }
        public ActionResult TerminarReconteo(int id, int idOperation = 1625)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);

            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                Capa_Negocio.Almacen_NEG.TablasSql.OIAR_N oiarN = new Capa_Negocio.Almacen_NEG.TablasSql.OIAR_N(); Capa_Negocio.Almacen_NEG.TablasSql.IPE2_N ipe2N = new Capa_Negocio.Almacen_NEG.TablasSql.IPE2_N();
                OIAR_E obj = oiarN.Buscar(id);
                ViewBag.Lotes = ipe2N.listarArticulos(new IPE2_E { DocEntry = obj.DocEntryPer, ItemCode = obj.ItemCode, WhsCode = obj.WhsCode });
                return View(obj);
            }
            else
            {
                return resultadoAcceso;
            }
        }
        public ActionResult TerminarReconteoPost(Capa_Entidad.Almacen_ENT.TablasSql.OIAR_E obj, int idOperation = 1625)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);

            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                OIAR_N oiarN = new OIAR_N();
                try
                {
                    Usuario_E user = (Usuario_E)Session["UsuarioId"];
                    obj.DetFases[0].Operario = $"{user.Nombres} {user.Apellidos}";
                    oiarN.TerminarReconteo(obj);
                    return RedirectToAction("GestionReconteos", new { DocEntry = obj.DocEntry });
                }
                catch (Exception e)
                {
                    return RedirectToAction("TerminarReconteo", new { id = obj.DocEntry, msj = e.Message });
                }
            }
            else
            {
                return resultadoAcceso;
            }
        }
        public ActionResult RevertirTerminoReconteo(Capa_Entidad.Almacen_ENT.TablasSql.OIAR_E obj, int idOperation = 1626)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);

            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                OIAR_N oiarN = new OIAR_N();
                try
                {
                    Usuario_E user = (Usuario_E)Session["UsuarioId"];
                    obj.DetFases[0].Operario = $"{user.Nombres} {user.Apellidos}";
                    oiarN.RevertirTerminoReconteo(obj);
                    return RedirectToAction("GestionReconteos", new { DocEntry = obj.DocEntry });
                }
                catch (Exception e)
                { return RedirectToAction("TerminarReconteo", new { id = obj.DocEntry, msj = e.Message }); }
            }
            else
            {
                return resultadoAcceso;
            }
        }

        /******************************* A N Á L I S I S   C O N T E O   I N V E N T A R I O *******************************/
        public ActionResult GestionAnalisisConteos(Capa_Entidad.Almacen_ENT.TablasSql.OIAR_E filtro, int idOperation = 1627, string tipo = "Analisis")
        {
            var resultadoAcceso = VerificarPermiso(idOperation);

            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                OIAR_N oiarN = new OIAR_N();
                Capa_Negocio.General_NEG.Tablas.OWHS_N owhsN = new Capa_Negocio.General_NEG.Tablas.OWHS_N();
                OIEQ_N oieqN = new OIEQ_N();
                Usuario_E user = (Usuario_E)Session["UsuarioId"];
                filtro.Fase = 5;
                ViewBag.Oiar = filtro;
                try
                {
                    List<Capa_Entidad.Almacen_ENT.TablasSql.OIEQ_E> equipoUsrPer2 = oieqN.buscarPertenenciaEquipo(user, Capa_Entidad.Almacen_ENT.TablasSql.OIPE_E.PeriodoSeleccionado, tipo);
                    ViewBag.Almacenes = owhsN.ListarAlmacenes();
                    return View(oiarN.Listar(filtro, user, tipo));
                }
                catch (Exception e)
                {
                    return RedirectToAction("ContabilizacionInventario", new { msj = e.Message });
                }
            }
            else
            {
                return resultadoAcceso;
            }
        }
        public ActionResult IniciarAnalisisConteo(int id, int idOperation = 1628)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);

            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                Capa_Negocio.Seguridad_NEG.Usuario_N ouN = new Capa_Negocio.Seguridad_NEG.Usuario_N(); Capa_Negocio.Almacen_NEG.TablasSql.OIAR_N oiarN = new Capa_Negocio.Almacen_NEG.TablasSql.OIAR_N();
                ViewBag.Miembros = ouN.ListaUsuarios(null);
                return View(oiarN.Buscar(id));
            }
            else
            {
                return resultadoAcceso;
            }
        }
        public ActionResult IniciarAnalisisConteoPost(Capa_Entidad.Almacen_ENT.TablasSql.OIAR_E obj, int idOperation = 1628)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);

            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                OIAR_N oiarN = new OIAR_N();
                try
                {
                    Usuario_E user = (Usuario_E)Session["UsuarioId"];
                    obj.DetFases[0].Operario = $"{user.Nombres} {user.Apellidos}";
                    oiarN.IniciarAnalisisConteo(obj);
                    return RedirectToAction("GestionAnalisisConteos", new { DocEntry = obj.DocEntry });
                }
                catch (Exception e)
                { return RedirectToAction("IniciarReconteo", new { id = obj.DocEntry, msj = e.Message }); }
            }
            else
            {
                return resultadoAcceso;
            }
        }
        public ActionResult RevertirInicioAnalisisConteo(Capa_Entidad.Almacen_ENT.TablasSql.OIAR_E obj, int idOperation = 1629)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);

            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                OIAR_N oiarN = new OIAR_N();
                try
                {
                    Usuario_E user = (Usuario_E)Session["UsuarioId"];
                    obj.DetFases[0].Operario = $"{user.Nombres} {user.Apellidos}";
                    oiarN.RevertirIniciarAnalisisConteo(obj);
                    return RedirectToAction("GestionAnalisisConteos", new { DocEntry = obj.DocEntry });
                }
                catch (Exception e)
                { return RedirectToAction("IniciarReconteo", new { id = obj.DocEntry, msj = e.Message }); }
            }
            else
            {
                return resultadoAcceso;
            }
        }
        public ActionResult TerminarAnalisisConteo(int id, int idOperation = 1630)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);

            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                Capa_Negocio.Almacen_NEG.TablasSql.OIAR_N oiarN = new Capa_Negocio.Almacen_NEG.TablasSql.OIAR_N(); Capa_Negocio.Almacen_NEG.TablasSql.IPE2_N ipe2N = new Capa_Negocio.Almacen_NEG.TablasSql.IPE2_N();
                OIAR_E obj = oiarN.Buscar(id);
                ViewBag.Lotes = ipe2N.listarArticulos(new IPE2_E { DocEntry = obj.DocEntryPer, ItemCode = obj.ItemCode, WhsCode = obj.WhsCode });
                return View(obj);
            }
            else
            {
                return resultadoAcceso;
            }
        }
        public ActionResult TerminarAnalisisConteoPost(Capa_Entidad.Almacen_ENT.TablasSql.OIAR_E obj, int idOperation = 1630)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);

            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                OIAR_N oiarN = new OIAR_N();
                try
                {
                    Usuario_E user = (Usuario_E)Session["UsuarioId"];
                    obj.DetFases[0].Operario = $"{user.Nombres} {user.Apellidos}";
                    oiarN.TerminarAnalisisConteo(obj);
                    return RedirectToAction("GestionAnalisisConteos", new { DocEntry = obj.DocEntry });
                }
                catch (Exception e)
                {
                    return RedirectToAction("TerminarAnalisisConteo", new { id = obj.DocEntry, msj = e.Message });
                }
            }
            else
            {
                return resultadoAcceso;
            }
        }
        public ActionResult RevertirTerminoAnalisisConteo(Capa_Entidad.Almacen_ENT.TablasSql.OIAR_E obj, int idOperation = 1631)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);

            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                OIAR_N oiarN = new OIAR_N();
                try
                {
                    Usuario_E user = (Usuario_E)Session["UsuarioId"];
                    obj.DetFases[0].Operario = $"{user.Nombres} {user.Apellidos}";
                    oiarN.RevertirTerminoAnalisisConteo(obj);
                    return RedirectToAction("GestionAnalisisConteos", new { DocEntry = obj.DocEntry });
                }
                catch (Exception e)
                { return RedirectToAction("TerminarAnalisisConteo", new { id = obj.DocEntry, msj = e.Message }); }
            }
            else
            {
                return resultadoAcceso;
            }
        }

        /******************* R E P O R T E S   I N V E N T A R I O *******************/
        public ActionResult ReportesContabilizacionInventario(int idOperation = 1632)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);

            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                OMRC_N omrcN = new OMRC_N();
                OIAR_N oiarN = new OIAR_N();
                try
                {
                    if (Capa_Entidad.Almacen_ENT.TablasSql.OIPE_E.PeriodoSeleccionado == null || Capa_Entidad.Almacen_ENT.TablasSql.OIPE_E.PeriodoSeleccionado.DocEntry == 0) { throw new Exception("no hay periodo Seleccionado"); }
                    oiarN.validarVistaReportes(Capa_Entidad.Almacen_ENT.TablasSql.OIPE_E.PeriodoSeleccionado.DocEntry);
                    ViewBag.Laboratorios = omrcN.listarFabricantes();
                    return View();
                }
                catch (Exception e)
                {
                    return RedirectToAction("ContabilizacionInventario", new { msj = e.Message });
                }
            }
            else
            {
                return resultadoAcceso;
            }
        }

        /*********************** V A L I D A C I O N E S ***********************/
        public ActionResult validarNuevoPeriodo(Capa_Entidad.Almacen_ENT.TablasSql.OIPE_E obj)
        {
            OIPE_N oipeN = new OIPE_N();
            string status = "true";
            try
            {
                oipeN.validarNuevoPeriodo(obj);
                return Content(status);
            }
            catch (Exception e) { return Content(e.Message); }
        }
        public ActionResult validarEquipo(Capa_Entidad.Almacen_ENT.TablasSql.OIEQ_E obj)
        {
            OIEQ_N oieqN = new OIEQ_N();
            string status = "true";
            try
            {
                oieqN.validarEquipo(obj);
                return Content(status);
            }
            catch (Exception e) { return Content(e.Message); }
        }
        public ActionResult validarNuevoConteo(Capa_Entidad.Almacen_ENT.TablasSql.OIAR_E obj)
        {
            OIAR_N oiarN = new OIAR_N();
            string status = "true";
            try
            {
                oiarN.validarNuevoConteo(obj);
                return Content(status);
            }
            catch (Exception e) { return Content(e.Message); }
        }
        public ActionResult validarInicioConteo(Capa_Entidad.Almacen_ENT.TablasSql.OIAR_E obj)
        {
            OIAR_N oiarN = new OIAR_N();
            string status = "true";
            try
            {
                oiarN.validarInicioConteo(obj);
                return Content(status);
            }
            catch (Exception e) { return Content(e.Message); }
        }
        public ActionResult validarRevertirInicioConteo(Capa_Entidad.Almacen_ENT.TablasSql.OIAR_E obj)
        {
            OIAR_N oiarN = new OIAR_N();
            string status = "true";
            try
            {
                Usuario_E user = (Usuario_E)Session["UsuarioId"];
                obj.DetFases[0].Operario = $"{user.Nombres} {user.Apellidos}";
                oiarN.validarRevertirInicioConteo(obj);
                return Content(status);
            }
            catch (Exception e) { return Content(e.Message); }
        }
        public ActionResult validarTerminoConteo(Capa_Entidad.Almacen_ENT.TablasSql.OIAR_E obj)
        {
            OIAR_N oiarN = new OIAR_N();
            string status = "true";
            try
            {
                Usuario_E user = (Usuario_E)Session["UsuarioId"];
                obj.DetFases[0].Operario = $"{user.Nombres} {user.Apellidos}";
                oiarN.validarTerminoConteo(obj);
                return Content(status);
            }
            catch (Exception e) { return Content(e.Message); }
        }
        public ActionResult validarRevertirTerminoConteo(Capa_Entidad.Almacen_ENT.TablasSql.OIAR_E obj)
        {
            OIAR_N oiarN = new OIAR_N();
            string status = "true";
            try
            {
                Usuario_E user = (Usuario_E)Session["UsuarioId"];
                obj.DetFases[0].Operario = $"{user.Nombres} {user.Apellidos}";
                oiarN.validarRevertirTerminoConteo(obj);
                return Content(status);
            }
            catch (Exception e) { return Content(e.Message); }
        }
        public ActionResult validarInicioReconteo(Capa_Entidad.Almacen_ENT.TablasSql.OIAR_E obj)
        {
            OIAR_N oiarN = new OIAR_N();
            string status = "true";
            try
            {
                oiarN.validarInicioReconteo(obj);
                return Content(status);
            }
            catch (Exception e) { return Content(e.Message); }
        }
        public ActionResult validarRevertirInicioReconteo(Capa_Entidad.Almacen_ENT.TablasSql.OIAR_E obj)
        {
            OIAR_N oiarN = new OIAR_N();
            string status = "true";
            try
            {
                Usuario_E user = (Usuario_E)Session["UsuarioId"];
                obj.DetFases[0].Operario = $"{user.Nombres} {user.Apellidos}";
                oiarN.validarRevertirInicioReconteo(obj);
                return Content(status);
            }
            catch (Exception e) { return Content(e.Message); }
        }
        public ActionResult validarTerminoReconteo(Capa_Entidad.Almacen_ENT.TablasSql.OIAR_E obj)
        {
            OIAR_N oiarN = new OIAR_N();
            string status = "true";
            try
            {
                Usuario_E user = (Usuario_E)Session["UsuarioId"];
                obj.DetFases[0].Operario = $"{user.Nombres} {user.Apellidos}";
                oiarN.validarTerminoReconteo(obj);
                return Content(status);
            }
            catch (Exception e) { return Content(e.Message); }
        }
        public ActionResult validarRevertirTerminoReconteo(Capa_Entidad.Almacen_ENT.TablasSql.OIAR_E obj)
        {
            OIAR_N oiarN = new OIAR_N();
            string status = "true";
            try
            {
                Usuario_E user = (Usuario_E)Session["UsuarioId"];
                obj.DetFases[0].Operario = $"{user.Nombres} {user.Apellidos}";
                oiarN.validarRevertirTerminoReconteo(obj);
                return Content(status);
            }
            catch (Exception e) { return Content(e.Message); }
        }
        public ActionResult validarInicioAnalisisConteo(Capa_Entidad.Almacen_ENT.TablasSql.OIAR_E obj)
        {
            OIAR_N oiarN = new OIAR_N();
            string status = "true";
            try
            {
                oiarN.validarInicioAnalisisConteo(obj);
                return Content(status);
            }
            catch (Exception e) { return Content(e.Message); }
        }
        public ActionResult validarRevertirInicioAnalisisConteo(Capa_Entidad.Almacen_ENT.TablasSql.OIAR_E obj)
        {
            OIAR_N oiarN = new OIAR_N();
            string status = "true";
            try
            {
                Usuario_E user = (Usuario_E)Session["UsuarioId"];
                obj.DetFases[0].Operario = $"{user.Nombres} {user.Apellidos}";
                oiarN.validarRevertirInicioAnalisisConteo(obj);
                return Content(status);
            }
            catch (Exception e) { return Content(e.Message); }
        }
        public ActionResult validarTerminoAnalisisConteo(Capa_Entidad.Almacen_ENT.TablasSql.OIAR_E obj)
        {
            OIAR_N oiarN = new OIAR_N();
            string status = "true";
            try
            {
                Usuario_E user = (Usuario_E)Session["UsuarioId"];
                obj.DetFases[0].Operario = $"{user.Nombres} {user.Apellidos}";
                oiarN.validarTerminoAnalisisConteo(obj);
                return Content(status);
            }
            catch (Exception e) { return Content(e.Message); }
        }
        public ActionResult validarRevertirTerminoAnalisisConteo(Capa_Entidad.Almacen_ENT.TablasSql.OIAR_E obj)
        {
            OIAR_N oiarN = new OIAR_N();
            string status = "true";
            try
            {
                Usuario_E user = (Usuario_E)Session["UsuarioId"];
                obj.DetFases[0].Operario = $"{user.Nombres} {user.Apellidos}";
                oiarN.validarRevertirTerminoAnalisisConteo(obj);
                return Content(status);
            }
            catch (Exception e) { return Content(e.Message); }
        }
        // metodos 
        public JsonResult listarArticulosUsrEquiSap(Capa_Entidad.Almacen_ENT.TablasSql.OIEQ_E eq)
        {
            Capa_Negocio.Almacen_NEG.TablasSql.OIEQ_N oieqN = new Capa_Negocio.Almacen_NEG.TablasSql.OIEQ_N();
            Capa_Negocio.Almacen_NEG.TablasSql.IPE2_N ipe2N = new Capa_Negocio.Almacen_NEG.TablasSql.IPE2_N();
            List<Capa_Entidad.Almacen_ENT.TablasSql.IPE2_E> obj = ipe2N.listarArticulosUsrEquiSap(oieqN.buscarEquipos(eq.DocEntry));
            return Json(obj);
        }
        public JsonResult listarArticulosUsrEquiSql(Capa_Entidad.Almacen_ENT.TablasSql.OIEQ_E eq)
        {
            Capa_Negocio.Almacen_NEG.TablasSql.OIEQ_N oieqN = new Capa_Negocio.Almacen_NEG.TablasSql.OIEQ_N();
            Capa_Negocio.Almacen_NEG.TablasSql.IPE2_N ipe2N = new Capa_Negocio.Almacen_NEG.TablasSql.IPE2_N();
            List<Capa_Entidad.Almacen_ENT.TablasSql.IPE2_E> obj = ipe2N.listarArticulosUsrEqui(oieqN.buscarEquipos(eq.DocEntry));
            return Json(obj);
        }
        public JsonResult listarLotesSinStock(Capa_Entidad.Almacen_ENT.TablasSql.IPE2_E ip)
        {
            Capa_Negocio.Almacen_NEG.TablasSql.IPE2_N ipe2N = new Capa_Negocio.Almacen_NEG.TablasSql.IPE2_N();
            List<Capa_Entidad.Almacen_ENT.TablasSql.IPE2_E> obj = ipe2N.listarLotesSinStock(ip);
            return Json(obj);
        }
        public ActionResult dataListArticulos(Capa_Entidad.Almacen_ENT.Tablas.OITM_E o)
        {
            OITM_N oitmN = new OITM_N();
            return Content(oitmN.datalistArticulos(o));
        }
        //reportes
        public ActionResult reporteViewer()
        {
            return View();
        }
        public ActionResult tbReporteContabilizacionInventario(Capa_Entidad.Almacen_ENT.TablasSql.OIAR_E o)
        {
            o.DocEntryPer = Capa_Entidad.Almacen_ENT.TablasSql.OIPE_E.PeriodoSeleccionado.DocEntry;
            Capa_Negocio.Almacen_NEG.ReportesSql.ContInv_N contInv_N = new Capa_Negocio.Almacen_NEG.ReportesSql.ContInv_N();
            ReportViewer rp = new ReportViewer();
            try
            {
                rp.ProcessingMode = ProcessingMode.Local;
                rp.SizeToReportContent = true;
                if (o.Fase == 3)
                {
                    rp.LocalReport.ReportPath =
                        Request.MapPath(Request.ApplicationPath) + @"Reportes\RptInventario\RptContInvC.rdlc";
                    rp.LocalReport.DataSources.Add(new ReportDataSource("DS_RptContInv", contInv_N.tbRptContInv(o)));
                }
                else if (o.Fase == 5)
                {
                    rp.LocalReport.ReportPath =
                        Request.MapPath(Request.ApplicationPath) + @"Reportes\RptInventario\RptContInvR.rdlc";
                    rp.LocalReport.DataSources.Add(new ReportDataSource("DS_RptContInv", contInv_N.tbRptContInv(o)));
                }
                else if (o.Fase == 7)
                {
                    rp.LocalReport.ReportPath =
                        Request.MapPath(Request.ApplicationPath) + @"Reportes\RptInventario\RptContInvA.rdlc";
                    rp.LocalReport.DataSources.Add(new ReportDataSource("DS_RptContInv", contInv_N.tbRptContInv(o)));
                }

                ViewBag.REPORTE = rp;
            }
            catch { }
            return View("reporteViewer");
        }

        public JsonResult ObtenerDatosProducto(Capa_Entidad.Almacen_ENT.Tablas.OIBT_E datos, string limite = "1")
        {
            //verificacionAccesos(0); // Validar sesion logueada, solo para ajax
            if (!string.IsNullOrEmpty(datos.ItemCode) || !string.IsNullOrEmpty(datos.ItemName))
            {
                Capa_Negocio.Almacen_NEG.Tablas.OIBT_N oibtN = new Capa_Negocio.Almacen_NEG.Tablas.OIBT_N();
                var result = oibtN.listarArticulosLotes(datos, false, limite);

                return Json(result);
            }
            else
            {
                return null;
            }
        }
    }
}