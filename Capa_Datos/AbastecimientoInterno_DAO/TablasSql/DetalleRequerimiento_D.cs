using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Capa_Entidad.AbastecimientoInterno_ENT.TablasSql;
using Capa_Entidad;
using System.Data;

namespace Capa_Datos.AbastecimientoInterno_DAO.TablasSql
{
    public class DetalleRequerimiento_D
    {
        readonly Utilitarios uti = new Utilitarios();

        public (Helper_E, List<DetalleRequerimientos_E>) ObtenerDetalleRequerimiento(string condicion, Dictionary<string, object> parametros)
        {
            var lista = new List<DetalleRequerimientos_E>();
            Helper_E _helper = new Helper_E();

            try
            {
                using (SqlConnection cn = new SqlConnection(uti.cadSql2))
                {
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = cn;

                    var sb = new StringBuilder();

                    sb.AppendLine($"SELECT DET.Id, DET.RequerimientoId, DET.ItemCode, DET.ItemName, DET.BatchNum, DET.CodigoUbicacionOrigen, DET.CodigoUbicacionDestino, DET.UmAlm, DET.ValorUmAlm, DET.QuantityMaster, DET.QuantitySaldo,");
                    sb.AppendLine("DET.QuantityUnidadesCajas, DET.AtendidoReserva, DET.AtendidoPicking");
                    sb.AppendLine("FROM DetalleRequerimientos DET");
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
                                var obj = new DetalleRequerimientos_E();

                                if (!dr.IsDBNull(0)) obj.Id = dr.GetInt32(0);
                                if (!dr.IsDBNull(1)) obj.RequerimientoId = dr.GetInt32(1);
                                if (!dr.IsDBNull(2)) obj.ItemCode = dr.GetString(2);
                                if (!dr.IsDBNull(3)) obj.ItemName = dr.GetString(3);
                                if (!dr.IsDBNull(4)) obj.BatchNum = dr.GetString(4);
                                if (!dr.IsDBNull(5)) obj.CodigoUbicacionOrigen = dr.GetString(5);
                                if (!dr.IsDBNull(6)) obj.CodigoUbicacionDestino = dr.GetString(6);
                                if (!dr.IsDBNull(7)) obj.UmAlm = dr.GetString(7);
                                if (!dr.IsDBNull(8)) obj.ValorUmAlm = dr.GetInt32(8);
                                if (!dr.IsDBNull(9)) obj.QuantityMaster = dr.GetInt32(9);
                                if (!dr.IsDBNull(10)) obj.QuantitySaldo = dr.GetInt32(10);
                                if (!dr.IsDBNull(11)) obj.QuantityUnidadesCajas = dr.GetInt32(11);
                                if (!dr.IsDBNull(12)) obj.AtendidoReserva = dr.GetInt32(12);
                                if (!dr.IsDBNull(13)) obj.AtendidoPicking = dr.GetInt32(13);

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
                LogHelper.RegistrarError(ex, "Error inesperado en DetalleRequerimiento_D - ObtenerDetalleRequerimiento()");

                _helper.Titulo = "Error";
                _helper.Mensajes.Add("Ocurrió un error al obtener datos.");
                _helper.Mensajes.Add("Por favor, comuníquese con el área de Sistemas para más información.");
                _helper.Icono = "error";
            }

            return (_helper, lista);
        }

        public Helper_E EliminarItem(int id, string operarioRegistra)
        {
            string mensaje, icono;
            Helper_E _helper = new Helper_E();

            try
            {
                using (SqlConnection cn = new SqlConnection(uti.cadSql2))
                {
                    cn.Open();
                    using (SqlCommand cmd = new SqlCommand("sp_MantenimientoDetalleRequerimiento", cn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@TipoMantenimiento", "ELIMINAR_ITEM");
                        cmd.Parameters.AddWithValue("@Id", id);
                        cmd.Parameters.AddWithValue("@OperarioRegistra", operarioRegistra);

                        cmd.ExecuteNonQuery();

                        _helper.Titulo = "Acción completada";
                        _helper.Mensajes = new List<string> { "Se eliminó el ítem correctamente." };
                        _helper.Icono = "success";
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.RegistrarError(ex, "Error inesperado en DetalleRequerimiento_D - EliminarItem()");
                _helper.Titulo = "Error";
                _helper.Mensajes = new List<string> { "Ocurrió un error al eliminar ítem. Comuníquese con el área de Sistemas para más información." };
                _helper.Icono = "error";
            }

            return _helper;
        }
    }
}