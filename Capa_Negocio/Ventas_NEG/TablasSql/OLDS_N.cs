using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Capa_Datos.Ventas_DAO.TablasSql;
using Capa_Entidad.Ventas_ENT.TablasSql;

namespace Capa_Negocio.Ventas_NEG.TablasSql
{
    public class OLDS_N
    {
        OLDS_D ld = new OLDS_D();
        public List<OLDS_E> listarLibrosSaldo(OLDS_E li)
        {
            return ld.listarLibrosSaldo(li);
        }
        public OLDS_E obtenerLibroSaldo(string CardCode)
        {
            return ld.obtenerLibroSaldo(CardCode);
        }
        public int crearLibroSaldo(OLDS_E l)
        {
            return ld.crearLibroSaldo(l);
        }
        public List<LDS1_E> obtenerDetLibroSaldo(string CardCode)
        {
            return ld.obtenerDetLibroSaldo(CardCode);
        }
        public int agregarDetLibroSaldo(LDS1_E d)
        {
            return ld.agregarDetLibroSaldo(d);
        }
    }
}
