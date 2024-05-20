using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Capa_Entidad.Compras_ENT;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using Sap.Data.Hana;
using Capa_Entidad.Compras_ENT.Reportes;
using Capa_Entidad.Almacen_ENT.Tablas;
using Capa_Entidad.Compras_ENT.Tablas;
using Capa_Entidad.SocioNegocios_ENT.Tablas;
using Capa_Datos.Almacen_DAO.Tablas;
using Capa_Datos.SocioNegocios_DAO.Tablas;
using Capa_Datos.Compras_DAO.TablasSql;
using Capa_Datos.Compras_DAO.Tablas;

namespace Capa_Datos.Compras_DAO
{
    public class ContratoRebate_D 
    {
        Utilitarios uti = new Utilitarios();OCRD_D ocrdD = new OCRD_D();
        LineaProduccion_D lpD = new LineaProduccion_D(); OMRC_D omrcD = new OMRC_D();
        DBHelper db = new DBHelper(); LNPV_D lnpvD = new LNPV_D();
        OPCH_D opchD = new OPCH_D();
        public List<ContratoRebate_E> listarContratosRebate(ContratoRebate_E ct)
        {
            List<ContratoRebate_E> lista = new List<ContratoRebate_E>();
            SqlConnection cn = new SqlConnection(uti.cadSql);
            try
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand(queryFiltroContratoR(ct), cn);
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    ContratoRebate_E c = obtenerContratoRebate(dr.GetInt32(0));
                    lista.Add(c);
                }
                dr.Close();
                cn.Close();
            }catch { cn.Close(); }
            return lista;
        }
        private string queryFiltroContratoR(ContratoRebate_E ct)
        {
            string query = "select top 100 * from cc.OCRT";
            if (ct != null)
            {
                query += " where id>0";
                if (ct.id > 0) { query += " and id=" + ct.id; }
                if (ct.Tipo != null) { query += " and Tipo='" + ct.Tipo + "'"; }
                if (ct.SocioDesc != null) { query += " and SocioDesc like '%" + ct.SocioDesc + "%'"; }
                if (ct.Titulo != null) { query += " and Titulo like '%" + ct.Titulo + "%'"; }
                query += " order by 1 desc";
            }
            return query;
        }
        public ContratoRebate_E obtenerContratoRebate(int id)
        {
            ContratoRebate_E c = new ContratoRebate_E() { Det = new List<DetContratoRebate_E>() };
            string query = "select * from cc.OCRT where id=@id";
            SqlConnection cn = new SqlConnection(uti.cadSql);
            try
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand(query, cn);
                cmd.Parameters.AddWithValue("@id", id);
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    c.id = dr.GetInt32(0);
                    if (!dr.IsDBNull(1)) { c.Tipo = dr.GetString(1); }
                    if (!dr.IsDBNull(2)) { c.SocioDesc = dr.GetString(2); }
                    if (!dr.IsDBNull(3)) { c.SocioCod = dr.GetString(3); }
                    if (!dr.IsDBNull(4)) { c.Titulo = dr.GetString(4); }
                    if (!dr.IsDBNull(5)) { c.Contenido = dr.GetString(5); }
                    if (!dr.IsDBNull(6)) { c.PerValIni = dr.GetDateTime(6).ToString("yyyy-MM-dd"); }
                    if (!dr.IsDBNull(7)) { c.PerValFin = dr.GetDateTime(7).ToString("yyyy-MM-dd"); }
                    if (!dr.IsDBNull(8)) { c.ObjPer = dr.GetDecimal(8); }
                    if (!dr.IsDBNull(9)) { c.EncCompras = dr.GetString(9); }
                    if (!dr.IsDBNull(10)) { c.EncSocio = dr.GetString(10); }
                    if (!dr.IsDBNull(11)) { c.Estado = dr.GetString(11); }
                    if (!dr.IsDBNull(12)) { c.ConfDesc = dr.GetString(12); }
                    c.Det = obtenerDetCRT(c.id);
                }
                dr.Close();
                cn.Close();
            }catch { cn.Close(); }
            return c;
        }
        private List<DetContratoRebate_E> obtenerDetCRT(int idOCRT)
        {
            List<DetContratoRebate_E> lista = new List<DetContratoRebate_E>();
            SqlConnection cn = new SqlConnection(uti.cadSql);
            try
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand("select * from cc.CRT1 where idOCRT=@idOCRT", cn);
                cmd.Parameters.AddWithValue("idOCRT", idOCRT);
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    DetContratoRebate_E det = new DetContratoRebate_E();
                    if (!dr.IsDBNull(0)) { det.idOCRT = dr.GetInt32(0); }
                    if (!dr.IsDBNull(1)) { det.Linea = dr.GetInt32(1); }
                    if (!dr.IsDBNull(2)) { det.idORLP = dr.GetInt32(2); }
                    if (!dr.IsDBNull(3)) { det.Descripcion = dr.GetString(3); }
                    if (!dr.IsDBNull(4)) { det.U_SYP_DESC = dr.GetString(4); }
                    if (!dr.IsDBNull(5)) { det.CardName = dr.GetString(5); }
                    if (!dr.IsDBNull(6)) { det.PeriodoRebate = dr.GetString(6); }
                    if (!dr.IsDBNull(7)) { det.SubTipo = dr.GetString(7); }
                    if (!dr.IsDBNull(8)) { det.ConDev = dr.GetString(8); }
                    det.EspDet2 = obtenerEspDetCRT(det.idOCRT,det.Linea);
                    lista.Add(det);
                }
                dr.Close();
                cn.Close();
            }catch{ cn.Close(); }
            return lista;
        }
        private List<EspDetContratoRebate2_E> obtenerEspDetCRT(int idOCRT,int BaseLinea)
        {
            List<EspDetContratoRebate2_E> lista = new List<EspDetContratoRebate2_E>();
            SqlConnection cn = new SqlConnection(uti.cadSql);
            try
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand("select * from cc.CRT12 where idOCRT=@idOCRT and BaseLinea=@BaseLinea ", cn);
                cmd.Parameters.AddWithValue("@idOCRT", idOCRT);
                cmd.Parameters.AddWithValue("@BaseLinea",BaseLinea);
                SqlDataReader dr = cmd.ExecuteReader();
                while(dr.Read())
                {
                    EspDetContratoRebate2_E esp = new EspDetContratoRebate2_E();
                    if (!dr.IsDBNull(0)) { esp.idOCRT = dr.GetInt32(0) ; }
                    if (!dr.IsDBNull(1)) { esp.BaseLinea = dr.GetInt32(1); }
                    if (!dr.IsDBNull(2)) { esp.Linea = dr.GetInt32(2); }
                    if (!dr.IsDBNull(3)) { esp.Rango = dr.GetDateTime(3).ToString("yyyy-MM-dd"); }
                    if (!dr.IsDBNull(4)) { esp.CuotaMin = dr.GetDecimal(4); }
                    if (!dr.IsDBNull(5)) { esp.Rebate = dr.GetDecimal(5); }
                    if (!dr.IsDBNull(6)) { esp.MaxDia = dr.GetInt32(6); }
                    if (!dr.IsDBNull(7)) { esp.InfoDia = dr.GetInt32(7); }
                    if (!dr.IsDBNull(8)) { esp.UltimasFacturas = dr.GetInt32(8); }
                    if (!dr.IsDBNull(9)) { esp.MinDia = dr.GetInt32(9); }
                    if (!dr.IsDBNull(10)) { esp.NroFactura = dr.GetString(10); }
                    if (!dr.IsDBNull(11)) { esp.RangoF = dr.GetDateTime(11).ToString("yyyy-MM-dd"); }
                    if (!dr.IsDBNull(12)) { esp.Displays = dr.GetDecimal(12); }
                    lista.Add(esp);
                }
                dr.Close();
                cn.Close();
            }catch { cn.Close(); }
            return lista;
        }
        public int CrearContratoRebate(ContratoRebate_E c)
        {
            int status = 0;
            SqlConnection cn = new SqlConnection(uti.cadSql);
            try
            {
                cn.Open();
                SqlTransaction tran = cn.BeginTransaction("transaccion1");
                try
                {
                    SqlCommand cmd = new SqlCommand("MANT_OCRT", cn);
                    cmd.Transaction = tran;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@TipoMantenimiento","A");
                    cmd.Parameters.AddWithValue("@id",c.id).Direction=ParameterDirection.InputOutput;
                    cmd.Parameters.AddWithValue("@Tipo",c.Tipo);
                    cmd.Parameters.AddWithValue("@SocioDesc",c.SocioDesc);
                    cmd.Parameters.AddWithValue("@SocioCod",c.SocioCod);
                    cmd.Parameters.AddWithValue("@Titulo",c.Titulo);
                    cmd.Parameters.AddWithValue("@Contenido",c.Contenido);
                    cmd.Parameters.AddWithValue("@PerValIni",c.PerValIni);
                    cmd.Parameters.AddWithValue("@PerValFin",c.PerValFin );
                    cmd.Parameters.AddWithValue("@ObjPer",c.ObjPer );
                    cmd.Parameters.AddWithValue("@EncCompras",c.EncCompras );
                    cmd.Parameters.AddWithValue("@EncSocio",c.EncSocio );

                    SqlParameter tbDet = new SqlParameter("@TPCRT1", SqlDbType.Structured);
                    tbDet.Value = DetContratoRebate_E.tbDetalle(c.Det);
                    tbDet.TypeName = "dbo.TPCRT1";
                    cmd.Parameters.AddWithValue("@TPCRT1", tbDet.Value);

                    SqlParameter tbEsp2 = new SqlParameter("@TPCRT12",SqlDbType.Structured);
                    tbEsp2.Value = DetContratoRebate_E.tbEspDet2(c.Det);
                    tbEsp2.TypeName = "dbo.TPCRT12";
                    cmd.Parameters.AddWithValue("@TPCRT12", tbEsp2.Value);

                    cmd.ExecuteNonQuery();
                    status = int.Parse(cmd.Parameters["@id"].Value.ToString());
                    //post transacciones
                    SqlCommand cmd2 = new SqlCommand("POST_TRANSACCIONES", cn);
                    cmd2.Transaction = tran;
                    cmd2.CommandType = CommandType.StoredProcedure;
                    cmd2.Parameters.AddWithValue("@Tipo", "A");
                    cmd2.Parameters.AddWithValue("@Tabla", "OCRT");
                    cmd2.Parameters.AddWithValue("@DocNum", cmd.Parameters["@id"].Value);
                    cmd2.Parameters.AddWithValue("@DocEntry", cmd.Parameters["@id"].Value);
                    cmd2.ExecuteNonQuery();
                    tran.Commit();
                }
                catch {tran.Rollback(); cn.Close(); throw new Exception("Error en creacion: "); }
                cn.Close();
            }catch(Exception e2) { cn.Close(); throw new Exception("Error en creacion y conexion: " + e2.Message); }
            return status;
        }
        public int EliminarContratoRebate(int id)
        {
            int status = 0;
            SqlConnection cn = new SqlConnection(uti.cadSql);
            try
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand("MANT_OCRT", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@TipoMantenimiento", "D");
                cmd.Parameters.AddWithValue("@id", id).Direction = ParameterDirection.InputOutput;
                cmd.ExecuteNonQuery();
                status = 1;
                cn.Close();
            }
            catch (Exception e) { cn.Close(); throw new Exception("Error en eliminacion: " + e.Message); }
            return status;
        }
        public int EditarContratoRebate(string TipoMantenimiento,ContratoRebate_E c)
        {
            int status = 0;
            SqlConnection cn = new SqlConnection(uti.cadSql);
            try
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand("MANT_OCRT", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@TipoMantenimiento", TipoMantenimiento);
                cmd.Parameters.AddWithValue("@id", c.id).Direction = ParameterDirection.InputOutput;
                cmd.Parameters.AddWithValue("@ConfDesc", c.ConfDesc);
                cmd.ExecuteNonQuery();
                status = 1;
                cn.Close();
            }catch(Exception e) { cn.Close(); status =-1; throw new Exception("Error en edicion: " + e.Message); }
            return status;
        }
        //reportes cuadre
        public CuadreContrato_E GenerarCuadreContrato(int id) // conexion a hana
        {
            ContratoRebate_E c = obtenerContratoRebate(id);
            CuadreContrato_E cu = new CuadreContrato_E() { Det = new List<DetCuadreContrato_E>()};
            cu.SocioDesc = c.SocioDesc;cu.Tipo = c.Tipo;
            cu.PerIni = c.PerValIni;cu.PerFin = c.PerValFin;
            cu.Estado = c.Estado;
            foreach(DetContratoRebate_E d in c.Det)
            {
                DetCuadreContrato_E dcu = new DetCuadreContrato_E() {Esp = new List<EspCuadreContrato_E>() };
                LineaProduccion_E lp = lpD.obtenerLineaProduccion(d.idORLP);
                dcu.Descripcion = lp.Descripcion;
                dcu.U_SYP_DESC = lp.Fabricante.U_SYP_DESC;
                dcu.CardCode = lp.Proveedor.CardCode;
                dcu.CardName = lp.Proveedor.CardName;
                dcu.SubTipo = d.SubTipo;
                dcu.Esp = obtenerEspCuadreContrato(lp, c,d);
                cu.Det.Add(dcu);
            }
            return cu;
        }
        private List<EspCuadreContrato_E> obtenerEspCuadreContrato(LineaProduccion_E l,ContratoRebate_E c,DetContratoRebate_E d)
        {
            List<EspCuadreContrato_E> lista = new List<EspCuadreContrato_E>();
            string perIniC = c.PerValIni;string perFinC = c.PerValFin;
            string FirmName = l.Fabricante.FirmName;
            string proveedoresListaNegra="''";
            foreach(ProveedorListaNegra_E pvl in lnpvD.listarProveedoresListaNegra())
            {
                proveedoresListaNegra += ",'" + pvl.CardCode + "'";
            }
            string productos = "";
            foreach(DetLineaProduccion_E dt in l.Det)
            {
                if(dt==l.Det[0])
                {
                    productos += "'" + dt.ItemCode + "'";
                }
                else
                {
                    productos += ",'" + dt.ItemCode + "'";
                }
            }           
            string PeriodoRebate = d.PeriodoRebate; string SubTipo = d.SubTipo;
            foreach(EspDetContratoRebate2_E esp in d.EspDet2)
            {
                string query = "";
                string perIni = perIniC; string perFin = perFinC;
                string perIniNC = perIniC; string perFinNC = perFinC;
                if(esp.Rango!=null && esp.RangoF!=null) // generando las fechas de rango que se usaran en los querys
                {
                    perIni = esp.Rango; perFin = esp.RangoF;
                    perIniNC = DateTime.Parse(perFin).AddDays(1).ToString("yyyy-MM-dd");
                    //perFinNC = DateTime.Parse(perIniNC).AddDays((DateTime.Parse(perFin)-DateTime.Parse(perIni)).TotalDays).ToString("yyyy-MM-dd");
                    perFinNC = DateTime.Parse(perIniNC).AddDays(248).ToString("yyyy-MM-dd");
                }
                if (SubTipo.Equals("Compra") && esp.CuotaMin > 0 && esp.Rebate > 0 )
                {                 
                    if (d.CardName != null && !d.CardName.Equals(""))
                    {
                        query= "SELECT distinct T0.\"CardName\"" +
                                        " FROM " + uti.schemaHana + "OPCH T0" +
                                        " INNER JOIN " + uti.schemaHana + "PCH1 T1 ON T1.\"DocEntry\" = T0.\"DocEntry\" "+//AND T1.\"Quantity\">=" +esp.Displays+
                                        " INNER JOIN " + uti.schemaHana + "OITM T2 ON T2.\"ItemCode\" = T1.\"ItemCode\" " +
                                                                             " AND T2.\"FirmCode\" = (select \"FirmCode\" from " + uti.schemaHana + "omrc where \"FirmName\"='" + FirmName + "') " +
                                                                             " AND T2.\"ItemCode\" in(" + productos + ")" +
                                        " WHERE T0.\"TaxDate\" BETWEEN '" + perIni + "' AND '" + perFin +"'"+
                                                 " AND T0.\"CardName\"='"+d.CardName+"' " +
                                                 " AND T0.\"CANCELED\"='N'" +
                                        " group by T0.\"CardCode\",T0.\"CardName\"" +
                                        " order by T0.\"CardCode\"";
                    }
                    else
                    {
                        query = "SELECT distinct T0.\"CardName\"" +
                                        " FROM " + uti.schemaHana + "OPCH T0" +
                                        " INNER JOIN " + uti.schemaHana + "PCH1 T1 ON T1.\"DocEntry\" = T0.\"DocEntry\" "+//AND T1.\"Quantity\">=" + esp.Displays +
                                        " INNER JOIN " + uti.schemaHana + "OITM T2 ON T2.\"ItemCode\" = T1.\"ItemCode\" " +
                                                                             " AND T2.\"FirmCode\" = (select \"FirmCode\" from " + uti.schemaHana + "omrc where \"FirmName\"='" + FirmName + "') " +
                                                                             " AND T2.\"ItemCode\" in(" + productos + ")" +
                                        " WHERE T0.\"TaxDate\" BETWEEN '" + perIni + "' AND '" + perFin +
                                                 "' AND T0.\"CardCode\" not in ("+proveedoresListaNegra+") " +
                                        " group by T0.\"CardCode\",T0.\"CardName\"" +
                                        " order by T0.\"CardCode\"";
                    }
                    EspCuadreContrato_E ecu = new EspCuadreContrato_E() { TranProveedor = new List<TranCuadreContrato_E>(),TranFabricante= new List<TranCuadreContrato_E>() };
                        ecu.Rango = esp.Rango;
                        ecu.RangoF = esp.RangoF;
                        ecu.CuotaMin=esp.CuotaMin;
                        ecu.Rebate=esp.Rebate;
                        ecu.Displays = esp.Displays;
                        HanaConnection hcn = new HanaConnection(uti.cadHana);
                        try
                        {       
                            hcn.Open();
                            HanaCommand hcmd = new HanaCommand(query, hcn);
                            HanaDataReader hdr = hcmd.ExecuteReader(); 
                            while (hdr.Read())
                            {
                                TranCuadreContrato_E tr= new TranCuadreContrato_E() { SubTranProv = new List<SubTranCuadreContrato>(), SubtranLabo = new SubTranCuadreContrato() }; ;
                                tr.SubTranProv= obtenerSubTranCCFt(hdr.GetString(0),perIni,perFin,productos,FirmName,esp.Displays);
                                tr.SubTranProvNC = obtenerSubTranCCNc(hdr.GetString(0),perIniNC,perFinNC,d.PeriodoRebate,tr.SubTranProv);
                                tr.SubTranProvNCArt = obtenerSubTranCCNcArt(hdr.GetString(0), tr.SubTranProv,perIni, perFin,productos,d.ConDev);
                                tr.SocioName = hdr.GetString(0);
                                ecu.TranProveedor.Add(tr);
                            }
                            hdr.Close();
                            hcn.Close();                      
                        }
                        catch { hcn.Close(); }
                    lista.Add(ecu);
                }
                else if(SubTipo.Equals("GrupoDias") && esp.Rebate>0 && esp.MinDia>0 && esp.MaxDia>0 && esp.MaxDia>esp.MinDia)
                {
                    DateTime auxFc = DateTime.Parse(perFin);
                    perIni = new DateTime(auxFc.Year, auxFc.Month, esp.MinDia).ToString("yyyy-MM-dd");
                    perFin = new DateTime(auxFc.Year, auxFc.Month, esp.MaxDia).ToString("yyyy-MM-dd");
                    if (d.CardName != null && !d.CardName.Equals(""))
                    {
                        query= "SELECT distinct T0.\"CardName\"" +
                                        " FROM " + uti.schemaHana + "OPCH T0" +
                                        " INNER JOIN " + uti.schemaHana + "PCH1 T1 ON T1.\"DocEntry\" = T0.\"DocEntry\" "+//AND T1.\"Quantity\">=" + esp.Displays +
                                        " INNER JOIN " + uti.schemaHana + "OITM T2 ON T2.\"ItemCode\" = T1.\"ItemCode\" " +
                                                                             " AND T2.\"FirmCode\" = (select \"FirmCode\" from " + uti.schemaHana + "omrc where \"FirmName\"='" + FirmName + "') " +
                                                                             " AND T2.\"ItemCode\" in(" + productos + ")" +
                                        " WHERE T0.\"TaxDate\" BETWEEN '" + perIni + "' AND '" + perFin +"'"+
                                                 " AND T0.\"CardName\"='"+d.CardName+"' " +
                                                 " AND T0.\"CANCELED\"='N'" +
                                        " group by T0.\"CardCode\",T0.\"CardName\",T0.\"TaxDate\"" +
                                        " order by T0.\"CardCode\",T0.\"TaxDate\"";
                    }
                    else
                    {
                        query = "SELECT distinct T0.\"CardName\"" +
                                        " FROM " + uti.schemaHana + "OPCH T0" +
                                        " INNER JOIN " + uti.schemaHana + "PCH1 T1 ON T1.\"DocEntry\" = T0.\"DocEntry\" "+//AND T1.\"Quantity\">=" + esp.Displays +
                                        " INNER JOIN " + uti.schemaHana + "OITM T2 ON T2.\"ItemCode\" = T1.\"ItemCode\" " +
                                                                             " AND T2.\"FirmCode\" = (select \"FirmCode\" from " + uti.schemaHana + "omrc where \"FirmName\"='" + FirmName + "') " +
                                                                             " AND T2.\"ItemCode\" in(" + productos + ")" +
                                        " WHERE T0.\"TaxDate\" BETWEEN '" + perIni + "' AND '" + perFin +
                                                 "' /*AND T0.\"CardCode\"=CardCode*/ " +
                                        " group by T0.\"CardCode\",T0.\"CardName\"" +
                                        " order by T0.\"CardCode\"";
                    }
                    EspCuadreContrato_E ecu = new EspCuadreContrato_E() { TranProveedor = new List<TranCuadreContrato_E>(),TranFabricante= new List<TranCuadreContrato_E>() };
                        ecu.Rango = esp.Rango;
                        ecu.CuotaMin=esp.CuotaMin;
                        ecu.Rebate=esp.Rebate;
                        ecu.RangoF = esp.RangoF;
                        ecu.Displays = esp.Displays;
                        HanaConnection hcn = new HanaConnection(uti.cadHana);
                        try
                        {
                            hcn.Open();
                            HanaCommand hcmd = new HanaCommand(query, hcn);
                            HanaDataReader hdr = hcmd.ExecuteReader(); 
                            while (hdr.Read())
                            {
                                TranCuadreContrato_E tr= new TranCuadreContrato_E() { SubTranProv = new List<SubTranCuadreContrato>(), SubtranLabo = new SubTranCuadreContrato() }; ;
                                tr.SubTranProv= obtenerSubTranCCFt(hdr.GetString(0),perIni,perFin,productos,FirmName,esp.Displays);
                                tr.SubTranProvNC = obtenerSubTranCCNc(hdr.GetString(0),perIniNC,perFinNC,d.PeriodoRebate,tr.SubTranProv);
                                //tr.SubTranProvNCArt = obtenerSubTranCCNcArt(hdr.GetString(0),tr.SubTranProv,productos,d.ConDev);
                                tr.SubTranProvNCArt = obtenerSubTranCCNcArt(hdr.GetString(0), tr.SubTranProv,perIni, perFin,productos,d.ConDev);
                                tr.SocioName = hdr.GetString(0);
                                ecu.TranProveedor.Add(tr);
                            }
                            hdr.Close();
                            hcn.Close();                      
                        }
                        catch { hcn.Close(); }
                    lista.Add(ecu);
                }
                else if(SubTipo.Equals("Informacion")&& esp.Rebate>0)
                {
                    if (d.CardName != null && !d.CardName.Equals(""))
                    {
                        query= "SELECT distinct T0.\"CardName\"" +
                                        " FROM " + uti.schemaHana + "OPCH T0" +
                                        " INNER JOIN " + uti.schemaHana + "PCH1 T1 ON T1.\"DocEntry\" = T0.\"DocEntry\" "+//AND T1.\"Quantity\">=" + esp.Displays +
                                        " INNER JOIN " + uti.schemaHana + "OITM T2 ON T2.\"ItemCode\" = T1.\"ItemCode\" " +
                                                                             " AND T2.\"FirmCode\" = (select \"FirmCode\" from " + uti.schemaHana + "omrc where \"FirmName\"='" + FirmName + "') " +
                                                                             " AND T2.\"ItemCode\" in(" + productos + ")" +
                                        " WHERE T0.\"TaxDate\" BETWEEN '" + perIni + "' AND '" + perFin +"'"+
                                                 " AND T0.\"CardName\"='"+d.CardName+"' " +
                                                 " AND T0.\"CANCELED\"='N'" +
                                        " group by T0.\"CardCode\",T0.\"CardName\",T0.\"TaxDate\"" +
                                        " order by T0.\"CardCode\",T0.\"TaxDate\"";
                    }
                    else
                    {
                        query = "SELECT distinct T0.\"CardName\"" +
                                        " FROM " + uti.schemaHana + "OPCH T0" +
                                        " INNER JOIN " + uti.schemaHana + "PCH1 T1 ON T1.\"DocEntry\" = T0.\"DocEntry\" "+//AND T1.\"Quantity\">=" + esp.Displays +
                                        " INNER JOIN " + uti.schemaHana + "OITM T2 ON T2.\"ItemCode\" = T1.\"ItemCode\" " +
                                                                             " AND T2.\"FirmCode\" = (select \"FirmCode\" from " + uti.schemaHana + "omrc where \"FirmName\"='" + FirmName + "') " +
                                                                             " AND T2.\"ItemCode\" in(" + productos + ")" +
                                        " WHERE T0.\"TaxDate\" BETWEEN '" + perIni + "' AND '" + perFin +
                                                 "' /*AND T0.\"CardCode\"=CardCode*/ " +
                                        " group by T0.\"CardCode\",T0.\"CardName\"" +
                                        " order by T0.\"CardCode\"";
                    }
                    EspCuadreContrato_E ecu = new EspCuadreContrato_E() { TranProveedor = new List<TranCuadreContrato_E>(),TranFabricante= new List<TranCuadreContrato_E>() };
                        ecu.Rango = esp.Rango;
                        ecu.CuotaMin=esp.CuotaMin;
                        ecu.Rebate=esp.Rebate;
                        ecu.RangoF = esp.RangoF;
                        ecu.Displays = esp.Displays;
                        HanaConnection hcn = new HanaConnection(uti.cadHana);
                        try
                        {               
                            hcn.Open();
                            HanaCommand hcmd = new HanaCommand(query, hcn);
                            HanaDataReader hdr = hcmd.ExecuteReader(); 
                            while (hdr.Read())
                            {
                                TranCuadreContrato_E tr= new TranCuadreContrato_E() { SubTranProv = new List<SubTranCuadreContrato>(), SubtranLabo = new SubTranCuadreContrato() }; 
                                tr.SubTranProv= obtenerSubTranCCFt(hdr.GetString(0),perIni,perFin,productos,FirmName,esp.Displays);
                                tr.SubTranProvNC = obtenerSubTranCCNc(hdr.GetString(0),perIniNC,perFinNC,d.PeriodoRebate,tr.SubTranProv);
                                //tr.SubTranProvNCArt = obtenerSubTranCCNcArt(hdr.GetString(0),tr.SubTranProv,productos,d.ConDev);
                                tr.SubTranProvNCArt = obtenerSubTranCCNcArt(hdr.GetString(0), tr.SubTranProv,perIni, perFin,productos,d.ConDev);
                                tr.SocioName = hdr.GetString(0);
                                ecu.TranProveedor.Add(tr);
                            }
                            hdr.Close();
                            hcn.Close();                      
                        }
                        catch { hcn.Close(); }
                    lista.Add(ecu);
                }
                else if(SubTipo.Equals("UltimasFacturas") && esp.Rebate>0 && esp.UltimasFacturas>0)
                {
                    if (d.CardName != null && !d.CardName.Equals(""))
                    {
                        query= "SELECT distinct T0.\"CardName\"" +
                                        " FROM " + uti.schemaHana + "OPCH T0" +
                                        " INNER JOIN " + uti.schemaHana + "PCH1 T1 ON T1.\"DocEntry\" = T0.\"DocEntry\" "+//AND T1.\"Quantity\">=" + esp.Displays +
                                        " INNER JOIN " + uti.schemaHana + "OITM T2 ON T2.\"ItemCode\" = T1.\"ItemCode\" " +
                                                                             " AND T2.\"FirmCode\" = (select \"FirmCode\" from " + uti.schemaHana + "omrc where \"FirmName\"='" + FirmName + "') " +
                                                                             " AND T2.\"ItemCode\" in(" + productos + ")" +
                                        " WHERE T0.\"TaxDate\" BETWEEN '" + perIni + "' AND '" + perFin +"'"+
                                                 " AND T0.\"CardName\"='"+d.CardName+"' " +
                                                 " AND T0.\"CANCELED\"='N'" +
                                        " group by T0.\"CardCode\",T0.\"CardName\",T0.\"TaxDate\"" +
                                        " order by T0.\"CardCode\",T0.\"TaxDate\"";
                    }
                    else
                    {
                        query = "SELECT distinct T0.\"CardName\"" +
                                        " FROM " + uti.schemaHana + "OPCH T0" +
                                        " INNER JOIN " + uti.schemaHana + "PCH1 T1 ON T1.\"DocEntry\" = T0.\"DocEntry\" "+//AND T1.\"Quantity\">=" + esp.Displays +
                                        " INNER JOIN " + uti.schemaHana + "OITM T2 ON T2.\"ItemCode\" = T1.\"ItemCode\" " +
                                                                             " AND T2.\"FirmCode\" = (select \"FirmCode\" from " + uti.schemaHana + "omrc where \"FirmName\"='" + FirmName + "') " +
                                                                             " AND T2.\"ItemCode\" in(" + productos + ")" +
                                        " WHERE T0.\"TaxDate\" BETWEEN '" + perIni + "' AND '" + perFin +
                                                 "' /*AND T0.\"CardCode\"=CardCode*/ " +
                                        " group by T0.\"CardCode\",T0.\"CardName\"" +
                                        " order by T0.\"CardCode\"";
                    }
                    EspCuadreContrato_E ecu = new EspCuadreContrato_E() { TranProveedor = new List<TranCuadreContrato_E>(),TranFabricante= new List<TranCuadreContrato_E>() };
                        ecu.Rango = esp.Rango;
                        ecu.CuotaMin=esp.CuotaMin;
                        ecu.Rebate=esp.Rebate;
                        ecu.RangoF = esp.RangoF;
                        ecu.Displays = esp.Displays;
                        HanaConnection hcn = new HanaConnection(uti.cadHana);
                        try
                        {
                            hcn.Open();
                            HanaCommand hcmd = new HanaCommand(query, hcn);
                            HanaDataReader hdr = hcmd.ExecuteReader(); 
                            while (hdr.Read())
                            {
                                TranCuadreContrato_E tr= new TranCuadreContrato_E() { SubTranProv = new List<SubTranCuadreContrato>(), SubtranLabo = new SubTranCuadreContrato() }; ;
                                tr.SubTranProv= obtenerSubTranCCFt(hdr.GetString(0),perIni,perFin,productos,FirmName,esp.Displays,esp.UltimasFacturas);
                                tr.SubTranProvNC = obtenerSubTranCCNc(hdr.GetString(0),perIniNC,perFinNC,d.PeriodoRebate,tr.SubTranProv);
                                //tr.SubTranProvNCArt = obtenerSubTranCCNcArt(hdr.GetString(0),tr.SubTranProv,productos,d.ConDev);
                                tr.SubTranProvNCArt = obtenerSubTranCCNcArt(hdr.GetString(0), tr.SubTranProv,perIni, perFin,productos,d.ConDev);
                                tr.SocioName = hdr.GetString(0);
                                ecu.TranProveedor.Add(tr);
                            }
                            hdr.Close();
                            hcn.Close();                      
                        }
                        catch { hcn.Close(); }
                    lista.Add(ecu);
                }
                else if(SubTipo.Equals("FacturasEspecificas") && esp.Rebate>0 && esp.NroFactura!=null)
                {
                    EspCuadreContrato_E ecu = new EspCuadreContrato_E() 
                    { TranProveedor = new List<TranCuadreContrato_E>(), TranFabricante = new List<TranCuadreContrato_E>() };
                    ecu.Rango = esp.Rango;
                    ecu.CuotaMin = esp.CuotaMin;
                    ecu.Rebate = esp.Rebate;
                    ecu.RangoF = esp.RangoF;

                    TranCuadreContrato_E tr = new TranCuadreContrato_E() 
                    { SubTranProv = new List<SubTranCuadreContrato>(), SubtranLabo = new SubTranCuadreContrato() }; ;
                    tr.SubTranProv = obtenerSubTranCCFt(d.CardName, perIni, perFin, productos, FirmName,esp.Displays,0,esp.NroFactura);
                    tr.SubTranProvNC = obtenerSubTranCCNc(d.CardName, perIniNC, perFinNC, d.PeriodoRebate, tr.SubTranProv);
                    //tr.SubTranProvNCArt = obtenerSubTranCCNcArt(d.CardName, tr.SubTranProv,productos,d.ConDev);
                    tr.SubTranProvNCArt = obtenerSubTranCCNcArt(d.CardName, tr.SubTranProv, perIni, perFin, productos, d.ConDev);
                    tr.SocioName = d.CardName;
                    ecu.TranProveedor.Add(tr);

                    lista.Add(ecu);
                }
            }
            return lista;
        }
        private List<SubTranCuadreContrato> obtenerSubTranCCFt(string CardName, string perIni, string perFin
                                                            ,string productos,string FirmName,decimal Displays,int top=0,string NumAtCard=null)
        {
            List<SubTranCuadreContrato> lista = new List<SubTranCuadreContrato>();
            DateTime dtIni = DateTime.Parse(perIni);
            DateTime dtFin = DateTime.Parse(perFin);
            TimeSpan difPer = dtFin.Subtract(dtIni);
            string query = "";
            if(NumAtCard!=null)
            {
                query = "SELECT T0.\"CardName\",T0.\"TaxDate\",T0.\"NumAtCard\",T0.\"DocTotal\",T0.\"DocEntry\",'',0" +
                                        " FROM " + uti.schemaHana + "OPCH T0" +
                                        " WHERE T0.\"NumAtCard\"='"+NumAtCard+"'"+
                                                 " AND T0.\"CANCELED\"='N'"+
                                        " group by T0.\"CardCode\",T0.\"CardName\",T0.\"NumAtCard\" ,T0.\"TaxDate\",T0.\"DocTotal\",T0.\"DocEntry\"" +
                                        " order by T0.\"CardCode\",T0.\"TaxDate\",T0.\"NumAtCard\" ";
            }
            else
            {
                query = "SELECT T0.\"CardName\",T0.\"TaxDate\",T0.\"NumAtCard\",sum(T1.\"GTotal\"),T0.\"DocEntry\",T1.\"FreeTxt\",sum(T1.\"Quantity\") " +
                                        " FROM " + uti.schemaHana + "OPCH T0" +
                                        " INNER JOIN " + uti.schemaHana + "PCH1 T1 ON T1.\"DocEntry\" = T0.\"DocEntry\" "+//AND T1.\"Quantity\">=" + Displays +
                                        " INNER JOIN " + uti.schemaHana + "OITM T2 ON T2.\"ItemCode\" = T1.\"ItemCode\" " +
                                                                             //" AND T2.\"FirmCode\" = (select \"FirmCode\" from " + uti.schema + "omrc where \"FirmName\"='" + FirmName + "') " +
                                                                             " AND T2.\"ItemCode\" in(" + productos + ")" +
                                        " WHERE T0.\"TaxDate\" BETWEEN '" + perIni + "' AND '" + perFin +
                                                 "' AND T0.\"CardName\"='" + CardName + "' " +
                                                 " AND T0.\"CANCELED\"='N'"+
                                        " group by T0.\"CardCode\",T0.\"CardName\",T0.\"NumAtCard\" ,T0.\"TaxDate\",T0.\"DocEntry\",T1.\"FreeTxt\" " +
                                        " order by T0.\"CardCode\",T0.\"TaxDate\",T0.\"NumAtCard\" ";
            }
            HanaConnection hcn = new HanaConnection(uti.cadHana);
            try
            {
                hcn.Open();
                HanaCommand hcmd2 = new HanaCommand(query, hcn);
                HanaDataReader hdr2 = hcmd2.ExecuteReader();
                while (hdr2.Read())
                {
                    SubTranCuadreContrato str = new SubTranCuadreContrato();
                    str.TaxDate = hdr2.GetDateTime(1).ToString("dd/MM/yyyy");
                    str.NumAtCard = hdr2.GetString(2);
                    str.DocTotal = hdr2.GetDecimal(3);
                    str.DocEntry = hdr2.GetInt32(4);
                    if (!hdr2.IsDBNull(5)) {
                        if ((hdr2.GetString(5).Equals("REVALORIZACION") || hdr2.GetString(5).Equals("REVALORIZACIÓN")) && difPer.Days>31)
                        { str.SoloSuma = "Si"; }
                        else { str.SoloSuma = "No"; }
                    }
                    if (!hdr2.IsDBNull(6)){ str.Displays = hdr2.GetDecimal(6); }
                    lista.Add(str);
                }
                hdr2.Close();
                hcn.Close();
            }
            catch  { hcn.Close(); }
            if (top > 0) 
            { 
                List<SubTranCuadreContrato> listaAux = new List<SubTranCuadreContrato>();
                for(int i=top;i>0;i--)
                {
                    listaAux.Add(lista[lista.Count-i]);
                }
                return listaAux;
            }
            return lista;
        }
        private List<SubTranCuadreContrato> obtenerSubTranCCNc(string CardName,string perIniNC
                                            ,string perFinNC,string periodoRebate,List<SubTranCuadreContrato> SbTranCCFt)
        {
            List<SubTranCuadreContrato> lista = new List<SubTranCuadreContrato>();
            string facturas = "''";
            foreach(SubTranCuadreContrato stf in SbTranCCFt)
            {
                facturas += ",'" + stf.NumAtCard + "'";
            }
            string queryNC = "SELECT \"CardName\",\"TaxDate\",\"NumAtCard\",\"DocTotal\",\"DocEntry\"" +
                                        " FROM " + uti.schemaHana + "ORPC " +
                                        " WHERE (   (\"Comments\" like '%REBATE%' AND \"U_SYP_MOTNCND\" like '%"+periodoRebate.ToUpper()+"%')" +
                                                            "AND((\"U_SYP_MDTO\"||'-'||\"U_SYP_MDSO\"||'-'||\"U_SYP_MDCO\") in(" +facturas+"))" +
                                                    " OR " +
                                                    "(\"Comments\" like '%DESCUENTO%' " +
                                                            "AND ((\"U_SYP_MDTO\"||'-'||\"U_SYP_MDSO\"||'-'||\"U_SYP_MDCO\") in("+facturas+"))" +
                                                     "))" +
                                                 " AND \"TaxDate\" BETWEEN '" + perIniNC + "' AND '" + perFinNC + "'" +
                                                 " AND \"CardName\"='" + CardName + "' " +
                                                 " AND \"CANCELED\"='N'" +
                                        " group by \"CardCode\",\"CardName\",\"NumAtCard\" ,\"TaxDate\",\"DocTotal\",\"DocEntry\"" +
                                        " order by \"CardCode\",\"TaxDate\"";
            HanaConnection hcn = new HanaConnection(uti.cadHana);
            try
            {
                hcn.Open();
                HanaCommand hcmd2 = new HanaCommand(queryNC, hcn);
                HanaDataReader hdr2 = hcmd2.ExecuteReader();
                //string CardName = "";
                while (hdr2.Read())
                {
                    SubTranCuadreContrato str = new SubTranCuadreContrato();
                    str.TaxDate = hdr2.GetDateTime(1).ToString("dd/MM/yyyy");
                    str.NumAtCard = hdr2.GetString(2);
                    str.DocTotal = hdr2.GetDecimal(3);
                    str.DocEntry = hdr2.GetInt32(4);
                    lista.Add(str);
                }
                hdr2.Close();
                hcn.Close();
            }
            catch { hcn.Close(); }
            return lista;
        }
        private List<SubTranCuadreContrato> obtenerSubTranCCNcArt(string CardName,List<SubTranCuadreContrato> SbTranCCFt,string perIni,
            string perFin,string productos,string ConDev)
        {
            List<SubTranCuadreContrato> lista = new List<SubTranCuadreContrato>();
            if(ConDev!=null && ConDev.Equals("No")) { return lista; }
            foreach(SubTranCuadreContrato stf in SbTranCCFt)
            {
                string query = "SELECT T0.\"CardName\",T0.\"TaxDate\",T0.\"NumAtCard\",sum(T1.\"GTotal\"),T0.\"DocEntry\",sum(T1.\"Quantity\")" +
                                      " FROM " + uti.schemaHana + "ORPC T0" +
                                      " INNER JOIN " + uti.schemaHana + "RPC1 T1 ON T1.\"DocEntry\" = T0.\"DocEntry\" AND T1.\"ItemCode\" in(" + productos + ")" +
                                      " WHERE T0.\"DocType\"='I'"+
                                                " AND ((T0.\"U_SYP_MDTO\"||T0.\"U_SYP_MDSO\"||T0.\"U_SYP_MDCO\")" +
                                                        "=(select \"U_SYP_MDTD\"||\"U_SYP_MDSD\"||\"U_SYP_MDCD\" from " + uti.schemaHana+"opch where \"DocEntry\"="+stf.DocEntry+"))" +
                                                " AND T0.\"CardName\"='" + CardName + "' " +
                                                " AND T0.\"CANCELED\"='N'" +
                                                " AND T0.\"TaxDate\" BETWEEN '" + perIni + "' AND '" + perFin + "'" +
                                      " group by T0.\"CardCode\",T0.\"CardName\",T0.\"NumAtCard\" ,T0.\"TaxDate\",T0.\"DocEntry\"" +
                                        " order by T0.\"CardCode\",T0.\"TaxDate\"";
                HanaConnection hcn = new HanaConnection(uti.cadHana);
                try
                {
                    hcn.Open();
                    HanaCommand hcmd = new HanaCommand(query, hcn);
                    HanaDataReader hdr = hcmd.ExecuteReader();
                    while (hdr.Read())
                    {
                        SubTranCuadreContrato str = new SubTranCuadreContrato();
                        str.TaxDate = hdr.GetDateTime(1).ToString("dd/MM/yyyy");
                        str.NumAtCard = hdr.GetString(2);
                        str.DocTotal = hdr.GetDecimal(3);
                        str.DocEntry = hdr.GetInt32(4);
                        if (!hdr.IsDBNull(5)) { str.Displays = hdr.GetDecimal(5); }
                        lista.Add(str);
                    }
                    hdr.Close();
                    hcn.Close();
                }
                catch { hcn.Close(); }
            }       
            return lista;
        }
        public List<ResumenRebate_E> listarResumenCuadreContrato(DetCuadreContrato_E filtro,int año)
        {
            List<DetCuadreContrato_E> lista = new List<DetCuadreContrato_E>();
            List<CuadreContrato_E> listaCuadres = new List<CuadreContrato_E>();
            string fil = " where T1.idOCRT>0 and year(T0.PerValFin)="+año;
            if(filtro!=null)
            {
                if(filtro.CardName!= null) { fil += " and T1.CardName like '%" + filtro.CardName + "%'"; }
                if (filtro.U_SYP_DESC != null) { fil += " and T1.U_SYP_DESC like '%" + filtro.U_SYP_DESC + "%'"; }
                if (filtro.Descripcion != null) { fil += " and T1.Descripcion like '%" + filtro.Descripcion + "%'"; }
            }
            string query = "select top 15 T1.idOCRT from cc.OCRT T0 inner join cc.CRT1 T1 on T1.idOCRT = T0.id " + fil+ " group by T1.idOCRT";
            try 
            {
                SqlDataReader dr = db.ExecuteReaderNoSp(query);
                while(dr.Read())
                {
                    listaCuadres.Add(GenerarCuadreContrato(dr.GetInt32(0)));
                }
                dr.Close();
            }catch { }
            foreach(CuadreContrato_E c in  listaCuadres)
            {
                foreach(DetCuadreContrato_E d in c.Det)
                {
                    d.EstadoDet = c.Estado;
                    lista.Add(d);
                }
            }
            lista = lista.OrderBy(l => l.U_SYP_DESC).ToList();
            //finalizando
            List<BaseResumenRebate_E> listaBase = new List<BaseResumenRebate_E>();
            List<ResumenRebate_E> listaRe = new List<ResumenRebate_E>();
            foreach(DetCuadreContrato_E d in lista)
            {
                foreach(EspCuadreContrato_E d1 in d.Esp)
                {
                    foreach(TranCuadreContrato_E d2 in d1.TranProveedor)
                    {
                        BaseResumenRebate_E baux = new BaseResumenRebate_E();
                        baux.Fabricante = d.U_SYP_DESC;
                        baux.Proveedor = d.CardName;
                        baux.Descripcion=d.Descripcion;
                        baux.SubTipo=d.SubTipo;
                        baux.Rebate=d1.Rebate;
                        baux.PerIni=d1.Rango;
                        baux.PerFin=d1.RangoF;
                        baux.DisplayPactada=d1.Displays;
                        baux.DisplayActual=d2.CalcularTotalDisplays();
                        baux.CuotaPactada=d1.CuotaMin;
                        baux.TotalComprado=d2.CalcularTotalFacturas() - d2.CalcularTotalNCArt();
                        baux.Diferencia=d2.CalcularTotalNC() - d2.CalcularRebate(d1.Rebate);
                        baux.Estado = d.EstadoDet;
                        listaBase.Add(baux);
                    }
                }
            }
            var xx = distinguirGrupoBase();
            foreach (BaseResumenRebate_E ba in distinguirGrupoBase())
            {
                ResumenRebate_E raux = new ResumenRebate_E();
                var yy = obtenerGrupoBase(ba);
                foreach (BaseResumenRebate_E ob in obtenerGrupoBase(ba))
                {
                    raux.Fabricantes.Add(ob.Fabricante);
                    raux.Proveedores.Add(ob.Proveedor);
                    raux.Descripciones.Add(ob.Descripcion);
                    raux.SubTipos.Add(ob.SubTipo);
                    raux.Rebates.Add(ob.Rebate);
                    raux.PerInis.Add(ob.PerIni);
                    raux.PerFins.Add(ob.PerFin);
                    raux.DisplayPactadas.Add(ob.DisplayPactada);
                    raux.DisplayActuales.Add(ob.DisplayActual);
                    raux.CuotaPactadas.Add(ob.CuotaPactada);
                    raux.TotalComprados.Add(ob.TotalComprado);
                    raux.Diferencias.Add(ob.Diferencia);
                    raux.Estados.Add(ob.Estado);
                }
                listaRe.Add(raux);
            }
            List<BaseResumenRebate_E> distinguirGrupoBase()
            {
                List<BaseResumenRebate_E> lst = new List<BaseResumenRebate_E>();
                bool repite = false;
                foreach(BaseResumenRebate_E b in listaBase)
                {
                    if(lst.Count==0){lst.Add(b);}
                    else
                    {
                        foreach(BaseResumenRebate_E b1 in lst)
                        {
                            string bdir = b.Descripcion.Replace(" ", "");bdir = bdir.Replace("X", "");
                            string b1dir = b1.Descripcion.Replace(" ", ""); b1dir = b1dir.Replace("X", "");
                            if (b.Fabricante==b1.Fabricante 
                                && string.Compare(bdir,b1dir, CultureInfo.CurrentCulture, CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreCase)==0
                                && b.SubTipo==b1.SubTipo && b.Rebate==b1.Rebate && b.PerIni==b1.PerIni && b.PerFin == b1.PerFin)
                            {
                                repite = true;break;
                            }
                        }
                        if (repite == false) { lst.Add(b); }
                    }
                    repite = false;
                }
                return lst;
            }
            List<BaseResumenRebate_E> obtenerGrupoBase(BaseResumenRebate_E ba)
            {
                List<BaseResumenRebate_E> lst = new List<BaseResumenRebate_E>();
                foreach(BaseResumenRebate_E y in listaBase)
                {
                    string badir = ba.Descripcion.Replace(" ", ""); badir = badir.Replace("X", "");
                    string ydir = y.Descripcion.Replace(" ", ""); ydir = ydir.Replace("X", "");
                    if (ba.Fabricante == y.Fabricante 
                        && string.Compare(badir, ydir, CultureInfo.CurrentCulture, CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreCase) == 0
                                && ba.SubTipo == y.SubTipo && ba.Rebate == y.Rebate && ba.PerIni == y.PerIni && ba.PerFin == y.PerFin)
                    {
                        lst.Add(y);
                    }
                }
                return lst;
            }
            return listaRe;
        }
        //metodo de filtrado
        private List<DetContratoRebate_E> listarDestallesContratoRebate(ContratoRebate_E c)
        {
            List<DetContratoRebate_E> lista = new List<DetContratoRebate_E>();
            foreach (LineaProduccion_E l in lpD.listarLineasProduccion(prepararFiltrosACR(c)))
            {
                DetContratoRebate_E d = new DetContratoRebate_E();
                d.idORLP = l.id;
                d.Descripcion = l.Descripcion;
                d.U_SYP_DESC = l.Fabricante.U_SYP_DESC;
                d.CardName = l.Proveedor.CardName;
                lista.Add(d);
            }
            return lista;
        }
        private LineaProduccion_E prepararFiltrosACR(ContratoRebate_E c)
        {
            LineaProduccion_E l = new LineaProduccion_E() { Fabricante = new OMRC_E(), Proveedor = new OCRD_E() };
            if(c.Tipo.Equals("Fabricante") || c.Tipo.Equals("InfoFabricante") || c.Tipo.Equals("FactFabricante"))
            {
                l.Fabricante.FirmName = c.SocioCod;
            }
            if(c.Tipo.Equals("Proveedor") || c.Tipo.Equals("InfoProveedor") || c.Tipo.Equals("FactProveedor"))
            {
                l.Proveedor.CardCode = c.SocioCod;
            }
            return l;
        }
        // **********infos para formularios
        public string infoListarSocios(string Tipo)
        {
            string info = "";
                if(Tipo.Equals("Fabricante")||Tipo.Equals("InfoFabricante")||Tipo.Equals("FactFabricante"))
                {
                    foreach (OMRC_E f in omrcD.listarFabricantes())
                    { info += "<option SocioCod='" + f.FirmName + "' value='" + f.U_SYP_DESC + "'></option>"; }
                    return info;
                }
                if(Tipo.Equals("Proveedor") || Tipo.Equals("InfoProveedor") || Tipo.Equals("FactProveedor"))
                {
                    foreach(OCRD_E p in ocrdD.listarSociosDeNegocios(new OCRD_E { CardType="S"}))
                    { info += "<option SocioCod='" +p.CardCode + "' value='" + p.CardName + "'></option>"; }
                    return info;
                }
            return info;
        }
        public string infoListarDetallesContratoRebate(ContratoRebate_E c)
        {
            string info = "<tr><th>Retirar</th><th>#</th><th>Descripcion</th><th>Fabricante</th><th>Proveedor</th><th>Periodo</th><th>SubTipo</th><th>ConsideraDev.</th><tr>";
            int i = 0;
            string registro(int l,DetContratoRebate_E dt)
            {
                return "<tr id='celda"+l+"'>" +
                              "<td style='text-align:center;' ><input type='checkbox' name='Det["+l+"].Status' checked onclick=\"borrarCelda('celda"+l+"',this);\" ></td>"+
                              "<td><input type='hidden' name='Det["+l+"].idORLP' value="+dt.idORLP+" >" +
                                    "<input type='text' name='Det["+l+"].Linea' value="+l+ " readonly size=2 style='background:#D8D8D8;'></td>" +
                              "<td><input type='text' name='Det["+l+"].Descripcion' value='"+dt.Descripcion+ "' readonly style='background:#D8D8D8;'></td>" +
                              "<td><input type='text' name='Det["+l+"].U_SYP_DESC' value='"+dt.U_SYP_DESC+ "' readonly size=10 style='background:#D8D8D8;'></td>" +
                              "<td><input type='text' name='Det["+l+"].CardName' value='"+dt.CardName+ "' readonly size=10 style='background:#D8D8D8;'></td>" +
                              "<td><select name='Det["+l+"].PeriodoRebate' id='Det["+l+"].PeriodoRebate' onchange=\"cargaEsp("+l+",'Det["+l+"].PeriodoRebate','Det["+l+ "].SubTipo',"+dt.idORLP+");\" >" +
                                        "<option value=''>Seleccione</option>"+
                                        "<option value='Mensual'>Mensual</option>" +
                                        "<option value='Bimestral'>Bimestral</option>" +
                                        "<option value='Trimestral'>Trimestral</option>" +
                                        "<option value='Semestral'>Semestral</option>" +
                                        "<option value='Anual'>Anual</option>" +
                                   "</select>" +
                              "</td>"+
                              "<td><select name='Det["+l+"].SubTipo' id='Det["+l+"].SubTipo' onchange=\"cargaEsp("+l+",'Det["+l+"].PeriodoRebate','Det["+l+ "].SubTipo'," + dt.idORLP + ")\" >" +
                                        "<option value='Compra'>Compra</option>" +
                                        "<option value='GrupoDias'>GrupoDias</option>" +
                                        "<option value='Informacion'>Informacion</option>" +
                                        "<option value='UltimasFacturas'>UltimasFacturas</option>" +
                                        "<option value='FacturasEspecificas'>FacturasEspecificas</option>" +
                                  "</select>" +
                              "</td>" +
                              "<td><select name='Det["+l+"].ConDev' id='Det["+l+"].ConDev' >" +
                                        "<option value='Si'>Si</option>" +
                                        "<option value='No'>No</option>" +
                                    "</select>" +
                              "</td>"
                        + "</tr>" 
                            +"<tr id='celda"+l+"tb'>" +
                                "<td></td><td colspan=8 ><table border=1 id='Esp2"+l+"'></table></td>" +
                            "</tr>"
                            ;
            }
            foreach(DetContratoRebate_E d in listarDestallesContratoRebate(c))
            {
                info += registro(i, d);
                info += registro(++i, d)+ registro(++i, d)+ registro(++i, d)+ registro(++i, d);
                i++;
            }
            return info;
        }
        public string infoListarEspDetCR2(string PeriodoRebate,int Linea,string SubTipo, string CardCode, int idLp, string FecIni, string FecFin)
        {
            string info = "";
            string[] meses = DateTimeFormatInfo.CurrentInfo.MonthNames;
            string[] bimestres = {"Enero-Febrero","Marzo-Abril","Mayo-Junio","Julio-Agosto","Septiembre-Octubre","Noviembre-Diciembre" };
            string[] trimestres = { "Ene-Feb-Mar", "Abr-May-Jun", "Jul-Ago-Set", "Oct-Nov-Dic" };
            string[] semestres = { "En-Fe-Ma-Ab-Ma-Ju", "Jl-Ag-Se-Oc-Nv-Dc" };
            string[] años = { "2020", "2021", "2022" };
            List<OPCH_E> facturas = opchD.listarFacturasProveedoresContrato(CardCode,FecIni,FecFin,lpD.obtenerArticulosDeLp(idLp));
            string opcionesFacturas = "";
            foreach(OPCH_E f in facturas)
            {
                opcionesFacturas += "<option value='" + f.NumAtCard + "'></option>";
            }
            if(PeriodoRebate==null || PeriodoRebate.Equals("")) { info = "";return info; }
            if(SubTipo.Equals("Compra"))
            {
                info= "<tr><th>#</th><th>RangoIni</th><th>RangoFin</th><th>Displays</th><th>CuotaMin</th><th>Rebate %</th></tr>";
                if (PeriodoRebate.Equals("Mensual"))
                {
                    for (int i = 0; i <= 11; i++)
                    {
                        info += "<tr>" +
                                "<td><input type='text' name='Det[" + Linea + "].EspDet2[" + i + "].Linea' value='" + i + "' size=4 readonly style='background:#D8D8D8;'></td>" +
                                "<td><input type='date' name='Det[" + Linea + "].EspDet2[" + i + "].Rango'  size=12 style='background:#D8D8D8; width:110px;font-size:11px;'></td>" +
                                "<td><input type='date' name='Det[" + Linea + "].EspDet2[" + i + "].RangoF'  size=12 style='background:#D8D8D8; width:110px;font-size:11px;'></td>" +
                                "<td><input type='text' name='Det[" + Linea + "].EspDet2[" + i + "].Displays' style='100px;font-size:11px;'>" +
                                "<td><input type='text' name='Det[" + Linea + "].EspDet2[" + i + "].CuotaMin' size=10>" +
                                "<td><input type='text' name='Det[" + Linea + "].EspDet2[" + i + "].Rebate' style='width:50px;'>"
                            + "</tr>";
                    }
                }
                if (PeriodoRebate.Equals("Bimestral"))
                {
                    for (int i = 0; i < bimestres.Length; i++)
                    {
                        info += "<tr>" +
                                "<td><input type='text' name='Det[" + Linea + "].EspDet2[" + i + "].Linea' value='" + i + "' size=4 readonly style='background:#D8D8D8;'></td>" +
                                "<td><input type='date' name='Det[" + Linea + "].EspDet2[" + i + "].Rango'  size=12 style='background:#D8D8D8; width:110px;font-size:11px;'></td>" +
                                "<td><input type='date' name='Det[" + Linea + "].EspDet2[" + i + "].RangoF'  size=12 style='background:#D8D8D8; width:110px;font-size:11px;'></td>" +
                                "<td><input type='text' name='Det[" + Linea + "].EspDet2[" + i + "].Displays' style='100px;font-size:11px;'>" +
                                "<td><input type='text' name='Det[" + Linea + "].EspDet2[" + i + "].CuotaMin' size=10>" +
                                "<td><input type='text' name='Det[" + Linea + "].EspDet2[" + i + "].Rebate' style='width:50px;'>"
                            + "</tr>";
                    }
                }
                if (PeriodoRebate.Equals("Trimestral"))
                {
                    for (int i = 0; i < trimestres.Length; i++)
                    {
                        info += "<tr>" +
                                "<td><input type='text' name='Det[" + Linea + "].EspDet2[" + i + "].Linea' value='" + i + "' size=4 readonly style='background:#D8D8D8;'></td>" +
                                "<td><input type='date' name='Det[" + Linea + "].EspDet2[" + i + "].Rango'  size=12 style='background:#D8D8D8; width:110px;font-size:11px;'></td>" +
                                "<td><input type='date' name='Det[" + Linea + "].EspDet2[" + i + "].RangoF'  size=12 style='background:#D8D8D8; width:110px;font-size:11px;'></td>" +
                                "<td><input type='text' name='Det[" + Linea + "].EspDet2[" + i + "].Displays' style='100px;font-size:11px;'>" +
                                "<td><input type='text' name='Det[" + Linea + "].EspDet2[" + i + "].CuotaMin' size=10>" +
                                "<td><input type='text' name='Det[" + Linea + "].EspDet2[" + i + "].Rebate' style='width:50px;'>"
                            + "</tr>";
                    }
                }
                if (PeriodoRebate.Equals("Semestral"))
                {
                    for (int i = 0; i < semestres.Length; i++)
                    {
                        info += "<tr>" +
                                "<td><input type='text' name='Det[" + Linea + "].EspDet2[" + i + "].Linea' value='" + i + "' size=4 readonly style='background:#D8D8D8;'></td>" +
                                "<td><input type='date' name='Det[" + Linea + "].EspDet2[" + i + "].Rango'  size=12 style='background:#D8D8D8; width:110px;font-size:11px;'></td>" +
                                "<td><input type='date' name='Det[" + Linea + "].EspDet2[" + i + "].RangoF'  size=12 style='background:#D8D8D8; width:110px;font-size:11px;'></td>" +
                                "<td><input type='text' name='Det[" + Linea + "].EspDet2[" + i + "].Displays' style='100px;font-size:11px;'>" +
                                "<td><input type='text' name='Det[" + Linea + "].EspDet2[" + i + "].CuotaMin' size=10>" +
                                "<td><input type='text' name='Det[" + Linea + "].EspDet2[" + i + "].Rebate' style='width:50px;'>"
                            + "</tr>";
                    }
                }
                if (PeriodoRebate.Equals("Anual"))
                {
                    for (int i = 0; i < años.Length; i++)
                    {
                        info += "<tr>" +
                                "<td><input type='text' name='Det[" + Linea + "].EspDet2[" + i + "].Linea' value='" + i + "' size=4 readonly style='background:#D8D8D8;'></td>" +
                                "<td><input type='date' name='Det[" + Linea + "].EspDet2[" + i + "].Rango'  size=12 style='background:#D8D8D8; width:110px;font-size:11px;'></td>" +
                                "<td><input type='date' name='Det[" + Linea + "].EspDet2[" + i + "].RangoF'  size=12 style='background:#D8D8D8; width:110px;font-size:11px;'></td>" +
                                "<td><input type='text' name='Det[" + Linea + "].EspDet2[" + i + "].Displays' style='100px;font-size:11px;'>" +
                                "<td><input type='text' name='Det[" + Linea + "].EspDet2[" + i + "].CuotaMin' size=10>" +
                                "<td><input type='text' name='Det[" + Linea + "].EspDet2[" + i + "].Rebate' style='width:50px;'>"
                            + "</tr>";
                    }
                }
            }
            else if(SubTipo.Equals("Informacion"))
            {
                info = "<tr><th>#</th><th>RangoIni</th><th>RangoFin</th><th>Displays</th><th>Rebate %</th></tr>";
                if (PeriodoRebate.Equals("Mensual"))
                {
                    for (int i = 0; i <= 11; i++)
                    {
                        info += "<tr>" +
                                "<td><input type='text' name='Det[" + Linea + "].EspDet2[" + i + "].Linea' value='" + i + "' size=4 readonly style='background:#D8D8D8;'></td>" +
                                "<td><input type='date' name='Det[" + Linea + "].EspDet2[" + i + "].Rango'  size=12 style='background:#D8D8D8; width:110px;font-size:11px;'></td>" +
                                "<td><input type='date' name='Det[" + Linea + "].EspDet2[" + i + "].RangoF'  size=12 style='background:#D8D8D8; width:110px;font-size:11px;'></td>" +
                                "<td><input type='text' name='Det[" + Linea + "].EspDet2[" + i + "].Displays' style='100px;font-size:11px;'>" +
                                "<td><input type='text' name='Det[" + Linea + "].EspDet2[" + i + "].Rebate' style='width:50px;'>"
                            + "</tr>";
                    }
                }
                if (PeriodoRebate.Equals("Bimestral"))
                {
                    for (int i = 0; i < bimestres.Length; i++)
                    {
                        info += "<tr>" +
                                "<td><input type='text' name='Det[" + Linea + "].EspDet2[" + i + "].Linea' value='" + i + "' size=4 readonly style='background:#D8D8D8;'></td>" +
                                "<td><input type='date' name='Det[" + Linea + "].EspDet2[" + i + "].Rango'  size=12 style='background:#D8D8D8; width:110px;font-size:11px;'></td>" +
                                "<td><input type='date' name='Det[" + Linea + "].EspDet2[" + i + "].RangoF'  size=12 style='background:#D8D8D8; width:110px;font-size:11px;'></td>" +
                                "<td><input type='text' name='Det[" + Linea + "].EspDet2[" + i + "].Displays' style='100px;font-size:11px;'>" +
                                "<td><input type='text' name='Det[" + Linea + "].EspDet2[" + i + "].Rebate' style='width:50px;'>"
                            + "</tr>";
                    }
                }
                if (PeriodoRebate.Equals("Trimestral"))
                {
                    for (int i = 0; i < trimestres.Length; i++)
                    {
                        info += "<tr>" +
                                "<td><input type='text' name='Det[" + Linea + "].EspDet2[" + i + "].Linea' value='" + i + "' size=4 readonly style='background:#D8D8D8;'></td>" +
                                "<td><input type='date' name='Det[" + Linea + "].EspDet2[" + i + "].Rango'  size=12 style='background:#D8D8D8; width:110px;font-size:11px;'></td>" +
                                "<td><input type='date' name='Det[" + Linea + "].EspDet2[" + i + "].RangoF'  size=12 style='background:#D8D8D8; width:110px;font-size:11px;'></td>" +
                                "<td><input type='text' name='Det[" + Linea + "].EspDet2[" + i + "].Displays' style='100px;font-size:11px;'>" +
                                "<td><input type='text' name='Det[" + Linea + "].EspDet2[" + i + "].Rebate' style='width:50px;'>"
                            + "</tr>";
                    }
                }
                if (PeriodoRebate.Equals("Semestral"))
                {
                    for (int i = 0; i < semestres.Length; i++)
                    {
                        info += "<tr>" +
                                "<td><input type='text' name='Det[" + Linea + "].EspDet2[" + i + "].Linea' value='" + i + "' size=4 readonly style='background:#D8D8D8;'></td>" +
                                "<td><input type='date' name='Det[" + Linea + "].EspDet2[" + i + "].Rango'  size=12 style='background:#D8D8D8; width:110px;font-size:11px;'></td>" +
                                "<td><input type='date' name='Det[" + Linea + "].EspDet2[" + i + "].RangoF'  size=12 style='background:#D8D8D8; width:110px;font-size:11px;'></td>" +
                                "<td><input type='text' name='Det[" + Linea + "].EspDet2[" + i + "].Displays' style='100px;font-size:11px;'>" +
                                "<td><input type='text' name='Det[" + Linea + "].EspDet2[" + i + "].Rebate' style='width:50px;'>"
                            + "</tr>";
                    }
                }
                if (PeriodoRebate.Equals("Anual"))
                {
                    for (int i = 0; i < años.Length; i++)
                    {
                        info += "<tr>" +
                                "<td><input type='text' name='Det[" + Linea + "].EspDet2[" + i + "].Linea' value='" + i + "' size=4 readonly style='background:#D8D8D8;'></td>" +
                                "<td><input type='date' name='Det[" + Linea + "].EspDet2[" + i + "].Rango'  size=12 style='background:#D8D8D8; width:110px;font-size:11px;'></td>" +
                                "<td><input type='date' name='Det[" + Linea + "].EspDet2[" + i + "].RangoF'  size=12 style='background:#D8D8D8; width:110px;font-size:11px;'></td>" +
                                "<td><input type='text' name='Det[" + Linea + "].EspDet2[" + i + "].Displays' style='100px;font-size:11px;'>" +
                                "<td><input type='text' name='Det[" + Linea + "].EspDet2[" + i + "].Rebate' style='width:50px;'>"
                            + "</tr>";
                    }
                }
            }
            else if(SubTipo.Equals("UltimasFacturas"))
            {
                info = "<tr><th>#</th><th>RangoIni</th><th>RangoFin</th><th>Displays</th><th># Facturas</th><th>Rebate %</th></tr>";
                if (PeriodoRebate.Equals("Mensual"))
                {
                    for (int i = 0; i <= 11; i++)
                    {
                        info += "<tr>" +
                                "<td><input type='text' name='Det[" + Linea + "].EspDet2[" + i + "].Linea' value='" + i + "' size=4 readonly style='background:#D8D8D8;'></td>" +
                                "<td><input type='date' name='Det[" + Linea + "].EspDet2[" + i + "].Rango'  size=12 style='background:#D8D8D8; width:110px;font-size:11px;'></td>" +
                                "<td><input type='date' name='Det[" + Linea + "].EspDet2[" + i + "].RangoF'  size=12 style='background:#D8D8D8; width:110px;font-size:11px;'></td>" +
                                "<td><input type='text' name='Det[" + Linea + "].EspDet2[" + i + "].Displays' style='100px;font-size:11px;'>" +
                                "<td><input type='text' name='Det[" + Linea + "].EspDet2[" + i + "].UltimasFacturas' style='width:55px;'>" +
                                "<td><input type='text' name='Det[" + Linea + "].EspDet2[" + i + "].Rebate' style='width:50px;'>"
                            + "</tr>";
                    }
                }
                if (PeriodoRebate.Equals("Bimestral"))
                {
                    for (int i = 0; i < bimestres.Length; i++)
                    {
                        info += "<tr>" +
                                "<td><input type='text' name='Det[" + Linea + "].EspDet2[" + i + "].Linea' value='" + i + "' size=4 readonly style='background:#D8D8D8;'></td>" +
                                "<td><input type='date' name='Det[" + Linea + "].EspDet2[" + i + "].Rango'  size=12 style='background:#D8D8D8; width:110px;font-size:11px;'></td>" +
                                "<td><input type='date' name='Det[" + Linea + "].EspDet2[" + i + "].RangoF'  size=12 style='background:#D8D8D8; width:110px;font-size:11px;'></td>" +
                                "<td><input type='text' name='Det[" + Linea + "].EspDet2[" + i + "].Displays' style='100px;font-size:11px;'>" +
                                "<td><input type='text' name='Det[" + Linea + "].EspDet2[" + i + "].UltimasFacturas' style='width:55px;'>" +
                                "<td><input type='text' name='Det[" + Linea + "].EspDet2[" + i + "].Rebate' style='width:50px;'>"
                            + "</tr>";
                    }
                }
                if (PeriodoRebate.Equals("Trimestral"))
                {
                    for (int i = 0; i < trimestres.Length; i++)
                    {
                        info += "<tr>" +
                                "<td><input type='text' name='Det[" + Linea + "].EspDet2[" + i + "].Linea' value='" + i + "' size=4 readonly style='background:#D8D8D8;'></td>" +
                                "<td><input type='date' name='Det[" + Linea + "].EspDet2[" + i + "].Rango'  size=12 style='background:#D8D8D8; width:110px;font-size:11px;'></td>" +
                                "<td><input type='date' name='Det[" + Linea + "].EspDet2[" + i + "].RangoF'  size=12 style='background:#D8D8D8; width:110px;font-size:11px;'></td>" +
                                "<td><input type='text' name='Det[" + Linea + "].EspDet2[" + i + "].Displays' style='100px;font-size:11px;'>" +
                                "<td><input type='text' name='Det[" + Linea + "].EspDet2[" + i + "].UltimasFacturas' style='width:55px;'>" +
                                "<td><input type='text' name='Det[" + Linea + "].EspDet2[" + i + "].Rebate' style='width:50px;'>"
                            + "</tr>";
                    }
                }
                if (PeriodoRebate.Equals("Semestral"))
                {
                    for (int i = 0; i < semestres.Length; i++)
                    {
                        info += "<tr>" +
                                "<td><input type='text' name='Det[" + Linea + "].EspDet2[" + i + "].Linea' value='" + i + "' size=4 readonly style='background:#D8D8D8;'></td>" +
                                "<td><input type='date' name='Det[" + Linea + "].EspDet2[" + i + "].Rango'  size=12 style='background:#D8D8D8; width:110px;font-size:11px;'></td>" +
                                "<td><input type='date' name='Det[" + Linea + "].EspDet2[" + i + "].RangoF'  size=12 style='background:#D8D8D8; width:110px;font-size:11px;'></td>" +
                                "<td><input type='text' name='Det[" + Linea + "].EspDet2[" + i + "].Displays' style='100px;font-size:11px;'>" +
                                "<td><input type='text' name='Det[" + Linea + "].EspDet2[" + i + "].UltimasFacturas' style='width:55px;'>" +
                                "<td><input type='text' name='Det[" + Linea + "].EspDet2[" + i + "].Rebate' style='width:50px;'>"
                            + "</tr>";
                    }
                }
                if (PeriodoRebate.Equals("Anual"))
                {
                    for (int i = 0; i < años.Length; i++)
                    {
                        info += "<tr>" +
                                "<td><input type='text' name='Det[" + Linea + "].EspDet2[" + i + "].Linea' value='" + i + "' size=4 readonly style='background:#D8D8D8;'></td>" +
                                "<td><input type='date' name='Det[" + Linea + "].EspDet2[" + i + "].Rango'  size=12 style='background:#D8D8D8; width:110px;font-size:11px;'></td>" +
                                "<td><input type='date' name='Det[" + Linea + "].EspDet2[" + i + "].RangoF'  size=12 style='background:#D8D8D8; width:110px;font-size:11px;'></td>" +
                                "<td><input type='text' name='Det[" + Linea + "].EspDet2[" + i + "].Displays' style='100px;font-size:11px;'>" +
                                "<td><input type='text' name='Det[" + Linea + "].EspDet2[" + i + "].UltimasFacturas' style='width:55px;'>" +
                                "<td><input type='text' name='Det[" + Linea + "].EspDet2[" + i + "].Rebate' style='width:50px;'>"
                            + "</tr>";
                    }
                }
            }
            else if(SubTipo.Equals("GrupoDias"))
            {
                info = "<tr><th>#</th><th>RangoIni</th><th>RangoFin</th><th>Displays</th><th>CuotaMin</th><th>Rebate %</th></tr>";
                if (PeriodoRebate.Equals("Mensual"))
                {
                    for (int i = 0; i <= 11; i++)
                    {
                        info += "<tr>" +
                                "<td><input type='text' name='Det["+Linea+"].EspDet2["+i+"].Linea' value='"+i+"' size=4 readonly style='background:#D8D8D8;'></td>" +
                                "<td><input type='date' name='Det[" + Linea + "].EspDet2[" + i + "].Rango'  size=12 style='background:#D8D8D8; width:110px;font-size:11px;'></td>" +
                                "<td><input type='date' name='Det[" + Linea + "].EspDet2[" + i + "].RangoF'  size=12 style='background:#D8D8D8; width:110px;font-size:11px;'></td>" +
                                "<td><input type='text' name='Det[" + Linea + "].EspDet2[" + i + "].Displays' style='100px;font-size:11px;'>" +
                                "<td><input type='text' name='Det[" +Linea+"].EspDet2["+i+"].CuotaMin' size=10>" +
                                "<td><input type='text' name='Det["+Linea+"].EspDet2["+i+"].Rebate' style='width:50px;'>"
                            + "</tr>";
                    }
                }
            }
            else if(SubTipo.Equals("FacturasEspecificas"))
            {
                info = "<tr><th>#</th><th>NroFactura</th><th>Rebate %</th></tr>";
                for(int i = 0;i<=5;i++)
                {
                    info += "<tr>" +
                                "<td><input type='text' name='Det["+Linea+"].EspDet2["+i+"].Linea' value='"+i+"' size=4 readonly style='background:#D8D8D8;'></td>" +
                                "<td><input type='search' name='Det["+Linea+"].EspDet2["+i+ "].NroFactura' list='ListaFacturas"+Linea+"' style='width:200px;'>" +
                                    "<datalist id = 'ListaFacturas"+Linea+"' > " +
                                        opcionesFacturas+
                                    "</datalist>" + 
                                "</td>" +
                                "<td><input type='text' name='Det["+Linea+"].EspDet2["+i+"].Rebate' style='width:50px;'></td>"+
                            "</tr>";
                }
                
            }
            return info;
        }
    }
}
