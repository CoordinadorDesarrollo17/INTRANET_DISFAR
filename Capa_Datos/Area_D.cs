using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Capa_Entidad;
using System.Data;
using System.Data.SqlClient;
using System.Data.SQLite;
namespace Capa_Datos
{
    public class Area_D
    {
        Utilitarios uti = new Utilitarios();
        public List<Area_E> listarAreas(string tb="AREA")
        {
            List<Area_E> lista = new List<Area_E>();
            SqlConnection cn = new SqlConnection(uti.cadSql);
            try
            {   
                cn.Open();
                SqlCommand cmd = new SqlCommand("SELECT id,descripcion FROM " + tb, cn);
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    Area_E a = new Area_E();
                    a.id = dr.GetInt32(0);
                    a.descripcion = dr.GetString(1);
                    lista.Add(a);
                }
                dr.Close();
                cn.Close();
            }
            catch{ cn.Close();}
            return lista;
        }
        public List<AreaFc_E> listarAreasFc(string tb = "REA1")
        {
            List<AreaFc_E> lista = new List<AreaFc_E>();
            SqlConnection cn = new SqlConnection(uti.cadSql);
            try
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand("SELECT T1.id as id,isnull(T1.linea, 0) as linea" +
                                                        ", isnull(T1.proceso, '') as proceso" +
                                                        ", isnull(T1.subproceso, '') as subproceso" +
                                                        ", isnull(T1.accion, '#') as accion" +
                                                  " FROM " + tb + " T1", cn);
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    AreaFc_E a = new AreaFc_E();
                    a.id = dr.GetInt32(0);
                    a.linea = dr.GetInt32(1);
                    a.proceso = dr.GetString(2);
                    a.subproceso = dr.GetString(3);
                    a.accion = dr.GetString(4);
                    lista.Add(a);
                }
                dr.Close();
                cn.Close();
            }catch { cn.Close();}
            return lista;
        }

    }
}
