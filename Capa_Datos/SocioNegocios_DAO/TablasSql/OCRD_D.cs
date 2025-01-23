using Capa_Entidad.SocioNegocios_ENT.Tablas;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Datos.SocioNegocios_DAO.TablasSql
{
    //Tablas SQL- CLIENTES
    public class OCRD_D
    {
        Utilitarios uti = new Utilitarios();
        public List<OCRD_E> BuscarCliente(OCRD_E cliente)
        {
            List<OCRD_E> lista = new List<OCRD_E>();
            string condWhere = string.Empty;

            // TipoCliente "P" (proveedor), este filtro es para la búsqueda de proveedor en Nueva Devolución / Editar Devolución (VIEW: Almacén -> Devolución de Mercancías)
            if (cliente.TipoCliente != null && cliente.TipoCliente.Equals("P"))
            {
                condWhere += " AND CardCode LIKE 'P%'";
            }

            using (SqlConnection cn = new SqlConnection(uti.cadSql))
            {
                string query = $"SELECT * FROM dbo.OCRD WHERE CardName LIKE '%{cliente.CardName}%' {condWhere} ORDER BY CardName ASC";

                SqlCommand cmd = new SqlCommand(query, cn);         // prepara
                cn.Open();

                try
                {
                    SqlDataReader dr = cmd.ExecuteReader();             // ejecuta

                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            OCRD_E ocrd = new OCRD_E();

                            if (!dr.IsDBNull(0)) { ocrd.CardCode = dr.GetString(0); }
                            if (!dr.IsDBNull(1)) { ocrd.CardName = dr.GetString(1); }
                            if (!dr.IsDBNull(2)) { ocrd.Address = dr.GetString(2); }

                            lista.Add(ocrd);
                        }
                    }

                    dr.Close();
                }
                catch (Exception e)
                {
                    throw new Exception(e.Message);
                }

                cn.Close();
            }

            return lista;
        }
    }
}
