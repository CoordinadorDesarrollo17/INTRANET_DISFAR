using Capa_Datos.DireccionTecnica_DAO.TablasSql;
using Capa_Entidad.DireccionTecnica_ENT.Reportes;
using Capa_Entidad.DireccionTecnica_ENT.Reportes.BalanceControlados;
using Capa_Entidad.DireccionTecnica_ENT.TablasSql;
using Capa_Entidad.ReportesDigemid_ENT.Formularios;
using Capa_Entidad.ReportesDigemid_ENT.Reportes;
using Sap.Data.Hana;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;

namespace Capa_Datos.DireccionTecnica_DAO.Reportes
{
    public class ReportesDigemid_D
    {
        Utilitarios uti = new Utilitarios();
        DBHelper db = new DBHelper();
        public List<RptKardexAlmacenes_E> ReporteKardexAlmacenes(FrmKardex_E f)
        {
            List<RptKardexAlmacenes_E> lista = new List<RptKardexAlmacenes_E>();
            HanaConnection cn = new HanaConnection(uti.cadHana);
            try
            {
                CultureInfo culture = new CultureInfo("en-US");
                string HANASQL = string.Empty;
                switch (f.WhsCode)
                {
                    case "01":
                        if (f.Lote == null || f.Lote == "") { HANASQL = "CALL " + uti.schemaHana + "MARIA_KARDEX303_V1('" + f.FecIni + "', '" + f.FecFin + "', '" + f.ItemCode + "')"; }
                        else { HANASQL = "CALL " + uti.schemaHana + "MARIA_KARDEX303_LOTES_V1('" + f.FecIni + "', '" + f.FecFin + "', '" + f.ItemCode + "','" + f.Lote + "')"; }
                        break;
                    case "02":
                        if (f.Lote == null || f.Lote == "") { HANASQL = "CALL " + uti.schemaHana + "DIEGO_KARDEX502_V1('" + f.FecIni + "', '" + f.FecFin + "', '" + f.ItemCode + "')"; }
                        else { HANASQL = "CALL " + uti.schemaHana + "DIEGO_KARDEX502_LOTES_V1('" + f.FecIni + "', '" + f.FecFin + "', '" + f.ItemCode + "','" + f.Lote + "')"; }
                        break;
                    case "03":
                        if (f.Lote == null || f.Lote == "") { HANASQL = "CALL " + uti.schemaHana + "DIEGO_KARDEX_ALM03_V1('" + f.FecIni + "', '" + f.FecFin + "', '" + f.ItemCode + "')"; }
                        else { HANASQL = "CALL " + uti.schemaHana + "DIEGO_KARDEX_ALM03_LOTES_V1('" + f.FecIni + "', '" + f.FecFin + "', '" + f.ItemCode + "','" + f.Lote + "')"; }
                        break;
                    case "04":
                        if (f.Lote == null || f.Lote == "") { HANASQL = "CALL " + uti.schemaHana + "DIEGO_KARDEX_ALM04_V1('" + f.FecIni + "', '" + f.FecFin + "', '" + f.ItemCode + "')"; }
                        else { HANASQL = "CALL " + uti.schemaHana + "DIEGO_KARDEX_ALM04_LOTES_V1('" + f.FecIni + "', '" + f.FecFin + "', '" + f.ItemCode + "','" + f.Lote + "')"; }
                        break;
                    case "05":
                        if (f.Lote == null || f.Lote == "") { HANASQL = "CALL " + uti.schemaHana + "DIEGO_KARDEX_ALM05_V1('" + f.FecIni + "', '" + f.FecFin + "', '" + f.ItemCode + "')"; }
                        else { HANASQL = "CALL " + uti.schemaHana + "DIEGO_KARDEX_ALM05_LOTES_V1('" + f.FecIni + "', '" + f.FecFin + "', '" + f.ItemCode + "','" + f.Lote + "')"; }
                        break;
                    case "09":
                        if (f.Lote == null || f.Lote == "") { HANASQL = "CALL " + uti.schemaHana + "MARIA_KARDEX_ALM06_V1('" + f.FecIni + "', '" + f.FecFin + "', '" + f.ItemCode + "')"; }
                        else { HANASQL = "CALL " + uti.schemaHana + "MARIA_KARDEX_ALM06_LOTES_V1('" + f.FecIni + "', '" + f.FecFin + "', '" + f.ItemCode + "','" + f.Lote + "')"; }
                        break;
                    case "ALM07":
                        if (f.Lote == null || f.Lote == "") { HANASQL = "CALL " + uti.schemaHana + "DIEGO_KARDEX_ALM07_V1('" + f.FecIni + "', '" + f.FecFin + "', '" + f.ItemCode + "')"; }
                        else { HANASQL = "CALL " + uti.schemaHana + "DIEGO_KARDEX_ALM07_LOTES_V1('" + f.FecIni + "', '" + f.FecFin + "', '" + f.ItemCode + "','" + f.Lote + "')"; }
                        break;
                }
                cn.Open();
                HanaCommand cmd = new HanaCommand(HANASQL, cn);
                cmd.CommandType = CommandType.Text;
                HanaDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    if (dr.GetString(18).Equals("MOSTRAR") && !string.IsNullOrEmpty(dr.GetString(6)) && !dr.GetString(6).Equals("00000001"))
                    {
                        RptKardexAlmacenes_E kdx = new RptKardexAlmacenes_E();
                        kdx.DescProducto = dr.GetString(0);
                        kdx.CodProducto = dr.GetString(1);
                        kdx.RegSanitario = dr.GetString(2);
                        kdx.FechaCont = dr.GetDateTime(3).ToString("dd/MM/yyyy");
                        kdx.FacturaGuiaBoleta = dr.GetString(4);
                        kdx.ProvEstab = dr.GetString(5);
                        kdx.Ruc = dr.GetString(6);
                        kdx.Lote = dr.GetString(7);
                        kdx.CantLoteIngreso = Math.Round(decimal.Parse(dr.GetString(8)));
                        kdx.CantLoteSalida = Math.Round(decimal.Parse(dr.GetString(9)));
                        kdx.CantIngreso = Math.Round(decimal.Parse(dr.GetString(10)));
                        kdx.CantSalida = Math.Round(decimal.Parse(dr.GetString(11)));
                        kdx.AcumuladoLote = Math.Round(decimal.Parse(dr.GetString(12), culture));
                        kdx.AcumuladoProducto = Math.Round(decimal.Parse(dr.GetString(13), culture));
                        kdx.BaseType = dr.GetDouble(14);
                        kdx.Direction = dr.GetInt32(15);
                        kdx.Warehouse = dr.GetString(16);
                        kdx.CreatedBy = dr.GetInt32(17);
                        lista.Add(kdx);
                    }
                }
                dr.Close();
            }
            catch { }
            finally
            {
                cn.Close();
            }
            return lista;
        }

        /************************* B A L A N C E   C O N T R O L A D O S *************************/
        public List<RptBalanceControladosIngreso_E> ReporteBalanceControladosIngreso(FrmBalanceControlados_E f)
        {
            List<RptBalanceControladosIngreso_E> lista = new List<RptBalanceControladosIngreso_E>();

            string query = "call " + uti.schemaHana + "DIEGO_BAL_CONT_INGRESO_V1('" + f.FecIni + "','" + f.FecFin + "','" + f.TipoControlado + "')";
            try
            {
                HanaDataReader hdr = db.HanaExecuteReaderNoSp(query);
                while (hdr.Read())
                {
                    RptBalanceControladosIngreso_E bc = new RptBalanceControladosIngreso_E();
                    if (!hdr.IsDBNull(0)) { bc.CodProducto = hdr.GetString(0); }
                    if (!hdr.IsDBNull(1)) { bc.NombreGenerico = hdr.GetString(1); }
                    if (!hdr.IsDBNull(2)) { bc.NombreComercial = hdr.GetString(2); }
                    if (!hdr.IsDBNull(3)) { bc.RegSanitario = hdr.GetString(3); }
                    if (!hdr.IsDBNull(4)) { bc.Concentracion = hdr.GetString(4); }
                    if (!hdr.IsDBNull(5)) { bc.FormaPresentacion = hdr.GetString(5); }
                    if (!hdr.IsDBNull(6)) { bc.FormaFamaceutica = hdr.GetString(6); }
                    if (!hdr.IsDBNull(7)) { bc.NroLote = hdr.GetString(7); }
                    if (!hdr.IsDBNull(8)) { bc.CantLote = Math.Round(hdr.GetDecimal(8), 0); }
                    if (!hdr.IsDBNull(9)) { bc.Proveedor = hdr.GetString(9); }
                    if (!hdr.IsDBNull(10)) { bc.RucProveedor = hdr.GetString(10); }
                    if (!hdr.IsDBNull(11)) { bc.CalleJrAvN = hdr.GetString(11); }
                    if (!hdr.IsDBNull(12)) { bc.Distrito = hdr.GetString(12); }
                    if (!hdr.IsDBNull(13)) { bc.Provincia = hdr.GetString(13); }
                    if (!hdr.IsDBNull(14)) { bc.Departamento = hdr.GetString(14); }
                    if (!hdr.IsDBNull(15)) { bc.NroFacturaNcredito = hdr.GetString(15); }
                    if (!hdr.IsDBNull(16)) { bc.Fecha = hdr.GetDateTime(16).ToString("dd/MM/yyyy"); }
                    if (f.TipoControlado == "S") { bc.TipoControlado = "PSICOTROPICOS"; }
                    else if (f.TipoControlado == "P") { bc.TipoControlado = "PRECURSORES"; }
                    else if (f.TipoControlado == "E") { bc.TipoControlado = "ESTUPEFACIENTES"; }
                    if (!hdr.IsDBNull(15) && !hdr.GetValue(15).Equals("AC--"))
                    {
                        lista.Add(bc);
                    }

                }
                hdr.Close();
            }
            catch { }

            return lista;
        }

        public List<RptBalanceControladosEgreso_E> ReporteBalanceControladosEgreso(FrmBalanceControlados_E f)
        {
            List<RptBalanceControladosEgreso_E> lista = new List<RptBalanceControladosEgreso_E>();

            string query = "call " + uti.schemaHana + "DIEGO_BAL_CONT_EGRESO_V2('" + f.FecIni + "','" + f.FecFin + "','" + f.TipoControlado + "')";
            try
            {
                HanaDataReader hdr = db.HanaExecuteReaderNoSp(query);
                while (hdr.Read())
                {
                    RptBalanceControladosEgreso_E egr = new RptBalanceControladosEgreso_E();
                    if (!hdr.IsDBNull(0)) { egr.CodProducto = hdr.GetString(0); }
                    if (!hdr.IsDBNull(1)) { egr.NombreGenerico = hdr.GetString(1); }
                    if (!hdr.IsDBNull(2)) { egr.NombreComercial = hdr.GetString(2); }
                    if (!hdr.IsDBNull(3)) { egr.RegSanitario = hdr.GetString(3); }
                    if (!hdr.IsDBNull(4)) { egr.Concentracion = hdr.GetString(4); }
                    if (!hdr.IsDBNull(5)) { egr.FormaPresentacion = hdr.GetString(5); }
                    if (!hdr.IsDBNull(6)) { egr.FormaFamaceutica = hdr.GetString(6); }
                    if (!hdr.IsDBNull(7)) { egr.NroLote = hdr.GetString(7); }
                    if (!hdr.IsDBNull(8)) { egr.CantLote = Math.Round(hdr.GetDecimal(8), 0); }
                    if (!hdr.IsDBNull(9)) { egr.Establecimiento = hdr.GetString(9); }
                    if (!hdr.IsDBNull(10)) { egr.RucEstab = hdr.GetString(10); }
                    if (!hdr.IsDBNull(11)) { egr.CalleJrAvN = hdr.GetString(11); }
                    if (!hdr.IsDBNull(12)) { egr.Distrito = hdr.GetString(12); }
                    if (!hdr.IsDBNull(13)) { egr.Provincia = hdr.GetString(13); }
                    if (!hdr.IsDBNull(14)) { egr.Departamento = hdr.GetString(14); }
                    if (!hdr.IsDBNull(15)) { egr.NroFactura = hdr.GetString(15); }
                    if (!hdr.IsDBNull(16)) { egr.Fecha = hdr.GetDateTime(16).ToString("dd/MM/yyyy"); }
                    if (f.TipoControlado == "S") { egr.TipoControlado = "PSICOTROPICOS"; }
                    else if (f.TipoControlado == "P") { egr.TipoControlado = "PRECURSORES"; }
                    else if (f.TipoControlado == "E") { egr.TipoControlado = "ESTUPEFACIENTES"; }
                    if (!(hdr.IsDBNull(15) || hdr.IsDBNull(8) || Math.Round(hdr.GetDecimal(8), 0) == 0.00M))
                    {
                        lista.Add(egr);
                    }

                }
                hdr.Close();
            }
            catch { }

            return lista;
        }

        public List<RptBalanceControladosConsolidado_E> ReporteBalanceControladosConsolidado(FrmBalanceControlados_E f)
        {
            List<RptBalanceControladosConsolidado_E> lista = new List<RptBalanceControladosConsolidado_E>();

            string query = "call " + uti.schemaHana + "COB_BAL_CONT_CONSOLIDADO('" + f.FecIni + "'  ,  '" + f.TipoControlado + "')";
            try
            {
                HanaDataReader hdr = db.HanaExecuteReaderNoSp(query);
                while (hdr.Read())
                {
                    RptBalanceControladosConsolidado_E con = new RptBalanceControladosConsolidado_E();
                    if (!hdr.IsDBNull(0)) { con.RazonSocial = hdr.GetString(0); }
                    if (!hdr.IsDBNull(1)) { con.NmComercial = hdr.GetString(1); }
                    if (!hdr.IsDBNull(2)) { con.RucCob = hdr.GetString(2); }
                    if (!hdr.IsDBNull(3)) { con.Direccion = hdr.GetString(3); }
                    if (!hdr.IsDBNull(4)) { con.Telefono2 = "01-3267430 anexo 201"; }
                    if (!hdr.IsDBNull(5)) { con.Quimico = "PAMELA COLLAHUA SENOSAIN"; }
                    if (!hdr.IsDBNull(6)) { con.Correo = hdr.GetString(6); }
                    if (!hdr.IsDBNull(7)) { con.Anio = hdr.GetInt32(7); }
                    if (!hdr.IsDBNull(8)) { con.CodProducto = hdr.GetString(8); }
                    if (!hdr.IsDBNull(9)) { con.NombreGenerico = hdr.GetString(9); }
                    if (!hdr.IsDBNull(10)) { con.NombreComercial = hdr.GetString(10); }
                    if (!hdr.IsDBNull(11)) { con.Concentracion = hdr.GetString(11); }
                    if (!hdr.IsDBNull(12)) { con.FormaFamaceutica = hdr.GetString(12); }
                    if (!hdr.IsDBNull(13)) { con.SaldoAnterior = Math.Round(hdr.GetDecimal(13), 0); }
                    if (!hdr.IsDBNull(14)) { con.Compra = Math.Round(hdr.GetDecimal(14), 0); }
                    if (!hdr.IsDBNull(14)) { con.Compra = Math.Round(hdr.GetDecimal(14), 0); }
                    if (!hdr.IsDBNull(15)) { con.Venta = Math.Round(hdr.GetDecimal(15), 0); }
                    if (!hdr.IsDBNull(16)) { con.OtrosIngresosNC = Math.Round(hdr.GetDecimal(16), 0); }
                    if (!hdr.IsDBNull(17)) { con.OtrosEgresosDEV = Math.Round(hdr.GetDecimal(17), 0); }
                    if (!hdr.IsDBNull(18)) { con.BaseType = hdr.GetInt32(18); }
                    if (!hdr.IsDBNull(19)) { con.CreatedBy = hdr.GetInt32(19); }
                    if (f.TipoControlado == "S") { con.TipoControlado = "PSICOTROTICOS"; }
                    else if (f.TipoControlado == "P") { con.TipoControlado = "PRECURSORES"; }
                    else if (f.TipoControlado == "E") { con.TipoControlado = "ESTUPEFACIENTES"; }
                    DateTime dt = DateTime.Parse(f.FecIni);
                    if (dt.Month == 1) { con.Trimestre = "I"; }
                    else if (dt.Month == 4) { con.Trimestre = "II"; }
                    else if (dt.Month == 7) { con.Trimestre = "III"; }
                    else if (dt.Month == 10) { con.Trimestre = "IV"; }

                    lista.Add(con);

                }
                hdr.Close();
            }
            catch { }

            return lista;
        }

        public List<RptBalanceControladosLibroControlados_E> ReporteBalanceControladosLibroControlados(FrmBalanceControlados_E f)
        {
            List<RptBalanceControladosLibroControlados_E> lista = new List<RptBalanceControladosLibroControlados_E>();

            string query = "call " + uti.schemaHana + "DIEGO_BAL_CONT_INGRESO_V1('" + f.FecIni + "','" + f.FecFin + "','" + f.TipoControlado + "')";
            try
            {
                HanaDataReader hdr = db.HanaExecuteReaderNoSp(query);
                while (hdr.Read())
                {
                    RptBalanceControladosLibroControlados_E lib = new RptBalanceControladosLibroControlados_E();

                    if (!hdr.IsDBNull(0)) { lib.CodProducto = hdr.GetString(0); }
                    if (!hdr.IsDBNull(1)) { lib.NombreGenerico = hdr.GetString(1); }
                    if (!hdr.IsDBNull(2)) { lib.NombreComercial = hdr.GetString(2); }
                    if (!hdr.IsDBNull(4)) { lib.Concentracion = hdr.GetString(4); }
                    if (!hdr.IsDBNull(5)) { lib.FormaPresentacion = hdr.GetString(5); }
                    if (!hdr.IsDBNull(6)) { lib.FormaFamaceutica = hdr.GetString(6); }
                    if (!hdr.IsDBNull(8)) { lib.Tipo = "INGRESO"; }
                    if (!hdr.IsDBNull(8)) { lib.CantLote = Math.Round(hdr.GetDecimal(8), 0); }
                    if (!hdr.IsDBNull(9)) { lib.Proveedor = hdr.GetString(9); }
                    if (!hdr.IsDBNull(15)) { lib.NroFacturaNcredito = hdr.GetString(15); }
                    if (!hdr.IsDBNull(16)) { lib.Fecha = hdr.GetDateTime(16).ToString("dd/MM/yyyy"); }
                    if (f.TipoControlado == "S") { lib.TipoControlado = "PSICOTROTICOS"; }
                    else if (f.TipoControlado == "P") { lib.TipoControlado = "PRECURSORES"; }
                    else if (f.TipoControlado == "E") { lib.TipoControlado = "ESTUPEFACIENTES"; }
                    lista.Add(lib);
                }
                hdr.Close();
            }
            catch { }


            string queryEgreso = "call " + uti.schemaHana + "DIEGO_BAL_CONT_EGRESO_V2('" + f.FecIni + "','" + f.FecFin + "','" + f.TipoControlado + "')";
            try
            {
                HanaDataReader hdr = db.HanaExecuteReaderNoSp(queryEgreso);
                while (hdr.Read())
                {
                    RptBalanceControladosLibroControlados_E lib = new RptBalanceControladosLibroControlados_E();

                    if (!hdr.IsDBNull(0)) { lib.CodProducto = hdr.GetString(0); }
                    if (!hdr.IsDBNull(1)) { lib.NombreGenerico = hdr.GetString(1); }
                    if (!hdr.IsDBNull(2)) { lib.NombreComercial = hdr.GetString(2); }
                    if (!hdr.IsDBNull(4)) { lib.Concentracion = hdr.GetString(4); }
                    if (!hdr.IsDBNull(5)) { lib.FormaPresentacion = hdr.GetString(5); }
                    if (!hdr.IsDBNull(6)) { lib.FormaFamaceutica = hdr.GetString(6); }
                    if (!hdr.IsDBNull(8)) { lib.Tipo = "EGRESO"; }
                    if (!hdr.IsDBNull(8)) { lib.CantLote = Math.Round(hdr.GetDecimal(8), 0); }
                    if (!hdr.IsDBNull(9)) { lib.Proveedor = hdr.GetString(9); }
                    if (!hdr.IsDBNull(15)) { lib.NroFacturaNcredito = hdr.GetString(15); }
                    if (!hdr.IsDBNull(16)) { lib.Fecha = hdr.GetDateTime(16).ToString("dd/MM/yyyy"); }
                    if (f.TipoControlado == "S") { lib.TipoControlado = "PSICOTROTICOS"; }
                    else if (f.TipoControlado == "P") { lib.TipoControlado = "PRECURSORES"; }
                    else if (f.TipoControlado == "E") { lib.TipoControlado = "ESTUPEFACIENTES"; }

                    if (!(hdr.IsDBNull(15) || hdr.IsDBNull(8) || Math.Round(hdr.GetDecimal(8), 0) == 0.00M))
                    {
                        lista.Add(lib);
                    }

                }
                hdr.Close();
            }
            catch { }

            return lista.OrderBy(x => x.NombreComercial).ToList();
        }
        /******************************************************************************************/

        public List<RptRegistroSanitario_E> ReporteRegistroSanitario(string codArticulo, string firmCode)
        {
            DBHelper db = new DBHelper();
            List<RptRegistroSanitario_E> lista = new List<RptRegistroSanitario_E>();

            string condWhere = string.Empty;
            Dictionary<string, string> estadoOpciones = new Dictionary<string, string>()
            {
                { "01", "VIGENTE" },
                { "02", "AGOTAMIENTO DE STOCK" },
                { "03", "VENCIDO" },
                { "04", "PROCESO DE REINSCRIPCIÓN" },
                { "05", "CANCELADO" },
            };

            if (!string.IsNullOrEmpty(codArticulo) || !string.IsNullOrEmpty(firmCode))
            {
                OORS_D orsD = new OORS_D();
                var datosObs = orsD.ObtenerDatosObsRS("", codArticulo, firmCode);

                if (datosObs != null && datosObs.Count >= 1)
                {
                    foreach (var reg in datosObs)
                    {
                        RptRegistroSanitario_E obj = new RptRegistroSanitario_E
                        {
                            CodArticulo = reg.CodArticulo,
                            RegistroSanitario = reg.RegistroSanitario,
                            Descripcion = reg.Descripcion,
                            RegistradoPor = reg.RegistradoPor,
                            FechaRegistro = reg.FechaRegistro,
                            HoraRegistro = reg.HoraRegistro
                        };

                        string query = "SELECT TOP 100 BTN.\"MnfSerial\", ITM.\"ItemCode\", ITM.\"ItemName\", TO_CHAR(ITM.\"U_COB_FECH_RS\", 'DD/MM/YYYY'), ITM.\"U_COB_OREGS\", ITM.\"U_COB_ESTRS\" " +
                                        $"FROM {uti.schemaHana}OITM ITM LEFT JOIN {uti.schemaHana}OBTN BTN ON BTN.\"ItemCode\" = ITM.\"ItemCode\" " +
                                        $"WHERE BTN.\"MnfSerial\" != '' AND ITM.\"ItemCode\" = '{reg.CodArticulo}'";

                        try
                        {
                            HanaDataReader hdr = db.HanaExecuteReaderNoSp(query);
                            hdr.Read();

                            if (!hdr.IsDBNull(2)) { obj.DescArticulo = hdr.GetString(2); }
                            if (!hdr.IsDBNull(3)) { obj.FechaVenc = hdr.GetString(3); }
                            if (!hdr.IsDBNull(4)) { obj.Comentario = hdr.GetString(4); }
                            if (!hdr.IsDBNull(5)) { obj.Estado = estadoOpciones[hdr.GetString(5)]; }

                            hdr.Close();
                        }
                        catch (Exception e)
                        {
                            throw new Exception(e.Message);
                        }

                        lista.Add(obj);
                    }
                }
            }

            return lista;
        }
    }
}