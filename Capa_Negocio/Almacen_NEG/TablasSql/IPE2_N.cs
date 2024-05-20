using Capa_Datos.Almacen_DAO.TablasSql;
using Capa_Entidad.Almacen_ENT.TablasSql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Negocio.Almacen_NEG.TablasSql
{
    public class IPE2_N
    {
        IPE2_D ipe2D = new IPE2_D();
        //meotodos de hana
        public List<IPE2_E> listarArticulosUsrEqui(OIEQ_E eq)
        {
            if (eq == null) { return new List<IPE2_E>(); }
            if (eq.DetFabricantes == null) { return new List<IPE2_E>(); }
            return ipe2D.listarArticulosUsrEqui(eq);
        }
        public List<IPE2_E> listarArticulosUsrEquiSap(OIEQ_E eq)
        {
            return ipe2D.listarArticulosUsrEquiSap(eq);
        }
        public List<IPE2_E> listarLotesSinStock(IPE2_E obj)
        {
            return ipe2D.listarLotesSinStock(obj);
        }
        //metodos de sql
        public List<IPE2_E> listarArticulos(IPE2_E filtro)
        {
            return ipe2D.listarArticulos(filtro);
        }
    }
}
