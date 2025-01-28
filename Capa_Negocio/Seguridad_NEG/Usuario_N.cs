using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Caching;
using Capa_Datos;
using Capa_Entidad;
using Capa_Entidad.RecursosHumanos_ENT.TablasSQL;
using Capa_Entidad.Seguridad_ENT;
using Capa_Negocio.RecursosHumanos_NEG.TablasSQL;
using Capa_Negocio.Seguridad_NEG.TablasSql;

namespace Capa_Negocio.Seguridad_NEG
{
    public class Usuario_N
    {
        Usuario_D ousrD = new Usuario_D();
        Helpers helper = new Helpers();

        public List<Usuario_E> ListaUsuarios(Usuario_E filtro)
        {
            return ousrD.ListaUsuarios(filtro);
        }
        public List<Usuario_E> listaUsuariosPermisos(Usuario_E filtro, int idRol)
        {
            return ousrD.listaUsuariosPermisos(filtro, idRol);
        }
        public Dictionary<string, string> generarId(int idRol)
        {
            return ousrD.generarId(idRol);
        }
        public Usuario_E buscarUsuario(int DocEntry)
        {
            return ousrD.buscarUsuario(DocEntry);
        }
        public Usuario_E buscarUsuarioSesion(string user, string pass)
        {
            //liberar los datos de inyecciones
            return ousrD.buscarUsuarioSesion(user, pass);
        }
        public Helper_E crearUsuario(Usuario_E datosPost, String opRegistro)
        {
            if (datosPost.EmpleadoID > 0)
            {
                var empleado = new OEMPL_N().ListarEmpleados(new OEMPL_E { Id = datosPost.EmpleadoID });

                if (empleado != null && empleado.Count >= 1)
                {
                    datosPost.Nombres = empleado[0].Nombres;
                    datosPost.Apellidos = empleado[0].Apellidos;
                }
            }

            if (string.IsNullOrWhiteSpace(datosPost.Nombres) || string.IsNullOrWhiteSpace(datosPost.Nombres.Trim())) { throw new Exception("El campo Nombres no puede ser vacío"); }
            if (string.IsNullOrWhiteSpace(datosPost.Apellidos) || string.IsNullOrWhiteSpace(datosPost.Apellidos.Trim())) { throw new Exception("El campo Apellidos no puede ser vacío"); }

            // Buscamos si el usuario cuenta con un usuario con el mismo rol
            var busquedaUsuario = ousrD.BuscarUsuarioRol(datosPost.Nombres.Trim(), datosPost.Apellidos.Trim(), datosPost.IdRol);
            if (busquedaUsuario > 0) { throw new Exception("Se detectó un usuario con los mismos Nombres y Apellidos"); }

            if (string.IsNullOrWhiteSpace(datosPost.Id)) { throw new Exception("No puede registrar un ID vacío"); }
            if (datosPost.IdRol <= 0) { throw new Exception("Debe seleccionar un ID de Rol válido"); }
            if (string.IsNullOrWhiteSpace(opRegistro)) { throw new Exception("Por favor cerrar sesión y volver a ingresar, gracias."); }

            // Verificar la longitud mínima del nombre completo
            if (datosPost.Nombres.TrimEnd().Length < 4) { throw new Exception("El campo Nombres debe tener más de 3 caracteres"); }
            if (datosPost.Apellidos.TrimEnd().Length < 8) { throw new Exception("El campo Apellidos debe tener más de 7 caracteres."); }

            if (!string.IsNullOrWhiteSpace(datosPost.Email) && helper.EsCorreo(datosPost.Email) == false)
            {
                throw new Exception("Ingresar un correo válido.");
            }

            // Generamos una contraseña por default
            datosPost.Password = GenerarContrasena(datosPost.Nombres.Trim(), datosPost.Apellidos.Trim(), datosPost.Id);
            var result = ousrD.CrearUsuario(datosPost, opRegistro);

            if (result.DocEntry > 0)
            {
                var operaciones = new ROL_OPE_N().ObtenerOperacionesPorRol(datosPost.IdRol);
                new OUSR_OPE_N().AsignarPermisosPorUsuario(operaciones, result.DocEntry);
            }

            return result;
        }
        public string EditarUsuario(Usuario_E datosPost)
        {
            if (string.IsNullOrWhiteSpace(datosPost.Password))
            {
                return "Debe ingresar una contraseña válida.";
            }

            if (!datosPost.Password.Equals(datosPost.Password2))
            {
                return "Las contraseñas no coinciden.";
            }

            if (datosPost.Password.Length < 7)
            {
                return "La contraseña debe tener mínimo 7 caracteres.";
            }

            if (!string.IsNullOrWhiteSpace(datosPost.Email) && helper.EsCorreo(datosPost.Email) == false)
            {
                return "Ingresar un correo válido.";
            }

            //if (helper.ContieneCaracterEspecial(datosPost.Password) == false)
            //{
            //    return "La contraseña debe contener al menos 1 caracter especial.";
            //}

            return ousrD.EditarUsuario(datosPost);
        }
        public string Inactivar(Usuario_E obj)
        {
            return ousrD.Inactivar(obj);
        }
        public string Activar(Usuario_E obj)
        {
            return ousrD.Activar(obj);
        }
        public Usuario_E BuscarDocEntryUsuario(string Usuario)
        {
            return ousrD.BuscarDocEntryUsuario(Usuario);
        }
        static string GenerarContrasena(string nombre, string apellido, string id)
        {
            // Obtener los primeros 3 caracteres del nombre y apellido
            string primerosTresNombre = nombre.Length >= 3 ? nombre.Substring(0, 3).ToUpper() : nombre;
            string primerosTresApellido = apellido.Length >= 3 ? apellido.Substring(0, 3).ToUpper() : apellido;

            // Combinar los caracteres y agregar el número 24
            string contrasena = primerosTresNombre + primerosTresApellido + id;

            return contrasena;
        }
        public List<string> listaCopilotos()
        {
            return ousrD.listaCopilotos();
        }
    }
}
