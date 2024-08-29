using Capa_Datos.SocioNegocios_DAO.Tablas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Negocio.SocioNegocios_NEG.Tablas
{
    public class CRD1_N
    {
        CRD1_D crd1D = new CRD1_D();
        public string BuscarZonaPedido(string shipToCode, string cardCode)
        {
            return crd1D.BuscarZonaPedido(shipToCode, cardCode);
        }
    }
}
