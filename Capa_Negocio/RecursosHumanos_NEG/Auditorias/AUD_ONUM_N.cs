using Capa_Datos.RecursosHumanos_DAO.Auditorias;
using Capa_Entidad.RecursosHumanos_ENT.Auditorias;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Negocio.RecursosHumanos_NEG
{
    public class AUD_ONUM_N
    {
        AUD_ONUM_D audNumD = new AUD_ONUM_D();

        public List<AUD_ONUM_E> AuditarNumero(AUD_ONUM_E filtros)
        {
            return audNumD.AuditarNumero(filtros);
        }
    }
}
