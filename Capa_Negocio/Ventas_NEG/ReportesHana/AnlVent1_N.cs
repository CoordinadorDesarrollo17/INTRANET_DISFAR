using Capa_Datos.Ventas_DAO.ReportesHana;
using Capa_Entidad.Ventas_ENT.ReportesHana;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Negocio.Ventas_NEG.ReportesHana
{
    public class AnlVent1_N
    {
        AnlVent1_D anlVent1_D = new AnlVent1_D();
        public DataTable tbRptAnlVent1(string FecIni, string FecFin, int SlpCode, string CardCode, int FirmCode)
        {
            if(FecIni==null || FecIni == "") { throw new Exception("Debe llenar fecha inicial"); }
            if (FecFin == null || FecFin == "") { throw new Exception("Debe llenar fecha final"); }
            try
            {
                DateTime DtFecIni = DateTime.Parse(FecIni);
                DateTime DtFecFin = DateTime.Parse(FecFin);
                if (DtFecIni.Year != DtFecFin.Year) { throw new Exception("El rango de fechas debenser el mismo año"); }
                if (DtFecIni.CompareTo(DtFecFin) >0) { throw new Exception("La fecha inicial no puede ser mayor a la fecha final"); }
            }
            catch { }
            if (SlpCode <= 0) { throw new Exception("Debe elegir un vendedor"); }
            if (CardCode == null || CardCode == "") { throw new Exception("Debe elegir un cliente"); }
            if (FirmCode <= 0) { throw new Exception("Debe elegir un laboratorio"); }
            return anlVent1_D.tbRptAnlVent1(FecIni, FecFin, SlpCode, CardCode, FirmCode);
        }
    }
}
