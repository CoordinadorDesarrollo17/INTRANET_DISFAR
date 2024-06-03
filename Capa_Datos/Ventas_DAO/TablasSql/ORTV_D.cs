using Capa_Datos.Ventas_DAO.Tablas;
using Capa_Entidad.ReportesDigemid_ENT;
using Capa_Entidad.Rutas_ENT.ReportesSql;
using Capa_Entidad.Rutas_ENT.TablasSql;
using Capa_Entidad.Seguridad_ENT;
using Capa_Entidad.SocioNegocios_ENT.Tablas;
using Capa_Entidad.Ventas_ENT.Reportes;
using Capa_Entidad.Ventas_ENT.Tablas;
using Capa_Entidad.Ventas_ENT.TablasSql;
using Microsoft.ReportingServices.ReportProcessing.ReportObjectModel;
using Sap.Data.Hana;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Capa_Datos.Ventas_DAO.TablasSql
{
    public class ORTV_D
    {
        readonly Utilitarios uti = new Utilitarios();
        readonly DBHelper db = new DBHelper();
        CC_ORTV_D ccTicket = new CC_ORTV_D();
        OLDS_D lD = new OLDS_D();

        //consultas a hana
        public List<OCRD_E> listarClientes(string Fecha)
        {
            List<OCRD_E> lista = new List<OCRD_E>();
            HanaConnection hcn = new HanaConnection(uti.cadHana);
            try
            {
                hcn.Open();
                HanaCommand hcmd = new HanaCommand("SELECT DISTINCT \"CardCode\",\"CardName\" FROM " + uti.schemaHana + "ORDR WHERE \"DocDate\" " +
                    "BETWEEN ADD_DAYS('" + Fecha + "',0) AND '" + Fecha + "' ORDER BY \"CardName\"", hcn);
                HanaDataReader hdr = hcmd.ExecuteReader();
                while (hdr.Read())
                {
                    OCRD_E c = new OCRD_E();
                    c.CardCode = hdr.GetString(0);
                    c.CardName = hdr.GetString(1);
                    lista.Add(c);
                }
                hdr.Close();
                hcn.Close();
            }
            catch { hcn.Close(); }
            return lista;
        }
        public List<RTV3_E> listarDirDestinos(string CardCode)
        {
            List<RTV3_E> lista = new List<RTV3_E>();
            HanaConnection hcn = new HanaConnection(uti.cadHana);
            try
            {
                hcn.Open();
                HanaCommand hcmd = new HanaCommand("select \"Address\"||': '||ifnull(\"Street\",'')||' '||ifnull(\"Block\",'') ||' '||ifnull(\"City\",'')||' '||ifnull(\"County\",'')as \"Dir\", \"Block\",\"City\",\"County\", \"ZipCode\", \"Street\" ,\"Address2\"  from " + uti.schemaHana + "crd1 where \"CardCode\"='" + CardCode + "' and \"Address\" like 'ENV%' ORDER BY \"LineNum\" ", hcn);
                HanaDataReader hdr = hcmd.ExecuteReader();
                while (hdr.Read())
                {
                    RTV3_E obj = new RTV3_E();

                    if (!hdr.IsDBNull(0)) { obj.DirDestino = hdr.GetString(0); }
                    if (!hdr.IsDBNull(1)) { obj.Distrito = hdr.GetString(1); }
                    if (!hdr.IsDBNull(2)) { obj.Provincia = hdr.GetString(2); }
                    if (!hdr.IsDBNull(3)) { obj.Departamento = hdr.GetString(3); }
                    if (!hdr.IsDBNull(4)) { obj.Ubigeo = hdr.GetInt32(4); }
                    if (!hdr.IsDBNull(5)) { obj.Calle = hdr.GetString(5); }
                    if (!hdr.IsDBNull(6)) { obj.Zona = hdr.GetString(6); }

                    lista.Add(obj);
                }
                hdr.Close();
                hcn.Close();
            }
            catch { hcn.Close(); }
            return lista;
        }
        public List<OrdenDeVenta_E> ListarOrdenesdeVenta(string fecha, string cardCode, int docNum) //HANA TABLA ORDR (ORDEN DE VENTA)
        {
            var lista = new List<OrdenDeVenta_E>();
            HanaConnection hcn = new HanaConnection(uti.cadHana);
            try
            {
                hcn.Open();
                HanaCommand hcmd = new HanaCommand("SELECT T0.\"DocNum\",(select \"SlpName\" from " + uti.schemaHana + "oslp where \"SlpCode\" = T0.\"SlpCode\") " +
                       " ,\"DocTotal\",(select \"Name\" from " + uti.schemaHana + "\"@COB_LUG_ENTREGA\" where \"Code\"=T0.\"U_COB_LUGAREN\") , T1.\"WhsCode\" " +
                       "FROM " + uti.schemaHana + "ORDR T0 inner join " + uti.schemaHana +
                       "RDR1 T1 on T1.\"DocEntry\"= T0.\"DocEntry\" WHERE T0.\"DocDate\" = '" + fecha + "' " +
                       "AND T0.\"CardCode\" = '" + cardCode + "' AND T0.\"Comments\"='" + docNum + "' AND T0.\"CANCELED\"= 'N' GROUP BY T0.\"DocEntry\", " +
                       "T0.\"DocNum\", T0.\"SlpCode\", T0.\"DocTotal\", T0.\"U_COB_LUGAREN\", T1.\"WhsCode\", T0.\"DocDate\", T0.\"CardCode\"" +
                       " ORDER BY T0.\"DocDate\",T0.\"CardCode\",T0.\"DocEntry\"", hcn);
                hcmd.CommandType = CommandType.Text;
                HanaDataReader hdr = hcmd.ExecuteReader();
                while (hdr.Read())
                {
                    OrdenDeVenta_E o = new OrdenDeVenta_E();
                    o.DocNum = hdr.GetInt32(0);
                    o.CardCode = cardCode;
                    o.SlpName = hdr.GetString(1);
                    o.DocTotal = hdr.GetDecimal(2);
                    o.LugarDeEntrega = hdr.GetString(3);
                    o.AlmacenSalida = hdr.GetString(4);
                    lista.Add(o);
                }
                hdr.Close();
                hcn.Close();
            }
            catch { hcn.Close(); }
            return lista;
        }
        public List<OrdenDeVenta_E> ListarOrdenesdeVentaFinales(string fecha, string cardCode, int docNum)
        {
            List<OrdenDeVenta_E> lista = new List<OrdenDeVenta_E>();
            SqlConnection cn = new SqlConnection(uti.cadSql);
            try
            {
                cn.Open();
                //Primero se realiza un foreach de la primera lista que obtenemos de HANA para obtener la lista general con los filtros enviados
                // Para porteriormente discriminar todos los tickets que su @Estado = 'Anulado'  O 'CANCELADO' porque este estado solo se obtiene en la base intermedia(SQL Server) mas no en HANA

                foreach (OrdenDeVenta_E o in ListarOrdenesdeVenta(fecha, cardCode, docNum))
                {
                    SqlCommand cmd = new SqlCommand("select T1.NroSap from " +
                        " vt.ORTV T0 inner join vt.RTV2 T1 on T1.DocEntry = T0.DocEntry " +
                         " where T0.Estado not in ('CANCELADO','ANULADO') and NroSap=" + o.DocNum, cn);
                    int DocNum = Convert.ToInt32(cmd.ExecuteScalar());
                    if (DocNum != o.DocNum)
                    {
                        lista.Add(o);
                    }
                }
                cn.Close();
            }
            catch (Exception ex) { cn.Close(); Console.WriteLine($"Error de SQL: {ex.Message}"); }
            return lista;
        }
        //consultas a sql
        //metodo principal que alimenta todos los listados de ticket
        public List<ORTV_E> listarTicketsVenta(Usuario_E user, ORTV_E t)
        {
            List<ORTV_E> lista = new List<ORTV_E>();
            string condWhere = string.Empty, subConsulta = string.Empty;

            if (user.IdRol == 7) { condWhere += $" AND t0.CodSapVendedor='{user.CodigoSap}'"; }

            if (t != null)
            {
                if (t.DocNum > 0) { condWhere += $" AND t0.DocNum like  '%{t.DocNum}%'"; }
                if (t.FechaSapTicket != null) { condWhere += $" AND t0.FechaSapTicket='{t.FechaSapTicket}'"; }
                if (t.CardName != null) { condWhere += $" AND t0.CardName like '%{t.CardName}%'"; }
                if (t.LugarDestino != null) { condWhere += $" AND t0.LugarDestino='{t.LugarDestino}'"; }
                if (t.Vendedor != null) { condWhere += $" AND t0.Vendedor like '%{t.Vendedor}%'"; }
                if (t.MontoTotal > 0) { condWhere += $" AND t0.MontoTotal like '{t.MontoTotal}%'"; }
                if (t.MontoFinal > 0) { condWhere += $" AND t0.MontoFinal like '{t.MontoFinal}%'"; }
                if (t.EstadoPago != null) { condWhere += $" AND t0.EstadoPago='{t.EstadoPago}'"; }
                if (t.Estado != null) { condWhere += $" AND t0.Estado='{t.Estado}'"; }
                if (t.Flete == 0.01M) { condWhere += " AND t0.Flete>0"; }
                if (t.DescuentoNC == 0.01M) { condWhere += " AND t0.DescuentoNC>0"; }
                if (t.PagoEnv == 0.01M) { condWhere += " AND t0.PagoEnv>0"; }
                if (t.EstadoFacturacion != null) { condWhere += $" AND t0.EstadoFacturacion='{t.EstadoFacturacion}'"; }
                if (t.TipoVenta != null) { condWhere += $" AND t0.TipoVenta ='{t.TipoVenta}'"; }
                if (t.TiempoEntrega != null)
                {
                    if (t.NombreVista != null)
                    {
                        if (t.NombreVista.Equals("ListadoTicketsDespacho") && t.TiempoEntrega != null)
                        {
                            condWhere += $" AND CONVERT(varchar, t0.TiempoEntrega , 121) = '{Convert.ToDateTime(t.TiempoEntrega).ToString("yyyy-MM-dd HH:mm:ss.fff")}'";
                        }
                    }
                    else
                    {
                        condWhere += $" AND CONVERT(char(10), t0.TiempoEntrega,126) = '{Convert.ToDateTime(t.TiempoEntrega).ToString("yyyy-MM-dd")}'";
                    }
                }
                if (t.NombreVista != null)
                {
                    if (t.NombreVista.Equals("ListadoTickets"))
                    {
                        subConsulta = "AND (SELECT top 1 FechaOperacion from vt.CC_ORTV where Operacion='SEPARAR' and DocEntry=t0.DocEntry  order by FechaOperacion DESC, HoraOperacion desc) between dateadd(day,-1000,getdate()) and getdate()";
                    }
                }
                if (t.EstadoGasto != null) { condWhere += $" AND t0.EstadoGasto='{t.EstadoGasto}'"; }
                if (!string.IsNullOrEmpty(t.AlmProcedencia))
                {
                    condWhere += $" AND (SELECT TOP 1 T2.AlmacenSalida FROM vt.RTV2 T2 WHERE T2.AlmacenSalida NOT IN ('07','06') AND T2.DocEntry = T0.DocEntry) ='" + t.AlmProcedencia + "' ";
                }
            }

            using (SqlConnection cn = new SqlConnection(uti.cadSql))
            {
                //LOS QUERYS COMENTADOS BUSCAN MINIMIZAR LA CONSULTA EN UN RANGO DE 2022 A 2024, AUN NO SE APLICA
                //string select = "TOP 100 t0.DocEntry, t0.DocNum FROM vt.ORTV t0 inner join vt.CC_ORTV t1 on t1.DocEntry=t0.DocEntry ";
                string select = "TOP 100 t0.DocEntry, t0.DocNum FROM vt.ORTV t0 ";
                string query = $"SELECT {select} WHERE t0.DocEntry>0 {subConsulta} {condWhere} ORDER BY t0.DocNum DESC";
                //string query = $"SELECT {select} WHERE t1.Operacion='SEPARAR' and t1.FechaOperacion between '2022-01-01' and getdate() {subConsulta} {condWhere} ORDER BY t0.DocNum DESC";

                //string query = $"SELECT {select} WHERE (select TOP 1 FechaOperacion from vt.CC_ORTV where DocEntry=t0.DocEntry and Operacion='SEPARAR') between '2022-01-01' and getdate() {subConsulta} {condWhere} ORDER BY t0.DocNum DESC";
                SqlCommand cmd = new SqlCommand(query, cn)
                {
                    CommandType = CommandType.Text
                };
                cn.Open();

                try
                {
                    SqlDataReader dr = cmd.ExecuteReader();
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            ORTV_E ticket = new ORTV_E();
                            CC_ORTV_D ccORTV = new CC_ORTV_D();
                            RTV11_D datosRTV11 = new RTV11_D();
                            RTV12_D datosRTV12 = new RTV12_D();
                            RTV13_D datosRTV13 = new RTV13_D();
                            ticket = obtenerTicket(dr.GetInt32(0));
                            ticket.Vendedor = (ticket.Vendedor.Length > 15) ? ticket.Vendedor.Substring(0, 15) : ticket.Vendedor;
                            ticket.FechaSapTicket = (ticket.FechaSapTicket != null) ? Convert.ToDateTime(ticket.FechaSapTicket).ToString("dd/MM/yyyy") : null;  //usando funcion local

                            List<CC_ORTV_E> ticketRecibido = ccORTV.ListarCC_ORTV(ticket.DocEntry, "RECIBIR");
                            // Substring para solo ver HH:mm ya que nunca se guardan los segundos en la BD
                            if (!string.IsNullOrEmpty(ticketRecibido[0].HoraOperacion)) { ticket.HoraRecibir = ticketRecibido[0].HoraOperacion.Substring(0, 5); }

                            List<CC_ORTV_E> ticketAbierto = ccORTV.ListarCC_ORTV(ticket.DocEntry, "REGISTRAR");
                            // Substring para solo ver HH:mm ya que nunca se guardan los segundos en la BD
                            ticket.FechaAbierto = ticketAbierto[0].FechaOperacion;
                            if (!string.IsNullOrEmpty(ticketAbierto[0].HoraOperacion)) { ticket.HoraAbierto = ticketAbierto[0].HoraOperacion.Substring(0, 5); }
                            //Buscamos el ultimo estado del ticket excluyendo a los estados que no trascienden en las operaciones del ticket.
                            ticket.ultimoCCEstado = ccTicket.ListarCC_ORTV(ticket.DocEntry, null, true).FirstOrDefault().Operacion;
                            ticket.aptoFinVerificar = false; ticket.aptoIniVerificar = false;
                            /**************************************************************************************/
                            ticket.hayFinPicking = false; ticket.hayFinVerificar = false; ticket.hayFinEmpacar = false;
                            ticket.hayIniPicking = false; ticket.hayIniVerificar = false; ticket.hayIniEmpacar = false;
                            /**************************************************************************************/

                            // Revisamos si hay INICIO PICKING
                            List<CC_ORTV_E> ticketIniPicking = ccORTV.ListarCC_ORTV(ticket.DocEntry, "INICIO PICKING");
                            // Revisamos si hay ANULAR INICIO PICKING
                            List<CC_ORTV_E> ticketAnularIniPicking = ccORTV.ListarCC_ORTV(ticket.DocEntry, "ANULAR INICIO PICKING");
                            List<CC_ORTV_E> listaIPick = new List<CC_ORTV_E>() { ticketIniPicking[0], ticketAnularIniPicking[0] };
                            var listaIPickOrd = listaIPick.OrderByDescending(x => x.Id);
                            if (listaIPickOrd.FirstOrDefault().Operacion == "INICIO PICKING") { ticket.hayIniPicking = true; }
                            else if (listaIPickOrd.FirstOrDefault().Operacion == "ANULAR INICIO PICKING") { ticket.hayIniPicking = false; }
                            if (ticket.Estado.Equals("PICKEANDO")) { ticket.hayIniPicking = true; }
                            /*************************************************************/

                            // Revisamos si hay INICIO EMPACAR
                            List<CC_ORTV_E> ticketIniEmpacar = ccORTV.ListarCC_ORTV(ticket.DocEntry, "INICIO EMPACAR");
                            // Revisamos si hay ANULAR INICIO EMPACAR
                            List<CC_ORTV_E> ticketAnularIniEmpacar = ccORTV.ListarCC_ORTV(ticket.DocEntry, "ANULAR INICIO EMPACAR");
                            List<CC_ORTV_E> listaEmp = new List<CC_ORTV_E>() { ticketIniEmpacar[0], ticketAnularIniEmpacar[0] };
                            var listaEmpOrd = listaEmp.OrderByDescending(x => x.Id);
                            if (listaEmpOrd.FirstOrDefault().Operacion == "INICIO EMPACAR") { ticket.hayIniEmpacar = true; }
                            else if (listaEmpOrd.FirstOrDefault().Operacion == "ANULAR INICIO EMPACAR") { ticket.hayIniEmpacar = false; }
                            if (ticket.Estado.Equals("EMPACANDO")) { ticket.hayIniEmpacar = true; }
                            /*************************************************************/
                            //El boton "INICIO VERIFICAR" solo se habilita en casos que el ticket se encuentre PICKEANDO pero que a su vez no exista un previo registro de INICIO VERIFICAR
                            //en el control de cambios, validando a la par que si hay un ANULAR INICIO VERIFICAR se tendra que comparar la fecha y hora para saber con certeza que el INICIO VERIFICAR se ha dado

                            // Revisamos si hay INICIO VERIFICAR
                            List<CC_ORTV_E> ticketIniVerificar = ccORTV.ListarCC_ORTV(ticket.DocEntry, "INICIO VERIFICAR");
                            // Revisamos si hay ANULAR INICIO VERIFICAR
                            List<CC_ORTV_E> ticketAnularIniVerificar = ccORTV.ListarCC_ORTV(ticket.DocEntry, "ANULAR INICIO VERIFICAR");
                            List<CC_ORTV_E> listaVerif = new List<CC_ORTV_E>() { ticketIniVerificar[0], ticketAnularIniVerificar[0] };
                            var listaVerifOrd = listaVerif.OrderByDescending(x => x.Id);
                            if (listaVerifOrd.FirstOrDefault().Operacion == "INICIO VERIFICAR") { ticket.aptoIniVerificar = false; ticket.hayIniVerificar = true; }
                            else if (listaVerifOrd.FirstOrDefault().Operacion == "ANULAR INICIO VERIFICAR") { ticket.aptoIniVerificar = true; ticket.hayIniVerificar = false; }
                            if (ticket.hayIniPicking) { ticket.aptoIniVerificar = true; }
                            if (ticket.hayIniVerificar) { ticket.aptoIniVerificar = false; }
                            if (ticket.Estado != "PICKEANDO") { ticket.aptoIniVerificar = false; }

                            /********************************************************/
                            // Revisamos si hay FIN VERIFICAR
                            List<CC_ORTV_E> ticketFinVerificar = ccORTV.ListarCC_ORTV(ticket.DocEntry, "FIN VERIFICAR");
                            // Revisamos si hay ANULAR FIN VERIFICAR
                            List<CC_ORTV_E> ticketAnularFinVerificar = ccORTV.ListarCC_ORTV(ticket.DocEntry, "ANULAR FIN VERIFICAR");
                            List<CC_ORTV_E> listaFVerif = new List<CC_ORTV_E>() { ticketFinVerificar[0], ticketAnularFinVerificar[0] };
                            var listaFVerifOrd = listaFVerif.OrderByDescending(x => x.Id);
                            if (listaFVerifOrd.FirstOrDefault().Operacion == "FIN VERIFICAR") { ticket.hayFinVerificar = true; }
                            else if (listaFVerifOrd.FirstOrDefault().Operacion == "ANULAR FIN VERIFICAR") { ticket.hayFinVerificar = false; }


                            /****************************************************************************************************/

                            //El boton "FIN VERIFICAR" solo se habilita en dos casos, si el ticket tiene en su tabla de control de cambios el FIN PICKING,
                            //pero si en caso tambien se encuentra un ANULAR FIN PICKING se tendra que comparar la fecha
                            //y hora para saber con certeza que el FIN PICKING se ha dado, o tambien si el ticket esta en estado general VERIFICANDO

                            // Revisamos si hay FIN PICKING
                            List<CC_ORTV_E> ticketFinPicking = ccORTV.ListarCC_ORTV(ticket.DocEntry, "FIN PICKING");
                            // Revisamos si hay ANULAR FIN PICKING
                            List<CC_ORTV_E> ticketAnularFinPicking = ccORTV.ListarCC_ORTV(ticket.DocEntry, "ANULAR FIN PICKING");
                            List<CC_ORTV_E> listaPicking = new List<CC_ORTV_E>() { ticketFinPicking[0], ticketAnularFinPicking[0] };
                            var listaPickingOrd = listaPicking.OrderByDescending(x => x.Id);
                            if (listaPickingOrd.FirstOrDefault().Operacion == "FIN PICKING") { ticket.aptoFinVerificar = true; ticket.hayFinPicking = true; }
                            else if (listaPickingOrd.FirstOrDefault().Operacion == "ANULAR FIN PICKING") { ticket.aptoFinVerificar = false; ticket.hayFinPicking = false; }

                            /****************************************************************************************************/
                            //El boton "FIN EMPACAR" solo se habilita en dos casos, si el ticket tiene en su tabla de control de cambios el FIN PICKING,
                            //pero si en caso tambien se encuentra un ANULAR FIN PICKING se tendra que comparar la fecha
                            //y hora para saber con certeza que el FIN PICKING se ha dado, o tambien si el ticket esta en estado general VERIFICANDO

                            // Revisamos si hay FIN EMPACAR
                            List<CC_ORTV_E> ticketFinEmpacar = ccORTV.ListarCC_ORTV(ticket.DocEntry, "FIN EMPACAR");
                            // Revisamos si hay ANULAR FIN EMPACAR
                            List<CC_ORTV_E> ticketAnularFinEmpacar = ccORTV.ListarCC_ORTV(ticket.DocEntry, "ANULAR FIN EMPACAR");
                            List<CC_ORTV_E> listaFEmpac = new List<CC_ORTV_E>() { ticketFinEmpacar[0], ticketAnularFinEmpacar[0] };
                            var listaFEmpacOrd = listaFEmpac.OrderByDescending(x => x.Id);
                            if (listaFEmpacOrd.FirstOrDefault().Operacion == "FIN EMPACAR") { ticket.hayFinEmpacar = true; }
                            else if (listaFEmpacOrd.FirstOrDefault().Operacion == "ANULAR FIN EMPACAR") { ticket.hayFinEmpacar = false; }

                            if (ticket.Estado == "VERIFICANDO") { ticket.aptoFinVerificar = true; }
                            if (ticket.Estado != "VERIFICANDO" && ticket.Estado != "PICKEANDO") { ticket.aptoFinVerificar = false; }
                            if (ticket.ultimoCCEstado == "FIN VERIFICAR") { ticket.aptoFinVerificar = false; }
                            if (ticket.hayIniVerificar == false) { ticket.aptoFinVerificar = false; }
                            if (ticket.hayFinVerificar) { ticket.aptoFinVerificar = false; }
                            /**********************************************************************************/
                            if (ticket.hayFinPicking)
                            {
                                // Trae el OpSacado principal 
                                List<CC_ORTV_E> ticketSacando = ccORTV.ListarCC_ORTV(ticket.DocEntry, "FIN PICKING");
                                if (ticketSacando != null && ticketSacando.Count > 0)
                                {
                                    ticket.OpSacando = ticketSacando[0].Operario;
                                }
                                // Trae los operarios sacando de apoyo 
                                List<string> operariosSacando = datosRTV11.BuscarOperariosSacando(ticket.DocEntry);
                                if (operariosSacando != null && operariosSacando.Count > 0)
                                {
                                    ticket.OpSacandoApoyo = operariosSacando;
                                }
                            }
                            if (ticket.hayFinVerificar)
                            {
                                // Trae el operario de verificacion principal 
                                List<CC_ORTV_E> ticketVerificando = ccORTV.ListarCC_ORTV(ticket.DocEntry, "FIN VERIFICAR");
                                if (ticketVerificando != null && ticketVerificando.Count > 0)
                                {
                                    ticket.OpVerificado = ticketVerificando[0].Operario;

                                    // Trae los operarios de verificado de apoyo
                                    List<string> operariosChequeando = datosRTV12.BuscarOperariosChequeando(ticket.DocEntry);
                                    if (operariosChequeando != null && operariosChequeando.Count > 0)
                                    {
                                        ticket.OpVerificadoApoyo = operariosChequeando;
                                    }
                                }

                                if (ticket.hayFinEmpacar && ticket.Cajas >= 1)
                                {
                                    // Trae e l operario de empaque principal/
                                    List<CC_ORTV_E> ticketEmpacando = ccORTV.ListarCC_ORTV(ticket.DocEntry, "FIN EMPACAR");
                                    if (ticketEmpacando != null && ticketEmpacando.Count > 0) { ticket.OpEmpacado = ticketEmpacando[0].Operario; }
                                }
                                // Trae los operarios de empacado de apoyo/
                                List<string> operariosEmpacando = datosRTV13.BuscarOperariosEmpacando(ticket.DocEntry);
                                if (operariosEmpacando != null && operariosEmpacando.Count > 0)
                                {
                                    ticket.OpEmpacadoApoyo = operariosEmpacando;
                                }
                            }
                            lista.Add(ticket);
                        }
                    }

                    dr.Close();
                    cn.Close();
                }
                catch (Exception e) { cn.Close(); throw new Exception(e.Message); }
            }

            return lista;
        }
        //metodo usa un procedure para buscar tickets vinculados, solo se usa en la creacion y agregacion de tickets (Editar) en las hojas de ruta
        public List<string> BuscarVinculados(int DocEntry, int DocNum)
        {
            List<string> listaVinculados = new List<string>();

            using (SqlConnection cn = new SqlConnection(uti.cadSql))
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand("vt.TicketsVinculados", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@DocEntry", DocEntry);
                cmd.Parameters.AddWithValue("@DocNum", DocNum);
                SqlDataReader dr = cmd.ExecuteReader();
                string vinculadosCadena;
                while (dr.Read())
                {
                    if (!dr.IsDBNull(0))
                    {
                        vinculadosCadena = dr.GetString(0);
                        string[] elementos = vinculadosCadena.Split(',');

                        foreach (string elemento in elementos)
                        {
                            // Agregar el elemento a la lista si no está vacío y no existe en la lista, para evitar valores repetidos en caso lo hayan vinculado mas de una vez.
                            if (!string.IsNullOrWhiteSpace(elemento) && !listaVinculados.Contains(elemento.Trim()))
                            {
                                listaVinculados.Add(elemento.Trim());
                            }
                        }
                    }
                }
                dr.Close(); cn.Close();
            }
            return listaVinculados;
        }
        //metodo usado solo en la creacion de tickets
        public List<ORTV_E> listarTicketsSeparados(int Id)
        {
            List<ORTV_E> lista = new List<ORTV_E>();

            using (SqlConnection cn = new SqlConnection(uti.cadSql))
            {
                string query = "SELECT DocEntry, DocNum FROM vt.ORTV WHERE Estado = 'SEPARADO' AND CodSapVendedor = @Id";
                SqlCommand cmd = new SqlCommand(query, cn);

                cmd.Parameters.AddWithValue("@Id", Id);

                cn.Open();

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            ORTV_E t = new ORTV_E
                            {
                                DocEntry = dr.GetInt32(0),
                                DocNum = dr.GetInt32(1)
                            };
                            lista.Add(t);
                        }
                    }
                }
            }

            return lista;
        }

        public ORTV_E obtenerTicket(int DocEntry)
        {
            ORTV_E t = new ORTV_E();
            SqlConnection cn = new SqlConnection(uti.cadSql);
            try
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand("select DocEntry,DocNum,CardCode, CardName,Estado,TipoVenta,LugarDestino, DirDestino,Referencia,Agencia,EnvioAgencia,Embalaje,CodSapVendedor,Vendedor,MontoTotal,Flete,GastoEnvio,EstadoGasto,PagoEnv,ClaveEnv,TiempoEntrega,DescuentoNC,DeudaCliente,DeudaEmpresa,MontoFinal,FormaPago,MontoRecibido,EstadoPago,FechaPago,HoraPago,Cajero,Comentario,Cajas,NroMesa,FechaNC,EstadoFacturacion,FechaFacturacion,HoraFacturacion,OpFacturacion, Observaciones,Observaciones2,Observaciones3,FechaSapTicket, (Select top 1 FechaOperacion from vt.CC_ORTV where DocEntry=" + DocEntry + " and Operacion='REGISTRAR' order by FechaOperacion DESC,HoraOperacion DESC ) AS 'FECHA REGISTRO', (Select top 1 HoraOperacion from vt.CC_ORTV where DocEntry=" + DocEntry + " and Operacion='REGISTRAR' order by FechaOperacion,HoraOperacion desc ) AS 'HORA REGISTRO' ,Zona,Notificado,Visible  from vt.ORTV where DocEntry=" + DocEntry, cn);
                cmd.CommandType = CommandType.Text;
                SqlDataReader dr = cmd.ExecuteReader();
                dr.Read();
                t.DocEntry = dr.GetInt32(0);
                t.DocNum = dr.GetInt32(1);
                if (!dr.IsDBNull(2)) { t.CardCode = dr.GetString(2); }
                if (!dr.IsDBNull(3)) { t.CardName = dr.GetString(3); }
                if (!dr.IsDBNull(4)) { t.Estado = dr.GetString(4); }
                if (!dr.IsDBNull(5)) { t.TipoVenta = dr.GetString(5); }
                if (!dr.IsDBNull(6)) { t.LugarDestino = dr.GetString(6); }
                if (!dr.IsDBNull(7)) { t.DirDestino = dr.GetString(7); }
                if (!dr.IsDBNull(8)) { t.Referencia = dr.GetString(8); }
                if (!dr.IsDBNull(9)) { t.Agencia = dr.GetString(9); }
                if (!dr.IsDBNull(10)) { t.EnvioAgencia = dr.GetString(10); }
                if (!dr.IsDBNull(11)) { t.Embalaje = dr.GetString(11); }
                if (!dr.IsDBNull(12)) { t.CodSapVendedor = dr.GetInt32(12); }
                if (!dr.IsDBNull(13)) { t.Vendedor = dr.GetString(13); }
                if (!dr.IsDBNull(14)) { t.MontoTotal = dr.GetDecimal(14); }
                if (!dr.IsDBNull(15)) { t.Flete = dr.GetDecimal(15); }
                if (!dr.IsDBNull(16)) { t.GastoEnvio = dr.GetDecimal(16); }
                if (!dr.IsDBNull(17)) { t.EstadoGasto = dr.GetString(17); }
                if (!dr.IsDBNull(18)) { t.PagoEnv = dr.GetDecimal(18); }
                if (!dr.IsDBNull(19)) { t.ClaveEnv = dr.GetString(19); }
                if (!dr.IsDBNull(20)) { t.TiempoEntrega = dr.GetDateTime(20); }
                if (!dr.IsDBNull(21)) { t.DescuentoNC = dr.GetDecimal(21); }
                if (!dr.IsDBNull(22)) { t.DeudaCliente = dr.GetDecimal(22); }
                if (!dr.IsDBNull(23)) { t.DeudaEmpresa = dr.GetDecimal(23); }
                if (!dr.IsDBNull(24)) { t.MontoFinal = dr.GetDecimal(24); }
                if (!dr.IsDBNull(25)) { t.FormaPago = dr.GetString(25); }
                if (!dr.IsDBNull(26)) { t.MontoRecibido = dr.GetDecimal(26); }
                if (!dr.IsDBNull(27)) { t.EstadoPago = dr.GetString(27); }
                if (!dr.IsDBNull(28)) { t.FechaPago = dr.GetDateTime(28).ToString("yyyy-MM-dd"); }
                if (!dr.IsDBNull(29)) { t.HoraPago = dr.GetTimeSpan(29).ToString(); }
                if (!dr.IsDBNull(30)) { t.Cajero = dr.GetString(30); }
                if (!dr.IsDBNull(31)) { t.Comentario = dr.GetString(31); }
                if (!dr.IsDBNull(32)) { t.Cajas = dr.GetInt32(32); }
                if (!dr.IsDBNull(33)) { t.NroMesa = dr.GetInt32(33); }
                if (!dr.IsDBNull(34)) { t.FechaNC = dr.GetDateTime(34).ToString("yyyy-MM-dd"); }
                if (!dr.IsDBNull(35)) { t.EstadoFacturacion = dr.GetString(35); }
                if (!dr.IsDBNull(36)) { t.FechaFacturacion = dr.GetDateTime(36).ToString("yyyy-MM-dd"); }
                if (!dr.IsDBNull(37)) { t.HoraFacturacion = dr.GetTimeSpan(37).ToString(); }
                if (!dr.IsDBNull(38)) { t.OpFacturacion = dr.GetString(38); }
                if (!dr.IsDBNull(39)) { t.Observaciones = dr.GetString(39); }
                if (!dr.IsDBNull(40)) { t.Observaciones2 = dr.GetString(40); }
                if (!dr.IsDBNull(41)) { t.Observaciones3 = dr.GetString(41); }
                if (!dr.IsDBNull(42)) { t.FechaSapTicket = dr.GetDateTime(42).ToString("yyyy-MM-dd"); }
                if (!dr.IsDBNull(43)) { t.FechaRegistro = dr.GetDateTime(43).ToString("yyyy-MM-dd"); }
                if (!dr.IsDBNull(44)) { t.HoraRegistro = dr.GetTimeSpan(44).ToString(); }
                if (!dr.IsDBNull(45)) { t.Zona = dr.GetString(45); }
                if (!dr.IsDBNull(46)) { t.Notificado = dr.GetInt32(46); }
                if (!dr.IsDBNull(47)) { t.Visible = dr.GetString(47); }
                dr.Close();
                cn.Close();

                t.Det1 = obtenerDet1Ticket(DocEntry); if (t.Det1.Count == 0) { t.Det1 = null; }      //Datos de recojo
                t.Det2 = obtenerDet2Ticket(DocEntry); if (t.Det2.Count == 0) { t.Det2 = null; }     //Ordenes de venta
                t.Det3 = obtenerDet3Ticket(DocEntry); if (t.Det3.Count == 0) { t.Det3 = null; }       //Direcciones
                t.Det4 = obtenerDet4Ticket(DocEntry); if (t.Det4.Count == 0) { t.Det4 = null; }       //Notas de credito
                t.Det5 = obtenerDet5Ticket(DocEntry); if (t.Det5.Count == 0) { t.Det5 = null; }       // Regalos
                t.Det6 = obtenerDet6Ticket(DocEntry); if (t.Det6.Count == 0) { t.Det6 = null; }       // Pesos
                t.Det7 = obtenerDet7Ticket(DocEntry); if (t.Det7.Count == 0) { t.Det7 = null; }       // Tickets vinculados para reparto

            }
            catch (Exception e) { cn.Close(); throw new Exception(e.Message); }
            return t;
        }
        private List<RTV1_E> obtenerDet1Ticket(int DocEntry)
        {
            List<RTV1_E> lista = new List<RTV1_E>();
            string query = "select DocEntry,Linea,NombrePer,TelfPer,TipoDocPer,DocPer from vt.RTV1 where DocEntry=" + DocEntry + " order by Linea";
            SqlDataReader dr = db.ExecuteReaderNoSp(query, new List<string> { "@DocEntry" }, DocEntry);
            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    RTV1_E d = new RTV1_E();

                    if (!dr.IsDBNull(0)) { d.DocEntry = dr.GetInt32(0); }
                    if (!dr.IsDBNull(1)) { d.Linea = dr.GetInt32(1); }
                    if (!dr.IsDBNull(2)) { d.NombrePer = dr.GetString(2); }
                    if (!dr.IsDBNull(3)) { d.TelfPer = dr.GetString(3); }
                    if (!dr.IsDBNull(4)) { d.TipoDocPer = dr.GetString(4); }
                    if (!dr.IsDBNull(5)) { d.DocPer = dr.GetString(5); }
                    lista.Add(d);
                }
            }
            dr.Close();
            return lista;
        }
        private List<RTV2_E> obtenerDet2Ticket(int DocEntry)
        {
            List<RTV2_E> lista = new List<RTV2_E>();
            string query = "select * from vt.RTV2 WHERE DocEntry=@DocEntry order by Linea";
            try
            {
                SqlDataReader dr = db.ExecuteReaderNoSp(query, new List<string> { "@DocEntry" }, DocEntry);
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        RTV2_E o = new RTV2_E();

                        if (!dr.IsDBNull(0)) { o.DocEntry = dr.GetInt32(0); }
                        if (!dr.IsDBNull(1)) { o.Linea = dr.GetInt32(1); }
                        if (!dr.IsDBNull(2)) { o.Monto = dr.GetDecimal(2); }
                        if (!dr.IsDBNull(3)) { o.NroSap = dr.GetInt32(3); }
                        if (!dr.IsDBNull(4)) { o.TipoComprobante = dr.GetString(4); }
                        if (!dr.IsDBNull(5)) { o.Vendedor = dr.GetString(5); }
                        if (!dr.IsDBNull(6)) { o.LugarDeEntrega = dr.GetString(6); }
                        if (!dr.IsDBNull(7)) { o.Observaciones = dr.GetString(7); }
                        if (!dr.IsDBNull(8)) { o.AlmacenSalida = dr.GetString(8); }
                        lista.Add(o);
                    }
                }

                dr.Close();
            }
            catch (Exception e) { throw new Exception(e.Message); }


            return lista;
        }
        private List<RTV3_E> obtenerDet3Ticket(int DocEntry)
        {
            List<RTV3_E> lista = new List<RTV3_E>();
            string query = string.Empty;

            query = "select * from vt.RTV3 where DocEntry=@DocEntry order by IdDireccion";
            try
            {
                SqlDataReader dr = db.ExecuteReaderNoSp(query, new List<string> { "@DocEntry" }, DocEntry);
                while (dr.Read())
                {
                    RTV3_E d3 = new RTV3_E();

                    if (!dr.IsDBNull(0)) { d3.DocEntry = dr.GetInt32(0); }
                    if (!dr.IsDBNull(1)) { d3.IdDireccion = dr.GetInt32(1); }
                    if (!dr.IsDBNull(2)) { d3.Ubigeo = dr.GetInt32(2); }
                    if (!dr.IsDBNull(3)) { d3.Distrito = dr.GetString(3); }
                    if (!dr.IsDBNull(4)) { d3.Provincia = dr.GetString(4); }
                    if (!dr.IsDBNull(5)) { d3.Departamento = dr.GetString(5); }
                    if (!dr.IsDBNull(6)) { d3.Calle = dr.GetString(6); }

                    lista.Add(d3);
                }
                dr.Close();
            }
            catch { }


            return lista;
        }
        public List<RTV4_E> obtenerDet4Ticket(int DocEntry, int DocNum = 0)
        {
            List<RTV4_E> lista = new List<RTV4_E>();
            string query = string.Empty;
            if (DocNum == 0)
            {
                query = "select * from vt.RTV4 where DocEntry=@DocEntry order by Linea";
                try
                {
                    SqlDataReader dr = db.ExecuteReaderNoSp(query, new List<string> { "@DocEntry" }, DocEntry);
                    while (dr.Read())
                    {
                        RTV4_E d4 = new RTV4_E();
                        ORIN_E Nc = new ORIN_E();

                        if (!dr.IsDBNull(0)) { d4.DocEntry = dr.GetInt32(0); }
                        if (!dr.IsDBNull(1)) { d4.Linea = dr.GetInt32(1); }
                        if (!dr.IsDBNull(2)) { Nc.CardCode = dr.GetString(2); }
                        if (!dr.IsDBNull(3)) { Nc.CardName = dr.GetString(3); }
                        if (!dr.IsDBNull(4)) { Nc.DocTotal = dr.GetDecimal(4); }
                        if (!dr.IsDBNull(5)) { Nc.DocDate = dr.GetDateTime(5).ToString("yyyy-MM-dd"); }
                        if (!dr.IsDBNull(6)) { Nc.DocNum = dr.GetInt32(6); }
                        if (Nc != null) { d4.Nc = Nc; }

                        lista.Add(d4);
                    }
                    dr.Close();
                }
                catch { }
            }
            else if (DocNum > 0)
            {
                //DocNum de la N.C
                query = "select t0.* FROM vt.RTV4 t0 INNER JOIN vt.ORTV t1 on t1.DocEntry = t0.DocEntry AND t1.Estado not in ('ANULADO','CANCELADO') WHERE t0.DocNum = @DocNum order by t0.Linea";
                try
                {
                    SqlDataReader dr = db.ExecuteReaderNoSp(query, new List<string>() { "@DocNum" }, DocNum);
                    while (dr.Read())
                    {
                        RTV4_E d4 = new RTV4_E();
                        ORIN_E Nc = new ORIN_E();

                        if (!dr.IsDBNull(0)) { d4.DocEntry = dr.GetInt32(0); }
                        if (!dr.IsDBNull(1)) { d4.Linea = dr.GetInt32(1); }
                        if (!dr.IsDBNull(2)) { Nc.CardCode = dr.GetString(2); }
                        if (!dr.IsDBNull(3)) { Nc.CardName = dr.GetString(3); }
                        if (!dr.IsDBNull(4)) { Nc.DocTotal = dr.GetDecimal(4); }
                        if (!dr.IsDBNull(5)) { Nc.DocDate = dr.GetDateTime(5).ToString("yyyy-MM-dd"); }
                        if (!dr.IsDBNull(6)) { Nc.DocNum = dr.GetInt32(6); }
                        if (Nc != null) { d4.Nc = Nc; }

                        lista.Add(d4);
                    }
                    dr.Close();
                }
                catch { }
            }
            return lista;
        }
        private List<RTV5_E> obtenerDet5Ticket(int DocEntry)
        {
            List<RTV5_E> lista = new List<RTV5_E>();
            string query = "select * from vt.RTV5 where DocEntry=@DocEntry order by Linea";
            try
            {
                SqlDataReader dr = db.ExecuteReaderNoSp(query, new List<string> { "@DocEntry" }, DocEntry);
                while (dr.Read())
                {
                    RTV5_E d2 = new RTV5_E();

                    if (!dr.IsDBNull(0)) { d2.DocEntry = dr.GetInt32(0); }
                    if (!dr.IsDBNull(1)) { d2.Linea = dr.GetInt32(1); }
                    if (!dr.IsDBNull(2)) { d2.IdReg = dr.GetInt32(2); }
                    if (!dr.IsDBNull(3)) { d2.RegTipo = dr.GetString(3); }
                    if (!dr.IsDBNull(4)) { d2.RegCate = dr.GetString(4); }
                    if (!dr.IsDBNull(5)) { d2.RegCant = dr.GetDecimal(5); }
                    if (!dr.IsDBNull(6)) { d2.RegEstado = dr.GetString(6); }

                    lista.Add(d2);
                }
                dr.Close();
            }
            catch { }

            return lista;
        }
        private List<RTV6_E> obtenerDet6Ticket(int DocEntry)
        {
            List<RTV6_E> lista = new List<RTV6_E>();
            string query = "select DocEntry,Linea,Peso,UniMed,PrecioEnv from vt.RTV6 where DocEntry=@DocEntry order by Linea";
            try
            {
                SqlDataReader dr = db.ExecuteReaderNoSp(query, new List<string> { "@DocEntry" }, DocEntry);
                while (dr.Read())
                {
                    RTV6_E bean = new RTV6_E();

                    if (!dr.IsDBNull(0)) { bean.DocEntry = dr.GetInt32(0); }
                    if (!dr.IsDBNull(1)) { bean.Linea = dr.GetInt32(1); }
                    if (!dr.IsDBNull(2)) { bean.Peso = dr.GetDecimal(2); }
                    if (!dr.IsDBNull(3)) { bean.UniMed = dr.GetString(3); }
                    if (!dr.IsDBNull(4)) { bean.PrecioEnv = dr.GetDecimal(4); }

                    lista.Add(bean);
                }
                dr.Close();
            }
            catch (Exception e) { throw new Exception("Error al obtener Det6 " + e.Message, e); }
            return lista;
        }
        private List<RTV7_E> obtenerDet7Ticket(int DocEntry)
        {
            List<RTV7_E> lista = new List<RTV7_E>();
            string query = "select DocEntry,Linea,DocNumVinc,CardCode,CardName,MontoFinal from vt.RTV7 where DocEntry=@DocEntry order by Linea";
            try
            {
                SqlDataReader dr = db.ExecuteReaderNoSp(query, new List<string> { "@DocEntry" }, DocEntry);
                while (dr.Read())
                {
                    RTV7_E bean = new RTV7_E();

                    if (!dr.IsDBNull(0)) { bean.DocEntry = dr.GetInt32(0); }
                    if (!dr.IsDBNull(1)) { bean.Linea = dr.GetInt32(1); }
                    if (!dr.IsDBNull(2)) { bean.DocNumVinc = dr.GetInt32(2); }
                    if (!dr.IsDBNull(3)) { bean.CardCode = dr.GetString(3); }
                    if (!dr.IsDBNull(4)) { bean.CardName = dr.GetString(4); }
                    if (!dr.IsDBNull(5)) { bean.MontoFinal = dr.GetDecimal(5); }

                    lista.Add(bean);
                }
                dr.Close();
            }
            catch (Exception e) { throw new Exception("Error al obtener Det7 " + e.Message, e); }
            return lista;
        }
        //metodos que cubren los diferentes estados del ticket
        //SEPARADO
        //ABIERTO
        //RECIBIDO
        //PICKEANDO
        //PICKEADO
        //VERIFICANDO
        //VERIFICADO
        //EMPACANDO
        //EMPACADO
        public ORTV_E separarTicket(Usuario_E u)
        {
            //tipo mantenimiento AS(add separo) transaccion tipo A(add)
            ORTV_E t = new ORTV_E();
            SqlConnection cn = new SqlConnection(uti.cadSql);
            try
            {
                cn.Open();
                SqlTransaction tran = cn.BeginTransaction("TR01");
                try
                {
                    SqlCommand cmd = new SqlCommand("vt.MANT_ORTV", cn);
                    cmd.Transaction = tran;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@TipoMantenimiento", "AS");
                    cmd.Parameters.AddWithValue("@DocEntry", 0).Direction = ParameterDirection.Output;
                    cmd.Parameters.AddWithValue("@DocNum", 0).Direction = ParameterDirection.Output;
                    cmd.Parameters.AddWithValue("@CodSapVendedor", u.CodigoSap);
                    cmd.Parameters.AddWithValue("@Vendedor ", u.Nombres + " " + u.Apellidos);
                    cmd.ExecuteNonQuery();
                    t.DocEntry = int.Parse(cmd.Parameters["@DocEntry"].Value.ToString());
                    t.DocNum = int.Parse(cmd.Parameters["@DocNum"].Value.ToString());
                    t.Estado = "SEPARADO";
                    t.Vendedor = u.Nombres + " " + u.Apellidos;
                    t.CodSapVendedor = u.DocEntry;

                    SqlCommand cmd2 = new SqlCommand("dbo.POST_TRANSACCIONES", cn);
                    cmd2.Transaction = tran;
                    cmd2.CommandType = CommandType.StoredProcedure;
                    cmd2.Parameters.AddWithValue("@Tipo", "A");
                    cmd2.Parameters.AddWithValue("@Tabla", "ORTV");
                    cmd2.Parameters.AddWithValue("@DocNum", cmd.Parameters["@DocNum"].Value);
                    cmd2.Parameters.AddWithValue("@DocEntry", cmd.Parameters["@DocEntry"].Value);
                    cmd2.ExecuteNonQuery();
                    tran.Commit();
                }
                catch (Exception e1) { tran.Rollback(); cn.Close(); throw new Exception("Error en creacion: " + e1.Message); }
                cn.Close();
            }
            catch (Exception e2) { cn.Close(); throw new Exception("Error en creacion y conexion: " + e2.Message); }
            return t;
        }
        public int registrarTicket(ORTV_E ticket)
        {
            int status = -1; string ZonaTk = string.Empty;
            SqlConnection cn = new SqlConnection(uti.cadSql);
            try
            {
                cn.Open();
                SqlTransaction tran = cn.BeginTransaction();
                try
                {
                    switch (ticket.LugarDestino)
                    {
                        case "Agencia":
                            ZonaTk = "AGENCIA";
                            break;
                        case "Agencia Courier":
                            ZonaTk = "AGENCIA";
                            break;
                        case "Centro":
                            ZonaTk = "CONO CENTRO";
                            break;
                        case "Arriola":
                            ZonaTk = "ARRIOLA";
                            break;
                        case "Domicilio":
                            ZonaTk = ticket.Zona;
                            break;
                    }

                    SqlCommand cmd = new SqlCommand("vt.MANT_ORTV", cn);
                    cmd.Transaction = tran;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@TipoMantenimiento", "UR");
                    cmd.Parameters.AddWithValue("@DocEntry", ticket.DocEntry).Direction = ParameterDirection.InputOutput;
                    cmd.Parameters.AddWithValue("@DocNum", ticket.DocNum).Direction = ParameterDirection.Output;
                    cmd.Parameters.AddWithValue("@FechaSapTicket", ticket.FechaSapTicket);
                    cmd.Parameters.AddWithValue("@CardCode", ticket.CardCode);
                    cmd.Parameters.AddWithValue("@CardName", ticket.CardName);
                    cmd.Parameters.AddWithValue("@TipoVenta", ticket.TipoVenta);
                    cmd.Parameters.AddWithValue("@LugarDestino", ticket.LugarDestino);
                    cmd.Parameters.AddWithValue("@EnvioAgencia", ticket.EnvioAgencia);
                    cmd.Parameters.AddWithValue("@Referencia", ticket.Referencia);
                    cmd.Parameters.AddWithValue("@Agencia", ticket.Agencia);
                    cmd.Parameters.AddWithValue("@DirDestino", ticket.DirDestino);
                    cmd.Parameters.AddWithValue("@Operario", ticket.Vendedor);
                    cmd.Parameters.AddWithValue("@Estado", ticket.Estado);
                    cmd.Parameters.AddWithValue("@Observaciones", ticket.Observaciones);
                    cmd.Parameters.AddWithValue("@Embalaje", ticket.Embalaje);
                    cmd.Parameters.AddWithValue("@Comentario", ticket.Comentario);
                    cmd.Parameters.AddWithValue("@MontoTotal", ticket.MontoTotal);
                    cmd.Parameters.AddWithValue("@Flete", ticket.Flete);
                    cmd.Parameters.AddWithValue("@GastoEnvio", ticket.GastoEnvio);
                    cmd.Parameters.AddWithValue("@DescuentoNC", ticket.DescuentoNC);
                    cmd.Parameters.AddWithValue("@DeudaCliente", ticket.DeudaCliente);
                    cmd.Parameters.AddWithValue("@DeudaEmpresa", ticket.DeudaEmpresa);
                    cmd.Parameters.AddWithValue("@MontoFinal", ticket.MontoFinal);
                    cmd.Parameters.AddWithValue("@Observaciones2", ticket.Observaciones2);
                    cmd.Parameters.AddWithValue("@Observaciones3", ticket.Observaciones3);
                    cmd.Parameters.AddWithValue("@FormaPago", ticket.FormaPago);
                    cmd.Parameters.AddWithValue("@Zona", ZonaTk);
                    cmd.Parameters.AddWithValue("@FechaNC", ticket.FechaNC);
                    cmd.Parameters.AddWithValue("@Visible", ticket.Visible);
                    if (ticket.TiempoEntrega != null)
                    {
                        cmd.Parameters.AddWithValue("@TiempoEntrega", ticket.TiempoEntrega);
                    }

                    if (ticket.Det1 != null && ticket.Det1.Count >= 1)
                    {
                        bool enviarDatosRTV1 = false;

                        if (!string.IsNullOrEmpty(ticket.Det1[0].NombrePer) && (ticket.LugarDestino.Equals("Centro") || ticket.LugarDestino.Equals("Arriola")))
                        {
                            enviarDatosRTV1 = true;
                        }
                        //revisar
                        else if (!string.IsNullOrEmpty(ticket.Det1[0].NombrePer) && (!ticket.LugarDestino.Equals("Centro") && !ticket.LugarDestino.Equals("Arriola")))
                        {
                            enviarDatosRTV1 = true;
                        }

                        if (enviarDatosRTV1)
                        {
                            SqlParameter tbDet = new SqlParameter("@TPRTV1", SqlDbType.Structured);
                            tbDet.Value = RTV1_E.tbDetalle(ticket.Det1, ticket.DocEntry);
                            tbDet.TypeName = "vt.TPRTV1";
                            cmd.Parameters.AddWithValue("@TPRTV1", tbDet.Value);
                        }
                    }

                    // datos de ordenes de venta
                    if (ticket.Det2 != null && ticket.Det2.Count >= 1)
                    {
                        SqlParameter tbDet2 = new SqlParameter("@TPRTV2", SqlDbType.Structured);
                        tbDet2.Value = RTV2_E.tbDetalle(ticket.Det2, ticket.DocEntry);
                        tbDet2.TypeName = "vt.TPRTV2";
                        cmd.Parameters.AddWithValue("@TPRTV2", tbDet2.Value);
                    }

                    // datos de direcciones
                    if (ticket.Det3 != null && ticket.Det3.Count >= 1)
                    {
                        SqlParameter tbDet3 = new SqlParameter("@TPRTV3", SqlDbType.Structured);
                        tbDet3.Value = RTV3_E.tbDetalle(ticket.Det3, ticket.DocEntry);
                        tbDet3.TypeName = "vt.TPRTV3";
                        cmd.Parameters.AddWithValue("@TPRTV3", tbDet3.Value);
                    }
                    // datos de notas de credito
                    if (ticket.Det4 != null && ticket.Det4.Count >= 1)
                    {
                        SqlParameter tbDet4 = new SqlParameter("@TPRTV4", SqlDbType.Structured);
                        tbDet4.Value = RTV4_E.tbDetalle(ticket.Det4, ticket);
                        tbDet4.TypeName = "vt.TPRTV4";
                        cmd.Parameters.AddWithValue("@TPRTV4", tbDet4.Value);
                    }

                    // Regalos
                    if (ticket.Det5 != null && ticket.Det5.Count >= 1)
                    {
                        if (!string.IsNullOrEmpty(ticket.Det5[0].RegCate) && !string.IsNullOrEmpty(ticket.Det5[0].RegTipo) && ticket.Det5[0].RegCant > 0)
                        {
                            ticket.Det5[0].RegEstado = "Pendiente";
                            SqlParameter tbDet5 = new SqlParameter("@TPRTV5", SqlDbType.Structured);
                            tbDet5.Value = RTV5_E.tbDetalle(ticket.Det5, ticket.DocEntry);
                            tbDet5.TypeName = "vt.TPRTV5";
                            cmd.Parameters.AddWithValue("@TPRTV5", tbDet5.Value);
                        }
                    }
                    //si el ticket tiene vinculacion "SI" en su campo Observaciones2 se manda los datos de RTV7
                    if (ticket.Observaciones2 == "SI" && ticket.Det7 != null && ticket.Det7.Count >= 1)
                    {
                        if (!string.IsNullOrEmpty(ticket.Det7[0].CardName) && ticket.Det7[0].MontoFinal > 0 && ticket.Det7[0].DocNumVinc > 0)
                        {
                            // ticket.Det7.ForEach(detalle => detalle.OpVinculación = ticket.Vendedor);
                            SqlParameter tbDet7 = new SqlParameter("@TPRTV7", SqlDbType.Structured);
                            tbDet7.Value = RTV7_E.GenerarDataTable(ticket.Det7, ticket.DocEntry);
                            tbDet7.TypeName = "vt.TPRTV7";
                            cmd.Parameters.AddWithValue("@TPRTV7", tbDet7.Value);
                        }
                    }

                    if (ticket.GastoEnvio > 0)
                    {
                        cmd.Parameters.AddWithValue("@EstadoGasto", "PENDIENTE");
                    }
                    cmd.ExecuteNonQuery();
                    status = ticket.DocNum;

                    if (ticket.CardCode != null && ticket.CardName != null)
                    {
                        SqlCommand cmd2 = new SqlCommand("vt.MANT_OLDS", cn);
                        cmd2.Transaction = tran;
                        cmd2.CommandType = CommandType.StoredProcedure;
                        cmd2.Parameters.AddWithValue("@TipoMantenimiento", "AC");
                        cmd2.Parameters.AddWithValue("@C_CardCode", ticket.CardCode);
                        cmd2.Parameters.AddWithValue("@C_CardName", ticket.CardName);
                        cmd2.ExecuteNonQuery();
                        if (ticket.DeudaCliente > 0)
                        {
                            SqlCommand cmd4 = new SqlCommand("vt.MANT_OLDS", cn);
                            cmd4.Transaction = tran;
                            cmd4.CommandType = CommandType.StoredProcedure;
                            cmd4.Parameters.AddWithValue("@TipoMantenimiento", "AD");
                            cmd4.Parameters.AddWithValue("@C_CardCode", ticket.CardCode);
                            cmd4.Parameters.AddWithValue("@FechaOpe", ticket.FechaSapTicket);
                            cmd4.Parameters.AddWithValue("@Operacion", "VENTA");
                            cmd4.Parameters.AddWithValue("@DetOpe", "VENTA DeudaCliente, ticket:" + ticket.DocNum + " MR:" + ticket.MontoFinal);
                            cmd4.Parameters.AddWithValue("@Ingreso", ticket.DeudaCliente);
                            cmd4.Parameters.AddWithValue("@OperarioRegistro", ticket.OpRegistro);
                            cmd4.ExecuteNonQuery();
                        }
                        if (ticket.DeudaEmpresa > 0)
                        {
                            SqlCommand cmd5 = new SqlCommand("vt.MANT_OLDS", cn);
                            cmd5.Transaction = tran;
                            cmd5.CommandType = CommandType.StoredProcedure;
                            cmd5.Parameters.AddWithValue("@TipoMantenimiento", "AD");
                            cmd5.Parameters.AddWithValue("@C_CardCode", ticket.CardCode);
                            cmd5.Parameters.AddWithValue("@FechaOpe", ticket.FechaSapTicket);
                            cmd5.Parameters.AddWithValue("@Operacion", "VENTA");
                            cmd5.Parameters.AddWithValue("@DetOpe", "SALIDASALDO DeudaEmpresa, ticket:" + ticket.DocNum + " MR:" + ticket.MontoFinal);
                            cmd5.Parameters.AddWithValue("@Egreso", ticket.DeudaEmpresa);
                            cmd5.Parameters.AddWithValue("@OperarioRegistro", ticket.OpRegistro);
                            cmd5.ExecuteNonQuery();
                        }
                    }
                    //regalos
                    if (ticket.Det5 != null && ticket.Det5.Count >= 1)
                    {
                        if (ticket.Det5[0].IdReg > 0)
                        {
                            OREG_D oregD = new OREG_D();
                            oregD.CompromisosStock(ticket);
                        }
                    }
                   
                    tran.Commit();
                    //FUTURA CONEXION A SOPHOS SERVER PARA TRAER DATOS DE UN TICKET.
                    /*using (SqlConnection cnSophos = new SqlConnection(uti.cadSophos))
                    {
                        cnSophos.Open();
                        Console.WriteLine("Conexión exitosa.");
                        cnSophos.Close();
                    }*/
                }
                catch (Exception e1) { status = 0; tran.Rollback(); cn.Close(); throw new Exception("Error en registro: " + e1.Message); }
                cn.Close();
            }
            catch (Exception e2) { status = 0; cn.Close(); throw new Exception("Error en registro y conexion: " + e2.Message); }
            return status;
        }
        public int editarTicket(int DocEntry, ORTV_E ticket)
        {
            int status = -1; string ZonaTk = string.Empty;
            ORTV_E auxTK = obtenerTicket(DocEntry);
            SqlConnection cn = new SqlConnection(uti.cadSql);
            try
            {
                cn.Open();
                SqlTransaction tran = cn.BeginTransaction();
                try
                {
                    switch (ticket.LugarDestino)
                    {
                        case "Agencia":
                            ZonaTk = "AGENCIA";
                            break;
                        case "Agencia Courier":
                            ZonaTk = "AGENCIA";
                            break;
                        case "Centro":
                            ZonaTk = "CONO CENTRO";
                            break;
                        case "Arriola":
                            ZonaTk = "ARRIOLA";
                            break;
                        case "Domicilio":
                            ZonaTk = ticket.Zona;
                            break;
                    }

                    SqlCommand cmd = new SqlCommand("vt.MANT_ORTV", cn, tran);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@TipoMantenimiento", "UU");
                    cmd.Parameters.AddWithValue("@Estado", ticket.Estado);
                    cmd.Parameters.AddWithValue("@TipoVenta", ticket.TipoVenta);
                    cmd.Parameters.AddWithValue("@LugarDestino", ticket.LugarDestino);
                    cmd.Parameters.AddWithValue("@Agencia", ticket.Agencia);
                    cmd.Parameters.AddWithValue("@DirDestino", ticket.DirDestino);
                    cmd.Parameters.AddWithValue("@Embalaje", ticket.Embalaje);
                    cmd.Parameters.AddWithValue("@Comentario", ticket.Comentario);
                    cmd.Parameters.AddWithValue("@Flete", ticket.Flete);
                    cmd.Parameters.AddWithValue("@GastoEnvio", ticket.GastoEnvio);
                    cmd.Parameters.AddWithValue("@DescuentoNC", ticket.DescuentoNC);
                    cmd.Parameters.AddWithValue("@DeudaCliente", ticket.DeudaCliente);
                    cmd.Parameters.AddWithValue("@DeudaEmpresa", ticket.DeudaEmpresa);
                    cmd.Parameters.AddWithValue("@MontoFinal", ticket.MontoFinal);
                    cmd.Parameters.AddWithValue("@Observaciones", ticket.Observaciones);
                    cmd.Parameters.AddWithValue("@Observaciones2", ticket.Observaciones2);
                    cmd.Parameters.AddWithValue("@Observaciones3", ticket.Observaciones3);
                    cmd.Parameters.AddWithValue("@FormaPago", ticket.FormaPago);
                    cmd.Parameters.AddWithValue("@FechaNC", ticket.FechaNC);
                    if (ticket.TiempoEntrega != null)
                    {
                        cmd.Parameters.AddWithValue("@TiempoEntrega", ticket.TiempoEntrega);
                    }
                    cmd.Parameters.AddWithValue("@EstadoGasto", ticket.EstadoGasto);
                    cmd.Parameters.AddWithValue("@EnvioAgencia", ticket.EnvioAgencia);
                    cmd.Parameters.AddWithValue("@Referencia", ticket.Referencia);
                    cmd.Parameters.AddWithValue("@DocEntry", DocEntry);
                    cmd.Parameters.AddWithValue("@DocNum", ticket.DocNum);
                    cmd.Parameters.AddWithValue("@Operario", ticket.Vendedor);
                    cmd.Parameters.AddWithValue("@Zona", ZonaTk);
                    cmd.Parameters.AddWithValue("@Visible", ticket.Visible);

                    // datos de persona de recojo
                    if (ticket.Det1 != null && ticket.Det1.Count > 0)
                    {
                        SqlParameter tbDet = new SqlParameter("@TPRTV1", SqlDbType.Structured);
                        tbDet.Value = RTV1_E.tbDetalle(ticket.Det1, ticket.DocEntry);
                        tbDet.TypeName = "vt.TPRTV1";
                        cmd.Parameters.AddWithValue("@TPRTV1", tbDet.Value);
                    }
                    // datos de persona de recojo
                    if (ticket.Det2 != null && ticket.Det2.Count > 0)
                    {
                        SqlParameter tbDet2 = new SqlParameter("@TPRTV2", SqlDbType.Structured);
                        tbDet2.Value = RTV2_E.tbDetalle(ticket.Det2, ticket.DocEntry);
                        tbDet2.TypeName = "vt.TPRTV2";
                        cmd.Parameters.AddWithValue("@TPRTV2", tbDet2.Value);
                    }

                    // datos de direcciones
                    if (ticket.Det3 != null && ticket.Det3.Count > 0)
                    {
                        SqlParameter tbDet3 = new SqlParameter("@TPRTV3", SqlDbType.Structured);
                        tbDet3.Value = RTV3_E.tbDetalle(ticket.Det3, ticket.DocEntry);
                        tbDet3.TypeName = "vt.TPRTV3";
                        cmd.Parameters.AddWithValue("@TPRTV3", tbDet3.Value);
                    }

                    // Regalos
                    if (ticket.Det5 != null && ticket.Det5.Count > 0)
                    {
                        ticket.Det5[0].DocEntry = ticket.DocEntry;
                        ticket.Det5[0].RegEstado = "Pendiente";
                        SqlParameter tbDet5 = new SqlParameter("@TPRTV5", SqlDbType.Structured);
                        tbDet5.Value = RTV5_E.tbDetalle(ticket.Det5, ticket.DocEntry);
                        tbDet5.TypeName = "vt.TPRTV5";
                        cmd.Parameters.AddWithValue("@TPRTV5", tbDet5.Value);
                    }
                    //si el ticket tiene vinculacion "SI" en su campo Observaciones2 se manda los datos de RTV7
                    if (ticket.Observaciones2 == "SI" && ticket.Det7 != null && ticket.Det7.Count >= 1)
                    {
                        SqlParameter tbDet7 = new SqlParameter("@TPRTV7", SqlDbType.Structured);
                        tbDet7.Value = RTV7_E.GenerarDataTable(ticket.Det7, ticket.DocEntry);
                        tbDet7.TypeName = "vt.TPRTV7";
                        cmd.Parameters.AddWithValue("@TPRTV7", tbDet7.Value);
                    }
                    cmd.ExecuteNonQuery();
                    //status = Int32.Parse(cmd.Parameters["@DocNum"].Value.ToString());
                    status = ticket.DocNum;

                    tran.Commit();
                    cn.Close();
                }
                catch (Exception e) { status = 0; tran.Rollback(); cn.Close(); throw new Exception("Error en edicion: " + e.Message); }
            }
            catch (Exception e2) { status = 0; cn.Close(); throw new Exception("Error en edicion y conexion: " + e2.Message); }

            if (status >= 1)
            {
                OREG_D oregD = new OREG_D();

                if (auxTK.Det5 != null && auxTK.Det5.Count >= 1)
                {
                    if (auxTK.Det5[0].IdReg > 0)
                    {
                        auxTK.Det5[0].RegCant = -1 * auxTK.Det5[0].RegCant;
                        oregD.CompromisosStock(auxTK);
                    }
                }

                if (ticket.Det5 != null && ticket.Det5.Count >= 1)
                {
                    if (ticket.Det5[0].IdReg > 0)
                    {
                        oregD.CompromisosStock(ticket);
                    }
                }
            }

            return status;
        }
        public int editarVisibilidadTicket(int DocEntry)
        {
            SqlConnection cn = new SqlConnection(uti.cadSql);
            try
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand("UPDATE vt.ortv SET Visible='SI' where DocEntry=@DocEntry", cn);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@DocEntry", DocEntry);
                cmd.ExecuteNonQuery();
                cn.Close();
            }
            catch { }
            return DocEntry;
        }
        //metodo extraordinario para editar el ticket, *solo para supervisores de venta
        public void editarTicketSup(int DocEntry, ORTV_E ticket)
        {
            SqlConnection cn = new SqlConnection(uti.cadSql);
            try
            {
                string ZonaTk = string.Empty;
                cn.Open();

                switch (ticket.LugarDestino)
                {
                    case "Agencia":
                    case "Agencia Courier":
                        ZonaTk = "AGENCIA";
                        break;
                    case "Centro":
                        ZonaTk = "CONO CENTRO";
                        break;
                    case "Arriola":
                        ZonaTk = "ARRIOLA";
                        break;
                    case "Domicilio":
                        ZonaTk = ticket.Zona;
                        break;
                }

                using (SqlCommand cmd = new SqlCommand("vt.MANT_ORTV", cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@TipoMantenimiento", "USUPV");
                    cmd.Parameters.AddWithValue("@Estado", ticket.Estado);
                    cmd.Parameters.AddWithValue("@DocEntry", DocEntry);
                    cmd.Parameters.AddWithValue("@DocNum", ticket.DocNum);
                    cmd.Parameters.AddWithValue("@TipoVenta", ticket.TipoVenta);
                    cmd.Parameters.AddWithValue("@DirDestino", ticket.DirDestino);
                    cmd.Parameters.AddWithValue("@Zona", ZonaTk);
                    cmd.Parameters.AddWithValue("@FormaPago", ticket.FormaPago);
                    cmd.Parameters.AddWithValue("@Observaciones", ticket.Observaciones);
                    cmd.Parameters.AddWithValue("@Observaciones2", ticket.Observaciones2);
                    cmd.Parameters.AddWithValue("@Observaciones3", ticket.Observaciones3);
                    cmd.Parameters.AddWithValue("@Operario", ticket.OpRegistro);
                    cmd.Parameters.AddWithValue("@LugarDestino", ticket.LugarDestino);
                    cmd.Parameters.AddWithValue("@Referencia", ticket.Referencia);

                    if (ticket.LugarDestino.Equals("Agencia") || ticket.LugarDestino.Equals("Agencia Courier"))
                    {
                        cmd.Parameters.AddWithValue("@Agencia", ticket.Agencia);
                        cmd.Parameters.AddWithValue("@EnvioAgencia", ticket.EnvioAgencia);
                    }

                    if (ticket.Estado.Equals("ABIERTO"))
                    {
                        cmd.Parameters.AddWithValue("@TiempoEntrega", ticket.TiempoEntrega);
                    }

                    if (ticket.Det1 != null && ticket.Det1.Count >= 1)
                    {
                        SqlParameter tbDet1 = new SqlParameter("@TPRTV1", SqlDbType.Structured);
                        tbDet1.Value = RTV1_E.tbDetalle(ticket.Det1, ticket.DocEntry);
                        tbDet1.TypeName = "vt.TPRTV1";
                        cmd.Parameters.AddWithValue("@TPRTV1", tbDet1.Value);
                    }
                    if (ticket.Det2 != null && ticket.Det2.Count >= 1)
                    {
                        SqlParameter tbDet2 = new SqlParameter("@TPRTV2", SqlDbType.Structured);
                        tbDet2.Value = RTV2_E.tbDetalle(ticket.Det2, ticket.DocEntry);
                        tbDet2.TypeName = "vt.TPRTV2";
                        cmd.Parameters.AddWithValue("@TPRTV2", tbDet2.Value);
                    }
                    if (ticket.Det3 != null && ticket.Det3.Count >= 1)
                    {
                        SqlParameter tbDet3 = new SqlParameter("@TPRTV3", SqlDbType.Structured);
                        tbDet3.Value = RTV3_E.tbDetalle(ticket.Det3, ticket.DocEntry);
                        tbDet3.TypeName = "vt.TPRTV3";
                        cmd.Parameters.AddWithValue("@TPRTV3", tbDet3.Value);
                    }
                    // Si el ticket tiene vinculación "SI" en su campo Observaciones2 se mandan los datos de RTV7
                    if (ticket.Observaciones2 == "SI" && ticket.Det7 != null && ticket.Det7.Count >= 1)
                    {
                        SqlParameter tbDet7 = new SqlParameter("@TPRTV7", SqlDbType.Structured);
                        tbDet7.Value = RTV7_E.GenerarDataTable(ticket.Det7, ticket.DocEntry);
                        tbDet7.TypeName = "vt.TPRTV7";
                        cmd.Parameters.AddWithValue("@TPRTV7", tbDet7.Value);
                    }

                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception e)
            {
                throw new Exception("Error en edición: " + e.Message);
            }
        }
        public int pagarTicket(int DocEntry, ORTV_E ticket)
        {   // tipo UPG update pago
            int status = -1;
            ORTV_E auxTK = obtenerTicket(DocEntry);
            SqlConnection cn = new SqlConnection(uti.cadSql);
            try
            {
                cn.Open();
                SqlTransaction tran = cn.BeginTransaction("transaccion1");
                try
                {
                    SqlCommand cmd = new SqlCommand("vt.MANT_ORTV", cn);
                    cmd.Transaction = tran;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@TipoMantenimiento", "UPG");
                    cmd.Parameters.AddWithValue("@DocEntry", DocEntry);
                    cmd.Parameters.AddWithValue("@DocNum", ticket.DocNum);
                    cmd.Parameters.AddWithValue("@FormaPago", ticket.FormaPago);
                    cmd.Parameters.AddWithValue("@MontoRecibido", ticket.MontoRecibido);
                    cmd.Parameters.AddWithValue("@CodSapCajero", ticket.CodSapCajero);
                    cmd.Parameters.AddWithValue("@Cajero", ticket.Cajero);
                    cmd.Parameters.AddWithValue("@Estado", ticket.Estado);
                    cmd.ExecuteNonQuery();
                    status = ticket.DocNum;

                    if (auxTK.CardCode != null && auxTK.CardName != null)
                    {
                        SqlCommand cmd2 = new SqlCommand("vt.MANT_OLDS", cn);
                        cmd2.Transaction = tran;
                        cmd2.CommandType = CommandType.StoredProcedure;
                        cmd2.Parameters.AddWithValue("@TipoMantenimiento", "AC");           // AC: ADD CABECERA
                        cmd2.Parameters.AddWithValue("@C_CardCode", auxTK.CardCode);
                        cmd2.Parameters.AddWithValue("@C_CardName", auxTK.CardName);
                        cmd2.ExecuteNonQuery();

                        if (auxTK.GastoEnvio > 0)
                        {
                            SqlCommand cmd3 = new SqlCommand("vt.MANT_OLDS", cn);
                            cmd3.Transaction = tran;
                            cmd3.CommandType = CommandType.StoredProcedure;
                            cmd3.Parameters.AddWithValue("@TipoMantenimiento", "AD");           // AD: ADD DETALLES
                            cmd3.Parameters.AddWithValue("@C_CardCode", auxTK.CardCode);
                            cmd3.Parameters.AddWithValue("@FechaOpe", auxTK.FechaSapTicket);
                            cmd3.Parameters.AddWithValue("@Operacion", ticket.FormaPago);
                            cmd3.Parameters.AddWithValue("@DetOpe", ticket.FormaPago + " GastoEnvio, ticket:" + auxTK.DocNum + " MR:" + ticket.MontoRecibido);
                            cmd3.Parameters.AddWithValue("@Ingreso", auxTK.GastoEnvio);
                            cmd3.Parameters.AddWithValue("@OperarioRegistro", ticket.Cajero);
                            cmd3.ExecuteNonQuery();
                        }
                    }
                    tran.Commit();
                    cn.Close();
                }
                catch { status = 0; tran.Rollback(); cn.Close(); }
            }
            catch (Exception e) { status = 0; cn.Close(); throw new Exception("Error en pago: " + e.Message); }
            return status;
        }
        public int anularPagoTicket(int DocEntry)
        {   //TIPO MANT UAPG UPDATE ANULAR PAGO
            int status = -1;
            ORTV_E auxTK = obtenerTicket(DocEntry);
            SqlConnection cn = new SqlConnection(uti.cadSql);
            try
            {
                cn.Open();
                SqlTransaction tran = cn.BeginTransaction("transaccion1");
                try
                {
                    SqlCommand cmd = new SqlCommand("vt.MANT_ORTV", cn);
                    cmd.Transaction = tran;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@TipoMantenimiento", "UAPG");
                    cmd.Parameters.AddWithValue("@DocEntry", DocEntry);
                    cmd.Parameters.AddWithValue("@DocNum", auxTK.DocNum);
                    cmd.Parameters.AddWithValue("@Estado", auxTK.Estado);
                    cmd.ExecuteNonQuery();
                    //status = int.Parse(cmd.Parameters["@DocNum"].Value.ToString());
                    status = auxTK.DocNum;

                    if (auxTK.CardCode != null && auxTK.CardName != null)
                    {
                        SqlCommand cmd2 = new SqlCommand("vt.MANT_OLDS", cn);
                        cmd2.Transaction = tran;
                        cmd2.CommandType = CommandType.StoredProcedure;
                        cmd2.Parameters.AddWithValue("@TipoMantenimiento", "AC");       // ADD CABECERA
                        cmd2.Parameters.AddWithValue("@C_CardCode", auxTK.CardCode);
                        cmd2.Parameters.AddWithValue("@C_CardName", auxTK.CardName);
                        cmd2.ExecuteNonQuery();

                        if (auxTK.GastoEnvio > 0)
                        {
                            SqlCommand cmd3 = new SqlCommand("vt.MANT_OLDS", cn);
                            cmd3.Transaction = tran;
                            cmd3.CommandType = CommandType.StoredProcedure;
                            cmd3.Parameters.AddWithValue("@TipoMantenimiento", "AD");
                            cmd3.Parameters.AddWithValue("@C_CardCode", auxTK.CardCode);
                            cmd3.Parameters.AddWithValue("@FechaOpe", auxTK.FechaPago);
                            cmd3.Parameters.AddWithValue("@Operacion", "ANULACIONPAGO");
                            cmd3.Parameters.AddWithValue("@DetOpe", "ANULACIONPAGO GastoEnvio, ticket:" + auxTK.DocNum + " MR:" + auxTK.MontoRecibido);
                            cmd3.Parameters.AddWithValue("@Egreso", auxTK.GastoEnvio);
                            cmd3.Parameters.AddWithValue("@OperarioRegistro", auxTK.Vendedor);
                            cmd3.ExecuteNonQuery();
                        }
                    }
                    tran.Commit();
                    cn.Close();
                }
                catch { status = 0; tran.Rollback(); cn.Close(); }
            }
            catch (Exception e) { status = 0; cn.Close(); throw new Exception("Error de anularpago: " + e.Message); }
            return status;
        }
        public List<Rpt_TicketVenta_E> listarTicketsAgencia()
        {
            List<Rpt_TicketVenta_E> lista = new List<Rpt_TicketVenta_E>();
            //SqlConnection cn = new SqlConnection(uti.cadSql);
            try
            {
                //cn.Open();// SqlTransaction tran = cn.BeginTransaction("transaccion1");
                SqlDataReader dr = db.ExecuteReader("RptFormatoAgencia");
                while (dr.Read())
                {
                    Rpt_TicketVenta_E t = new Rpt_TicketVenta_E();
                    if (!dr.IsDBNull(0)) { t.Almacen = dr.GetString(0); }
                    if (!dr.IsDBNull(1)) { t.GastoEnvio = dr.GetDecimal(1); }
                    if (!dr.IsDBNull(2)) { t.DocEntry = dr.GetInt32(2); }
                    if (!dr.IsDBNull(3)) { t.Observaciones2 = dr.GetString(3); }
                    if (!dr.IsDBNull(4)) { t.CardName = dr.GetString(4); }
                    if (!dr.IsDBNull(5)) { t.NombrePer1 = dr.GetString(5); }
                    if (!dr.IsDBNull(6)) { t.DocPer1 = dr.GetString(6); }
                    if (!dr.IsDBNull(7)) { t.TelfPer1 = dr.GetString(7); }
                    if (!dr.IsDBNull(8)) { t.PropietarioDesc = dr.GetString(8); }
                    if (!dr.IsDBNull(9)) { t.Agencia = dr.GetString(9); }
                    if (!dr.IsDBNull(10)) { t.DirDestino1 = dr.GetString(10); }
                    if (!dr.IsDBNull(11)) { t.DirDestino2 = dr.GetString(11); }
                    if (!dr.IsDBNull(12)) { t.Cajas = dr.GetInt32(12); }

                    lista.Add(t);
                }
                dr.Close();

            }
            catch { }

            return lista;
        }
        //public int anularTicket(int DocEntry, ORTV_E ticket)
        //{   //tipo mant UAN
        //	int status = -1;
        //	ORTV_E auxTK = obtenerTicket(DocEntry);
        //	SqlConnection cn = new SqlConnection(uti.cadSql);
        //	try
        //	{
        //		cn.Open();
        //		SqlTransaction tran = cn.BeginTransaction("transaccion1");
        //		try
        //		{
        //			SqlCommand cmd = new SqlCommand("vt.MANT_ORTV", cn);
        //			cmd.Transaction = tran;
        //			cmd.CommandType = CommandType.StoredProcedure;
        //			cmd.Parameters.AddWithValue("@TipoMantenimiento", "UAN");
        //			cmd.Parameters.AddWithValue("@DocEntry", DocEntry);
        //			cmd.Parameters.AddWithValue("@Estado", ticket.Estado);
        //			cmd.Parameters.AddWithValue("@DocNum", ticket.DocNum);
        //			cmd.Parameters.AddWithValue("@Operario", ticket.OpRegistro);
        //			cmd.ExecuteNonQuery();
        //			status = ticket.DocNum;

        //			if (auxTK.CardCode != null && auxTK.CardName != null)
        //			{
        //				SqlCommand cmd2 = new SqlCommand("vt.MANT_OLDS", cn);
        //				cmd2.Transaction = tran;
        //				cmd2.CommandType = CommandType.StoredProcedure;
        //				cmd2.Parameters.AddWithValue("@TipoMantenimiento", "AC");
        //				cmd2.Parameters.AddWithValue("@C_CardCode", auxTK.CardCode);
        //				cmd2.Parameters.AddWithValue("@C_CardName", auxTK.CardName);
        //				cmd2.ExecuteNonQuery();
        //				if (auxTK.DeudaCliente > 0)
        //				{
        //					SqlCommand cmd4 = new SqlCommand("vt.MANT_OLDS", cn);
        //					cmd4.Transaction = tran;
        //					cmd4.CommandType = CommandType.StoredProcedure;
        //					cmd4.Parameters.AddWithValue("@TipoMantenimiento", "AD");
        //					cmd4.Parameters.AddWithValue("@C_CardCode", auxTK.CardCode);
        //					cmd4.Parameters.AddWithValue("@FechaOpe", auxTK.FechaRegistro);
        //					cmd4.Parameters.AddWithValue("@Operacion", "ANULACIONVENTA");
        //					cmd4.Parameters.AddWithValue("@DetOpe", "ANULACIONVENTA DeudaCliente, ticket:" + auxTK.DocNum + " MR:" + auxTK.MontoFinal);
        //					cmd4.Parameters.AddWithValue("@Egreso", auxTK.DeudaCliente);
        //					cmd4.Parameters.AddWithValue("@OperarioRegistro", ticket.OpRegistro);
        //					cmd4.ExecuteNonQuery();
        //				}
        //				if (auxTK.DeudaEmpresa > 0)
        //				{
        //					SqlCommand cmd5 = new SqlCommand("vt.MANT_OLDS", cn);
        //					cmd5.Transaction = tran;
        //					cmd5.CommandType = CommandType.StoredProcedure;
        //					cmd5.Parameters.AddWithValue("@TipoMantenimiento", "AD");
        //					cmd5.Parameters.AddWithValue("@C_CardCode", auxTK.CardCode);
        //					cmd5.Parameters.AddWithValue("@FechaOpe", auxTK.FechaRegistro);
        //					cmd5.Parameters.AddWithValue("@Operacion", "ANULACIONVENTA");
        //					cmd5.Parameters.AddWithValue("@DetOpe", "ANULACION-SALIDASALDO DeudaEmpresa, ticket:" + auxTK.DocNum + " MR:" + auxTK.MontoFinal);
        //					cmd5.Parameters.AddWithValue("@Ingreso", auxTK.DeudaEmpresa);
        //					cmd5.Parameters.AddWithValue("@OperarioRegistro", ticket.OpRegistro);
        //					cmd5.ExecuteNonQuery();
        //				}
        //			}

        //			tran.Commit();
        //			cn.Close();
        //		}
        //		catch { tran.Rollback(); cn.Close(); throw new Exception("error y anulacion"); }
        //	}
        //	catch (Exception e) { status = 0; cn.Close(); throw new Exception("Error en anulacion: " + e.Message); }

        //	if (status >= 1)
        //	{
        //		//regalos
        //		if (auxTK.Det5.Count >= 1)
        //		{
        //			if (auxTK.Det5[0].IdReg > 0 && auxTK.Det5[0].RegCant > 0)
        //			{
        //				OREG_D oregD = new OREG_D();
        //				auxTK.Det5[0].RegCant = -1 * auxTK.Det5[0].RegCant;
        //				oregD.CompromisosStock(auxTK);
        //			}

        //		}
        //	}

        //	return status;
        //}
        public int cancelarTicket(int DocEntry, string Estado, string Operario)
        {   //tipo mant UAN
            int status = -1;
            ORTV_E auxTK = obtenerTicket(DocEntry);
            SqlConnection cn = new SqlConnection(uti.cadSql);
            try
            {
                cn.Open();
                SqlTransaction tran = cn.BeginTransaction();
                try
                {
                    SqlCommand cmd = new SqlCommand("vt.MANT_ORTV", cn);
                    cmd.Transaction = tran;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@TipoMantenimiento", "UCA");           // update cancelar ticket
                    cmd.Parameters.AddWithValue("@DocEntry", DocEntry);
                    cmd.Parameters.AddWithValue("@DocNum", auxTK.DocNum);
                    cmd.Parameters.AddWithValue("@Operario", Operario);
                    cmd.Parameters.AddWithValue("@Estado", Estado);
                    cmd.ExecuteNonQuery();
                    status = auxTK.DocNum;

                    if (auxTK.CardCode != null && auxTK.CardName != null)
                    {
                        SqlCommand cmd2 = new SqlCommand("vt.MANT_OLDS", cn);
                        cmd2.Transaction = tran;
                        cmd2.CommandType = CommandType.StoredProcedure;
                        cmd2.Parameters.AddWithValue("@TipoMantenimiento", "AC");
                        cmd2.Parameters.AddWithValue("@C_CardCode", auxTK.CardCode);
                        cmd2.Parameters.AddWithValue("@C_CardName", auxTK.CardName);
                        cmd2.ExecuteNonQuery();
                        if (auxTK.DeudaCliente > 0)
                        {
                            SqlCommand cmd4 = new SqlCommand("vt.MANT_OLDS", cn);
                            cmd4.Transaction = tran;
                            cmd4.CommandType = CommandType.StoredProcedure;
                            cmd4.Parameters.AddWithValue("@TipoMantenimiento", "AD");
                            cmd4.Parameters.AddWithValue("@C_CardCode", auxTK.CardCode);
                            cmd4.Parameters.AddWithValue("@FechaOpe", auxTK.FechaSapTicket);
                            cmd4.Parameters.AddWithValue("@Operacion", "ANULACIONVENTA");
                            cmd4.Parameters.AddWithValue("@DetOpe", "ANULACIONVENTA DeudaCliente, ticket:" + auxTK.DocNum + " MR:" + auxTK.MontoFinal);
                            cmd4.Parameters.AddWithValue("@Egreso", auxTK.DeudaCliente);
                            cmd4.Parameters.AddWithValue("@OperarioRegistro", Operario);
                            cmd4.ExecuteNonQuery();
                        }
                        if (auxTK.DeudaEmpresa > 0)
                        {
                            SqlCommand cmd5 = new SqlCommand("vt.MANT_OLDS", cn);
                            cmd5.Transaction = tran;
                            cmd5.CommandType = CommandType.StoredProcedure;
                            cmd5.Parameters.AddWithValue("@TipoMantenimiento", "AD");
                            cmd5.Parameters.AddWithValue("@C_CardCode", auxTK.CardCode);
                            cmd5.Parameters.AddWithValue("@FechaOpe", auxTK.FechaSapTicket);
                            cmd5.Parameters.AddWithValue("@Operacion", "ANULACIONVENTA");
                            cmd5.Parameters.AddWithValue("@DetOpe", "ANULACION-SALIDASALDO DeudaEmpresa, ticket:" + auxTK.DocNum + " MR:" + auxTK.MontoFinal);
                            cmd5.Parameters.AddWithValue("@Ingreso", auxTK.DeudaEmpresa);
                            cmd5.Parameters.AddWithValue("@OperarioRegistro", Operario);
                            cmd5.ExecuteNonQuery();
                        }
                    }

                    tran.Commit();
                    cn.Close();
                }
                catch (Exception e1) { tran.Rollback(); cn.Close(); throw new Exception("error y anulacion: " + e1.Message); }
            }

            catch (Exception e2) { status = 0; cn.Close(); throw new Exception("Error en anulacion: " + e2.Message); }

            if (status >= 1)
            {
                //Operaciones para regalos
                if (auxTK.Det5 != null && auxTK.Det5.Count >= 1)
                {
                    if (auxTK.Det5[0].IdReg > 0 && auxTK.Det5[0].RegCant > 0)
                    {
                        OREG_D oregD = new OREG_D();
                        auxTK.Det5[0].RegCant = -1 * auxTK.Det5[0].RegCant;
                        oregD.CompromisosStock(auxTK);
                    }
                }
            }

            return status;
        }
        /*
		 * Descripción: Método para editar el estado del ticket desde el view SeguimientoDeTicket
		 * Parámetros: (int) @docEntry, (string) @tipoMantenimiento
		 * Usos: SeguimientoDeTicket
		 */
        public int EditarTicketDesdeSeguimiento(Dictionary<string, Object> datos)
        {
            int docNum = -1;

            using (SqlConnection cn = new SqlConnection(uti.cadSql))
            {
                SqlCommand cmd = new SqlCommand("vt.MANT_ORTV", cn) { CommandType = CommandType.StoredProcedure };
                cmd.Parameters.AddWithValue("@TipoMantenimiento", datos["tipoMantenimiento"]);
                cmd.Parameters.AddWithValue("@DocEntry", datos["docEntryTicket"]);
                cmd.Parameters.AddWithValue("@DocNum", datos["docNumTicket"]);
                cmd.Parameters.AddWithValue("@Estado", datos["estadoTicket"]);
                if (datos["tipoMantenimiento"].ToString() != "UDEMP")
                {
                    cmd.Parameters.AddWithValue("@Operario", datos["opRegistro"]);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@NroMesa", datos["NroMesa"]);
                    cmd.Parameters.AddWithValue("@Cajas", datos["Cajas"]);
                }
                /*se añadio para anular entregado diferente a Agencia*/
                if (datos["tipoMantenimiento"].Equals("USAT"))
                {
                    cmd.Parameters.AddWithValue("@LugarDestino", datos["LugarDestino"]);
                }

                cn.Open();
                SqlTransaction tran = null;

                try
                {
                    tran = cn.BeginTransaction();
                    cmd.Transaction = tran;
                    cmd.ExecuteNonQuery();
                    tran.Commit();
                    docNum = Convert.ToInt32(datos["docNumTicket"]);
                }
                catch (Exception ex)
                {
                    tran?.Rollback();
                    throw new Exception("Error al editar en estado: " + datos["estadoTicket"] + " " + ex.Message);
                }

                cn.Close();
            }

            return docNum;
        }
        public int editarSeguimientoTicket(string Estado, int DocEntry, ORTV_E ticket)
        {
            int status = -1;
            bool gestionStock = false;
            string TipoMantenimiento = string.Empty;

            ORTV_E auxTK = obtenerTicket(DocEntry);
            if (Estado.Equals("RECIBIDO"))
            { TipoMantenimiento = "USRE"; }                     // update seguimiento recibir
            else if (Estado.Equals("ANULARRECIBIDO"))
            { TipoMantenimiento = "USAR"; }                     // update seguimiento anular recibir
            else if (Estado.Equals("INICIO PICKING"))
            { TipoMantenimiento = "UISSA"; }                    // update seguimiento inicio picking
            else if (Estado.Equals("ANULAR INICIO PICKING"))
            { TipoMantenimiento = "UISAS"; }                    // update seguimiento anular inicio picking
            else if (Estado.Equals("FIN PICKING"))
            { TipoMantenimiento = "USSA"; }                     // update seguimiento fin picking
            else if (Estado.Equals("ANULAR FIN PICKING"))
            { TipoMantenimiento = "USAS"; }                     // update seguimiento anular fin picking
            else if (Estado.Equals("INICIO VERIFICAR"))
            { TipoMantenimiento = "UISVE"; }                     // update seguimiento iniciar verificado
            else if (Estado.Equals("ANULAR INICIO VERIFICAR"))
            { TipoMantenimiento = "UISAV"; }                    // update seguimiento anular iniciar verificado
            else if (Estado.Equals("FIN VERIFICAR"))
            { TipoMantenimiento = "USVE"; }                     // update seguimiento fin verificado
            else if (Estado.Equals("ANULAR FIN VERIFICAR"))
            { TipoMantenimiento = "USAV"; }                     // update seguimiento anular fin verificado
            else if (Estado.Equals("INICIO EMPACAR"))
            { TipoMantenimiento = "UISEM"; }                   // update seguimiento iniciar empacado
            else if (Estado.Equals("ANULAR INICIO EMPACAR"))
            { TipoMantenimiento = "UISAE"; }                    // update seguimiento anular iniciar empacado
            else if (Estado.Equals("FIN EMPACAR"))
            {
                TipoMantenimiento = "USEM";                        // update seguimiento fin empacado
                if (ticket.Det5 != null && ticket.Det5.Count >= 1 && ticket.LugarDestino == "Agencia") { ticket.Det5[0].RegEstado = "Entregado"; }
            }
            else if (Estado.Equals("ANULAR FIN EMPACAR"))
            { TipoMantenimiento = "USAE"; }                     // update seguimiento anular fin empacado
            else if (Estado.Equals("PESADO"))
            { TipoMantenimiento = "USPE"; }                     // update seguimiento pesado
            else if (Estado.Equals("ANULARPESADO"))
            { TipoMantenimiento = "USAP"; }                     // update seguimiento anular pesado
            else if (Estado.Equals("ENVIADO"))
            { TipoMantenimiento = "USEN"; }                     // update seguimiento enviado
            else if (Estado.Equals("ANULARENVIADO"))
            { TipoMantenimiento = "USAN"; }                     // update seguimiento anular enviado
            else if (Estado.Equals("ENTREGADO"))
            { TipoMantenimiento = "USET"; }                      // update seguimiento entregado
            else if (Estado.Equals("ANULARENTREGADO"))
            { TipoMantenimiento = "USAT"; }                     // update seguimiento anular entregado

            SqlConnection cn = new SqlConnection(uti.cadSql);
            cn.Open();

            try
            {
                SqlTransaction tran = cn.BeginTransaction();
                try
                {
                    SqlCommand cmd = new SqlCommand("vt.MANT_ORTV", cn, tran) { CommandType = CommandType.StoredProcedure };
                    cmd.Parameters.AddWithValue("@TipoMantenimiento", TipoMantenimiento);
                    cmd.Parameters.AddWithValue("@DocEntry", DocEntry);
                    cmd.Parameters.AddWithValue("@DocNum", ticket.DocNum);
                    cmd.Parameters.AddWithValue("@Estado", ticket.Estado);
                    cmd.Parameters.AddWithValue("@Operario", ticket.OpRegistro);

                    // Detalles de OpSacando cuando existe más de 1 sacador
                    if (TipoMantenimiento == "USSA")
                    {
                        if (ticket.Det11 != null && ticket.Det11.Count > 1 && !string.IsNullOrEmpty(ticket.Det11[1].Operario))
                        {
                            ticket.Det11.RemoveAt(0);                           // Eliminamos el primer elemento porque este ya fue guardado en CC_ORTV
                            cmd.Parameters.AddWithValue("@MasOperariosSac", 1);
                            SqlParameter tbDet11 = new SqlParameter("@TPRTV11", SqlDbType.Structured);
                            tbDet11.Value = RTV11_E.tbDet11(ticket.Det11, ticket.DocEntry);
                            tbDet11.TypeName = "vt.TPRTV11";
                            cmd.Parameters.AddWithValue("@TPRTV11", tbDet11.Value);
                        }
                    }
                    if (TipoMantenimiento == "UISVE")
                    {
                        string EstadoNuevo = "PICKEANDO";
                        var UltimaOperacion = ccTicket.ListarCC_ORTV(ticket.DocEntry, null, true).FirstOrDefault().Operacion;
                        if (UltimaOperacion == "FIN PICKING") { EstadoNuevo = "VERIFICANDO"; }
                        cmd.Parameters.AddWithValue("@EstadoNuevo", EstadoNuevo);
                    }
                    if (TipoMantenimiento.Equals("USVE"))
                    {
                        // Verificadores de apoyo
                        if (ticket.Det12 != null && ticket.Det12.Count > 1 && !String.IsNullOrEmpty(ticket.Det12[1].Operario))
                        {
                            ticket.Det12.RemoveAt(0);
                            cmd.Parameters.AddWithValue("@MasOperariosChe", 1);
                            SqlParameter tbDet12 = new SqlParameter("@TPRTV12", SqlDbType.Structured);
                            tbDet12.Value = RTV12_E.tbDet12(ticket.Det12, ticket.DocEntry);
                            tbDet12.TypeName = "vt.TPRTV12";
                            cmd.Parameters.AddWithValue("@TPRTV12", tbDet12.Value);
                        }
                    }
                    if (TipoMantenimiento == "UISEM")
                    {
                        string EstadoNuevo = auxTK.Estado;
                        var UltimaOperacion = ccTicket.ListarCC_ORTV(ticket.DocEntry, null, true).FirstOrDefault().Operacion;
                        if (UltimaOperacion == "FIN VERIFICAR") { EstadoNuevo = "EMPACANDO"; }
                        if (UltimaOperacion == "FIN PICKING") { EstadoNuevo = "VERIFICANDO"; }
                        cmd.Parameters.AddWithValue("@EstadoNuevo", EstadoNuevo);
                    }

                    if (TipoMantenimiento.Equals("USAE"))
                    {
                        string EstadoNuevo = string.Empty;
                        if (!string.IsNullOrEmpty(ticket.Operario) && ticket.Operario == "07")
                        {
                            EstadoNuevo = "PICKEANDO";
                        }
                        else { EstadoNuevo = "EMPACANDO"; }
                        cmd.Parameters.AddWithValue("@EstadoNuevo", EstadoNuevo);
                        if (!string.IsNullOrEmpty(ticket.Operario) && ticket.Operario == "07")
                        { cmd.Parameters.AddWithValue("@Almacen", ticket.Operario); }
                    }

                    //  Párametros enviados solo cuando Empacamos ticket
                    if (TipoMantenimiento.Equals("USEM"))
                    { // update seguimiento empacado
                        cmd.Parameters.AddWithValue("@Cajas", ticket.Cajas);
                        cmd.Parameters.AddWithValue("@NroMesa", ticket.NroMesa);
                        cmd.Parameters.AddWithValue("@AlmProcedencia", ticket.AlmProcedencia);
                        if (!string.IsNullOrEmpty(ticket.Operario) &&  ticket.Operario == "07")
                        { cmd.Parameters.AddWithValue("@Almacen", ticket.Operario); }

                        if (ticket.Det13 != null && ticket.Det13.Count > 1)
                        {
                            if (!string.IsNullOrEmpty(ticket.Det13[1].Operario))
                            {
                                // Empacadores de apoyo
                                ticket.Det13.RemoveAt(0);                           // Eliminamos el primer elemento porque este ya fue guardado en CC_ORTV

                                cmd.Parameters.AddWithValue("@MasOperariosEmp", 1);
                                SqlParameter tbDet13 = new SqlParameter("@TPRTV13", SqlDbType.Structured);
                                tbDet13.Value = RTV13_E.tbDet13(ticket.Det13, ticket.DocEntry);
                                tbDet13.TypeName = "vt.TPRTV13";
                                cmd.Parameters.AddWithValue("@TPRTV13", tbDet13.Value);
                            }
                        }
                    }


                    // datos de pesos
                    if (TipoMantenimiento.Equals("USPE") && ticket.Det6 != null && ticket.Det6.Count > 0)
                    {
                        SqlParameter tbDet6 = new SqlParameter("@TPRTV6", SqlDbType.Structured);
                        tbDet6.Value = RTV6_E.tbDetalle(ticket.Det6, ticket.DocEntry);
                        tbDet6.TypeName = "vt.TPRTV6";
                        cmd.Parameters.AddWithValue("@TPRTV6", tbDet6.Value);
                    }

                    /*se añadio para anular entregado diferente a Agencia*/
                    if (TipoMantenimiento.Equals("USAT"))
                    {
                        cmd.Parameters.AddWithValue("@LugarDestino", ticket.LugarDestino);
                    }

                    // aqui va lo de regalos cuando es Entregado Ticket Venta
                    if (TipoMantenimiento.Equals("USET") && auxTK.Det5 != null && auxTK.Det5.Count >= 1)
                    {
                        if (auxTK.Det5[0].IdReg > 0 && auxTK.Det5[0].RegCant > 0)
                        {
                            cmd.Parameters.AddWithValue("@TieneRegalos", 1);
                            cmd.Parameters.AddWithValue("@RegEstado", ticket.Det5[0].RegEstado);

                            gestionStock = true;
                        }
                    }

                    cmd.ExecuteNonQuery();
                    status = ticket.DocNum;
                    tran.Commit();
                }
                catch (Exception e)
                {
                    tran.Rollback();
                    gestionStock = false;
                    throw new Exception("Error al editar en estado =>" + Estado + " " + e.Message);
                }
            }
            catch (Exception e2)
            {
                status = 0;
                gestionStock = false;
                throw new Exception(e2.Message);
            }
            cn.Close();


            if (gestionStock)
            {
                OREG_D oregD = new OREG_D();
                auxTK.Det5[0].RegCant = -1 * auxTK.Det5[0].RegCant;
                auxTK.OpRegistro = ticket.OpRegistro;
                oregD.CompromisosStock(auxTK);
                oregD.RegistrarGestionStock(new OREG_E() { Id = auxTK.Det5[0].IdReg, StockDisp = auxTK.Det5[0].RegCant }
                , new OTRC_E()
                {
                    IdReg = auxTK.Det5[0].IdReg,
                    RegName = auxTK.Det5[0].RegCate + " " + auxTK.Det5[0].RegTipo,
                    CardCode = auxTK.CardCode,
                    CardName = auxTK.CardName,
                    Sentido = "Salida",
                    Detalle = auxTK.DocNum.ToString(),
                    Cantidad = auxTK.Det5[0].RegCant,
                    Operario = auxTK.OpRegistro     // usuario en sesión
                });
            }

            return status;
        }
        public int emitirGuia(int DocEntry, Usuario_E u)
        {
            int status = -1;
            ORTV_E t;
            SqlConnection cn = new SqlConnection(uti.cadSql);
            try
            {
                t = obtenerTicket(DocEntry);
                cn.Open();
                SqlCommand cmd = new SqlCommand("vt.MANT_ORTV", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@TipoMantenimiento", "USGE");
                cmd.Parameters.AddWithValue("@Estado", t.Estado);
                cmd.Parameters.AddWithValue("@DocEntry", DocEntry);
                cmd.Parameters.AddWithValue("@DocNum", 0).Direction = ParameterDirection.Output;
                cmd.Parameters.AddWithValue("@OpFacturacion", $"{u.Nombres} {u.Apellidos}");
                cmd.ExecuteNonQuery();
                cn.Close();
                status = int.Parse(cmd.Parameters["@DocNum"].Value.ToString());
            }
            catch (Exception e) { status = 0; cn.Close(); throw new Exception("Error en emitir guia: " + e.Message); }
            return status;
        }
        public int revertirGuiasTicket(int DocEntry, string operario)
        {
            int status = -1;
            ORTV_E t;
            SqlConnection cn = new SqlConnection(uti.cadSql);
            try
            {
                t = obtenerTicket(DocEntry);
                cn.Open();
                SqlCommand cmd = new SqlCommand("vt.MANT_ORTV", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@TipoMantenimiento", "URVG");
                cmd.Parameters.AddWithValue("@Estado", t.Estado);
                cmd.Parameters.AddWithValue("@DocEntry", DocEntry);
                cmd.Parameters.AddWithValue("@Operario", operario);
                cmd.Parameters.AddWithValue("@DocNum", 0).Direction = ParameterDirection.Output;
                cmd.ExecuteNonQuery();
                cn.Close();
                status = int.Parse(cmd.Parameters["@DocNum"].Value.ToString());
            }
            catch (Exception e) { status = 0; cn.Close(); throw new Exception("Error en revertir guias: " + e.Message); }
            return status;
        }
        public int facturarTicket(int DocEntry, Usuario_E u)
        {
            int status = -1;
            ORTV_E t;
            SqlConnection cn = new SqlConnection(uti.cadSql);
            try
            {
                t = obtenerTicket(DocEntry);
                cn.Open();
                SqlCommand cmd = new SqlCommand("vt.MANT_ORTV", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@TipoMantenimiento", "USFC");
                cmd.Parameters.AddWithValue("@Estado", t.Estado);
                cmd.Parameters.AddWithValue("@DocEntry", DocEntry);
                cmd.Parameters.AddWithValue("@DocNum", 0).Direction = ParameterDirection.Output;
                cmd.Parameters.AddWithValue("@OpFacturacion", $"{u.Nombres} {u.Apellidos}");
                cmd.ExecuteNonQuery();
                cn.Close();
                status = int.Parse(cmd.Parameters["@DocNum"].Value.ToString());
            }
            catch (Exception e) { status = 0; cn.Close(); throw new Exception("Error en fact: " + e.Message); }
            return status;
        }
        public int revertirFacturarTicket(int DocEntry, string operario)
        {
            int status = -1;
            ORTV_E t;
            SqlConnection cn = new SqlConnection(uti.cadSql);
            try
            {
                t = obtenerTicket(DocEntry);
                cn.Open();
                SqlCommand cmd = new SqlCommand("vt.MANT_ORTV", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@TipoMantenimiento", "UAFC");
                cmd.Parameters.AddWithValue("@Estado", t.Estado);
                cmd.Parameters.AddWithValue("@DocEntry", DocEntry);
                cmd.Parameters.AddWithValue("@Operario", operario);
                cmd.Parameters.AddWithValue("@DocNum", 0).Direction = ParameterDirection.Output;
                cmd.ExecuteNonQuery();
                cn.Close();
                status = int.Parse(cmd.Parameters["@DocNum"].Value.ToString());
            }
            catch (Exception e) { status = 0; cn.Close(); throw new Exception("Error en AnFact: " + e.Message); }
            return status;
        }
        public List<ORIN_E> listarNotasDeCreditoV(string CardCode)
        {
            List<ORIN_E> lista = new List<ORIN_E>();
            string query = "SELECT \"DocEntry\",\"CardCode\",\"CardName\",\"DocTotal\",\"DocDate\",\"DocNum\" " +
                "FROM " + uti.schemaHana + "ORIN WHERE \"DocStatus\"='O' AND \"CANCELED\"='N' AND \"CardCode\"='" + CardCode + "'";
            HanaConnection hcn = new HanaConnection(uti.cadHana);
            try
            {
                hcn.Open();
                HanaCommand hcmd = new HanaCommand(query, hcn);
                HanaDataReader hdr = hcmd.ExecuteReader();
                while (hdr.Read())
                {
                    ORIN_E n = new ORIN_E()
                    {
                        DocEntry = hdr.GetInt32(0),
                        CardCode = hdr.GetString(1),
                        CardName = hdr.GetString(2),
                        DocTotal = hdr.GetDecimal(3),
                        DocDate = hdr.GetDateTime(4).ToString("yyyy-MM-dd"),
                        DocNum = hdr.GetInt32(5)
                    };
                    if (obtenerDet4Ticket(0, n.DocNum).Count == 0)
                    {
                        lista.Add(n);
                    }
                }
                hdr.Close();
                hcn.Close();
            }
            catch { hcn.Close(); }
            return lista;
        }
        //metodos para reportes
        private List<ORTV_E> RptAnalisisVentas(Capa_Entidad.Ventas_ENT.Formularios.FrmAnalisisVentas_E obj)
        {
            List<ORTV_E> lista = new List<ORTV_E>();
            string fil = string.Empty;
            // Operarios que sus acciones se encuentran en la tabla CC_ORTV
            string[] operarios = { "SEPARAR", "REGISTRAR", "RECIBIR", "INICIO PICKING", "FIN PICKING", "INICIO VERIFICAR", "FIN VERIFICAR", "INICIO EMPACAR", "FIN EMPACAR", "PESAR", "ENVIAR", "ENTREGAR", "ANULAR", "CANCELAR" };
            int indice = Array.IndexOf(operarios, obj.TipoOperario);


            if (!string.IsNullOrEmpty(obj.AlmIni) && !string.IsNullOrEmpty(obj.AlmFin)) { fil += " AND (select top 1 y.LugarDeEntrega from vt.RTV2 y where y.DocEntry=t0.DocEntry) between @AlmIni and @AlmFin"; }
            if (!string.IsNullOrEmpty(obj.FecIni) && !string.IsNullOrEmpty(obj.FecFin)) { fil += " AND t0.FechaSapTicket between @FecIni and @FecFin"; }
            if (!string.IsNullOrEmpty(obj.CardCode)) { fil += " AND t0.CardCode=@CardCode"; }
            if (!string.IsNullOrEmpty(obj.LugarDestino)) { fil += " AND t0.LugarDestino=@LugarDestino"; }
            if (!string.IsNullOrEmpty(obj.FormaPago)) { fil += " AND t0.FormaPago=@FormaPago"; }
            if (obj.TipoOperario != null)
            {
                // Si el Tipooperario se encuentra dentro de los valors de @operarios
                if (indice != -1)
                {
                    fil += " AND (SELECT TOP 1 Operario from vt.CC_ORTV where Operacion ='" + obj.TipoOperario + "' and DocEntry=t0.DocEntry order by concat(FechaOperacion,HoraOperacion)  desc )  like concat('%',@Operario,'%')";
                }
                else
                {
                    fil += " AND t0." + obj.TipoOperario + " LIKE CONCAT('%',@Operario,'%')";
                }
            }

            string query = "SELECT t0.DocNum, t0.FechaSapTicket, t0.CardCode, t0.CardName, (SELECT TOP 1 HoraOperacion from vt.CC_ORTV where DocEntry=t0.DocEntry and Operacion='REGISTRAR' order by FechaOperacion,HoraOperacion DESC)" +
                                    ",t0.TipoVenta, t0.Embalaje, t0.Vendedor, t0.MontoTotal, t0.Flete, t0.GastoEnvio, t0.DescuentoNC, t0.DeudaCliente" +
                                    ",t0.DeudaEmpresa, t0.MontoFinal, (SELECT STUFF((SELECT ', ' + cast(t1.NroSap as varchar(max)) FROM vt.RTV2 t1 " +
                                    "INNER JOIN  vt.ORTV x ON x.DocEntry = t1.DocEntry WHERE t1.DocEntry = t0.DocEntry " +
                                    "FOR XML PATH('')), 1,2, '')) as 'NroVentas'  " +
                                    ",(select top 1 y.Vendedor from vt.RTV2 y where y.DocEntry=t0.DocEntry) as 'VendedorSap', t0.FormaPago, t0.EstadoPago" +
                                    ",(select top 1 y.LugarDeEntrega from vt.RTV2 y where y.DocEntry=t0.DocEntry) as 'LugEntregaSap'" +
                                " FROM vt.ORTV t0 where t0.DocEntry>0 " + fil;
            SqlConnection cn = new SqlConnection(uti.cadSql);
            try
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand(query, cn);
                cmd.CommandType = CommandType.Text;
                if (!string.IsNullOrEmpty(obj.AlmIni)) { cmd.Parameters.AddWithValue("@AlmIni", obj.AlmIni); }
                if (!string.IsNullOrEmpty(obj.AlmFin)) { cmd.Parameters.AddWithValue("@AlmFin", obj.AlmFin); }
                if (!string.IsNullOrEmpty(obj.FecIni)) { cmd.Parameters.AddWithValue("@FecIni", obj.FecIni); }
                if (!string.IsNullOrEmpty(obj.FecFin)) { cmd.Parameters.AddWithValue("@FecFin", obj.FecFin); }
                if (!string.IsNullOrEmpty(obj.CardCode)) { cmd.Parameters.AddWithValue("@CardCode", obj.CardCode); }
                if (!string.IsNullOrEmpty(obj.LugarDestino)) { cmd.Parameters.AddWithValue("@LugarDestino", obj.LugarDestino); }
                if (!string.IsNullOrEmpty(obj.FormaPago)) { cmd.Parameters.AddWithValue("@FormaPago", obj.FormaPago); }

                if (!string.IsNullOrEmpty(obj.Operario)) { cmd.Parameters.AddWithValue("@Operario", obj.Operario); }

                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    ORTV_E o = new ORTV_E();
                    if (!dr.IsDBNull(0)) { o.DocNum = dr.GetInt32(0); }
                    if (!dr.IsDBNull(1)) { o.FechaSapTicket = dr.GetDateTime(1).ToString("dd/MM/yyyy"); }
                    if (!dr.IsDBNull(2)) { o.CardCode = dr.GetString(2); }
                    if (!dr.IsDBNull(3)) { o.CardName = dr.GetString(3); }
                    if (!dr.IsDBNull(4)) { o.HoraRegistro = dr.GetTimeSpan(4).ToString(); }
                    if (!dr.IsDBNull(5)) { o.TipoVenta = dr.GetString(5); }
                    if (!dr.IsDBNull(6)) { o.Embalaje = dr.GetString(6); }
                    if (!dr.IsDBNull(7)) { o.Vendedor = dr.GetString(7); }
                    if (!dr.IsDBNull(8)) { o.MontoTotal = dr.GetDecimal(8); }
                    if (!dr.IsDBNull(9)) { o.Flete = dr.GetDecimal(9); }
                    if (!dr.IsDBNull(10)) { o.GastoEnvio = dr.GetDecimal(10); }
                    if (!dr.IsDBNull(11)) { o.DescuentoNC = dr.GetDecimal(11); }
                    if (!dr.IsDBNull(12)) { o.DeudaCliente = dr.GetDecimal(12); }
                    if (!dr.IsDBNull(13)) { o.DeudaEmpresa = dr.GetDecimal(13); }
                    if (!dr.IsDBNull(14)) { o.MontoFinal = dr.GetDecimal(14); }
                    if (!dr.IsDBNull(15)) { o.NroVentas = dr.GetString(15); }
                    if (!dr.IsDBNull(16)) { o.VendedorSap = dr.GetString(16); }
                    if (!dr.IsDBNull(17)) { o.FormaPago = dr.GetString(17); }
                    if (!dr.IsDBNull(18)) { o.EstadoPago = dr.GetString(18); }
                    if (!dr.IsDBNull(19)) { o.LugEntrega = dr.GetString(19); }
                    lista.Add(o);
                }
                dr.Close();
                cn.Close();
            }
            catch { cn.Close(); }
            return lista;
        }
        public DataTable tbRptAnalisisVentas(Capa_Entidad.Ventas_ENT.Formularios.FrmAnalisisVentas_E obj)
        {
            List<string> campos = new List<string>();
            List<Type> tipos = new List<Type>();
            campos.Add("Orden"); tipos.Add(typeof(string));
            campos.Add("DocNum"); tipos.Add(typeof(string));
            campos.Add("FechaRegistro"); tipos.Add(typeof(string));
            campos.Add("CardCode"); tipos.Add(typeof(string));
            campos.Add("CardName"); tipos.Add(typeof(string));
            campos.Add("HoraRegistro"); tipos.Add(typeof(string));
            campos.Add("TipoVenta"); tipos.Add(typeof(string));
            campos.Add("Embalaje"); tipos.Add(typeof(string));
            campos.Add("Vendedor"); tipos.Add(typeof(string));
            campos.Add("MontoTotal"); tipos.Add(typeof(string));
            campos.Add("Flete"); tipos.Add(typeof(string));
            campos.Add("GastoEnvio"); tipos.Add(typeof(string));
            campos.Add("DescuentoNC"); tipos.Add(typeof(string));
            campos.Add("DeudaCliente"); tipos.Add(typeof(string));
            campos.Add("DeudaEmpresa"); tipos.Add(typeof(string));
            campos.Add("MontoFinal"); tipos.Add(typeof(string));
            campos.Add("NroVentas"); tipos.Add(typeof(string));
            campos.Add("VendedorSap"); tipos.Add(typeof(string));
            campos.Add("FormaPago"); tipos.Add(typeof(string));
            campos.Add("EstadoPago"); tipos.Add(typeof(string));
            campos.Add("LugEntregaSap"); tipos.Add(typeof(string));

            DataTable tb = definirTabla(campos, tipos, "DataTableReporteAnalisisVentas");
            int i = 0;
            foreach (ORTV_E p in RptAnalisisVentas(obj))
            {
                DataRow row = tb.NewRow();
                row["Orden"] = i++;
                row["DocNum"] = p.DocNum;
                row["FechaRegistro"] = p.FechaSapTicket;
                row["CardCode"] = p.CardCode;
                row["CardName"] = p.CardName;
                row["HoraRegistro"] = p.HoraRegistro;
                row["TipoVenta"] = p.TipoVenta;
                row["Embalaje"] = p.Embalaje;
                row["Vendedor"] = p.Vendedor;
                row["MontoTotal"] = p.MontoTotal;
                row["Flete"] = p.Flete;
                row["GastoEnvio"] = p.GastoEnvio;
                row["DescuentoNC"] = p.DescuentoNC;
                row["DeudaCliente"] = p.DeudaCliente;
                row["DeudaEmpresa"] = p.DeudaEmpresa;
                row["MontoFinal"] = p.MontoFinal;
                row["NroVentas"] = p.NroVentas;
                row["VendedorSap"] = p.VendedorSap;
                row["FormaPago"] = p.FormaPago;
                row["EstadoPago"] = p.EstadoPago;
                row["LugEntregaSap"] = p.LugEntrega;
                tb.Rows.Add(row);
            }
            return tb;
        }
        // Método para listar los números de tickets en el input "DocNumTicket" en el view "Registrar solicitud" en AtencionCliente
        public List<ORTV_E> ListarTicketsParaAtencion()
        {
            List<ORTV_E> lista = new List<ORTV_E>();

            String select = "DocEntry, DocNum, CardCode, CardName, LugarDestino, FechaFacturacion";
            String query = $"SELECT {select} from vt.ORTV WHERE Estado = 'ENTREGADO' and FechaSapTicket >=DATEADD(DAY,-25,getdate()) ORDER BY DocNum DESC";

            SqlConnection cn = new SqlConnection(uti.cadSql);
            SqlCommand cmd = new SqlCommand(query, cn) { CommandType = CommandType.Text };
            cn.Open();

            try
            {
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        ORTV_E ticket = new ORTV_E();
                        if (!dr.IsDBNull(0)) { ticket.DocEntry = dr.GetInt32(0); }
                        if (!dr.IsDBNull(1)) { ticket.DocNum = dr.GetInt32(1); }
                        if (!dr.IsDBNull(2)) { ticket.CardCode = dr.GetString(2); }
                        if (!dr.IsDBNull(3)) { ticket.CardName = dr.GetString(3); }
                        if (!dr.IsDBNull(4)) { ticket.LugarDestino = dr.GetString(4); }
                        if (!dr.IsDBNull(5)) { ticket.FechaFacturacion = dr.GetDateTime(5).ToString("dd/MM/yyyy"); }
                        lista.Add(ticket);
                    }
                }

                dr.Close();
            }
            catch (Exception e)
            {
                throw new Exception("Error: " + e.Message);
            }
            cn.Close();

            return lista;
        }
        public ORTV_E CalcularMontos(ORTV_E t)
        {
            t.MontoTotal = (decimal)(0.00);
            t.MontoFinal = (decimal)(0.00);
            if (t.Det2 != null)
            {
                foreach (RTV2_E d2 in t.Det2)
                {
                    if (d2.Verificar == "on")
                    {
                        t.MontoTotal += d2.Monto;
                    }
                }
            }

            if (t.Det4 != null)
            {
                t.DescuentoNC = 0.00M;
                foreach (RTV4_E d4 in t.Det4)
                {
                    if (d4.Verificar == "on")
                    {
                        t.DescuentoNC += d4.Nc.DocTotal;
                    }
                }
            }
            t.MontoFinal += t.MontoTotal + t.Flete + t.GastoEnvio + t.DeudaCliente - t.DescuentoNC - t.DeudaEmpresa;
            return t;
        }
        public ORTV_E CalcularPesoTotal(ORTV_E t)
        {
            t.PesoTotal = (decimal)(0.00);
            if (t.Det6 != null)
            {
                foreach (RTV6_E d in t.Det6)
                {
                    if (d.Verificar == "on")
                    {
                        t.PesoTotal += d.Peso;
                    }
                }
            }
            return t;
        }
        public void confGastEnvio(ORTV_E o)
        {
            string query = "update vt.ORTV set EstadoGasto='CONFIRMADO',PagoEnv=@PagoEnv where DocEntry=@DocEntry";
            SqlConnection cn = new SqlConnection(uti.cadSql);
            try
            {
                cn.Open();
                SqlTransaction tran = cn.BeginTransaction("TR02");
                try
                {
                    SqlCommand cmd = new SqlCommand(query, cn, tran);
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@DocEntry", o.DocEntry);
                    cmd.Parameters.AddWithValue("@PagoEnv", o.PagoEnv);
                    cmd.ExecuteNonQuery();
                    lD.agregarLDS1(new LDS1_E
                    {
                        CardCode = o.CardCode,
                        FechaOpe = DateTime.Now.ToString("yyyy-MM-dd"),
                        Operacion = "ConfEnvio",
                        DetOpe = "Tk: " + o.DocNum + " " + o.DetOpe,
                        Egreso = o.PagoEnv,
                        OperarioReg = o.OpRegistro
                    }, tran, cn);
                    tran.Commit();
                    cn.Close();
                }
                catch { tran.Rollback(); cn.Close(); }
            }
            catch { cn.Close(); }
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
        public string generaInfoListaClientes(string Fecha)
        {
            string info = "<datalist id='ListaClientes'>";
            foreach (OCRD_E x in listarClientes(Fecha))
            {
                info += "<option CardCode='" + x.CardCode + "' value='" + x.CardName + "'></option>";
            }
            info += "</datalist>";
            return info;
        }
        public string generaInfoListaDirDestinos(string CardCode)
        {
            string info = "<option value=''>Seleccione</option>";
            foreach (RTV3_E x in listarDirDestinos(CardCode))
            {
                info += "<option  value ='" + x.DirDestino + "' Zona='" + x.Zona + "' Distrito='" + x.Distrito + "' Provincia='" + x.Provincia + " 'Departamento='" + x.Departamento + "' Ubigeo='" + x.Ubigeo + "' Calle='" + x.Calle + "'>" + x.DirDestino + "</option>";
            }
            return info;
        }
        public string generaInfoListaOrdenesDeVenta(string fecha, string cardCode, int docNum)
        {
            string info = "<thead class='bg-dark text-white'><tr><th class='text-center'>#</th><th class='text-center'>VER</th><th class='text-center'>Monto</th>" +
                              "<th class='text-center'>Nro SAP</th><th class='text-center'>Tipo Comprobante</th><th class='text-center'>Vendedor</th>" +
                              "<th class='text-center'>Lugar de Entrega</th><th class='text-center'>ALM Salida</th><th class='text-center font-24'>Observación</th></tr></thead><tbody style='background: #D1D1D1'>";
            int linea = 1;
            List<OrdenDeVenta_E> lista = ListarOrdenesdeVentaFinales(fecha, cardCode, docNum);
            foreach (OrdenDeVenta_E o in lista)
            {
                info += "<tr><td  class='text-center'><input id='Linea" + linea + "' name='Det2[" + (linea - 1) + "].Linea' type='hidden' value='" + linea + "' readonly />" + linea + "</td>" +
                    "<td class='text-center'><input id='Verificar" + linea + "' name='Det2[" + (linea - 1) + "].Verificar' type='checkbox' onclick=\"validacionVerificarMontos('')\" /></td>" +
                    "<td class='text-center'><input id='Monto" + linea + "' name='Det2[" + (linea - 1) + "].Monto' type='hidden' value='" + String.Format("{0:0.00}", o.DocTotal) + "' readonly />" + String.Format("{0:0.00}", o.DocTotal) + "</td>" +
                    "<td class='text-center'><input id='NroSap" + linea + "' name='Det2[" + (linea - 1) + "].NroSap' type='hidden' value='" + o.DocNum + "' readonly size=8 />" + o.DocNum + "</td>" +
                    "<td class='text-center'><select id='TipoComprobante" + linea + "' name='Det2[" + (linea - 1) + "].TipoComprobante' class='form-control'><option value=''>Seleccione</option><option value='Factura'>Factura</option><option value='Boleta'>Boleta</option><option value='F/B'>F/B</option></select></td>" +
                    "<td class='text-center'><input id='Vendedor" + linea + "' name='Det2[" + (linea - 1) + "].Vendedor' type='hidden' value='" + o.SlpName + "' readonly />" + o.SlpName + "</td>" +
                    "<td class='text-center'><input id='LugarDeEntrega" + linea + "' name='Det2[" + (linea - 1) + "].LugarDeEntrega' type='hidden' value='" + o.LugarDeEntrega + "' readonly />" + o.LugarDeEntrega + "</td>" +
                    "<td class='text-center'><input id='AlmacenSalida" + linea + "' name='Det2[" + (linea - 1) + "].AlmacenSalida' type='hidden' value='" + o.AlmacenSalida + "' readonly />" + o.AlmacenSalida + "</td>" +
                    "<td class='text-center' style=width:'500px'><input id='Observaciones" + linea + "' name='Det2[" + (linea - 1) + "].Observaciones' type='text' size='30' class='form-control' /></td></tr>";
                linea++;
            }
            info += "</tbody>";
            return info;
        }
        public string generaInfoListaNotasDeCreditoV(string CardCode)
        {
            string info = string.Empty;
            int linea = 1;
            List<ORIN_E> lista = listarNotasDeCreditoV(CardCode);
            if (lista.Count == 0) { return string.Empty; }
            foreach (ORIN_E n in lista)
            {
                info += "<tr>" +
                            "<td><input type='checkbox' name='Det4[" + (linea - 1) + "].Verificar' class=\"form-control text-center\" onclick=\"validacionVerificarMontos('')\"></td>" +
                            "<td><input type='text' name='Det4[" + (linea - 1) + "].Linea' class=\"form-control text-center\" style=\"width:40px;\" value='" + linea + "' readonly></td>" +
                            "<td><input type='text' name='Det4[" + (linea - 1) + "].Nc.DocTotal' class=\"form-control text-center\" style=\"width:100px;\" value='" + String.Format("{0:0.00}", n.DocTotal) + "' readonly></td>" +
                            "<td><input type='text' name='Det4[" + (linea - 1) + "].Nc.DocDate' class=\"form-control text-center\" style=\"width:120px;\" value='" + n.DocDate + "' readonly></td>" +
                            "<td><input type='text' name='Det4[" + (linea - 1) + "].Nc.DocNum' class=\"form-control text-center\" style=\"width:120px;\" value='" + n.DocNum + "' readonly></td>" +
                        "</tr>";
                ++linea;
            }
            return info;
        }
        public List<TEMP_RRU01_E> GuiasRemisionSap(int DocEntry)
        {
            string guiasTicket = string.Empty;
            Tablas.ORDR_D ordrD = new Tablas.ORDR_D(); List<TEMP_RRU01_E> lista = new List<TEMP_RRU01_E>(); Tablas.ODLN_D odln = new Tablas.ODLN_D(); OINV_D oinv = new OINV_D();
            SqlConnection cn = new SqlConnection(uti.cadSql);
            try
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand("select NroSap from vt.rtv2 where DocEntry=" + DocEntry, cn);
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    if (!dr.IsDBNull(0))
                    {
                        guiasTicket += ordrD.guiasTraslado(dr.GetInt32(0));

                    }
                }
                //separamos las guias del concatenado y buscamos su detalle
                List<string> ItemGuias = guiasTicket.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();

                foreach (var NumAtCard in ItemGuias)
                {
                    TEMP_RRU01_E obj = new TEMP_RRU01_E();
                    obj = odln.obtenerGuiaRemision(NumAtCard);
                    if (string.IsNullOrEmpty(obj.U_SYP_MDCD))
                    {
                        obj = oinv.obtenerGuiaRemision(NumAtCard);
                    }

                    lista.Add(obj);
                }

                dr.Close();
                cn.Close();
            }
            catch { cn.Close(); }
            return lista;
        }
        public string GuiasTicket(int DocEntry)
        {
            string Guias = string.Empty;
            Tablas.ORDR_D ordrD = new Tablas.ORDR_D();
            SqlConnection cn = new SqlConnection(uti.cadSql);
            try
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand("select NroSap from vt.rtv2 where DocEntry=" + DocEntry, cn);
                SqlDataReader dr = cmd.ExecuteReader();
                string guiasTicket = string.Empty;
                while (dr.Read())
                {
                    if (!dr.IsDBNull(0))
                    {
                        var g = ordrD.guiasTraslado(dr.GetInt32(0));
                        if (g.Length > 6) { guiasTicket += g; }
                    }
                }
                Guias = guiasTicket;
                dr.Close();
                cn.Close();
            }
            catch { cn.Close(); }
            return Guias;
        }
        //Metodos desde Hojas de Reparto
        public void preenviar(int DocEntry, string Operario, SqlTransaction tran, SqlConnection cn)
        {
            ORTV_E tk = obtenerTicket(DocEntry);
            try
            {
                SqlCommand cmd = new SqlCommand("vt.MANT_ORTV", cn, tran)
                {
                    Transaction = tran,
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@TipoMantenimiento", "UPT");
                cmd.Parameters.AddWithValue("@DocEntry", DocEntry);
                cmd.Parameters.AddWithValue("@DocNum", 0).Direction = ParameterDirection.Output;
                cmd.Parameters.AddWithValue("@Estado", tk.Estado);
                cmd.Parameters.AddWithValue("@Operario", Operario);
                cmd.ExecuteNonQuery();
            }
            catch { tran.Rollback(); cn.Close(); }
        }
        public void liberar(int DocEntry, string Operario, SqlTransaction tran, SqlConnection cn)
        {
            ORTV_E tk = obtenerTicket(DocEntry);

            if (tk.Estado != "PREENVIO" && tk.Estado != "ENVIADO")
            {
                return;
            }

            try
            {
                using (SqlCommand cmd = new SqlCommand("vt.MANT_ORTV", cn, tran))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@TipoMantenimiento", "ULT");
                    cmd.Parameters.AddWithValue("@DocEntry", DocEntry);
                    cmd.Parameters.AddWithValue("@DocNum", 0).Direction = ParameterDirection.Output;
                    cmd.Parameters.AddWithValue("@Estado", tk.Estado);
                    cmd.Parameters.AddWithValue("@Operario", Operario);
                    cmd.Parameters.AddWithValue("@LugarDestino", tk.LugarDestino);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception e)
            {
                tran.Rollback();
                throw new Exception(e.Message);
            }
        }
        public void enviar(ORTV_E o, SqlTransaction tran, SqlConnection cn)
        {
            ORTV_E ortvE = obtenerTicket(o.DocEntry);
            if (ortvE.Estado != "PREENVIO") { throw new Exception("Error envio: El ticket " + ortvE.DocNum + " no esta preenvio"); }

            try
            {
                SqlCommand cmd = new SqlCommand("vt.MANT_ORTV", cn, tran)
                {
                    Transaction = tran,
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@TipoMantenimiento", "UET");
                cmd.Parameters.AddWithValue("@Operario", o.Operario);
                cmd.Parameters.AddWithValue("@DocEntry", o.DocEntry);
                cmd.Parameters.AddWithValue("@Estado", ortvE.Estado);   // Para la validación del Proc. Almacenado (que sea distinto ANULADO)
                cmd.Parameters.AddWithValue("@DocNum", 0).Direction = ParameterDirection.Output;
                cmd.ExecuteNonQuery();
            }
            catch (Exception e) { tran.Rollback(); cn.Close(); throw new Exception(e.Message); }
        }
        public void entregar(ORTV_E o, SqlTransaction tran, SqlConnection cn)
        {
            bool gestionarStock = false;
            ORTV_E ortvE = obtenerTicket(o.DocEntry);

            if (ortvE.Estado != "ENVIADO") { throw new Exception("Error entrega: El ticket " + ortvE.DocNum + " no esta enviado"); }
            //para las rutas hacia agencia con regalo siempre pasa a ENTREGADO internamente
            if (ortvE.LugarDestino == "Agencia" && ortvE.Det5 != null && ortvE.Det5.Count >= 1) { if (ortvE.Det5[0].IdReg > 0 && ortvE.Det5[0].RegCant > 0) { o.Det5[0].RegEstado = "Entregado"; } }

            try
            {
                SqlCommand cmd = new SqlCommand("vt.MANT_ORTV", cn, tran)
                {
                    Transaction = tran,
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@TipoMantenimiento", "UETR");              // update entregar ticket desde ruta 
                cmd.Parameters.AddWithValue("@Operario", o.Operario);
                cmd.Parameters.AddWithValue("@DocEntry", o.DocEntry);
                cmd.Parameters.AddWithValue("@Estado", ortvE.Estado);                           // Para la validación del Proc. Almacenado (que sea distinto ANULADO)
                cmd.Parameters.AddWithValue("@PagoEnv", ((object)o.PagoEnv) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@ClaveEnv", ((object)o.ClaveEnv) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@DocNum", 0).Direction = ParameterDirection.Output;


                if (ortvE.Det5 != null && ortvE.Det5.Count >= 1)
                {
                    if (ortvE.Det5[0].IdReg > 0 && ortvE.Det5[0].RegCant > 0)
                    {
                        if (o.Det5[0].RegEstado != "Entregado") { throw new Exception("Debe entregar regalo"); }
                        ortvE.Det5[0].RegEstado = o.Det5[0].RegEstado;

                        SqlParameter tbDet5 = new SqlParameter("@TPRTV5", SqlDbType.Structured);
                        tbDet5.Value = RTV5_E.tbDetalle(ortvE.Det5, ortvE.DocEntry);
                        tbDet5.TypeName = "vt.TPRTV5";
                        cmd.Parameters.AddWithValue("@TPRTV5", tbDet5.Value);

                        gestionarStock = true;
                    }
                }
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                tran.Rollback();
                gestionarStock = false;
                cn.Close();
                throw new Exception(e.Message);
            }

            // Regalos
            if (gestionarStock)
            {
                OREG_D oregD = new OREG_D();
                if (ortvE.Det5 != null && ortvE.Det5.Count > 0)
                {
                    ortvE.Det5[0].RegCant = -1 * ortvE.Det5[0].RegCant;
                    ortvE.OpRegistro = o.Operario;
                    oregD.CompromisosStock(ortvE);
                    oregD.RegistrarGestionStock(new OREG_E() { Id = ortvE.Det5[0].IdReg, StockDisp = ortvE.Det5[0].RegCant }
                    , new OTRC_E()
                    {
                        IdReg = ortvE.Det5[0].IdReg,
                        RegName = ortvE.Det5[0].RegCate + " " + ortvE.Det5[0].RegTipo
                                                                 ,
                        CardCode = ortvE.CardCode,
                        CardName = ortvE.CardName,
                        Sentido = "Salida"
                                                                 ,
                        Detalle = ortvE.DocNum.ToString(),
                        Cantidad = ortvE.Det5[0].RegCant,
                        Operario = ortvE.OpRegistro
                    });
                }
            }
        }
        public int entregarMasivoTicket(int DocEntry, Tickets t)
        {
            int status;
            int regalos = 0;

            SqlConnection cn = new SqlConnection(uti.cadSql);
            try
            {
                cn.Open();
                SqlTransaction tran = cn.BeginTransaction();
                try
                {
                    SqlCommand cmd = new SqlCommand("vt.MANT_ORTV", cn, tran);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@TipoMantenimiento", "USET");
                    cmd.Parameters.AddWithValue("@DocEntry", DocEntry);
                    cmd.Parameters.AddWithValue("@DocNum", 0).Direction = ParameterDirection.Output;
                    cmd.Parameters.AddWithValue("@Estado", t.Estado);


                    if (t.Det5 != null && t.Det5.Count > 0)
                    {
                        cmd.Parameters.AddWithValue("@RegEstado", (t.Det5[0].IdReg > 0 && t.Det5[0].RegCant > 0) ? "Entregado" : t.Det5[0].RegEstado);
                        regalos = 1;        // 1: SI
                    }

                    cmd.Parameters.AddWithValue("@TieneRegalos", regalos);
                    cmd.Parameters.AddWithValue("@Operario", t.Operario);
                    cmd.ExecuteNonQuery();

                    status = int.Parse(cmd.Parameters["@DocNum"].Value.ToString());
                    tran.Commit();
                    cn.Close();
                }
                catch (Exception e) { tran.Rollback(); cn.Close(); throw new Exception("Error al editarSegumimiento" + e.Message); }

                if (status >= 1)
                {
                    if (t.Det5 != null)
                    {
                        if (t.Det5[0].IdReg > 0 && t.Det5[0].RegCant > 0)
                        {
                            OREG_D oregD = new OREG_D();
                            t.Det5[0].RegCant = -1 * t.Det5[0].RegCant;
                            oregD.CompromisosStock(Tickets.ORTV_EntregaMasiva(t));
                            oregD.RegistrarGestionStock(new OREG_E() { Id = t.Det5[0].IdReg, StockDisp = t.Det5[0].RegCant }
                            , new OTRC_E()
                            {
                                IdReg = t.Det5[0].IdReg,
                                RegName = t.Det5[0].RegCate + " " + t.Det5[0].RegTipo,
                                CardCode = t.CardCode,
                                CardName = t.CardName,
                                Sentido = "Salida",
                                Detalle = t.DocNum.ToString(),
                                Cantidad = t.Det5[0].RegCant,
                                Operario = t.Operario
                            });
                        }
                    }
                }


            }
            catch (Exception e2) { status = 0; cn.Close(); throw new Exception(e2.Message); }
            return status;
        }
        public Tickets buscarTicket(int DocEntry)
        {
            Tickets result = new Tickets();
            string query = "select t0.DocEntry,t0.DocNum, t0.CardCode,t0.CardName, t0.Estado,t1.RegCant, t1.RegCate, t1.RegEstado, t1.IdReg  from vt.ORTV t0" +
                            "  LEFT JOIN  vt.RTV5 t1 on  t1.DocEntry= t0.DocEntry  where t0.DocEntry = @DocEntry";

            SqlConnection cn = new SqlConnection(uti.cadSql);
            try
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand(query, cn);         // prepara
                cmd.Parameters.AddWithValue("@DocEntry", DocEntry);
                SqlDataReader dr = cmd.ExecuteReader();             // ejecuta
                dr.Read();                                      // lectura 

                RTV5_E reg = new RTV5_E();
                List<RTV5_E> listaReg = new List<RTV5_E>();

                if (!dr.IsDBNull(0)) { result.DocEntry = dr.GetInt32(0); }
                if (!dr.IsDBNull(1)) { result.DocNum = dr.GetInt32(1); }
                if (!dr.IsDBNull(2)) { result.CardCode = dr.GetString(2); }
                if (!dr.IsDBNull(3)) { result.CardName = dr.GetString(3); }
                if (!dr.IsDBNull(4)) { result.Estado = dr.GetString(4); }
                if (!dr.IsDBNull(5)) { reg.RegCant = dr.GetDecimal(5); }
                if (!dr.IsDBNull(6)) { reg.RegCate = dr.GetString(6); }
                if (!dr.IsDBNull(7)) { reg.RegEstado = dr.GetString(7); }
                if (!dr.IsDBNull(8)) { reg.IdReg = dr.GetInt32(8); }
                if (reg.RegCant >= 1 && reg.RegCate != null && reg.RegEstado != null && reg.IdReg >= 1)
                {
                    listaReg.Add(reg);
                    result.Det5 = listaReg;
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
            return result;
        }
        public List<Tickets> buscarVariosTickets(int[] arrDocNum)
        {
            List<Tickets> lista = new List<Tickets>();
            int cantidadElementos = arrDocNum.Count();

            var parametros = arrDocNum.Select((x, i) => string.Concat("@arrDocNumConv2", i)).ToList();
            string query = $"select t0.DocEntry,t0.DocNum, t0.CardName, t0.Estado,t1.RegCant, t1.RegEstado,t1.RegTipo, t1.RegCate,(select top 1 Operario from vt.CC_ORTV" +
                $" where DocEntry=t0.DocEntry and Operacion='ENTREGAR' order by FechaOperacion,HoraOperacion desc), t1.IdReg,t0.CardCode  from vt.ORTV t0" +
                $"  inner join  vt.RTV5 t1 on  t1.DocEntry= t0.DocEntry where  t1.Linea=1 and t0.DocNum in ({string.Join(", ", parametros)})";
            SqlConnection cn = new SqlConnection(uti.cadSql);
            try
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand(query, cn);         // prepara
                for (var i = 0; i < cantidadElementos; i++)
                {
                    cmd.Parameters.AddWithValue(parametros[i], arrDocNum[i]);
                }
                SqlDataReader dr = cmd.ExecuteReader();             // ejecuta

                while (dr.Read())
                {
                    Tickets ticket = new Tickets();
                    RTV5_E reg = new RTV5_E();
                    List<RTV5_E> listaReg = new List<RTV5_E>();

                    if (!dr.IsDBNull(0)) { ticket.DocEntry = dr.GetInt32(0); }
                    if (!dr.IsDBNull(1)) { ticket.DocNum = dr.GetInt32(1); }
                    if (!dr.IsDBNull(2)) { ticket.CardName = dr.GetString(2); }
                    if (!dr.IsDBNull(3)) { ticket.Estado = dr.GetString(3); }
                    if (!dr.IsDBNull(4)) { reg.RegCant = dr.GetDecimal(4); }
                    if (!dr.IsDBNull(5)) { reg.RegEstado = dr.GetString(5); }
                    if (!dr.IsDBNull(6)) { reg.RegTipo = dr.GetString(6); }
                    if (!dr.IsDBNull(7)) { reg.RegCate = dr.GetString(7); }
                    if (!dr.IsDBNull(8)) { ticket.Operario = dr.GetString(8); }
                    if (!dr.IsDBNull(9)) { reg.IdReg = dr.GetInt32(9); }
                    if (!dr.IsDBNull(10)) { ticket.CardCode = dr.GetString(10); }

                    if (reg.RegCant >= 1 && reg.RegCate != null && reg.RegEstado != null && reg.IdReg >= 1)
                    {
                        listaReg.Add(reg);
                        ticket.Det5 = listaReg;
                    }

                    lista.Add(ticket);
                }

                dr.Close();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                cn.Close();
            }

            return lista;
        }
        public List<RptAnalisisTickets_E> ListarRptAnalisisTickets(RptFiltrosAnalisisTickets_E datosFiltro)
        {
            string condWhere = string.Empty;
            List<RptAnalisisTickets_E> lista = new List<RptAnalisisTickets_E>();

            Dictionary<string, string> operarios = new Dictionary<string, string>
                {
                    { "OpSeparo", "SEPARAR" },
                    { "OpAbierto", "REGISTRAR" },
                    { "OpRecibido", "RECIBIR" },
                    { "OpPicking", "FIN PICKING" },
                    { "OpVerificador", "FIN VERIFICAR" },
                    { "OpEmpacado", "FIN EMPACAR" },
                    { "OpEnvio", "ENVIAR" },
                    { "OpEntrega", "ENTREGAR" },
                    { "OpAnular", "ANULAR" },
                    { "OpCancelar", "CANCELAR" },

                };

            Dictionary<string, string> operariosApoyo = new Dictionary<string, string>
                {
                    { "OpPicking2", "vt.RTV11" },
                    { "OpVerificador2", "vt.RTV12" },
                    { "OpEmpacado2", "vt.RTV13" },
                };

            if (!string.IsNullOrEmpty(datosFiltro.TipoOperario) && datosFiltro.Operario != null)
            {
                if (!datosFiltro.TipoOperario.Equals("OpPicking2") && !datosFiltro.TipoOperario.Equals("OpVerificador2") && !datosFiltro.TipoOperario.Equals("OpEmpacado2"))
                {
                    if (operarios[datosFiltro.TipoOperario] != null)
                    {
                        condWhere += $" AND (SELECT TOP 1 Operario FROM vt.CC_ORTV WHERE Operacion='{operarios[datosFiltro.TipoOperario]}' and DocEntry=t0.DocEntry order by FechaOperacion,HoraOperacion desc) LIKE '%{datosFiltro.Operario}%'";
                    }
                }
                if (operariosApoyo.ContainsKey(datosFiltro.TipoOperario))
                {
                    condWhere += $" AND (SELECT STUFF((SELECT cast(x.operario as varchar(max)) + ', ' FROM {operariosApoyo[datosFiltro.TipoOperario]} x where t0.DocEntry= x.DocEntry FOR XML PATH('')), 1,2, ''))  LIKE concat('%', '{datosFiltro.Operario}', '%')";
                }
                else if (datosFiltro.TipoOperario.Equals("OpFacturacion"))
                {
                    condWhere += $" AND T0.OpFacturacion = '{datosFiltro.Operario}'";
                }
            }

            if (datosFiltro.AlmacenSalida != null)
            {
                condWhere += $" AND (SELECT TOP 1 AlmacenSalida from vt.RTV2 where DocEntry=t0.DocEntry) = '{datosFiltro.AlmacenSalida}'";
            }

            if (datosFiltro.AlmIni != null && datosFiltro.AlmFin != null)
            {
                condWhere += $" AND (T1.LugarDeEntrega BETWEEN '{datosFiltro.AlmIni}' and '{datosFiltro.AlmFin}')";
            }

            if (datosFiltro.Estado != null)
            {
                condWhere += $" AND T0.Estado='{datosFiltro.Estado}'";
            }

            if (datosFiltro.FecIni != null && datosFiltro.FecFin != null)
            {
                condWhere += $" AND (T0.FechaSapTicket BETWEEN '{datosFiltro.FecIni}' and '{datosFiltro.FecFin}' )";
            }

            if (datosFiltro.CardCode != null)
            {
                condWhere += $" AND T0.CardCode= '{datosFiltro.CardCode}'";
            }

            if (datosFiltro.LugarDestino != null)
            {
                condWhere += $" AND T0.LugarDestino= '{datosFiltro.LugarDestino}'";
            }

            if (datosFiltro.MontoFinalIni > 0 && datosFiltro.MontoFinalFin > 0)
            {
                condWhere += $" AND T0.MontoFinal BETWEEN {datosFiltro.MontoFinalIni} and {datosFiltro.MontoFinalFin}";
            }
            if (!string.IsNullOrEmpty(datosFiltro.FormaPagoFil))
            {
                condWhere += $" AND T0.FormaPago='{datosFiltro.FormaPagoFil}'";
            }

            using (SqlConnection cn = new SqlConnection(uti.cadSql))
            {
                string query = "SELECT T0.DocNum AS  'NRO TICKET', CONVERT(varchar, T0.FechaSapTicket, 103) AS 'FECHA SAP TICKET', T0.CardName AS 'CLIENTE', T0.MontoTotal AS 'MONTO TOTAL', T0.Vendedor AS 'VENDEDOR',T0.Estado AS 'ESTADO PEDIDO', " +
                                        "(Select top 1 CONVERT(varchar, FechaOperacion , 103) from vt.CC_ORTV where DocEntry=T0.DocEntry and Operacion='SEPARAR' order by FechaOperacion,HoraOperacion desc ) AS 'FECHA ABIERTO', " +
                                        "(Select top 1 CONVERT(varchar(8),HoraOperacion) from vt.CC_ORTV where DocEntry=T0.DocEntry and Operacion='SEPARAR' order by FechaOperacion,HoraOperacion desc ) as 'HORA ABIERTO', " +
                                        "(Select top 1 CONVERT(varchar, FechaOperacion , 103) from vt.CC_ORTV where DocEntry=T0.DocEntry and Operacion='RECIBIR' order by FechaOperacion,HoraOperacion desc ) AS 'FECHA RECIBIDO', " +
                                        "(Select top 1 CONVERT(varchar(8),HoraOperacion) from vt.CC_ORTV where DocEntry=T0.DocEntry and Operacion='RECIBIR' order by FechaOperacion,HoraOperacion desc ) as 'HORA RECIBIDO', " +
                                        "(Select top 1 Operario from vt.CC_ORTV where DocEntry=T0.DocEntry and Operacion='RECIBIR' order by FechaOperacion,HoraOperacion desc ) AS 'OP RECIBIDO', " +
                                        "(Select top 1 CONVERT(varchar, FechaOperacion , 103) from vt.CC_ORTV where DocEntry=T0.DocEntry and Operacion='INICIO PICKING' order by FechaOperacion,HoraOperacion DESC ) AS 'FECHA INICIO PICKING', " +
                                        "(Select top 1 CONVERT(varchar(8),HoraOperacion) from vt.CC_ORTV where DocEntry=T0.DocEntry and Operacion='INICIO PICKING' order by FechaOperacion,HoraOperacion DESC ) AS 'HORA INICIO PICKING', " +
                                        "(Select top 1 Operario from vt.CC_ORTV where DocEntry=T0.DocEntry and Operacion='INICIO PICKING' order by FechaOperacion,HoraOperacion DESC ) AS 'OP INICIO PICKING', " +
                                        "(Select top 1 CONVERT(varchar, FechaOperacion , 103) from vt.CC_ORTV where DocEntry=T0.DocEntry and Operacion='FIN PICKING' order by FechaOperacion,HoraOperacion desc ) AS 'FECHA FIN PICKING', " +
                                        "(Select top 1 CONVERT(varchar(8),HoraOperacion) from vt.CC_ORTV where DocEntry=T0.DocEntry and Operacion='FIN PICKING' order by FechaOperacion,HoraOperacion desc ) as 'HORA FIN PICKING', " +
                                        "(Select top 1 Operario from vt.CC_ORTV where DocEntry=T0.DocEntry and Operacion='FIN PICKING' order by FechaOperacion,HoraOperacion desc ) AS 'OP FIN PICKING', " +
                                        "(Select top 1 Operario from vt.RTV11 where DocEntry=T0.DocEntry AND Linea = 1) AS 'OP APOYO 1 FIN PICKING', " +
                                        "(Select top 1 Operario from vt.RTV11 where DocEntry=T0.DocEntry AND Linea = 2) AS 'OP APOYO 2 FIN PICKING', " +
                                        "(Select top 1 CONVERT(varchar, FechaOperacion , 103) from vt.CC_ORTV where DocEntry=T0.DocEntry and Operacion='INICIO VERIFICAR' order by FechaOperacion,HoraOperacion desc ) AS 'FECHA INICIO VERIFICAR', " +
                                        "(Select top 1 CONVERT(varchar(8),HoraOperacion) from vt.CC_ORTV where DocEntry=T0.DocEntry and Operacion='INICIO VERIFICAR' order by FechaOperacion,HoraOperacion desc ) as 'HORA INICIO VERIFICAR', " +
                                        "(Select top 1 Operario from vt.CC_ORTV where DocEntry=T0.DocEntry and Operacion='INICIO VERIFICAR' order by FechaOperacion,HoraOperacion desc ) AS 'OP INICIO VERIFICAR', " +
                                        "(Select top 1 CONVERT(varchar, FechaOperacion , 103) from vt.CC_ORTV where DocEntry=T0.DocEntry and Operacion='FIN VERIFICAR' order by FechaOperacion,HoraOperacion desc ) AS 'FECHA FIN VERIFICAR', " +
                                        "(Select top 1 CONVERT(varchar(8),HoraOperacion) from vt.CC_ORTV where DocEntry=T0.DocEntry and Operacion='FIN VERIFICAR' order by FechaOperacion,HoraOperacion desc ) as 'HORA FIN VERIFICAR', " +
                                        "(Select top 1 Operario from vt.CC_ORTV where DocEntry=T0.DocEntry and Operacion='FIN VERIFICAR' order by FechaOperacion,HoraOperacion desc ) AS 'OP FIN VERIFICAR', " +
                                        "(Select top 1 Operario from vt.RTV12 where DocEntry=T0.DocEntry AND Linea = 1) AS 'OP APOYO 1 FIN VERIFICAR', " +
                                        "(Select top 1 Operario from vt.RTV12 where DocEntry=T0.DocEntry AND Linea = 2) AS 'OP APOYO 2 FIN VERIFICAR', " +
                                        "(Select top 1 CONVERT(varchar, FechaOperacion , 103) from vt.CC_ORTV where DocEntry=T0.DocEntry and Operacion='INICIO EMPACAR' order by FechaOperacion,HoraOperacion desc ) AS 'FECHA INICIO EMPACAR', " +
                                        "(Select top 1 CONVERT(varchar(8),HoraOperacion) from vt.CC_ORTV where DocEntry=T0.DocEntry and Operacion='INICIO EMPACAR' order by FechaOperacion,HoraOperacion desc ) as 'HORA INICIO EMPACAR', " +
                                        "(Select top 1 Operario from vt.CC_ORTV where DocEntry=T0.DocEntry and Operacion='INICIO EMPACAR' order by FechaOperacion,HoraOperacion desc ) AS 'OP INICIO EMPACAR', " +
                                        "(Select top 1 CONVERT(varchar, FechaOperacion , 103) from vt.CC_ORTV where DocEntry=T0.DocEntry and Operacion='FIN EMPACAR' order by FechaOperacion,HoraOperacion desc ) AS 'FECHA FIN EMPACAR', " +
                                        "(Select top 1 CONVERT(varchar(8),HoraOperacion) from vt.CC_ORTV where DocEntry=T0.DocEntry and Operacion='FIN EMPACAR' order by FechaOperacion,HoraOperacion desc ) as 'HORA FIN EMPACAR', " +
                                        "(Select top 1 Operario from vt.CC_ORTV where DocEntry=T0.DocEntry and Operacion='FIN EMPACAR' order by FechaOperacion,HoraOperacion desc ) AS 'OP FIN EMPACAR', " +
                                        "(Select top 1 Operario from vt.RTV13 where DocEntry=T0.DocEntry AND Linea = 1) AS 'OP APOYO 1 FIN EMPACAR', " +
                                        "(Select top 1 Operario from vt.RTV13 where DocEntry=T0.DocEntry AND Linea = 2) AS 'OP APOYO 2 FIN EMPACAR', " +
                                        "T0.LugarDestino AS 'LUGAR DESTINO',  (SELECT STUFF((SELECT ', ' + cast(t1.NroSap as varchar(max)) FROM vt.RTV2 t1 INNER JOIN  vt.ORTV x ON x.DocEntry = t1.DocEntry WHERE t1.DocEntry = t0.DocEntry FOR XML PATH('')), 1,2, '')) AS 'NRO DE VENTAS', " +
                                        "(select COUNT(m.NroSap) from vt.RTV2 m where m.DocEntry=T0.DocEntry)  AS 'TOTAL NRO VENTAS', T0.Cajas AS 'CAJAS', T0.EstadoPago AS 'ESTADO PAGO',T0.FormaPago , T0.EstadoFacturacion AS 'ESTADO FACTURACION', CONVERT(varchar, T0.FechaFacturacion, 103) AS 'FECHA FACTURACION', " +
                                        "convert(varchar(8),T0.HoraFacturacion) as 'HORA FACTURACION', T0.OpFacturacion AS 'OP FACTURACION'," +
                                        "(Select top 1 CONVERT(varchar, FechaOperacion , 103) from vt.CC_ORTV where DocEntry=T0.DocEntry and Operacion='ENTREGAR' order by FechaOperacion,HoraOperacion desc ) AS 'FECHA ENTREGA', " +
                                        "(Select top 1 CONVERT(varchar(8),HoraOperacion) from vt.CC_ORTV where DocEntry=T0.DocEntry and Operacion='ENTREGAR' order by FechaOperacion,HoraOperacion desc ) as 'HORA ENTREGA', " +
                                        "(Select top 1 Operario from vt.CC_ORTV where DocEntry=T0.DocEntry and Operacion='ENTREGAR' order by FechaOperacion,HoraOperacion desc ) AS 'OP ENTREGA', " +
                                        "(Select top 1 CONVERT(varchar, FechaOperacion , 103) from vt.CC_ORTV where DocEntry=T0.DocEntry and Operacion='ANULAR' order by FechaOperacion,HoraOperacion desc ) AS 'FECHA ANULACION', " +
                                        "(Select top 1 CONVERT(varchar(8),HoraOperacion) from vt.CC_ORTV where DocEntry=T0.DocEntry and Operacion='ANULAR' order by FechaOperacion,HoraOperacion desc ) as 'HORA ANULACION', " +
                                        "(Select top 1 Operario from vt.CC_ORTV where DocEntry=T0.DocEntry and Operacion='ANULAR' order by FechaOperacion,HoraOperacion desc ) AS 'OP ANULACION', " +
                                        "(Select top 1 CONVERT(varchar, FechaOperacion , 103) from vt.CC_ORTV where DocEntry=T0.DocEntry and Operacion='CANCELAR' order by FechaOperacion,HoraOperacion desc ) AS 'FECHA CANCELADO', " +
                                        "(Select top 1 CONVERT(varchar(8),HoraOperacion) from vt.CC_ORTV where DocEntry=T0.DocEntry and Operacion='CANCELAR' order by FechaOperacion,HoraOperacion desc ) as 'HORA CANCELADO', " +
                                        "(Select top 1 Operario from vt.CC_ORTV where DocEntry=T0.DocEntry and Operacion='CANCELAR' order by FechaOperacion,HoraOperacion desc ) AS 'OP CANCELADO', " +
                                        "CONVERT(varchar, T0.TiempoEntrega , 103) AS 'FECHA ESTIMADA ENTREGA', CONVERT(varchar, T0.TiempoEntrega , 8) AS 'HORA ESTIMADA ENTREGA', T1.LugarDeEntrega AS 'LUGAR ENTREGA', T0.NroMesa AS 'NRO MESA', " +
                                        "(SELECT STUFF((SELECT ', ' + cast(t1.AlmacenSalida as varchar(max)) FROM vt.RTV2 t1 INNER JOIN  vt.ORTV x ON x.DocEntry = t1.DocEntry WHERE t1.DocEntry = t0.DocEntry FOR XML PATH('')), 1,2, '')) AS 'ALMACEN SALIDA',T0.Comentario " +
                                        "FROM VT.ORTV T0 INNER JOIN VT.RTV2 T1 ON T0.DocEntry=T1.DocEntry WHERE T1.Linea=1";

                query += condWhere;
                SqlCommand cmd = new SqlCommand(query, cn);         // prepara
                cn.Open();

                try
                {
                    SqlDataReader dr = cmd.ExecuteReader();             // ejecuta

                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            RptAnalisisTickets_E rpt = new RptAnalisisTickets_E();
                            if (!dr.IsDBNull(0)) { rpt.DocNum = dr.GetInt32(0); }
                            if (!dr.IsDBNull(1)) { rpt.FechaSapTicket = dr.GetString(1); }
                            if (!dr.IsDBNull(2)) { rpt.CardName = dr.GetString(2); }
                            if (!dr.IsDBNull(3)) { rpt.MontoTotal = dr.GetDecimal(3); }
                            if (!dr.IsDBNull(4)) { rpt.Vendedor = dr.GetString(4); }
                            if (!dr.IsDBNull(5)) { rpt.Estado = dr.GetString(5); }
                            if (!dr.IsDBNull(6)) { rpt.FechaAbierto = dr.GetString(6); }
                            if (!dr.IsDBNull(7)) { rpt.HoraAbierto = dr.GetString(7); }
                            if (!dr.IsDBNull(8)) { rpt.FechaRecibido = dr.GetString(8); }
                            if (!dr.IsDBNull(9)) { rpt.HoraRecibido = dr.GetString(9); }
                            if (!dr.IsDBNull(10)) { rpt.OpRecibido = dr.GetString(10); }
                            if (!dr.IsDBNull(11)) { rpt.FechaInicioPicking = dr.GetString(11); }
                            if (!dr.IsDBNull(12)) { rpt.HoraInicioPicking = dr.GetString(12); }
                            if (!dr.IsDBNull(13)) { rpt.OpInicioPicking = dr.GetString(13); }
                            if (!dr.IsDBNull(14)) { rpt.FechaFinPicking = dr.GetString(14); }
                            if (!dr.IsDBNull(15)) { rpt.HoraFinPicking = dr.GetString(15); }
                            if (!dr.IsDBNull(16)) { rpt.OpFinPicking1 = dr.GetString(16); }
                            if (!dr.IsDBNull(17)) { rpt.OpFinPicking2 = dr.GetString(17); }
                            if (!dr.IsDBNull(18)) { rpt.OpFinPicking3 = dr.GetString(18); }
                            if (!dr.IsDBNull(19)) { rpt.FechaInicioVerificar = dr.GetString(19); }
                            if (!dr.IsDBNull(20)) { rpt.HoraInicioVerificar = dr.GetString(20); }
                            if (!dr.IsDBNull(21)) { rpt.OpInicioVerificar = dr.GetString(21); }
                            if (!dr.IsDBNull(22)) { rpt.FechaFinVerificar = dr.GetString(22); }
                            if (!dr.IsDBNull(23)) { rpt.HoraFinVerificar = dr.GetString(23); }
                            if (!dr.IsDBNull(24)) { rpt.OpFinVerificador1 = dr.GetString(24); }
                            if (!dr.IsDBNull(25)) { rpt.OpFinVerificador2 = dr.GetString(25); }
                            if (!dr.IsDBNull(26)) { rpt.OpFinVerificador3 = dr.GetString(26); }
                            if (!dr.IsDBNull(27)) { rpt.FechaInicioEmpacar = dr.GetString(27); }
                            if (!dr.IsDBNull(28)) { rpt.HoraInicioEmpacar = dr.GetString(28); }
                            if (!dr.IsDBNull(29)) { rpt.OpInicioEmpacar = dr.GetString(29); }
                            if (!dr.IsDBNull(30)) { rpt.FechaFinEmpacar = dr.GetString(30); }
                            if (!dr.IsDBNull(31)) { rpt.HoraFinEmpacar = dr.GetString(31); }
                            if (!dr.IsDBNull(32)) { rpt.OpFinEmpacar1 = dr.GetString(32); }
                            if (!dr.IsDBNull(33)) { rpt.OpFinEmpacar2 = dr.GetString(33); }
                            if (!dr.IsDBNull(34)) { rpt.OpFinEmpacar3 = dr.GetString(34); }
                            if (!dr.IsDBNull(35)) { rpt.LugarDestino = dr.GetString(35); }
                            if (!dr.IsDBNull(36)) { rpt.NroVentas = dr.GetString(36); }
                            if (!dr.IsDBNull(37)) { rpt.TotalNroVentas = dr.GetInt32(37); }
                            if (!dr.IsDBNull(38)) { rpt.Cajas = dr.GetInt32(38); }
                            if (!dr.IsDBNull(39)) { rpt.EstadoPago = dr.GetString(39); }
                            if (!dr.IsDBNull(40)) { rpt.FormaPago = dr.GetString(40); }
                            if (!dr.IsDBNull(41)) { rpt.EstadoFacturacion = dr.GetString(41); }
                            if (!dr.IsDBNull(42)) { rpt.FechaFacturacion = dr.GetString(42); }
                            if (!dr.IsDBNull(43)) { rpt.HoraFacturacion = dr.GetString(43); }
                            if (!dr.IsDBNull(44)) { rpt.OpFacturacion = dr.GetString(44); }
                            if (!dr.IsDBNull(45)) { rpt.FechaEntrega = dr.GetString(45); }
                            if (!dr.IsDBNull(46)) { rpt.HoraEntrega = dr.GetString(46); }
                            if (!dr.IsDBNull(47)) { rpt.OpEntrega = dr.GetString(47); }
                            if (!dr.IsDBNull(48)) { rpt.FechaCancelacion = dr.GetString(48); }
                            if (!dr.IsDBNull(49)) { rpt.HoraCancelacion = dr.GetString(49); }
                            if (!dr.IsDBNull(50)) { rpt.OpCancelacion = dr.GetString(50); }
                            if (!dr.IsDBNull(51)) { rpt.FechaAnulacion = dr.GetString(51); }
                            if (!dr.IsDBNull(52)) { rpt.HoraAnulacion = dr.GetString(52); }
                            if (!dr.IsDBNull(53)) { rpt.OpAnulacion = dr.GetString(53); }
                            if (!dr.IsDBNull(54)) { rpt.FechaTiempoEntrega = dr.GetString(54); }
                            if (!dr.IsDBNull(55)) { rpt.HoraTiempoEntrega = dr.GetString(55); }
                            if (!dr.IsDBNull(56)) { rpt.LugarDeEntrega = dr.GetString(56); }
                            if (!dr.IsDBNull(57)) { rpt.NroMesa = dr.GetInt32(57); }
                            if (!dr.IsDBNull(58)) { rpt.AlmacenSalida = dr.GetString(58); }
                            if (!dr.IsDBNull(59)) { rpt.Comentario = dr.GetString(59); }

                            lista.Add(rpt);
                        }
                    }

                    dr.Close();
                }
                catch (Exception e)
                {
                    throw new Exception(e.Message);
                }

                cn.Close();
            }

            return lista;
        }
        public List<ORTV_E> listarTicketsParaRepartos(ORTV_E filtro, string[] estados, out int cantidadTicketsNoEnviados)
        {
            List<ORTV_E> lista = new List<ORTV_E>(); cantidadTicketsNoEnviados = 0;
            int cantidadElementos = estados.Count();
            var parametros = estados.Select((x, i) => string.Concat("@Estado", i)).ToList();
            SqlConnection cn = new SqlConnection(uti.cadSql);
            try
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand("al.ListarTicketsReparto", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@CantidadTicketsNoEnviados", 0).Direction = ParameterDirection.Output;
                for (var i = 0; i < cantidadElementos; i++)
                {
                    cmd.Parameters.AddWithValue(parametros[i], estados[i]);
                }
                cmd.Parameters.AddWithValue("@LugarDestino", filtro.LugarDestino);
                cmd.Parameters.AddWithValue("@Fecha", filtro.FechaSapTicket);
                cmd.Parameters.AddWithValue("@LugEntrega", filtro.LugEntrega);
                cmd.Parameters.AddWithValue("@Zona", filtro.Zona);
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    ORTV_E o = new ORTV_E();
                    if (!dr.IsDBNull(0)) { o.DocEntry = dr.GetInt32(0); }
                    if (!dr.IsDBNull(1)) { o.CardCode = dr.GetString(1); }
                    if (!dr.IsDBNull(2)) { o.CardName = dr.GetString(2); }
                    if (!dr.IsDBNull(3)) { o.DocNum = dr.GetInt32(3); }
                    if (!dr.IsDBNull(4)) { o.Cajas = dr.GetInt32(4); }
                    if (!dr.IsDBNull(5)) { o.Observaciones = dr.GetString(5); }
                    if (!dr.IsDBNull(6)) { o.GastoEnvio = dr.GetDecimal(6); }
                    if (!dr.IsDBNull(7)) { o.MontoFinal = dr.GetDecimal(7); }
                    if (!dr.IsDBNull(8)) { o.DirDestino = dr.GetString(8); }
                    if (!string.IsNullOrEmpty(o.DirDestino)) { if (o.DirDestino == " ,     ") { o.DirDestino = null; } }
                    if (!dr.IsDBNull(9)) { o.Agencia = dr.GetString(9); }
                    if (!dr.IsDBNull(10)) { o.LugarDestino = dr.GetString(10); }
                    if (!dr.IsDBNull(11)) { o.Zona = dr.GetString(11); }
                    if (!dr.IsDBNull(12)) { o.EstadoPago = dr.GetString(12); }
                    if (!dr.IsDBNull(13)) { o.EstadoFacturacion = dr.GetString(13); }
                    if (!dr.IsDBNull(14)) { o.TipoVenta = dr.GetString(14); }
                    if (!dr.IsDBNull(15)) { o.FechaPago = dr.GetDateTime(15).ToString("yyyy-MM-dd"); }
                    if (!dr.IsDBNull(16)) { o.HoraPago = dr.GetTimeSpan(16).ToString(); }
                    if (!dr.IsDBNull(17)) { o.Vinculados = dr.GetString(17); }

                    if (o.LugarDestino == "Domicilio" || o.LugarDestino == "Agencia")
                    { o.Guias = GuiasTicket(o.DocEntry); }
                    else
                    {
                        Almacen_DAO.Tablas.OWTR_D owtrD = new Almacen_DAO.Tablas.OWTR_D();
                        if (o.LugarDestino == "Arriola") { o.Guias = owtrD.GuiasTicketTransferencia(o.DocNum, "09"); }
                        else { o.Guias = owtrD.GuiasTicketTransferencia(o.DocNum, "01"); }
                    }
                    lista.Add(o);
                }

                dr.Close();
                if (cmd.Parameters.Contains("@CantidadTicketsNoEnviados") && cmd.Parameters["@CantidadTicketsNoEnviados"].Value != DBNull.Value)
                {
                    // Obtener el valor del parámetro de salida, colocar Despues de cerrar el DataReader 
                    cantidadTicketsNoEnviados = Convert.ToInt32(cmd.Parameters["@CantidadTicketsNoEnviados"].Value);
                }
                cn.Close();
            }
            catch { cn.Close(); }
            return lista;
        }
        public List<ORTV_E> listarTicketsRepartosNoEnviados(ORTV_E filtro, string[] estados)
        {
            List<ORTV_E> lista = new List<ORTV_E>();
            int cantidadElementos = estados.Count();
            var parametros = estados.Select((x, i) => string.Concat("@Estado", i)).ToList();
            SqlConnection cn = new SqlConnection(uti.cadSql);
            try
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand("al.ListarTicketsRepartoNE", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                for (var i = 0; i < cantidadElementos; i++)
                {
                    cmd.Parameters.AddWithValue(parametros[i], estados[i]);
                }
                cmd.Parameters.AddWithValue("@LugarDestino", filtro.LugarDestino);
                cmd.Parameters.AddWithValue("@Fecha", filtro.FechaSapTicket);
                cmd.Parameters.AddWithValue("@LugEntrega", filtro.LugEntrega);
                cmd.Parameters.AddWithValue("@Zona", filtro.Zona);
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    ORTV_E o = new ORTV_E();
                    if (!dr.IsDBNull(0)) { o.DocNum = dr.GetInt32(0); }
                    if (!dr.IsDBNull(1)) { o.FechaSapTicket = dr.GetDateTime(1).ToString("yyyy-MM-dd"); }
                    if (!dr.IsDBNull(2)) { o.CardName = dr.GetString(2); }
                    if (!dr.IsDBNull(3)) { o.MontoFinal = dr.GetDecimal(3); }
                    lista.Add(o);
                }
                dr.Close();
                cn.Close();
            }
            catch { cn.Close(); }
            return lista;
        }
    }
}
