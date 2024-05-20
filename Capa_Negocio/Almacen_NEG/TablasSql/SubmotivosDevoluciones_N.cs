using Capa_Entidad.Almacen_ENT.TablasSql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Negocio.Almacen_NEG.TablasSql
{
    public class SubmotivosDevoluciones_N
    {
        Capa_Datos.Almacen_DAO.TablasSql.SubmotivosDevoluciones_D sub = new Capa_Datos.Almacen_DAO.TablasSql.SubmotivosDevoluciones_D();
        public List<SubmotivosDevoluciones_E> ListarSubmotivosDevoluciones(SubmotivosDevoluciones_E filtro)
        {
            return sub.ListarSubmotivosDevoluciones(filtro);
        }

        public string RegistrarSubmotivoDevolucion(SubmotivosDevoluciones_E submotivo)
        {
            return sub.RegistrarSubmotivoDevolucion(submotivo);
        }

        public string EditarSubmotivoDevolucion(SubmotivosDevoluciones_E submotivo)
        {
            return sub.EditarSubmotivoDevolucion(submotivo);
        }
    }
}