using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Capa_Datos.Rutas_DAO;
namespace Capa_Negocio.Rutas_NEG
{
    public class FormRutas_N
    {
        FormRutas_D frm = new FormRutas_D();
        public string infoListaSocios(string FechaCont)
        {
            return frm.infoListaSocios(FechaCont);
        }
        public string infoListaTicketsVenta(string FechaCont, string Cardcode)
        {
            return frm.infoListaTicketsVenta(FechaCont, Cardcode);
        }
        public string infoListaProductosOWTQ(string guia, int linea)
        {
            return frm.infoListaProductosOWTQ(guia, linea);
        }
    }
}
