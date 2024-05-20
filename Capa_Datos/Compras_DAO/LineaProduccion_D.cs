using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Capa_Entidad.Compras_ENT;
using Sap.Data.Hana;
using System.Data.SqlClient;
using System.Data;
using Capa_Entidad.Almacen_ENT.Tablas;
using Capa_Entidad.SocioNegocios_ENT.Tablas;
using Capa_Datos.Almacen_DAO.Tablas;
using Capa_Datos.SocioNegocios_DAO.Tablas;

namespace Capa_Datos.Compras_DAO
{
    public class LineaProduccion_D
    {
        Utilitarios uti = new Utilitarios();
        OMRC_D omrcD = new OMRC_D();
        OCRD_D ocrdD = new OCRD_D();
        // conexion hana
        public List<OCRD_E> listarProveedores()
        {
            return ocrdD.listarSociosDeNegocios(new OCRD_E { CardType="S"});
        }
        public List<OMRC_E> listarFabricantes()
        {
            return omrcD.listarFabricantes();
        }
        public List<OITM_E> listarArticulos(int idLabVal,string idTipoVal)
        {
            List<OITM_E> lista = new List<OITM_E>();
            string query = "select \"ItemCode\",\"ItemName\" from "+uti.schemaHana+"oitm where \"FirmCode\"="+idLabVal;
            if(idTipoVal.Equals("Todos"))
            {
                query += "";
            }
            else if(idTipoVal.Equals("Controlada"))
            {
                query += " and \"ItmsGrpCod\"=103";
            }
            else if (idTipoVal.Equals("NoControlada"))
            {
                query += " and \"ItmsGrpCod\"<>103";
            }
            else if (idTipoVal.Equals("Generico"))
            {
                query += " and \"U_SYP_DFAM\" is not null and  \"FrgnName\" like '%'||UPPER(\"U_SYP_DFAM\")||'%'";
            }
            else if (idTipoVal.Equals("NoGenerico"))
            {
                query += " and ((\"U_SYP_DFAM\" is not null and  \"FrgnName\" not like '%'||UPPER(\"U_SYP_DFAM\")||'%') OR " +
                           "(\"U_SYP_TIPRO\"='TP-02')) ";
            }
            else if (idTipoVal.Equals("ConIgv"))
            {
                query += " and \"ItmsGrpCod\"<>102";
            }
            else if (idTipoVal.Equals("ExeIgv"))
            {
                query += " and \"ItmsGrpCod\"=102";
            }
            else
            {
                return lista;
            }
            query += " order by 2";
            HanaConnection hcn = new HanaConnection(uti.cadHana);
            try
            {
                hcn.Open();
                HanaCommand hcmd = new HanaCommand(query, hcn);
                HanaDataReader hdr = hcmd.ExecuteReader();
                while(hdr.Read())
                {
                    OITM_E a = new OITM_E();
                    a.ItemCode = hdr.GetString(0);
                    a.ItemName = hdr.GetString(1);
                    lista.Add(a);
                }
                hdr.Close();
                hcn.Close();
            }catch { hcn.Close(); }
            return lista;
        }
        public List<LineaProduccion_E> listarLineasProduccion(LineaProduccion_E lf)
        {
            List<LineaProduccion_E> lista = new List<LineaProduccion_E>();
            string query = queryFiltroListadoLp(lf);
            SqlConnection cn = new SqlConnection(uti.cadSql);
            try
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand(query, cn);
                SqlDataReader dr = cmd.ExecuteReader();
                while(dr.Read())
                {
                    LineaProduccion_E l = obtenerLineaProduccion(dr.GetInt32(0));
                    lista.Add(l);
                }
                dr.Close();
                cn.Close();
            }catch { cn.Close(); }
            return lista;
        }
        private string queryFiltroListadoLp(LineaProduccion_E l)
        {
            string query = "select top 30 * from orlp";
            if(l!=null)
            {
                query += " where id>0";
                if (l.id > 0) { query += " and id="+l.id; }
                if(l.Fabricante!=null)
                {
                    if (l.Fabricante.FirmCode > 0) { query += " and FirmCode="+l.Fabricante.FirmCode; }
                    if (l.Fabricante.FirmName != null) { query += " and FirmName='"+l.Fabricante.FirmName+"'"; }
                    if (l.Fabricante.U_SYP_DESC != null) { query += " and U_SYP_DESC like '%"+l.Fabricante.U_SYP_DESC+"%'"; }
                }
                if (l.Descripcion != null) { query += " and Descripcion like '%" + l.Descripcion + "%'"; }
                if (l.Proveedor!=null)
                {
                    if (l.Proveedor.CardCode!=null) { query += " and CardCode='" + l.Proveedor.CardCode + "'"; }
                    if (l.Proveedor.CardName != null) { query += " and CardName like'%" + l.Proveedor.CardName + "%'"; }
                }
                query += " order by 1 desc";
            }
            return query;
        }
        public LineaProduccion_E obtenerLineaProduccion(int id)
        {
            LineaProduccion_E l = new LineaProduccion_E() { Fabricante = new OMRC_E(),Proveedor=new OCRD_E(),Det= new List<DetLineaProduccion_E>() };
            string query = "select * from orlp where id=@id";
            string query2 = "select * from rlp1 where id=@id";
            SqlConnection cn = new SqlConnection(uti.cadSql);
            try
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand(query, cn);
                cmd.Parameters.AddWithValue("@id", id);
                SqlDataReader dr = cmd.ExecuteReader();
                dr.Read();
                    l.id = dr.GetInt32(0);
                    if (!dr.IsDBNull(1)) { l.Fabricante.FirmName = dr.GetString(1); }
                    if (!dr.IsDBNull(2)) { l.Fabricante.U_SYP_DESC = dr.GetString(2); }
                    if (!dr.IsDBNull(3)) { l.Descripcion = dr.GetString(3); }
                    if (!dr.IsDBNull(4)) { l.Proveedor.CardCode = dr.GetString(4); }
                    if (!dr.IsDBNull(5)) { l.Proveedor.CardName = dr.GetString(5); }
                    if (!dr.IsDBNull(6)) { l.PerContacto1 = dr.GetString(6); }
                    if (!dr.IsDBNull(7)) { l.TelefPerContacto1 = dr.GetString(7); }
                    if (!dr.IsDBNull(8)) { l.EmailPerContacto1 = dr.GetString(8); }
                    if (!dr.IsDBNull(9)) { l.TiempoCreacion = dr.GetDateTime(9).ToString(); }
                dr.Close();
                SqlCommand cmd2 = new SqlCommand(query2, cn);
                cmd2.Parameters.AddWithValue("@id", id);
                SqlDataReader dr2 = cmd2.ExecuteReader();
                while(dr2.Read())
                {
                    DetLineaProduccion_E dl = new DetLineaProduccion_E();
                    dl.id = dr2.GetInt32(0);
                    dl.Linea = dr2.GetInt32(1);
                    dl.ItemCode = dr2.GetString(2);
                    dl.ItemName = dr2.GetString(3);
                    l.Det.Add(dl);
                }
                dr2.Close();
                cn.Close();
            }catch { cn.Close(); }
            return l;
        }
        //conexion sql
        public int CrearLineaProduccion(LineaProduccion_E l)
        {
            int status = 0;
            SqlConnection cn = new SqlConnection(uti.cadSql);
            try
            {
                cn.Open();
                SqlTransaction tran = cn.BeginTransaction("transaccion1");
                try
                {
                    SqlCommand cmd = new SqlCommand("MANT_ORLP", cn);
                    cmd.Transaction = tran;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@TipoMantenimiento", "A");
                    cmd.Parameters.AddWithValue("@id", l.id).Direction = ParameterDirection.InputOutput;
                    cmd.Parameters.AddWithValue("@FirmName", l.Fabricante.FirmName);
                    cmd.Parameters.AddWithValue("@U_SYP_DESC", l.Fabricante.U_SYP_DESC);
                    cmd.Parameters.AddWithValue("@Descripcion", l.Descripcion);
                    cmd.Parameters.AddWithValue("@CardCode", l.Proveedor.CardCode);
                    cmd.Parameters.AddWithValue("@CardName", l.Proveedor.CardName);
                    cmd.Parameters.AddWithValue("@PerContacto1", l.PerContacto1);
                    cmd.Parameters.AddWithValue("@TelefPerContacto1", l.TelefPerContacto1);
                    cmd.Parameters.AddWithValue("@EmailPerContacto1", l.EmailPerContacto1);
                    cmd.Parameters.AddWithValue("@TiempoCreacion", l.TiempoCreacion);

                    SqlParameter tbDet = new SqlParameter("@TPRLP1", SqlDbType.Structured);
                    tbDet.Value = DetLineaProduccion_E.tbDetalle(l.FabricarDetalles(l.Det,l.Det2));
                    tbDet.TypeName = "dbo.TPRLP1";
                    cmd.Parameters.AddWithValue("@TPRLP1", tbDet.Value);

                    cmd.ExecuteNonQuery();
                    status = int.Parse(cmd.Parameters["@id"].Value.ToString());
                    //post transacciones
                    SqlCommand cmd2 = new SqlCommand("POST_TRANSACCIONES", cn);
                    cmd2.Transaction = tran;
                    cmd2.CommandType = CommandType.StoredProcedure;
                    cmd2.Parameters.AddWithValue("@Tipo", "A");
                    cmd2.Parameters.AddWithValue("@Tabla", "ORLP");
                    cmd2.Parameters.AddWithValue("@DocNum", cmd.Parameters["@id"].Value);
                    cmd2.Parameters.AddWithValue("@DocEntry", cmd.Parameters["@id"].Value);
                    cmd2.ExecuteNonQuery();
                    tran.Commit();
                }
                catch { tran.Rollback(); cn.Close(); throw new Exception("Error en creacion: "); }
                cn.Close();
            }catch(Exception e2) { cn.Close(); throw new Exception("Error en creacion y conexion: " + e2.Message); }
            return status;
        }
        public int EditarLineaProduccion(LineaProduccion_E l)
        {
            int status = 0;
            SqlConnection cn = new SqlConnection(uti.cadSql);
            try
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand("MANT_ORLP", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@TipoMantenimiento","U");
                cmd.Parameters.AddWithValue("@id", l.id).Direction = ParameterDirection.InputOutput;
                cmd.Parameters.AddWithValue("@Descripcion",l.Descripcion);
                cmd.Parameters.AddWithValue("@CardCode",l.Proveedor.CardCode);
                cmd.Parameters.AddWithValue("@CardName",l.Proveedor.CardName);
                cmd.Parameters.AddWithValue("@PerContacto1",l.PerContacto1);
                cmd.Parameters.AddWithValue("@TelefPerContacto1",l.TelefPerContacto1);
                cmd.Parameters.AddWithValue("@EmailPerContacto1",l.EmailPerContacto1);
                cmd.ExecuteNonQuery();
                status = int.Parse(cmd.Parameters["@id"].Value.ToString());
                cn.Close();
            }catch(Exception e) { cn.Close(); throw new Exception("Error en edicion: "+e.Message); }
            return status;
        }
        public int EliminarLineaProduccion(int id)
        {
            int status = 0;
            SqlConnection cn = new SqlConnection(uti.cadSql);
            try
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand("MANT_ORLP", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@TipoMantenimiento", "D");
                cmd.Parameters.AddWithValue("@id", id).Direction = ParameterDirection.InputOutput;
                cmd.ExecuteNonQuery();
                status = int.Parse(cmd.Parameters["@id"].Value.ToString());
                cn.Close();
            }
            catch (Exception e) { cn.Close(); throw new Exception("Error en edicion: " + e.Message); }
            return status;
        }
        public List<OITM_E> obtenerArticulosDeLp(int id)
        {
            List<OITM_E> lista = new List<OITM_E>();
            LineaProduccion_E l = obtenerLineaProduccion(id);
            foreach(DetLineaProduccion_E d in l.Det)
            {
                OITM_E a = new OITM_E()
                {
                    ItemCode = d.ItemCode,
                    ItemName = d.ItemName,
                    Status = d.Status
                };
                lista.Add(a);
            }
            return lista;
        }
        //*** operacion de formulario info
        public string infoListarArticulos(List<OITM_E> lista)
        {
            string info = "<table border=1 style='background-color:darkgrey;font-size:13px;'>"+
                                "<tr>"+
                                    "<th>#</th>"+
                                    "<th>Codigo</th>"+
                                    "<th>Nombre</th>"+
                                    "<th hidden>Retirar?</th>" +
                                "</tr>";
            for(int j=0;j<lista.Count;j++)
            {
                info += "<tr id='celda" + j + "'>" +
                           "<td><input type='text' name='Det[" + j + "].Linea' id='Det" + j + "Linea' value='" + j + "' readonly size=2>" +
                           "<td><input type='text' name='Det[" + j + "].ItemCode' id='Det" + j + "ItemCode' readonly size=8></td>" +
                           "<td><input type='search' name='Det[" + j + "].ItemName' id='Det" + j + "ItemName' list='ListaArticulos' size=40 onchange=\"valorCampoList('ListaArticulos','Det"+j+ "ItemName','Det" + j+ "ItemCode','ItemCode')\" autocomplete='off'></td>" +
                        "</tr>";
            }
            info += "<datalist id = 'ListaArticulos' > " +
                                infoListarArticulosManual(lista) +
                           "</datalist>" + 
                "</table>";
            return info;
        }
        public string infoListarArticulos2(List<OITM_E> lista)
        {
            string info = "<table border=1 style='background-color:darkgrey;font-size:13px;'>" +
                                "<tr>" +
                                    "<th>#</th>" +
                                    "<th>Codigo</th>" +
                                    "<th>Nombre</th>" +
                                    "<th hidden>Retirar?</th>" +
                                "</tr>";
            for (int j = 0; j < lista.Count; j++)
            {
                info += "<tr id='celda" + j + "'>" +
                           "<td><input type='text' name='Det2[" + j + "].Linea' id='Det2" + j + "Linea' value='" + j + "' readonly size=2>" +
                           "<td><input type='text' name='Det2[" + j + "].ItemCode' id='Det2" + j + "ItemCode' readonly size=8></td>" +
                           "<td><input type='search' name='Det2[" + j + "].ItemName' id='Det2" + j + "ItemName' list='ListaArticulos2' size=40 onchange=\"valorCampoList('ListaArticulos2','Det2" + j + "ItemName','Det2" + j + "ItemCode','ItemCode')\" autocomplete='off'></td>" +
                        "</tr>";
            }
            info += "<datalist id = 'ListaArticulos2' > " +
                                infoListarArticulosManual(lista) +
                           "</datalist>" +
                "</table>";
            return info;
        }
        public string infoListarArticulosManual(List<OITM_E> lista)
        {
            string info = "";
            foreach (OITM_E a in lista)
            {
                info += "<option value='"+a.ItemName+"' ItemCode='"+a.ItemCode+"'></option>";
            }
            return info;
        }
    }
}
