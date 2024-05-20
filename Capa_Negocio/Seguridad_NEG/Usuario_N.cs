using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Capa_Datos;
using Capa_Entidad;
using Capa_Entidad.Seguridad_ENT;

namespace Capa_Negocio.Seguridad_NEG
{
    public class Usuario_N
    {
        Usuario_D usuario_D = new Usuario_D();
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

        public string crearUsuario(Usuario_E u, String opRegistro)
        {
            if (string.IsNullOrEmpty(u.Nombres.Trim())) { throw new Exception("El campo NOMBRES no puede ser vacío"); }
            if (string.IsNullOrEmpty(u.Apellidos.Trim())) { throw new Exception("El campo APELLIDOS no puede ser vacío"); }

            // Buscamos si el usuario cuenta con un usuario con el mismo rol
            var busquedaUsuario = usuario_D.BuscarUsuarioRol(u.Nombres.Trim(), u.Apellidos.Trim(), u.IdRol);
            if (busquedaUsuario > 0) { throw new Exception("Se detectó un usuario con los mismos NOMBRES y APELLIDOS"); }

            if (string.IsNullOrEmpty(u.Id)) { throw new Exception("No puede registrar un ID vacío"); }
            if (u.IdRol <= 0) { throw new Exception("Debe seleccionar un ID de Rol válido"); }
            if (string.IsNullOrEmpty(u.Password)) { throw new Exception("El campo CONTRASEÑA no puede ser vacío"); }
            if (string.IsNullOrEmpty(opRegistro)) { throw new Exception("Por favor cerrar sesión y volver a ingresar, gracias."); }

            // Verificar la longitud mínima del nombre completo
            if (u.Nombres.TrimEnd().Length < 4) { throw new Exception("El campo NOMBRES debe tener más de 3 caracteres"); }
            if (u.Apellidos.TrimEnd().Length < 8) { throw new Exception("El campo APELLIDOS debe tener más de 7 caracteres."); }

            return usuario_D.CrearUsuario(u, opRegistro);
        }

        public string editarUsuario(Usuario_E u)
        {
            return usuario_D.editarUsuario(u);
        }
        public string eliminarUsuario(Usuario_E obj)
        {
            return usuario_D.eliminarUsuario(obj);
        }
        public Usuario_E BuscarDocEntryUsuario(string Usuario)
        {
            return usuario_D.BuscarDocEntryUsuario(Usuario);
        }
    }
}
