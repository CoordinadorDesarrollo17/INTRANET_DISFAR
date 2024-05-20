using Capa_Entidad.Almacen_ENT.TablasSql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Capa_Datos.Almacen_DAO.TablasSql
{
    public class RPD1_D
    {
        readonly Utilitarios uti = new Utilitarios();

        public List<RPD1_E> ListarDetalleDevolucion(int DocEntry)
        {
            List<RPD1_E> lista = new List<RPD1_E>();

            using (SqlConnection cn = new SqlConnection(uti.cadSql))
            {
                string query = "SELECT DET.DocEntry, DET.Linea, DET.ItemCode, DET.ItemName, DET.FirmCode, DET.BatchNum, DET.ExpDate, CONVERT(varchar,DET.ExpDate,103) AS ExpDateFormat, DET.Quantity, DET.NumInBuy, DET.BuyUnitMsr, DET.Motivo, DET.RefFactura, DET.Observacion, MD.Descripcion, DET.MaxQuantity, DET.Submotivo, SUB.Descripcion,DET.MaxQuantityOIBT,DET.NumInBuyKey FROM al.RPD1 DET INNER JOIN al.MotivosDevoluciones MD ON MD.IdMotivo = DET.Motivo LEFT JOIN al.SubmotivosDevoluciones SUB ON SUB.IdSubmotivo = DET.Submotivo WHERE DET.DocEntry = @DocEntry ORDER BY Linea ASC";
                SqlCommand cmd = new SqlCommand(query, cn);         // prepara
                cmd.Parameters.AddWithValue("@DocEntry", DocEntry);
                cn.Open();

                try
                {
                    SqlDataReader dr = cmd.ExecuteReader();             // ejecuta

                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            RPD1_E det = new RPD1_E();
                            if (!dr.IsDBNull(0)) { det.DocEntry = dr.GetInt32(0); }
                            if (!dr.IsDBNull(1)) { det.Linea = dr.GetInt32(1); }
                            if (!dr.IsDBNull(2)) { det.ItemCode = dr.GetString(2); }
                            if (!dr.IsDBNull(3)) { det.ItemName = dr.GetString(3); }
                            if (!dr.IsDBNull(4)) { det.FirmCode = dr.GetInt32(4); }
                            if (!dr.IsDBNull(5)) { det.BatchNum = dr.GetString(5); }
                            if (!dr.IsDBNull(6)) { det.ExpDate = dr.GetDateTime(6).ToString("yyyy-MM-dd"); }
                            if (!dr.IsDBNull(7)) { det.ExpDateFormat = dr.GetString(7); }
                            if (!dr.IsDBNull(8)) { det.Quantity = dr.GetDecimal(8); }
                            if (!dr.IsDBNull(9)) { det.NumInBuy = dr.GetDecimal(9); }
                            if (!dr.IsDBNull(10)) { det.BuyUnitMsr = dr.GetString(10); }
                            if (!dr.IsDBNull(11)) { det.Motivo = dr.GetInt32(11); }
                            if (!dr.IsDBNull(12)) { det.RefFactura = dr.GetString(12); }
                            if (!dr.IsDBNull(13)) { det.Observacion = dr.GetString(13); }
                            if (!dr.IsDBNull(14)) { det.DescMotivo = dr.GetString(14); }
                            if (!dr.IsDBNull(15)) { det.MaxQuantity = dr.GetDecimal(15); }
                            if (!dr.IsDBNull(16)) { det.Submotivo = dr.GetInt32(16); }
                            if (!dr.IsDBNull(17)) { det.DescSubmotivo = dr.GetString(17); }
                            if (!dr.IsDBNull(18)) { det.MaxQuantityOIBT = dr.GetDecimal(18); }
                            if (!dr.IsDBNull(19)) { det.NumInBuyKey = dr.GetDecimal(19); }
                            lista.Add(det);
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

