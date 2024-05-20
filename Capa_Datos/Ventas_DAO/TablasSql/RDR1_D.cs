using Capa_Entidad.Ventas_ENT.TablasSql;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Datos.Ventas_DAO.TablasSql
{
    public class RDR1_D
    {
        readonly Utilitarios uti = new Utilitarios();
        public List<ExcelDetallePedido> ExportarDetallePedidosOnline(int IdORDR)
        {
            string condWhere = string.Empty;

            List<ExcelDetallePedido> lista = new List<ExcelDetallePedido>();

            if (IdORDR > 0)
            {
                condWhere = $" AND IdORDR = {IdORDR}";
            }

            using (SqlConnection cn = new SqlConnection(uti.cadSql))
            {
                string query = $"SELECT ItemCode, ItemName, UndMed, Cantidad FROM vt.RDR1 WHERE IdORDR>0 {condWhere}";

                SqlCommand cmd = new SqlCommand(query, cn);         // prepara
                cn.Open();

                try
                {
                    SqlDataReader dr = cmd.ExecuteReader();             // ejecuta

                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            ExcelDetallePedido rdr1 = new ExcelDetallePedido();

                            if (!dr.IsDBNull(0)) { rdr1.ItemCode = dr.GetString(0); }
                            if (!dr.IsDBNull(1)) { rdr1.ItemName = dr.GetString(1); }
                            if (!dr.IsDBNull(2)) { rdr1.UndMed = dr.GetString(2); }
                            if (!dr.IsDBNull(3)) { rdr1.Cantidad = dr.GetInt32(3); }

                            lista.Add(rdr1);
                        }
                    }

                    dr.Close();
                }
                catch (Exception e)
                {
                    throw new Exception(e.Message);
                }
                finally
                {
                    cn.Close();
                }

            }

            return lista;
        }
    }
}
