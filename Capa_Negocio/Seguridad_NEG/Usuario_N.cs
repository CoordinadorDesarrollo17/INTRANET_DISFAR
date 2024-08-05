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
        Usuario_D usuario_D = new Usuario_D();
        Helpers helper = new Helpers();

        public List<Usuario_E> ListaUsuarios(Usuario_E filtro)
        {
            return usuario_D.ListaUsuarios(filtro);
        }
        public List<Usuario_E> listaUsuariosPermisos(Usuario_E filtro, int idRol)
        {
            return usuario_D.listaUsuariosPermisos(filtro, idRol);
        }
        public Dictionary<string, string> generarId(int idRol)
        {
            return usuario_D.generarId(idRol);
        }
        public Usuario_E buscarUsuario(int DocEntry)
        {
            return usuario_D.buscarUsuario(DocEntry);
        }
        public Usuario_E buscarUsuarioSesion(string user, string pass)
        {
            //liberar los datos de inyecciones
            return usuario_D.buscarUsuarioSesion(user, pass);
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

            if (string.IsNullOrEmpty(datosPost.Nombres.Trim())) { throw new Exception("El campo NOMBRES no puede ser vacío"); }
            if (string.IsNullOrEmpty(datosPost.Apellidos.Trim())) { throw new Exception("El campo APELLIDOS no puede ser vacío"); }

            // Buscamos si el usuario cuenta con un usuario con el mismo rol
            var busquedaUsuario = usuario_D.BuscarUsuarioRol(datosPost.Nombres.Trim(), datosPost.Apellidos.Trim(), datosPost.IdRol);
            if (busquedaUsuario > 0) { throw new Exception("Se detectó un usuario con los mismos NOMBRES y APELLIDOS"); }

            if (string.IsNullOrEmpty(datosPost.Id)) { throw new Exception("No puede registrar un ID vacío"); }
            if (datosPost.IdRol <= 0) { throw new Exception("Debe seleccionar un ID de Rol válido"); }
            if (string.IsNullOrEmpty(opRegistro)) { throw new Exception("Por favor cerrar sesión y volver a ingresar, gracias."); }

            // Verificar la longitud mínima del nombre completo
            if (datosPost.Nombres.TrimEnd().Length < 4) { throw new Exception("El campo NOMBRES debe tener más de 3 caracteres"); }
            if (datosPost.Apellidos.TrimEnd().Length < 8) { throw new Exception("El campo APELLIDOS debe tener más de 7 caracteres."); }

            if (!string.IsNullOrWhiteSpace(datosPost.Email) && helper.EsCorreo(datosPost.Email) == false)
            {
                throw new Exception("Ingresar un correo válido.");
            }

            // Generamos una contraseña por default
            datosPost.Password = GenerarContrasena(datosPost.Nombres.Trim(), datosPost.Apellidos.Trim(), datosPost.Id);
            var result = usuario_D.CrearUsuario(datosPost, opRegistro);

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

            if (datosPost.Password.Length < 8)
            {
                return "La contraseña debe tener mínimo 8 caracteres.";
            }

            if (!string.IsNullOrWhiteSpace(datosPost.Email) && helper.EsCorreo(datosPost.Email) == false)
            {
                return "Ingresar un correo válido.";
            }

            if (helper.ContieneCaracterEspecial(datosPost.Password) == false)
            {
                return "La contraseña debe contener al menos 1 caracter especial.";
            }

            return usuario_D.EditarUsuario(datosPost);
        }

        public string Inactivar(Usuario_E obj)
        {
            return usuario_D.Inactivar(obj);
        }

        public Usuario_E BuscarDocEntryUsuario(string Usuario)
        {
            return usuario_D.BuscarDocEntryUsuario(Usuario);
        }

        static string GenerarContrasena(string nombre, string apellido, string id)
        {
            // Obtener los primeros 3 caracteres del nombre y apellido
            string primerosTresNombre = nombre.Length >= 3 ? nombre.Substring(0, 3) : nombre;
            string primerosTresApellido = apellido.Length >= 3 ? apellido.Substring(0, 3) : apellido;

            // Combinar los caracteres y agregar el número 24
            string contrasena = primerosTresNombre + primerosTresApellido + id;

            return contrasena;
        }
    }
}
