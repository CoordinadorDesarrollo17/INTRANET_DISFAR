using Aspose.Pdf.Operators;
using Capa_Datos.General_DAO.TablasSql;
using Capa_Entidad.General_ENT.TablasSql;
using Capa_Entidad.RecursosHumanos_ENT.Auditorias;
using Capa_Entidad.RecursosHumanos_ENT.Reportes;
using Capa_Entidad.RecursosHumanos_ENT.TablasSQL;
using Capa_Entidad.Seguridad_ENT;
using Capa_Negocio.General_NEG.TablasSql;
using Capa_Negocio.RecursosHumanos_NEG.TablasSQL;
using Capa_Usuario.Helpers;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
namespace Capa_Usuario.Controllers
{
    public class RecursosHumanosController : Controller
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
        private readonly Capa_Negocio.Helpers helper = new Capa_Negocio.Helpers();
        private SEDE_N sedeN = new SEDE_N();
        private CARGO_N cargoN = new CARGO_N();
        private OEMPL_N emplN = new OEMPL_N();
        [HttpGet]
        public ActionResult Index(int idOperation = 4000)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                Usuario_E usu = (Usuario_E)Session["UsuarioId"];
                ViewBag.Departamentos = new Capa_Negocio.RecursosHumanos_NEG.TablasSQL.ODPTO_N().ListarDepartamentos(new ODPTO_E { Estado = "Y" });
                ViewBag.NumCorporativos = new Capa_Negocio.RecursosHumanos_NEG.TablasSQL.ONUM_N().ListarNumeros(new ONUM_E { Estado = "1", Asignado = "0" });
                var filtroEmpleado = new int[] { 1, 56 }.Contains(usu.IdRol) ? new OEMPL_E { Estado = "1" } : null;
                int? rolId = usu != null ? usu.IdRol : (int?)null;
                var datosEmpleados = emplN.ListarEmpleados(filtroEmpleado, rolId);
                ViewBag.CantidadEmpleados = datosEmpleados != null ? datosEmpleados.Count : 0;
                ViewBag.IdRol = usu?.IdRol ?? 0;
                return View();
            }
            else
            {
                return resultadoAcceso;
            }
        }
        [HttpGet]
        public ActionResult DirectorioTelefonico()
        {
            ViewBag.Departamentos = new Capa_Negocio.RecursosHumanos_NEG.TablasSQL.ODPTO_N().ListarDepartamentos(new ODPTO_E { Estado = "Y" }).Where(x => x.IdDepartamento != 18);
            ViewBag.NumCorporativos = new Capa_Negocio.RecursosHumanos_NEG.TablasSQL.ONUM_N().ListarNumeros(new ONUM_E { Estado = "1", Asignado = "0" });
            Usuario_E usu = (Usuario_E)Session["UsuarioId"];
            int? rolId = usu != null ? usu.IdRol : (int?)null;
            var datosEmpleados = emplN.ListarEmpleados(new OEMPL_E { VistaExterna = "SI" }, rolId);
            ViewBag.CantidadEmpleados = datosEmpleados != null ? datosEmpleados.Count : 0;
            ViewBag.Vista = "EXTERNA";
            return View("Index");
        }
        [HttpGet]
        public ActionResult ListarUbigeos(UBIG_E filtros, string mostrarSeleccionar, string mostrarZona)
        {
            var datosUbigeos = new UBIG_D().Listar(filtros);
            ViewBag.MostrarSeleccionar = !string.IsNullOrWhiteSpace(mostrarSeleccionar) ? mostrarSeleccionar.ToUpper() : "";
            ViewBag.MostrarZona = !string.IsNullOrWhiteSpace(mostrarZona) ? mostrarZona.ToUpper() : "";
            return PartialView("ListadoUbigeos", datosUbigeos);
        }
        /********************* S E D E S *********************/
        [HttpGet]
        public ActionResult ListarSedes(SEDE_E filtros)
        {
            var datosSedes = sedeN.ListarSedes(filtros);
            return PartialView("RecursosHumanos/ListadoSedes", datosSedes);
        }
        public JsonResult CargarSedes()
        {
            var sedes = sedeN.ListarSedes(new SEDE_E { Estado = "1" });
            bool listaValida = (sedes != null && sedes.Count >= 1);
            string mensaje = listaValida ? "OK" : "ERROR";
            return Json(new { Lista = sedes, Mensaje = mensaje });
        }
        public JsonResult AgregarSede(SEDE_E form, int idOperation = 4002)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                if (form == null)
                {
                    return Json(new { Mensaje = "Error crítico", Comentario = new List<string>() { "El objeto recibido es nulo." }, Icono = "error" });
                }
                Usuario_E usu = (Usuario_E)Session["UsuarioId"];
                form.UsuarioOperacion = $"{usu.Nombres} {usu.Apellidos}";
                var listaMensajes = sedeN.ValidarRegistroSede(form);
                bool errores = (listaMensajes != null && listaMensajes.Count >= 1);
                string mensaje = errores ? "No se pudo completar la acción" : "¡Acción realizada con éxito!";
                string iconoMensaje = errores ? "warning" : "success";
                return Json(new { Mensaje = mensaje, Comentario = listaMensajes, Icono = iconoMensaje });
            }
            else
            {
                return Json(new { Mensaje = "Error crítico", Comentario = new List<string>() { "No cuentas con permisos para realizar esta acción." }, Icono = "error" });
            }
        }
        public JsonResult EditarSede(SEDE_E form, int idOperation = 4002)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                if (form == null || (form != null && form.Id < 0))
                {
                    return Json(new { Mensaje = "Error crítico", Comentario = new List<string>() { "Sede no válida." }, Icono = "error" });
                }
                var mensajeError = sedeN.EditarSede(form);
                string mensaje = string.IsNullOrWhiteSpace(mensajeError) ? "¡Acción realizada con éxito!" : "No se pudo completar la acción";
                string iconoMensaje = string.IsNullOrWhiteSpace(mensajeError) ? "success" : "warning";
                return Json(new { Mensaje = mensaje, Comentario = new List<string>() { mensajeError }, Icono = iconoMensaje });
            }
            else
            {
                return Json(new { Mensaje = "Error crítico", Comentario = new List<string>() { "No cuentas con permisos para realizar esta acción." }, Icono = "error" });
            }
        }
        //Se espera que no haya eliminacion de sedes en el directorio.
        //public JsonResult EliminarSede(int idSede, int idOperation = 4002)
        //{
        //    string acceso = VerificarAccesos(idOperation);
        //    if (string.IsNullOrWhiteSpace(acceso) || !acceso.Equals("C_Access"))
        //    {
        //        return Json(new { Mensaje = "Error crítico", Comentario = new List<string>() { "No cuentas con permisos para realizar esta acción." }, Icono = "error" });
        //    }
        //    if (idSede <= 0)
        //    {
        //        return Json(new { Mensaje = "Error crítico", Comentario = new List<string>() { "Sede no válida." }, Icono = "error" });
        //    }
        //    var mensajeError = sedeN.EliminarSede(idSede);
        //    string mensaje = string.IsNullOrWhiteSpace(mensajeError) ? "¡Acción realizada con éxito!" : "No se pudo completar la acción";
        //    string iconoMensaje = string.IsNullOrWhiteSpace(mensajeError) ? "success" : "warning";
        //    return Json(new { Mensaje = mensaje, Comentario = new List<string>() { mensajeError }, Icono = iconoMensaje });
        //}
        /****************************************************/
        /********************* C A R G O S *********************/
        [HttpGet]
        public ActionResult ListarCargos(CARGO_E filtros)
        {
            var datosCargos = cargoN.ListarCargos(filtros);
            return PartialView("RecursosHumanos/ListadoCargos", datosCargos);
        }
        public JsonResult CargarCargos(int idOperation = 4003)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                var cargos = cargoN.ListarCargos(new CARGO_E { Estado = "1" });
                bool listaValida = (cargos != null && cargos.Count >= 1);
                string mensaje = listaValida ? "OK" : "ERROR";
                return Json(new { Lista = cargos, Mensaje = mensaje });
            }
            else
            {
                return Json(new { Mensaje = "Error crítico", Comentario = new List<string>() { "No cuentas con permisos para realizar esta acción." }, Icono = "error" });
            }
        }
        public JsonResult AgregarCargo(CARGO_E form, int idOperation = 4003)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                if (form == null)
                {
                    return Json(new { Mensaje = "Error crítico", Comentario = new List<string>() { "Verificar los datos ingresados o contactarse con SISTEMAS." }, Icono = "error" });
                }
                Usuario_E usu = (Usuario_E)Session["UsuarioId"];
                form.UsuarioOperacion = $"{usu.Nombres} {usu.Apellidos}";
                var listaMensajes = cargoN.ValidarRegistroCargo(form);
                bool errores = (listaMensajes != null && listaMensajes.Count >= 1);
                string mensaje = errores ? "No se pudo completar la acción" : "¡Acción realizada con éxito!";
                string iconoMensaje = errores ? "warning" : "success";
                return Json(new { Mensaje = mensaje, Comentario = listaMensajes, Icono = iconoMensaje });
            }
            else
            {
                return Json(new { Mensaje = "Error crítico", Comentario = new List<string>() { "No cuentas con permisos para realizar esta acción." }, Icono = "error" });
            }
        }
        public JsonResult EditarCargo(CARGO_E form, int idOperation = 4003)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                if (form == null || form.Id <= 0)
                {
                    return Json(new { Mensaje = "Error crítico", Comentario = new List<string>() { form == null ? "Verificar los datos ingresados o contactarse con SISTEMAS." : "Cargo no válido." }, Icono = "error" });
                }
                Usuario_E usu = (Usuario_E)Session["UsuarioId"];
                form.UsuarioOperacion = $"{usu.Nombres} {usu.Apellidos}";
                var mensajeError = cargoN.EditarCargo(form);
                string mensaje = string.IsNullOrWhiteSpace(mensajeError) ? "¡Acción realizada con éxito!" : "No se pudo completar la acción";
                string iconoMensaje = string.IsNullOrWhiteSpace(mensajeError) ? "success" : "warning";
                return Json(new { Mensaje = mensaje, Comentario = new List<string>() { mensajeError }, Icono = iconoMensaje });
            }
            else
            {
                return Json(new { Mensaje = "Error crítico", Comentario = new List<string>() { "No cuentas con permisos para realizar esta acción." }, Icono = "error" });
            }
        }
        public JsonResult EliminarCargo(int idCargo, int idOperation = 4003)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                if (idCargo <= 0)
                {
                    return Json(new { Mensaje = "Error crítico", Comentario = new List<string>() { "Cargo no válido." }, Icono = "error" });
                }
                var mensajeError = cargoN.EliminarCargo(idCargo);
                string mensaje = string.IsNullOrWhiteSpace(mensajeError) ? "¡Acción realizada con éxito!" : "No se pudo completar la acción";
                string iconoMensaje = string.IsNullOrWhiteSpace(mensajeError) ? "success" : "warning";
                return Json(new { Mensaje = mensaje, Comentario = new List<string>() { mensajeError }, Icono = iconoMensaje });
            }
            else
            {
                return Json(new { Mensaje = "Error crítico", Comentario = new List<string>() { "No cuentas con permisos para realizar esta acción." }, Icono = "error" });
            }
        }
        /****************************************************/
        /********************* A R E A S *********************/
        [HttpGet]
        public ActionResult ListarAreas(OAREA_E filtros)
        {
            var datosAreas = new Capa_Negocio.RecursosHumanos_NEG.TablasSQL.OAREA_N().ListarAreas(filtros);
            return PartialView("RecursosHumanos/ListadoAreas", datosAreas);
        }
        public JsonResult CargarAreas(int idDepartamento = 0)
        {
            if (idDepartamento > 0)
            {
                var areas = new Capa_Negocio.RecursosHumanos_NEG.TablasSQL.OAREA_N().ListarAreas(new OAREA_E { IdDepartamento = idDepartamento, Estado = "Y" });
                bool listaValida = (areas != null && areas.Count >= 1);
                string mensaje = listaValida ? "OK" : "ERROR";
                return Json(new { Lista = areas, Mensaje = mensaje });
            }
            else
            {
                return Json(new { Lista = new OAREA_E(), Mensaje = "El objeto recibido es nulo." });
            }
            
        }
        /****************************************************/
        /********************* D E P A R T A M E N T O S *********************/
        [HttpGet]
        public ActionResult ListarDepartamentos(ODPTO_E filtros)
        {
            var datosDepartamentos = new Capa_Negocio.RecursosHumanos_NEG.TablasSQL.ODPTO_N().ListarDepartamentos(filtros);
            return PartialView("RecursosHumanos/ListadoDepartamentos", datosDepartamentos);
        }
        /****************************************************/
        /********************* E M P L E A D O S *********************/
        [HttpGet]
        public ActionResult ListarEmpleados(OEMPL_E filtros)
        {
            Usuario_E usu = (Usuario_E)Session["UsuarioId"];
            ViewBag.IdRol = usu?.IdRol ?? 0;
            int? rolId = usu != null ? usu.IdRol : (int?)null;
            var datosEmpleados = emplN.ListarEmpleados(filtros, rolId);
            ViewBag.Mensaje = datosEmpleados == null ? "No se encontraron usuarios registrados." : "";
            filtros.PaginacionResultados = 0;
            var empleados = emplN.ListarEmpleados(filtros, rolId) ?? new List<OEMPL_E>();
            ViewBag.CantidadEmpleados = empleados.Count;
            return PartialView("RecursosHumanos/ListadoEmpleados", datosEmpleados);
        }
        public JsonResult AgregarEmpleado(OEMPL_E form, EMPL1_E form2, int idOperation = 4100)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                if (form == null || form2 == null)
                {
                    return Json(new { Mensaje = "Error crítico", Comentario = new List<string>() { "Verificar los datos ingresados o contactarse con SISTEMAS." }, Icono = "error" });
                }
                Usuario_E usu = (Usuario_E)Session["UsuarioId"];
                form.RegistradoPor = usu.DocEntry;
                form.UsuarioOperacion = $"{usu.Nombres} {usu.Apellidos}";
                var listaMensajes = emplN.ValidarRegistroEmpleado(form, form2);
                bool errores = (listaMensajes != null && listaMensajes.Count >= 1);
                string mensaje = errores ? "No se pudo completar la acción" : "¡Acción realizada con éxito!";
                string iconoMensaje = errores ? "warning" : "success";
                return Json(new { Mensaje = mensaje, Comentario = listaMensajes, Icono = iconoMensaje });
            }
            else
            {
                return Json(new { Mensaje = "Error crítico", Comentario = new List<string>() { "No cuentas con permisos para realizar esta acción." }, Icono = "error" });
            }
        }
        public JsonResult ObtenerDatosEmpleado(int id, string nroDocumento)
        {
            var datosEmpleado = emplN.ObtenerDatosEmpleado(id, nroDocumento);
            return Json(new { Empleado = datosEmpleado });
        }
        public JsonResult EditarEmpleado(OEMPL_E form, EMPL1_E form2, int idOperation = 4101)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                if (form == null || form2 == null || form2.IdOEMPL <= 0)
                {
                    return Json(new { Mensaje = "Error crítico", Comentario = new List<string>() { form == null ? "Verificar los datos ingresados o contactarse con SISTEMAS." : "Empleado no válido." }, Icono = "error" });
                }
                Usuario_E usu = (Usuario_E)Session["UsuarioId"];
                form.RegistradoPor = usu.DocEntry;
                form.UsuarioOperacion = $"{usu.Nombres} {usu.Apellidos}";
                form2.UsuarioOperacion = $"{usu.Nombres} {usu.Apellidos}";
                form2.NroDocumento = form.NroDocumento;
                var listaMensajes = emplN.EditarEmpleado(form, form2);
                bool errores = (listaMensajes != null && listaMensajes.Count >= 1);
                string mensaje = errores ? "No se pudo completar la acción" : "¡Acción realizada con éxito!";
                string iconoMensaje = errores ? "warning" : "success";
                return Json(new { Mensaje = mensaje, Comentario = listaMensajes, Icono = iconoMensaje });
            }
            else
            {
                return Json(new { Mensaje = "Error crítico", Comentario = new List<string>() { "No cuentas con permisos para realizar esta acción." }, Icono = "error" });
            }
        }
        public JsonResult AnexoCorreoCorporativoDuplicado(string anexo, string correoCorporativo, int id)
        {
            var listaMensajes = new EMPL1_N().BuscarAnexoCorreoDuplicado(anexo, correoCorporativo, id);
            bool errores = (listaMensajes != null && listaMensajes.Count >= 1);
            string mensaje = errores ? "No se pudo completar la acción" : string.Empty;
            string iconoMensaje = errores ? "info" : "success";
            return Json(new { Mensaje = mensaje, Comentario = listaMensajes, Icono = iconoMensaje });
        }
        public JsonResult EliminarEmpleado(int id, int idOperation = 4102)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                if (id <= 0)
                {
                    return Json(new { Mensaje = "Error crítico", Comentario = new List<string>() { "Empleado no válido." }, Icono = "error" });
                }
                var mensajeError = emplN.EliminarEmpleado(id);
                string mensaje = string.IsNullOrWhiteSpace(mensajeError) ? "¡Acción realizada con éxito!" : "No se pudo completar la acción";
                string iconoMensaje = string.IsNullOrWhiteSpace(mensajeError) ? "success" : "warning";
                return Json(new { Mensaje = mensaje, Comentario = mensajeError, Icono = iconoMensaje });
            }
            else
            {
                return Json(new { Mensaje = "Error crítico", Comentario = new List<string>() { "No cuentas con permisos para realizar esta acción." }, Icono = "error" });
            }
        }
        public ActionResult ExportarListadoEmpleados(OEMPL_E filtros)
        {
            int columnas = 8;
            var listado = emplN.ExportarListaEmpleados(filtros);
            if (listado != null && listado.Count >= 1)
            {
                using (var libro = new ExcelPackage())
                {
                    var worksheet = libro.Workbook.Worksheets.Add("Listado_Empleados");
                    // Agregar título
                    var tituloCelda = worksheet.Cells["A1:H1"];
                    tituloCelda.Merge = true; // Combinar celdas para el título
                    tituloCelda.Style.Font.Size = 24; // Establecer el tamaño de fuente en puntos
                    tituloCelda.Style.Font.Bold = true; // Hacer el título en negrita
                    tituloCelda.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center; // Centrar el título
                    tituloCelda.Value = "DIRECTORIO TELEFÓNICO - COBEFAR S.A.C."; // Establecer el texto del título
                    // Saltar una fila para dejar espacio para el título
                    var filaTabla = 3;
                    worksheet.Cells[$"A{filaTabla}"].LoadFromCollection(listado, PrintHeaders: true);
                    for (var col = 1; col <= columnas; col++)
                    {
                        worksheet.Column(col).AutoFit();
                    }
                    var tabla = worksheet.Tables.Add(new ExcelAddressBase(fromRow: filaTabla, fromCol: 1, toRow: listado.Count + filaTabla, toColumn: columnas), "Listado_Empleados");
                    tabla.ShowHeader = true;
                    tabla.TableStyle = OfficeOpenXml.Table.TableStyles.Medium2;
                    tabla.ShowFilter = true;        // Añadir filtro automático a la tabla
                    string excelContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    return File(libro.GetAsByteArray(), excelContentType, "Listado_Empleados.xlsx");
                }
            }
            else { return Content("No hay datos para exportar"); }
        }
        /****************************************************/
        /********************* N Ú M E R O S   C O R P O R A T I V O S *********************/
        [HttpGet]
        public ActionResult ListarNumerosCorporativos(ONUM_E filtros)
        {
            var datosNumerosCorporativos = new Capa_Negocio.RecursosHumanos_NEG.TablasSQL.ONUM_N().ListarNumeros(filtros);
            int asignados = datosNumerosCorporativos.Count(n => n.Asignado == "1" && n.Estado == "1");
            int noAsignados = datosNumerosCorporativos.Count(n => n.Asignado == "0" && n.Estado == "1");
            ViewBag.NumerosAsignados = asignados;
            ViewBag.NumerosNoAsignados = noAsignados;
            return PartialView("RecursosHumanos/ListadoNumerosCorporativos", datosNumerosCorporativos);
        }
        public JsonResult AgregarNumero(ONUM_E form, int idOperation = 4001)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                if (form == null)
                {
                    return Json(new { Mensaje = "Error crítico", Comentario = new List<string>() { "Verificar los datos ingresados o contactarse con SISTEMAS." }, Icono = "error" });
                }
                var listaMensajes = new Capa_Negocio.RecursosHumanos_NEG.TablasSQL.ONUM_N().ValidarRegistroNumero(form);
                bool errores = (listaMensajes != null && listaMensajes.Count >= 1);
                string mensaje = errores ? "No se pudo completar la acción" : "¡Acción realizada con éxito!";
                string iconoMensaje = errores ? "warning" : "success";
                return Json(new { Mensaje = mensaje, Comentario = listaMensajes, Icono = iconoMensaje });
            }
            else
            {
                return Json(new { Mensaje = "Error crítico", Comentario = new List<string>() { "No cuentas con permisos para realizar esta acción." }, Icono = "error" });
            }
        }
        public JsonResult CargarNumCorporativos(int idOperation = 40001)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                var numeros = new Capa_Negocio.RecursosHumanos_NEG.TablasSQL.ONUM_N().ListarNumeros(new ONUM_E { Estado = "1", Asignado = "0" });
                bool listaValida = (numeros != null && numeros.Count >= 1);
                string mensaje = listaValida ? "OK" : "ERROR";
                return Json(new { Lista = numeros, Mensaje = mensaje });
            }
            else
            {
                return Json(new { Mensaje = "Error crítico", Comentario = new List<string>() { "No cuentas con permisos para realizar esta acción." }, Icono = "error" });
            }
        }
        public JsonResult EditarNumero(ONUM_E form, int idOperation = 4001)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                if (form == null || (form != null && form.IdNumero <= 0))
                {
                    return Json(new { Mensaje = "Error crítico", Comentario = new List<string>() { "Número no válido." }, Icono = "error" });
                }
                Usuario_E usu = (Usuario_E)Session["UsuarioId"];
                form.RegistradoPor = usu.DocEntry;
                var mensajeError = new Capa_Negocio.RecursosHumanos_NEG.TablasSQL.ONUM_N().EditarNumero(form);
                string mensaje = string.IsNullOrWhiteSpace(mensajeError) ? "¡Acción realizada con éxito!" : "No se pudo completar la acción";
                string iconoMensaje = string.IsNullOrWhiteSpace(mensajeError) ? "success" : "warning";
                return Json(new { Mensaje = mensaje, Comentario = new List<string>() { mensajeError }, Icono = iconoMensaje });
            }
            else
            {
                return Json(new { Mensaje = "Error crítico", Comentario = new List<string>() { "No cuentas con permisos para realizar esta acción." }, Icono = "error" });
            }
        }
        public JsonResult EliminarNumero(int id, int idOperation = 4001)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode != 200)
            {
                return Json(new { Mensaje = "Error crítico", Comentario = new List<string>() { "No cuentas con permisos para realizar esta acción." }, Icono = "error" });
            }
            try
            {
                if (id < 0)
                {
                    return Json(new { Mensaje = "Error crítico", Comentario = new List<string>() { "Número no válido." }, Icono = "error" });
                }
                var mensajeError = new Capa_Negocio.RecursosHumanos_NEG.TablasSQL.ONUM_N().EliminarNumero(id);
                string mensaje = string.IsNullOrWhiteSpace(mensajeError) ? "¡Acción realizada con éxito!" : "No se pudo completar la acción";
                string iconoMensaje = string.IsNullOrWhiteSpace(mensajeError) ? "success" : "warning";
                return Json(new { Mensaje = mensaje, Comentario = new List<string>() { mensajeError }, Icono = iconoMensaje });
            }
            catch (Exception ex)
            {
                return Json(new { Mensaje = "Error crítico", Comentario = new List<string>() { ex.Message }, Icono = "error" });
            }
        }
        public JsonResult LiberarNumero(int id, string nroDocumento, int idOperation = 4001)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                try
                {
                    if (id < 0)
                    {
                        return Json(new { Mensaje = "Error crítico", Comentario = new List<string>() { "Número no válido." }, Icono = "error" });
                    }
                    var mensajeError = new Capa_Negocio.RecursosHumanos_NEG.TablasSQL.ONUM_N().LiberarNumero(id, nroDocumento);
                    string mensaje = string.IsNullOrWhiteSpace(mensajeError) ? "¡Acción realizada con éxito!" : "No se pudo completar la acción";
                    string iconoMensaje = string.IsNullOrWhiteSpace(mensajeError) ? "success" : "warning";
                    return Json(new { Mensaje = mensaje, Comentario = new List<string>() { mensajeError }, Icono = iconoMensaje });
                }
                catch (Exception ex)
                {
                    return Json(new { Mensaje = "Error crítico", Comentario = new List<string>() { ex.Message }, Icono = "error" });
                }
            }
            else
            {
                return Json(new { Mensaje = "Error crítico", Comentario = new List<string>() { "No cuentas con permisos para realizar esta acción." }, Icono = "error" });
            }
        }
        public JsonResult AuditarNumero(int id, int idOperation = 4001)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                if (id > 0)
                {
                    var auditoria = new Capa_Negocio.RecursosHumanos_NEG.AUD_ONUM_N().AuditarNumero(new AUD_ONUM_E { IdNumero = id });
                    bool listaValida = (auditoria != null && auditoria.Count >= 1);
                    string mensaje = listaValida ? "OK" : "ERROR";
                    return Json(new { Lista = auditoria, Mensaje = mensaje });
                }
                else
                {
                    // Devolver un mensaje de error si el IdArea recibido es inválido
                    return Json(new { Mensaje = "Error crítico", Comentario = new List<string>() { "Número no válido." }, Icono = "error" });
                }
            }
            else
            {
                // Devolver un mensaje de error si el IdArea recibido es inválido
                return Json(new { Mensaje = "Error crítico", Comentario = new List<string>() { "Sin accesos" }, Icono = "error" });
            }
        }
        public ActionResult ExportarListadoNumerosCorporativos(RptNumerosCorporativos_E filtro, int idOperation = 4001)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                int columnas = 11;
                var listado = new ONUM_N().ExportarListaNumeros(filtro);
                if (listado != null && listado.Count >= 1)
                {
                    using (var libro = new ExcelPackage())
                    {
                        var worksheet = libro.Workbook.Worksheets.Add("Listado_Numeros_Corporativos");
                        worksheet.Cells["A1"].LoadFromCollection(listado, PrintHeaders: true);
                        for (var col = 1; col <= columnas; col++)
                        {
                            worksheet.Column(col).AutoFit();
                        }
                        var tabla = worksheet.Tables.Add(new ExcelAddressBase(fromRow: 1, fromCol: 1, toRow: listado.Count + 1, toColumn: columnas), "Listado_Numeros_Corporativos");
                        tabla.ShowHeader = true;
                        tabla.TableStyle = OfficeOpenXml.Table.TableStyles.Medium2;
                        string excelContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                        return File(libro.GetAsByteArray(), excelContentType, "Listado_Numeros_Corporativos.xlsx");
                    }
                }
                else { return Content("No hay datos para exportar"); }
            }
            else
            {
                return resultadoAcceso;
            }
        }
    }
}