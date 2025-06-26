using Capa_Entidad.RecursosHumanos_ENT.TablasSQL;
using DocumentFormat.OpenXml.Vml;
using Sap.Data.Hana;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Capa_Datos.RecursosHumanos_DAO.TablasExternas
{
    public class ODPTO_D
    {
        readonly Utilitarios uti = new Utilitarios();

        //public void MigrarDepartamentosHANA()
        //{
        //    DBHelper db = new DBHelper();
        //    List<ODPTO_E> lista = null;

        //    string query = @"SELECT TOP 100 ""OcrCode"", ""OcrName"", ""Active"" , ""DimCode"" FROM" + uti.schemaHana + @"OOCR WHERE(""OcrName"" LIKE 'DEP%' AND ""DimCode"" in (1))";

        //    try
        //    {
        //        using (HanaDataReader hdr = db.HanaExecuteReaderNoSp(query))
        //        {
        //            lista = new List<ODPTO_E>();

        //            while (hdr.Read())
        //            {
        //                ODPTO_E obj = new ODPTO_E();

        //                if (!hdr.IsDBNull(0)) { obj.IdDepartamento = hdr.GetInt32(0); }
        //                if (!hdr.IsDBNull(1)) { obj.Nombre = hdr.GetString(1); }
        //                if (!hdr.IsDBNull(2)) { obj.Estado = hdr.GetString(2); }

        //                lista.Add(obj);
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        File.AppendAllText(uti.directorioLogs + "ODPTO_D - MigrarDepartamentosHANA.txt", $"{DateTime.Now}: {ex.Message}\n {ex.StackTrace}\n");        // Registro de error
        //    }

        //    if (lista != null && lista.Count >= 1)
        //    {
        //        new Capa_Datos.RecursosHumanos_DAO.TablasSQL.ODPTO_D().RegistrarDepartamentos(lista);
        //    }

        //}
    }
}
