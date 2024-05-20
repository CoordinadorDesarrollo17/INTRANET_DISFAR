using Capa_Datos.TI_Sistemas_DAO;
using Capa_Entidad.TI_Sistemas_ENT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Negocio.TI_Sistemas_NEG
{
    public class OTSO_N
    {
        OTSO_D otsoD = new OTSO_D();
        public List<OTSO_E> listarTicketsSoporte(OTSO_E obj)
        {
            return otsoD.listarTicketsSoporte(obj);
        }
        public string buscarRutaArchivo(int DocEntry)
        {
            return otsoD.buscarRutaArchivo(DocEntry);
        }
        public OTSO_E buscarTicketSoporte(int DocEntry)
        {
            return otsoD.buscarTicketSoporte(DocEntry);
        }
        public int ticketsVencidos()
        {
            return otsoD.ticketsVencidos();
        }
        public int ticketsUrgentes()
        {
            return otsoD.ticketsUrgentes();
        }
        public int ticketsNoAtendidos()
        {
            return otsoD.ticketsNoAtendidos();

        }
        public int ticketsAtendidos()
        {
            return otsoD.ticketsAtendidos();

        }
        public int registrarTicketSoporte(OTSO_E obj)
        {
            validarNuevoTicketSoporte(obj);
            return otsoD.registrarTicketSoporte(obj);
        }
        public int editarTicketSoporte(OTSO_E obj)
        {
            validarEditarTicketSoporte(obj);
            return otsoD.editarTicketSoporte(obj);
        }
        public int asignarTicketSoporte(OTSO_E obj)
        {
            validarAsignarTicketSoporte(obj);
            return otsoD.asignarTicketSoporte(obj);
        }
        public int atenderTicketSoporte(OTSO_E obj)
        {
            validarAtenderTicketSoporte(obj);
            return otsoD.atenderTicketSoporte(obj);
        }
        public int obtenerEstadoOperario(string Id)
        {
            return otsoD.obtenerEstadoOperario(Id);
        }

        //validaciones
        public void validarNuevoTicketSoporte(OTSO_E obj)
        {
            if (obj.Titulo == null) { throw new Exception("Debe ingresar un titulo"); }
            if (obj.Descripcion == null) { throw new Exception("Debe ingresar una descripcion"); }
            if (obj.Contacto == null) { throw new Exception("Debe ingresar un dato del contacto"); }
            if (obj.Prioridad == null) { throw new Exception("Debe elegir una prioridad"); }
            if (obj.Asistencia == null) { throw new Exception("Debe llenar la  forma de asistencia"); }
            if (obj.Area == null) { throw new Exception("Debe elegir el area"); }
            if (obj.Sede == null) { throw new Exception("Debe elegir la sede"); }
            if (obj.Archivo != null)
            {
                if (!(obj.Archivo.ContentType == "application/pdf" ||
                    obj.Archivo.ContentType == "application/x-zip-compressed" ||
                    obj.Archivo.ContentType == "application/zip" ||
                    obj.Archivo.ContentType == "application/octet-stream" ||
                    obj.Archivo.ContentType == "image/jpg" ||
                    obj.Archivo.ContentType == "image/jpeg" ||
                    obj.Archivo.ContentType == "image/png"))
                {
                    throw new Exception("Debe elegir un archivo valido .zip, .rar, .jpg, .png, .pdf");
                }
                if (obj.Archivo.ContentLength > 10485760) { throw new Exception("No puedes cargar un archivo superior a 10Mb"); }
            }
        }
        public void validarEditarTicketSoporte(OTSO_E obj)
        {
            if (obj.Operario != obj.Propietario) { throw new Exception("Solo el propietario del ticket puede editar la solicitud"); }
            if (obj.Estado != "CREADO") { throw new Exception("Solo se puede editar un ticket con estado Creado"); }
            if (obj.Descripcion == null) { throw new Exception("Debe ingresar una descripcion"); }
            if (obj.Contacto == null) { throw new Exception("Debe ingresar un dato del contacto"); }
            if (obj.Prioridad == null) { throw new Exception("Debe elegir una prioridad"); }
            if (obj.Asistencia == null) { throw new Exception("Debe llenar la  forma de asistencia"); }
            if (obj.Area == null) { throw new Exception("Debe elegir el area"); }
            if (obj.Sede == null) { throw new Exception("Debe elegir la sede"); }
            if (obj.Archivo != null)
            {
                if (!(obj.Archivo.ContentType == "application/pdf" ||
                    obj.Archivo.ContentType == "application/x-zip-compressed" ||
                    obj.Archivo.ContentType == "application/zip" ||
                    obj.Archivo.ContentType == "application/octet-stream" ||
                    obj.Archivo.ContentType == "image/jpg" ||
                    obj.Archivo.ContentType == "image/jpeg" ||
                    obj.Archivo.ContentType == "image/png"))
                {
                    throw new Exception("Debe elegir un archivo valido .zip,.rar,.jpg,.png,.pdf");
                }
                if (obj.Archivo.ContentLength > 10485760) { throw new Exception("No puedes cargar un archivo superior a 10Mb"); }
            }
        }
        public void validarAsignarTicketSoporte(OTSO_E obj)
        {
            if (obj.IdAsignado == null) { throw new Exception("Debe seleccionar un personal para la asignacion"); }
        }
        public void validarAtenderTicketSoporte(OTSO_E obj)
        {
            if (obj.Solucion == null || obj.Solucion == "") { throw new Exception("Debe ingresar una solucion al ticket"); }
        }

    }
}
