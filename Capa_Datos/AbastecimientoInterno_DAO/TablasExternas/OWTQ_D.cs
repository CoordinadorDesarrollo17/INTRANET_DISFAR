using Capa_Entidad;
using Capa_Entidad.AbastecimientoInterno_ENT.TablasExternas;
using Capa_Entidad.ComprobantesContables_ENT;
using Capa_Entidad.Ventas_ENT.TablasSql;
using DocumentFormat.OpenXml.Office2013.Excel;
using Sap.Data.Hana;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace Capa_Datos.AbastecimientoInterno_DAO.TablasExternas
{
    //Solicitudes de traslado
    public class OWTQ_D
    {
        readonly Utilitarios uti = new Utilitarios();
        readonly DBHelper db = new DBHelper();

        public OWTQ_E BuscarSolicitudDeTraslado(int DocNum)
        {
            OWTQ_E solicitud = null;
            string query =  $"CALL {uti.schemaHana}\"COB_BUSCAR_DOC_SOL_TRASLADO\"({DocNum}) ";
            try
            {
                HanaDataReader hdr = db.HanaExecuteReaderNoSp(query);
                if (hdr.HasRows)
                {
                    solicitud = new OWTQ_E { Detalle = new List<WTQ1_E>() };
                    while (hdr.Read())
                    {
                        if (solicitud.DocEntry == 0)
                        {
                            if (!hdr.IsDBNull(0)) { solicitud.DocEntry = hdr.GetInt32(0); }
                            if (!hdr.IsDBNull(1)) { solicitud.DocNum = hdr.GetInt32(1); }
                            if (!hdr.IsDBNull(2)) { solicitud.DocDate = hdr.GetString(2); }
                            if (!hdr.IsDBNull(3)) { solicitud.CardCode = hdr.GetString(3); }
                            if (!hdr.IsDBNull(4)) { solicitud.CardName = hdr.GetString(4); }
                            if (!hdr.IsDBNull(5)) { solicitud.NroGuia = hdr.GetString(5); }
                            if (!hdr.IsDBNull(6)) { solicitud.OperarioResponsable = hdr.GetString(6); }
                            if (!hdr.IsDBNull(7)) { solicitud.MotivoTraslado = hdr.GetString(7); }
                        }
                        string itemCode = hdr.IsDBNull(10) ? "" : hdr.GetString(10);

                        // Buscar si ya existe el detalle con el mismo ItemCode
                        var detalle = solicitud.Detalle.FirstOrDefault(d => d.ItemCode == itemCode);
                        if (detalle == null)
                        {
                            detalle = new WTQ1_E();

                                // Crear objeto detalle WTQ1_E con las demás columnas del detalle
                                if (!hdr.IsDBNull(8)) { detalle.AlmacenOrigen = hdr.GetString(8); }
                                if (!hdr.IsDBNull(9)) { detalle.AlmacenDestino = hdr.GetString(9); }
                                if (!hdr.IsDBNull(10)) { detalle.ItemCode = hdr.GetString(10); }
                                if (!hdr.IsDBNull(11)) { detalle.ItemName = hdr.GetString(11); }
                                if (!hdr.IsDBNull(12)) { detalle.CantidadTotalPorSKU = Math.Round(hdr.GetDecimal(12), 0); }
                                detalle. DetalleLotes = new List<WTQ1_Lotes_E>();

                            solicitud.Detalle.Add(detalle);
                        }
                        var detalleLote = new WTQ1_Lotes_E();
                        
                                    if (!hdr.IsDBNull(13)) { detalleLote.BatchNum = hdr.GetString(13); }
                                    if (!hdr.IsDBNull(14)) { detalleLote.CantidadTotalPorSKUyLote = Math.Round(hdr.GetDecimal(14), 0); }
                                    if (!hdr.IsDBNull(15)) { detalleLote.FechaAdmision = hdr.GetString(15); }
                                    if (!hdr.IsDBNull(16)) { detalleLote.FechaVencimiento = hdr.GetString(16); }

                        detalle.DetalleLotes.Add(detalleLote);

                    }
                    hdr.Close();
                }
            }
            catch (Exception e) { throw new Exception($"Error al buscar solicitud de traslado: {e.Message}");  }

            return solicitud;
        }
      
    }
}

