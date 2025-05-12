using Capa_Entidad.AbastecimientoInterno_ENT.TablasSql;
using Capa_Entidad;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace Capa_Datos.AbastecimientoInterno_DAO.TablasSql
{
    public class ProductosDisponiblesReserva_D
    {
        Utilitarios uti = new Utilitarios();

        public List<ProductosDisponiblesReserva_E> ObtenerProductosDisponiblesReserva()
        {
            List<ProductosDisponiblesReserva_E> listaKdx = new List<ProductosDisponiblesReserva_E>();
            try
            {
                using (SqlConnection cn = new SqlConnection(uti.cadSql2))
                {
                    cn.Open();
                    using (SqlCommand cmd = new SqlCommand("sp_ProductosDisponiblesReserva", cn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        SqlDataReader dr = cmd.ExecuteReader();

                        if (dr.HasRows)
                        {

                            while (dr.Read())
                            {
                                ProductosDisponiblesReserva_E kdx = new ProductosDisponiblesReserva_E();

                                if (!dr.IsDBNull(0)) { kdx.ItemCode = dr.GetString(0); }
                                if (!dr.IsDBNull(1)) { kdx.BatchNum = dr.GetString(1); }
                                if (!dr.IsDBNull(2)) { kdx.UmAlm = dr.GetString(2); }
                                if (!dr.IsDBNull(3)) { kdx.ValorUmAlm = dr.GetInt32(3); }
                                if (!dr.IsDBNull(4)) { kdx.CodigoUbicacionOrigen = dr.GetString(4); }
                                if (!dr.IsDBNull(5)) { kdx.DisponibleMaster = dr.GetInt32(5); }
                                if (!dr.IsDBNull(6)) { kdx.DisponibleSaldo = dr.GetInt32(6); }

                                listaKdx.Add(kdx);
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                LogHelper.RegistrarError(ex, "ProductosDisponiblesReserva_D - ObtenerProductosDisponiblesReserva");
                throw new Exception("Error en ObtenerProductosDisponiblesReserva.", ex);
            }

            return listaKdx;
        }
    }
}
