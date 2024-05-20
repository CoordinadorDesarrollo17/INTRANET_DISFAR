using Capa_Entidad.General_ENT.TablasSql;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Datos.General_DAO.TablasSql
{
    public class DNLAB_D
    {
        Utilitarios uti = new Utilitarios(); DBHelper db = new DBHelper();
        public int registrarDiaNoLaborable(DNLAB_E obj)
        {
            int status = -1;
            string query = "insert into DNLAB values (@Fecha,@OpRegistro);";
            try
            {
                db.ExecuteNonQueryTrxNoSp(query, new List<string>() { "@Fecha", "@OpRegistro" }
                , obj.Fecha, obj.OpRegistro);
                status = 1;
            }
            catch { status = 0; throw new Exception("Error en registro"); }
            return status;
        }

        public List<DNLAB_E> listadoDeDiasNLAB()
        {
            List<DNLAB_E> lista = new List<DNLAB_E>();
            string query = "select Fecha from DNLAB;";
            try
            {
                SqlDataReader dr = db.ExecuteReaderNoSp(query);
                while (dr.Read())
                {
                    DNLAB_E o = new DNLAB_E();
                    if (!dr.IsDBNull(0)) { o.Fecha = dr.GetString(0); }
                    lista.Add(o);
                }
                dr.Close();
            }
            catch { }
            return lista;
        }
    }
}

