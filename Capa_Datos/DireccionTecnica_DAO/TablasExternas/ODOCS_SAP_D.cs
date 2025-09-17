using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Capa_Entidad;
using Capa_Entidad.AbastecimientoInterno_ENT.TablasSql;
using Capa_Entidad.TablasSql;
using Sap.Data.Hana;

namespace Capa_Datos.DireccionTecnica_DAO.TablasExternas
{
    public class ODOCS_SAP_D
    {
        readonly Utilitarios uti = new Utilitarios();
        readonly DBHelper db = new DBHelper();

        public (Helper_E, ODOCS_E) BuscarDocEntradaMercaderia(decimal p_DocNum, string p_NumAtCard)
        {
            ODOCS_E documento = null;
            Helper_E helper = new Helper_E();

            string query = $"CALL {uti.schemaHana}\"COBE_BUSCAR_DOCENTRADA_MERCANCIA_INTERNAMIENTO\"({p_DocNum}, '{p_NumAtCard}')";

            try
            {
                HanaDataReader hdr = db.HanaExecuteReaderNoSp(query);
                if (hdr.HasRows)
                {
                    documento = new ODOCS_E();
                    while (hdr.Read())
                    {
                        if (documento.DocEntry == 0)
                        {
                            if (!hdr.IsDBNull(0)) documento.DocEntry = hdr.GetInt64(0);
                            if (!hdr.IsDBNull(1)) documento.DocNum = hdr.GetInt64(1);
                            if (!hdr.IsDBNull(2)) documento.CardCode = hdr.GetString(2);
                            if (!hdr.IsDBNull(3)) documento.CardName = hdr.GetString(3);
                            if (!hdr.IsDBNull(4)) documento.Guia = hdr.GetString(4);
                            if (!hdr.IsDBNull(5)) documento.ComprobanteVinculado = hdr.GetString(5);
                            if (!hdr.IsDBNull(6)) documento.FechaContabilizacion = hdr.GetString(6);
                            if (!hdr.IsDBNull(7)) documento.FechaInicioTraslado = hdr.GetString(7);
                        }

                        var detalle = new DOCS1_E();
                        detalle.Almacen = hdr.IsDBNull(8) ? "" : hdr.GetString(8);
                        detalle.CantidadTotal = hdr.IsDBNull(9) ? 0 : hdr.GetInt32(9);
                        detalle.ItemCode = hdr.IsDBNull(10) ? "" : hdr.GetString(10);
                        detalle.ItemName = hdr.IsDBNull(11) ? "" : hdr.GetString(11);
                        detalle.Fabricante = hdr.IsDBNull(12) ? "" : hdr.GetString(12);
                        detalle.Lote = hdr.IsDBNull(13) ? "" : hdr.GetString(13);
                        detalle.RegistroSanitario = hdr.IsDBNull(14) ? "" : hdr.GetString(14);
                        detalle.FechaVencimiento = hdr.IsDBNull(15) ? "" : hdr.GetString(15);

                        documento.Detalle.Add(detalle);
                    }

                    hdr.Close();
                    helper.Titulo = "Acción completada";
                    helper.Mensajes = new List<string> { "Documento cargado correctamente." };
                    helper.Icono = "success";
                }
            }
            catch (Exception ex)
            {
                helper.Titulo = "Error";
                helper.Mensajes = new List<string> { "Ocurrió un problema inesperado. Por favor, comunicarse con SISTEMAS." };
                helper.Icono = "error";
                LogHelper.RegistrarError(ex, "Error inesperado en ODOCS_SAP_D - BuscarDocEntradaMercaderia");
            }

            return (helper, documento);

        }

        public (Helper_E, ODOCS_E) BuscarDocSolicitudTraslado(decimal p_DocNum, string p_NumAtCard)
        {
            ODOCS_E documento = null;
            Helper_E helper = new Helper_E();

            string query = $"CALL {uti.schemaHana}\"COBE_BUSCAR_DOCSOLICITUDTRASLADO_INTERNAMIENTO\"({p_DocNum}, '{p_NumAtCard}')";

            try
            {
                HanaDataReader hdr = db.HanaExecuteReaderNoSp(query);
                if (hdr.HasRows)
                {
                    documento = new ODOCS_E();
                    while (hdr.Read())
                    {
                        if (documento.DocEntry == 0)
                        {
                            if (!hdr.IsDBNull(0)) documento.DocEntry = hdr.GetInt64(0);
                            if (!hdr.IsDBNull(1)) documento.DocNum = hdr.GetInt64(1);
                            if (!hdr.IsDBNull(2)) documento.CardCode = hdr.GetString(2);
                            if (!hdr.IsDBNull(3)) documento.CardName = hdr.GetString(3);
                            if (!hdr.IsDBNull(4)) documento.Guia = hdr.GetString(4);
                            if (!hdr.IsDBNull(5)) documento.FechaContabilizacion = hdr.GetString(5);
                            if (!hdr.IsDBNull(6)) documento.FechaInicioTraslado = hdr.GetString(6);
                        }

                        var detalle = new DOCS1_E();
                        detalle.Almacen = hdr.IsDBNull(7) ? "" : hdr.GetString(7);
                        int ordinalCantidad = hdr.GetOrdinal("CantidadTotalPzasPorLote");
                        detalle.CantidadTotal = hdr.IsDBNull(ordinalCantidad)
                            ? 0
                            : Convert.ToInt32(hdr.GetDecimal(ordinalCantidad));
                        detalle.ItemCode = hdr.IsDBNull(9) ? "" : hdr.GetString(9);
                        detalle.ItemName = hdr.IsDBNull(10) ? "" : hdr.GetString(10);
                        detalle.Fabricante = hdr.IsDBNull(11) ? "" : hdr.GetString(11);
                        detalle.Lote = hdr.IsDBNull(12) ? "" : hdr.GetString(12);
                        detalle.RegistroSanitario = hdr.IsDBNull(13) ? "" : hdr.GetString(13);
                        detalle.FechaVencimiento = hdr.IsDBNull(14) ? "" : hdr.GetString(14);
                        documento.Detalle.Add(detalle);
                    }

                    hdr.Close();
                    helper.Titulo = "Acción completada";
                    helper.Mensajes = new List<string> { "Documento cargado correctamente." };
                    helper.Icono = "success";
                }
            }
            catch (Exception ex)
            {
                helper.Titulo = "Error";
                helper.Mensajes = new List<string> { "Ocurrió un problema inesperado. Por favor, comunicarse con SISTEMAS." };
                helper.Icono = "error";
                LogHelper.RegistrarError(ex, "Error inesperado en ODOCS_SAP_D - BuscarDocTransferencia");
            }

            return (helper, documento);

        }
    }
}
