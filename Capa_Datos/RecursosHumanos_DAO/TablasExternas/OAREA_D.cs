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
    public class OAREA_D
    {
        readonly Utilitarios uti = new Utilitarios();
        //public void MigrarAreasHANA()
        //{
        //    DBHelper db = new DBHelper();
        //    List<OAREA_E> lista = null;

        //    string query = @"SELECT TOP 100  ""OcrCode"", ""OcrName"",  ""Active"",  ""DimCode""  FROM " + uti.schemaHana + @"OOCR  WHERE ""DimCode"" = '2'";

        //    try
        //    {
        //        using (HanaDataReader hdr = db.HanaExecuteReaderNoSp(query))
        //        {
        //            lista = new List<OAREA_E>();

        //            // Verificar si hay filas disponibles para leer
        //            if (hdr.HasRows)
        //            {
        //                while (hdr.Read())
        //                {
        //                    OAREA_E obj = new OAREA_E();

        //                    if (!hdr.IsDBNull(0))
        //                    {
        //                        string ocrCode = hdr.GetString(0);
        //                        string[] partes = ocrCode.Split('.');

        //                        string valorAntesDelPunto = partes[0];
        //                        obj.IdDepartamento = Convert.ToInt32(valorAntesDelPunto);

        //                        string valorDespuesDelPunto = partes.Length > 1 ? partes[1] : "";
        //                        obj.IdArea = Convert.ToInt32(valorDespuesDelPunto);

        //                        if (!hdr.IsDBNull(1)) { obj.Nombre = hdr.GetString(1); }
        //                        if (!hdr.IsDBNull(2)) { obj.Estado = hdr.GetString(2); }

        //                        lista.Add(obj);
        //                    }
        //                }
        //            }

        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        File.AppendAllText(uti.directorioLogs + "OAREA_D - MigrarAreasHANA.txt", $"{DateTime.Now}: {ex.Message}\n {ex.StackTrace}\n");        // Registro de error
        //    }

        //    if (lista != null && lista.Count >= 1)
        //    {
        //        new Capa_Datos.RecursosHumanos_DAO.TablasSQL.OAREA_D().RegistrarAreas(lista);
        //    }

        //}
    }
}
