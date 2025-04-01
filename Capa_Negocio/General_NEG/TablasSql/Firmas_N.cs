using Capa_Datos.General_DAO.TablasSql;
using Capa_Entidad.General_ENT.TablasSql;
using Microsoft.ReportingServices.ReportProcessing.ReportObjectModel;
using System.Collections.Generic;
using System.Text;

namespace Capa_Negocio.General_NEG.TablasSql
{
    public class Firmas_N
    {
        Firmas_D fir = new Firmas_D();

        public List<Firmas_E> ListarFirmas(Firmas_E filtros, Dictionary<string, object> parametros = null)
        {
            StringBuilder condicion = new StringBuilder();

            if (parametros == null)
                parametros = new Dictionary<string, object>();

            if (filtros != null)
            {
                if (filtros.DocEntryUsuario >= 1)
                {
                    condicion.AppendLine("AND FIR.[DocEntryUsuario] = @DocEntryUsuario");
                    parametros["@DocEntryUsuario"] = filtros.DocEntryUsuario;
                }

                if (filtros.ListaDocEntryUsuario != null && filtros.ListaDocEntryUsuario.Count >= 1)
                {
                    condicion.AppendLine("AND FIR.[DocEntryUsuario] IN (@DocEntryUsuario)");
                    parametros["@DocEntryUsuario"] = string.Join(",", filtros.ListaDocEntryUsuario);
                }

                if (!string.IsNullOrWhiteSpace(filtros.Nombres) || !string.IsNullOrWhiteSpace(filtros.Apellidos))
                {
                    condicion.AppendLine("AND CONCAT(FIR.[Nombres], ' ', FIR.[Apellidos]) LIKE @Nombres");
                    parametros["@Nombres"] = $"%{filtros.Nombres}%";
                }

                if (!string.IsNullOrWhiteSpace(filtros.TipoFirma))
                {
                    condicion.AppendLine("AND FIR.[TipoFirma] = @TipoFirma");
                    parametros["@TipoFirma"] = filtros.TipoFirma;
                }

                if (!string.IsNullOrWhiteSpace(filtros.CodigoAlmacen))
                {
                    condicion.AppendLine("AND FIR.[CodigoAlmacen] = @CodigoAlmacen");
                    parametros["@CodigoAlmacen"] = filtros.CodigoAlmacen;
                }
            }

            return fir.ListarFirmas(condicion.ToString(), parametros);
        }
    }
}