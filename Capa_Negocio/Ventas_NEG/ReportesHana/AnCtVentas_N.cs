using Capa_Datos.Ventas_DAO.ReportesHana;
using Capa_Datos.Ventas_DAO.TablasSql;
using Capa_Entidad.Seguridad_ENT;
using Capa_Entidad.Ventas_ENT.Formularios;
using Capa_Entidad.Ventas_ENT.ReportesHana;
using Capa_Entidad.Ventas_ENT.TablasSql;
using Capa_Negocio.Seguridad_NEG;
using System;
using System.Collections.Generic;

namespace Capa_Negocio.Ventas_NEG.ReportesHana
{
    public class AnCtVentas_N
    {
        AnCtVentas_D AnD = new AnCtVentas_D();
        Usuario_N ousrN = new Usuario_N();
        RTV2_D rvt2D = new RTV2_D();

        public List<AnCtVentas_E> rptAnCtVentas(FrmAnCtVentas_E frm)
        {
            Usuario_E usr;
            if (string.IsNullOrWhiteSpace(frm.FecIni)) { throw new Exception("Debe seleccionar fecha Desde"); }
            if (string.IsNullOrWhiteSpace(frm.FecFin)) { throw new Exception("Debe seleccionar fecha Hasta"); }
            /*
            if (frm.DayU <= 0)
            {
                DateTime Dia1 = new DateTime(frm.ÝearU, frm.MonthU, 1);
                frm.FecIni = Dia1.ToString("yyyy-MM-dd");
                frm.FecFin = Dia1.AddMonths(1).AddDays(-1).ToString("yyyy-MM-dd");
            }
            else
            {
                DateTime Dia1 = new DateTime(frm.ÝearU, frm.MonthU, frm.DayU);
                frm.FecIni = Dia1.ToString("yyyy-MM-dd");
                frm.FecFin = Dia1.ToString("yyyy-MM-dd");
            }*/
            if (frm.user.IdRol == 1 || frm.user.IdRol == 6)
            {
                usr = ousrN.buscarUsuario(frm.DocEntry);
            }
            else
            {
                usr = ousrN.buscarUsuario(frm.user.DocEntry);
            }

			frm.CodigoSap = usr.CodigoSap;
			frm.Usuario = usr.Prefijo + "" + usr.Id;
			frm.nombre = usr.Nombres + " " + usr.Apellidos;
			return AnD.rptAnCtVentas(frm);
		}
    }
}
