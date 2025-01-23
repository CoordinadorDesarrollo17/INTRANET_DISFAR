using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Negocio.General_NEG.TablasSql
{
    public class OAGE_N
    {
        public List<Capa_Entidad.General_ENT.TablasSql.OAGE_E> Listar()
        {
            return new Capa_Datos.General_DAO.TablasSql.OAGE_D().Listar();
        }
        public string Registrar(Capa_Entidad.General_ENT.TablasSql.OAGE_E bean)
        {
            return new Capa_Datos.General_DAO.TablasSql.OAGE_D().Registrar(bean);
        }
        public void Eliminar(int Id)
        {
            new Capa_Datos.General_DAO.TablasSql.OAGE_D().Eliminar(Id);
        }
    }
}
