using Capa_Datos.Rutas_DAO.TablasSql;
using Capa_Entidad.Rutas_ENT.TablasSql;
using System.Collections.Generic;
using System.Linq;

namespace Capa_Negocio.Rutas_NEG.TablasSql
{
    public class RRU01_N
    {
        RRU01_D datos = new RRU01_D();
        public RRU01_E BuscarCorrelativo(string Correlativo)
        {
            return datos.BuscarCorrelativo(Correlativo);
        }
        public void Liberar(int Id, int DocEntryRuta)
        {
            datos.Liberar(Id);
        }
        public void Agregar(RRU01_E obj)
        {
            datos.Agregar(obj);
        }
        public string BuscarComprobantes(int DocEntryTicket)
        {
            string tabla = $"<div class='table-responsive'><table border='2' style='width: 100%;border-color:green'>";
            int DocNum = DocEntryTicket + 2000000000;
            List<RRU01_E> lista = datos.BuscarComprobantes(DocEntryTicket);
            if (lista != null && lista.Count > 0)
            {
                var gruposPorIdentificador = lista.GroupBy(data => data.Identificador).OrderBy(grupo => grupo.Key);

                var correlativosPorIdentificador = new Dictionary<string, List<string>>();
                foreach (var grupo in gruposPorIdentificador)
                {
                    string identificador = grupo.Key;

                    correlativosPorIdentificador[identificador] = grupo
                        .OrderBy(data => $"{data.U_SYP_MDTD}-{data.U_SYP_MDSD}-{data.U_SYP_MDCD}")
                        .Select(data => $"{data.U_SYP_MDTD}-{data.U_SYP_MDSD}-{data.U_SYP_MDCD}")
                        .ToList();
                }
                // Obtener la cantidad máxima de correlativos en un solo identificador
                int maxCorrelativos = correlativosPorIdentificador.Values.Max(correlativos => correlativos.Count);
                for (int i = 0; i < maxCorrelativos; i++)
                {
                    tabla += "<tr>";

                    foreach (var correlativosList in correlativosPorIdentificador.Values)
                    {
                        string correlativo = i < correlativosList.Count ? correlativosList[i] : "";
                        string linkcorrelativo = $"<a onclick=\"ImprimirUnitarioDocumento('{correlativo}',{DocNum},false)\" href=\"#\">{correlativo}</a>";

                        tabla += $"<td class='text-dark'>{linkcorrelativo}</td>";
                    }

                    tabla += "</tr>";
                }
            }
            tabla += "</table></div>";
            return tabla;
        }
        public List<RRU01_E> Listar(int DocEntryTicket)
        {
            return datos.BuscarComprobantes(DocEntryTicket);
        }
    }
}
