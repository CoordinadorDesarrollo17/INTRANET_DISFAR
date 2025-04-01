using Capa_Datos.SocioNegocios_DAO.TablasExternas;
using Capa_Entidad.SocioNegocios_ENT.Tablas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Capa_Negocio.SocioNegocios_NEG.TablasExternas
{
    public class OCPR_N
    {
        OCPR_D oD = new OCPR_D();
        public List<OCPR_E> listarContactosVentasSocio(string CardCode)
        {
            return oD.listarContactosVentasSocio(CardCode);
        }
    }
}
