using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Capa_Entidad.AbastecimientoInterno_ENT.TablasSql;
using Capa_Entidad;

namespace Capa_Datos.AbastecimientoInterno_DAO.TablasSql
{
    public class DetalleTransferenciaReserva_D
    {
        readonly Utilitarios uti = new Utilitarios();

        public (Helper_E, List<DetalleTransferenciaReserva_E>) ObtenerDetalleTransferenciaReserva(string condicion, Dictionary<string, object> parametros)
        {
            var lista = new List<DetalleTransferenciaReserva_E>();
            Helper_E _helper = new Helper_E();

            try
            {
                using (SqlConnection cn = new SqlConnection(uti.cadSql2))
                {
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = cn;

                    var sb = new StringBuilder();

                    sb.AppendLine("SELECT Id, TransferenciaReservaId, ItemCode, ItemName, BatchNum, CONVERT(VARCHAR, ExpDate, 103), CONVERT(VARCHAR, InDate, 103), CodigoUbicacion, UmAlm, ValorUmAlm, QuantityMaster, QuantitySaldo,");
                    sb.AppendLine("QuantityUnidadesCajas, AtendidoReserva, Validado");
                    sb.AppendLine("FROM DetalleTransferenciaReserva");
                    sb.AppendLine(condicion?.ToString().Trim());

                    // Agregamos los parámetros dinámicamente
                    foreach (var prm in parametros)
                    {
                        cmd.Parameters.AddWithValue(prm.Key, prm.Value);
                    }

                    //sb.AppendLine("ORDER BY 1 DESC");
                    cmd.CommandText = sb.ToString();

                    cn.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.HasRows)
                        {
                            while (dr.Read())
                            {
                                var obj = new DetalleTransferenciaReserva_E();

                                if (!dr.IsDBNull(0)) obj.Id = dr.GetInt32(0);
                                if (!dr.IsDBNull(1)) obj.TransferenciaReservaId = dr.GetInt32(1);
                                if (!dr.IsDBNull(2)) obj.ItemCode = dr.GetString(2);
                                if (!dr.IsDBNull(3)) obj.ItemName = dr.GetString(3);
                                if (!dr.IsDBNull(4)) obj.BatchNum = dr.GetString(4);
                                if (!dr.IsDBNull(5)) obj.ExpDate = dr.GetString(5);
                                if (!dr.IsDBNull(6)) obj.InDate = dr.GetString(6);
                                if (!dr.IsDBNull(7)) obj.CodigoUbicacion = dr.GetString(7);
                                if (!dr.IsDBNull(8)) obj.UmAlm = dr.GetString(8);
                                if (!dr.IsDBNull(9)) obj.ValorUmAlm = dr.GetInt32(9);
                                if (!dr.IsDBNull(10)) obj.QuantityMaster = dr.GetInt32(10);
                                if (!dr.IsDBNull(11)) obj.QuantitySaldo = dr.GetInt32(11);
                                if (!dr.IsDBNull(12)) obj.QuantityUnidadesCajas = dr.GetInt32(12);
                                if (!dr.IsDBNull(13)) obj.AtendidoReserva = dr.GetInt32(13);
                                if (!dr.IsDBNull(14)) obj.Validado = dr.GetInt32(14);

                                lista.Add(obj);
                            }
                        }

                        _helper.Titulo = "Acción completada";
                        _helper.Mensajes.Add("Se obtuvo los datos correctamente.");
                        _helper.Icono = "success";
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.RegistrarError(ex, "Error inesperado en DetalleTransferenciaReserva_D - ObtenerDetalleTransferenciaReserva()");

                _helper.Titulo = "Error";
                _helper.Mensajes.Add("Ocurrió un error al obtener datos.");
                _helper.Mensajes.Add("Por favor, comuníquese con el área de Sistemas para más información.");
                _helper.Icono = "error";
            }

            return (_helper, lista);
        }
    }
}