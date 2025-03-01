using Capa_Entidad;
using Capa_Entidad.AbastecimientoInterno_ENT.TablasSql;
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

        public SolicitudesTraslado_E BuscarSolicitudDeTraslado(int DocNum)
        {
            SolicitudesTraslado_E solicitud = null;
            string query = $"CALL {uti.schemaHana}\"COB_BUSCAR_DOC_SOL_TRASLADO\"({DocNum}) ";

            try
            {
                HanaDataReader hdr = db.HanaExecuteReaderNoSp(query);
                if (hdr.HasRows)
                {
                    solicitud = new SolicitudesTraslado_E { Detalle = new Dictionary<string, DetalleSolicitudesTraslado_E>() };

                    while (hdr.Read())
                    {
                        if (solicitud.DocEntry == 0)
                        {
                            if (!hdr.IsDBNull(0)) solicitud.DocEntry = hdr.GetInt32(0);
                            if (!hdr.IsDBNull(1)) solicitud.DocNum = hdr.GetInt32(1);
                            if (!hdr.IsDBNull(2)) solicitud.DocDate = hdr.GetString(2);
                            if (!hdr.IsDBNull(3)) solicitud.CardCode = hdr.GetString(3);
                            if (!hdr.IsDBNull(4)) solicitud.CardName = hdr.GetString(4);
                            if (!hdr.IsDBNull(5)) solicitud.NroGuia = hdr.GetString(5);
                            if (!hdr.IsDBNull(6)) solicitud.OperarioResponsableSAP = hdr.GetString(6);
                            if (!hdr.IsDBNull(7)) solicitud.MotivoTraslado = hdr.GetString(7);
                        }

                        string itemCode = hdr.IsDBNull(10) ? "" : hdr.GetString(10);
                        string batchNum = hdr.IsDBNull(13) ? "" : hdr.GetString(13); // Lote
                        string uniqueKey = $"{itemCode}_{batchNum}"; // Clave única combinada

                        // Crear nueva instancia de detalle
                        var detalle = new DetalleSolicitudesTraslado_E
                        {
<<<<<<< HEAD
                            solicitud.Detalle[itemCode] = new DetalleSolicitudesTraslado_E
                            {
                                FromWhsCode = hdr.IsDBNull(8) ? "" : hdr.GetString(8),
                                ToWhsCode = hdr.IsDBNull(9) ? "" : hdr.GetString(9),
                                ItemCode = itemCode,
                                ItemName = hdr.IsDBNull(11) ? "" : hdr.GetString(11),
                                BatchNum = hdr.IsDBNull(13) ? "" : hdr.GetString(13),
                                QuantityCajas = hdr.IsDBNull(14) ? 0 : Math.Round(hdr.GetDecimal(14), 0),
                                InDate = hdr.IsDBNull(15) ? "" : hdr.GetString(15),
                                ExpDate = hdr.IsDBNull(16) ? "" : hdr.GetString(16)
                            };
=======
                            FromWhsCode = hdr.IsDBNull(8) ? "" : hdr.GetString(8),
                            ToWhsCode = hdr.IsDBNull(9) ? "" : hdr.GetString(9),
                            ItemCode = itemCode,
                            ItemName = hdr.IsDBNull(11) ? "" : hdr.GetString(11),
                            BatchNum = batchNum,
                            QuantityCajas = hdr.IsDBNull(14) ? 0 : Math.Round(hdr.GetDecimal(14), 0),
                            InDate = hdr.IsDBNull(15) ? "" : hdr.GetString(15),
                            ExpDate = hdr.IsDBNull(16) ? "" : hdr.GetString(16)
                        };

                        // Si la clave única no existe en el diccionario, agregar el detalle
                        if (!solicitud.Detalle.ContainsKey(uniqueKey))
                        {
                            solicitud.Detalle[uniqueKey] = detalle;
>>>>>>> Correciones_OWTQ
                        }
                    }

                    hdr.Close();
                }
            }
            catch (Exception e)
            {
                throw new Exception($"Error al buscar solicitud de traslado: {e.Message}");
            }

            return solicitud;
        }

    }
}

