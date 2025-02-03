using Capa_Entidad.Almacen_ENT.ReportesSql;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Capa_Entidad.Almacen_ENT.TablasSql;
using System.Data;

namespace Capa_Datos.Almacen_DAO.RptInternos
{
    public class ContInv_D
    {
        DBHelper db = new DBHelper(); Utilitarios uti = new Utilitarios();
        public List<ContInv_E> RptContInv(OIAR_E o)
        {
            List<ContInv_E> lista = new List<ContInv_E>();

            using (SqlConnection cn = new SqlConnection(uti.cadSql))
            {
                try
                {
                    cn.Open();
                    using (SqlCommand cmd = new SqlCommand("al.RptContInv", cn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 120; // Tiempo en segundos

                        // Agregar parámetros
                        cmd.Parameters.AddWithValue("@DocEntryPer", o.DocEntryPer);
                        cmd.Parameters.AddWithValue("@ItemCode", o.ItemCode);
                        cmd.Parameters.AddWithValue("@Fase", o.Fase);

                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            while (dr.Read())
                            {
                                ContInv_E bean = new ContInv_E
                                {
                                    DocEntryPer = dr.IsDBNull(0) ? 0 : dr.GetInt32(0),
                                    DescripcionPer = dr.IsDBNull(1) ? string.Empty : dr.GetString(1),
                                    FecIniPer = dr.IsDBNull(2) ? string.Empty : dr.GetDateTime(2).ToString("dd/MM/yyyy"),
                                    FecFinPer = dr.IsDBNull(3) ? string.Empty : dr.GetDateTime(3).ToString("dd/MM/yyyy"),
                                    Fase = dr.IsDBNull(4) ? 0 : dr.GetInt32(4),
                                    NombreFase = dr.IsDBNull(5) ? string.Empty : dr.GetString(5),
                                    WhsCode = dr.IsDBNull(6) ? string.Empty : dr.GetString(6),
                                    ItemCode = dr.IsDBNull(7) ? string.Empty : dr.GetString(7),
                                    ItemName = dr.IsDBNull(8) ? string.Empty : dr.GetString(8),
                                    QuantityCajas = dr.IsDBNull(9) ? 0 : dr.GetDecimal(9),
                                    QuantityPiezas = dr.IsDBNull(10) ? 0 : dr.GetDecimal(10),
                                    Equipos = dr.IsDBNull(11) ? string.Empty : dr.GetString(11),
                                    NumInBuy = dr.IsDBNull(12) ? 0 : dr.GetDecimal(12),
                                    QuantityTotalPzSist = dr.IsDBNull(13) ? 0 : dr.GetDecimal(13)
                                };

                                // Procesamiento por fases
                                switch (bean.Fase)
                                {
                                    case 3:
                                        bean.ParticipantesC = dr.IsDBNull(15) ? string.Empty : dr.GetString(15);
                                        bean.inicializarConteo();
                                        break;
                                    case 5:
                                        bean.DifConteo = dr.IsDBNull(14) ? 0 : dr.GetDecimal(14);
                                        bean.ParticipantesR = dr.IsDBNull(15) ? string.Empty : dr.GetString(15);
                                        bean.inicializarReconteo();
                                        break;
                                    case 7:
                                        bean.DifConteo = dr.IsDBNull(14) ? 0 : dr.GetDecimal(14);
                                        bean.DifReConteo = dr.IsDBNull(15) ? 0 : dr.GetDecimal(15);
                                        bean.DifAnalisis = dr.IsDBNull(16) ? 0 : dr.GetDecimal(16);
                                        bean.AvgPrice = dr.IsDBNull(17) ? 0 : dr.GetDecimal(17);
                                        bean.ObsLoteC = dr.IsDBNull(18) ? string.Empty : dr.GetString(18);
                                        bean.ObsLoteR = dr.IsDBNull(19) ? string.Empty : dr.GetString(19);
                                        bean.ObsLoteA = dr.IsDBNull(20) ? string.Empty : dr.GetString(20);
                                        bean.BatchNum = dr.IsDBNull(21) ? string.Empty : dr.GetString(21);
                                        bean.ExpDate = dr.IsDBNull(22) ? string.Empty : dr.GetDateTime(22).ToString("dd/MM/yyyy");
                                        bean.Pisos = dr.IsDBNull(23) ? string.Empty : dr.GetString(23);
                                        bean.ParticipantesC = dr.IsDBNull(24) ? string.Empty : dr.GetString(24);
                                        bean.ParticipantesR = dr.IsDBNull(25) ? string.Empty : dr.GetString(25);
                                        bean.ParticipantesA = dr.IsDBNull(26) ? string.Empty : dr.GetString(26);
                                        bean.inicializarAnalisis();
                                        break;
                                }

                                lista.Add(bean);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                }
            }

            return lista;
        }

        DataTable definirTabla(List<string> campos, List<Type> tipos, string nombre)
        {
            DataTable tb = new DataTable(nombre);
            int i = 0;
            foreach (string campo in campos)
            {
                DataColumn dc = new DataColumn(campo, tipos[i]);
                dc.ReadOnly = true;
                tb.Columns.Add(dc);
                i++;
            }
            return tb;
        }
        public DataTable tbRptContInv(OIAR_E o)
        {
            List<string> campos = new List<string>();
            List<Type> tipos = new List<Type>();
            campos.Add("Orden"); tipos.Add(typeof(string));
            campos.Add("DocEntryPer"); tipos.Add(typeof(string));
            campos.Add("DescripcionPer"); tipos.Add(typeof(string));
            campos.Add("FecIniPer"); tipos.Add(typeof(string));
            campos.Add("FecFinPer"); tipos.Add(typeof(string));
            campos.Add("Fase"); tipos.Add(typeof(string));
            campos.Add("NombreFase"); tipos.Add(typeof(string));
            campos.Add("WhsCode"); tipos.Add(typeof(string));
            campos.Add("ItemCode"); tipos.Add(typeof(string));
            campos.Add("ItemName"); tipos.Add(typeof(string));
            campos.Add("BatchNum"); tipos.Add(typeof(string));
            campos.Add("ExpDate"); tipos.Add(typeof(string));
            campos.Add("Pisos"); tipos.Add(typeof(string));
            campos.Add("QuantityCajas"); tipos.Add(typeof(string));
            campos.Add("QuantityPiezas"); tipos.Add(typeof(string));
            campos.Add("QuantityTotalPzCont"); tipos.Add(typeof(string));
            campos.Add("QuantityTotalPzSist"); tipos.Add(typeof(string));
            campos.Add("DifConteo"); tipos.Add(typeof(string));
            campos.Add("DifReConteo"); tipos.Add(typeof(string));
            campos.Add("DifAnalisis"); tipos.Add(typeof(string));
            campos.Add("DifDif"); tipos.Add(typeof(string));
            campos.Add("Resultado"); tipos.Add(typeof(string));
            campos.Add("Equipos"); tipos.Add(typeof(string));
            campos.Add("NumInBuy"); tipos.Add(typeof(string));
            campos.Add("AvgPrice"); tipos.Add(typeof(string));
            campos.Add("ObsLoteC"); tipos.Add(typeof(string));
            campos.Add("ObsLoteR"); tipos.Add(typeof(string));
            campos.Add("ObsLoteA"); tipos.Add(typeof(string));
            campos.Add("ParticipantesC"); tipos.Add(typeof(string));
            campos.Add("ParticipantesR"); tipos.Add(typeof(string));
            campos.Add("ParticipantesA"); tipos.Add(typeof(string));
            DataTable tb = definirTabla(campos, tipos, "DataTableReporteContInv");
            int i = 0;
            foreach (ContInv_E p in RptContInv(o))
            {
                DataRow row = tb.NewRow();
                row["Orden"] = i++;
                row["DocEntryPer"] = p.DocEntryPer;
                row["DescripcionPer"] = p.DescripcionPer;
                row["FecIniPer"] = p.FecIniPer;
                row["FecFinPer"] = p.FecFinPer;
                row["Fase"] = p.Fase;
                row["NombreFase"] = p.NombreFase;
                row["WhsCode"] = p.WhsCode;
                row["ItemCode"] = p.ItemCode;
                row["ItemName"] = p.ItemName;
                row["BatchNum"] = p.BatchNum;
                row["ExpDate"] = p.ExpDate;
                row["Pisos"] = p.Pisos;
                row["QuantityCajas"] = p.QuantityCajas;
                row["QuantityPiezas"] = p.QuantityPiezas;
                row["QuantityTotalPzCont"] = p.QuantityTotalPzCont;
                row["QuantityTotalPzSist"] = p.QuantityTotalPzSist;
                row["DifConteo"] = p.DifConteo;
                row["DifReConteo"] = p.DifReConteo;
                row["DifAnalisis"] = p.DifAnalisis;
                row["DifDif"] = p.DifDif;
                row["Resultado"] = p.Resultado;
                row["Equipos"] = p.Equipos;
                row["NumInBuy"] = p.NumInBuy;
                row["AvgPrice"] = p.AvgPrice;
                row["ObsLoteC"] = p.ObsLoteC;
                row["ObsLoteR"] = p.ObsLoteR;
                row["ObsLoteA"] = p.ObsLoteA;
                row["ParticipantesC"] = p.ParticipantesC;
                row["ParticipantesR"] = p.ParticipantesR;
                row["ParticipantesA"] = p.ParticipantesA;
                tb.Rows.Add(row);
            }
            return tb;
        }
    }
}