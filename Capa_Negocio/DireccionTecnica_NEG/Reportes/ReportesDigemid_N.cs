using Capa_Datos.DireccionTecnica_DAO.Reportes;
using Capa_Entidad.ReportesDigemid_ENT.Formularios;
using Capa_Entidad.DireccionTecnica_ENT.Reportes;
using Capa_Entidad.DireccionTecnica_ENT.Reportes.BalanceControlados;
using System;
using System.Collections.Generic;
using Capa_Entidad.ReportesDigemid_ENT.Reportes;

namespace Capa_Negocio.DireccionTecnica_NEG.Reportes
{
	public class ReportesDigemid_N
	{
		ReportesDigemid_D digD = new ReportesDigemid_D();

		public List<RptKardexAlmacenes_E> ReporteKardexAlmacenes(FrmKardex_E datos)
		{
			return digD.ReporteKardexAlmacenes(datos);
		}

		public List<RptBalanceControladosIngreso_E> ReporteBalanceControladosIngreso(FrmBalanceControlados_E datos)
		{
			return digD.ReporteBalanceControladosIngreso(datos);
		}

		public List<RptBalanceControladosEgreso_E> ReporteBalanceControladosEgreso(FrmBalanceControlados_E datos)
		{
			return digD.ReporteBalanceControladosEgreso(datos);
		}

		public List<RptBalanceControladosConsolidado_E> ReporteBalanceControladosConsolidado(FrmBalanceControlados_E datos)
		{
			return digD.ReporteBalanceControladosConsolidado(datos);
		}

		public List<RptBalanceControladosLibroControlados_E> ReporteBalanceControladosLibroControlados(FrmBalanceControlados_E datos)
		{
			return digD.ReporteBalanceControladosLibroControlados(datos);
		}

        public List<RptRegistroSanitario_E> ReporteRegistroSanitario(string codArticulo, string firmCode)
        {
            return digD.ReporteRegistroSanitario(codArticulo, firmCode);
        }

    }
}
