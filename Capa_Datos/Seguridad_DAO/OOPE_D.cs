using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Capa_Entidad.Seguridad_ENT;
using System.Data.SqlClient;

namespace Capa_Datos.Seguridad_DAO
{
    public class OOPE_D
    {
        Utilitarios uti = new Utilitarios();
        DBHelper db = new DBHelper();
        public List<OOPE_E> listarOperacionesRolModulo(int idModulo, int idRol)
        {
            List<OOPE_E> lista = new List<OOPE_E>();
            string query = "";
            if (idRol == 0)
            {
                query = "SELECT t0.* FROM OOPE t0 where t0.idModulo = " + idModulo;
            }
            else
            {
                query = "SELECT t0.* FROM OOPE t0 inner join ROL1 t1 on t1.idOperacion=t0.id " +
                                "where t0.idModulo = " + idModulo + " and t1.idRol =" + idRol;
            }

            try
            {
                SqlDataReader dr = db.ExecuteReaderNoSp(query);
                while (dr.Read())
                {
                    OOPE_E o = new OOPE_E();
                    o.id = dr.GetInt32(0);
                    o.nombre = dr.GetString(1);
                    o.idModulo = dr.GetInt32(2);
                    lista.Add(o);
                }
                dr.Close();
            }
            catch { }
            return lista;
        }

    }
}


