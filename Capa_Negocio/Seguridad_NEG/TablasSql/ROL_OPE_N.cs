using Capa_Datos.Seguridad_DAO.TablasSql;
using Capa_Entidad.Seguridad_ENT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Negocio.Seguridad_NEG.TablasSql
{
    public class ROL_OPE_N
    {
        ROL_OPE_D rolOpeD = new ROL_OPE_D();

        public int VerificarAccesoOperacion(int rolID, int opeID)
        {
            return rolOpeD.VerificarAccesoOperacion(rolID, opeID);
        }

        public List<int> ObtenerOperacionesPorRol(int rolID)
        {
            List<ROL_OPE_E> operaciones = rolOpeD.ListarGrupoOperacionesPorRol(rolID);
            return operaciones.Select(op => op.OpeID).ToList();
        }

        public string AsignarPermisosPorRol(List<int> operaciones, int rolID)
        {
            return rolOpeD.AsignarPermisosPorRol(operaciones, rolID);
        }

    }
}