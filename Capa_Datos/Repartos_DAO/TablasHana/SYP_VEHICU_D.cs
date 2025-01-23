using Capa_Entidad.Repartos_ENT.TablasHana;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Datos.Repartos_DAO.TablasHana
{
    public class SYP_VEHICU_D
    {
        private readonly Utilitarios uti = new Utilitarios();
        private readonly DBHelper db = new DBHelper();
        public List<SYP_VEHICU_E> listar()
        {

            var lista = new List<SYP_VEHICU_E>();
            string query = $@"
            SELECT
                ""Code"" AS ""Codigo"",
                ""U_SYP_VEMA"" AS ""Marca"",
                ""U_SYP_VEMO"" AS ""Modelo"",
                ""U_SYP_VEPL"" AS ""Placa"",
                ""U_SYP_CHOF"" AS ""Licencia""

            FROM 
                 {uti.schemaHana}""@SYP_VEHICU""";
            try
            {
                using (var hdr = db.HanaExecuteReaderNoSp(query))
                {
                    
                    while (hdr.Read())
                    {
                        SYP_VEHICU_E obj = new SYP_VEHICU_E();
                        if (!hdr.IsDBNull(0)){obj.Code=hdr.GetString(0);}
                        if (!hdr.IsDBNull(1)){obj.U_SYP_VEMA = hdr.GetString(1);}
                        if (!hdr.IsDBNull(2)){obj.U_SYP_VEMO = hdr.GetString(2);}
                        if (!hdr.IsDBNull(3)){obj.U_SYP_VEPL = hdr.GetString(3);}
                        if (!hdr.IsDBNull(4)){obj.U_SYP_CHOF = hdr.GetString(4);}
                        lista.Add(obj);
                    }
                }
            }
            catch (Exception ex){}

            return lista;
        }
        public (string placa, string conductor) buscarConductorYPlaca(string zona)
        {
            string placa = "";
            string conductor = "";
            string query = $@"
                    SELECT TOP 1 T0.""U_SYP_VEPL"",T1.""U_SYP_CHNO"" 
                    FROM {uti.schemaHana}""@SYP_VEHICU"" T0 INNER JOIN 
                    {uti.schemaHana}""@SYP_CONDUC"" 
                    T1 ON  T0.""U_SYP_CHOF""=T1.""U_SYP_CHLI"" 
                    WHERE T1.""U_SYP_DIST""='{zona}'";      

            try
            {
                using (var hdr = db.HanaExecuteReaderNoSp(query))
                {

                    while (hdr.Read())
                    {
                        if (!hdr.IsDBNull(0)) { placa = hdr.GetString(0); }
                        if (!hdr.IsDBNull(1)) { conductor = hdr.GetString(1); }
                    }
                }
            }
            catch (Exception ex){}

            return (placa, conductor);
        }
    }
}
