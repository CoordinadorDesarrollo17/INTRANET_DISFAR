using Capa_Entidad.Compras_ENT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace Capa_Datos.Compras_DAO.TablasSql
{
    public class LNPV_D
    {
        DBHelper db = new DBHelper();
        //lista negra proveedores
        public List<ProveedorListaNegra_E> listarProveedoresListaNegra()
        {
            List<ProveedorListaNegra_E> lista = new List<ProveedorListaNegra_E>();
            try
            {
                SqlDataReader dr = db.ExecuteReaderNoSp("select * from lnpv");
                while (dr.Read())
                {
                    ProveedorListaNegra_E p = new ProveedorListaNegra_E()
                    {
                        CardCode = dr.GetString(0),
                        CardName = dr.GetString(1)
                    };
                    lista.Add(p);
                }
                dr.Close();
            }
            catch { }
            return lista;
        }
        public int agregarProveedorListaNegra(ProveedorListaNegra_E p)
        {
            int status = 0;
            try
            {
                db.ExecuteNonQueryTrxNoSp("insert into lnpv values('" + p.CardCode + "','" + p.CardName + "')");
                status = 1;
            }
            catch (Exception e)
            {
                status = -1;
                throw new Exception(e.Message);
            }
            return status;
        }
        public int retirarProveedorListaNegra(string CardCode)
        {
            int status = 0;
            try
            {
                db.ExecuteNonQueryTrxNoSp("delete from lnpv where CardCode='" + CardCode + "'");
                status = 1;
            }
            catch (Exception e)
            {
                status = -1;
                throw new Exception(e.Message);
            }
            return status;
        }
    }
}
