using Capa_Datos.Seguridad_DAO.TablasSql;
using Capa_Entidad.Seguridad_ENT;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Negocio.Seguridad_NEG.TablasSql
{
    public class OUSR_OPE_N
    {
        OUSR_OPE_D usrOpeD = new OUSR_OPE_D();

        public int VerificarAccesoOperacion(OUSR_OPE_E filtros)
        {
            return usrOpeD.VerificarAccesoOperacion(filtros);
        }

        public Dictionary<string, List<int>> AgruparOperacionesPorUsuario(int usrDocEntry)
        {
            List<OUSR_OPE_E> operaciones = usrOpeD.ListarOperacionesPorUsuario(usrDocEntry);

            return operaciones
                .GroupBy(op => op.ModuloNombre)
                .ToDictionary(g => g.Key, g => g.Select(op => op.OpeID).ToList());
        }

        public List<int> ObtenerOperacionesPorUsuario(int usrDocEntry)
        {
            List<OUSR_OPE_E> operaciones = usrOpeD.ListarOperacionesPorUsuario(usrDocEntry);
            return operaciones.Select(op => op.OpeID).ToList();
        }

        public string AsignarPermisosPorUsuario(List<int> operaciones, int usrDocEntry)
        {

            return usrOpeD.AsignarPermisosPorUsuario(operaciones, usrDocEntry);
        }
    }
}