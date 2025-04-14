using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Capa_Entidad.TablasSql;
using Capa_Entidad;

namespace Capa_Datos.DireccionTecnica_DAO.TablasSql
{
    public class DOCS1_D
    {
        readonly Utilitarios uti = new Utilitarios();

        public Helper_E EditarItemDetalleDoc(DOCS1_E datos, string usuarioRegistro)
        {
            Helper_E result = new Helper_E();

            using (SqlConnection cn = new SqlConnection(uti.CadSql3))
            {
                cn.Open();
                using (SqlTransaction transaction = cn.BeginTransaction())
                {
                    try
                    {
                        using (SqlCommand cmd = new SqlCommand("sp_GestionarDocumentos", cn, transaction))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;

                            cmd.Parameters.AddWithValue("@Operacion", "EDITAR");
                            cmd.Parameters.AddWithValue("@Id", datos.Id);
                            cmd.Parameters.AddWithValue("@CertificadoAnalisis", datos.CertificadoAnalisis);
                            cmd.Parameters.AddWithValue("@ComentarioOrganoleptico", datos.ComentarioOrganoleptico);
                            cmd.Parameters.AddWithValue("@CantidadAprobados", datos.CantidadAprobados);
                            cmd.Parameters.AddWithValue("@CantidadBaja", datos.CantidadBaja);
                            cmd.Parameters.AddWithValue("@CantidadDevolucion", datos.CantidadDevolucion);

                            // Para [CC_ODOCS]
                            cmd.Parameters.AddWithValue("@UsuarioRegistro", usuarioRegistro);
                            cmd.Parameters.AddWithValue("@TipoOperacion", "EDITAR");

                            cmd.ExecuteNonQuery();

                            // Proceso para cargar archivo
                            string baseRuta = uti.directorioFileServer;
                            string rutaDirectorio = Path.Combine(baseRuta, "DireccionTecnica", "Internamiento", datos.ItemCode);

                            if (!Directory.Exists(rutaDirectorio))
                                Directory.CreateDirectory(rutaDirectorio);

                            if (datos.ArchivoET != null)
                            {
                                string extension = Path.GetExtension(datos.ArchivoET.FileName)?.ToLower();
                                string rutaCompleta = Path.Combine(rutaDirectorio, "ET" + extension);
                                datos.ArchivoET.SaveAs(rutaCompleta);
                            }

                            if (datos.ArchivoProtocolo != null)
                            {
                                string extension = Path.GetExtension(datos.ArchivoProtocolo.FileName)?.ToLower();
                                string rutaCompleta = Path.Combine(rutaDirectorio, datos.Lote + extension);
                                datos.ArchivoProtocolo.SaveAs(rutaCompleta);
                            }

                        }
                        transaction.Commit();

                        result.Titulo = "Acción completada";
                        result.Mensajes.Add("Detalle editado correctamente.");
                        result.Icono = "success";
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        LogHelper.RegistrarError(ex, "Error inesperado en DOCS1_D - EditarItemDetalleDoc()");

                        result.Titulo = "Error";
                        result.Mensajes.Add("Ocurrió un error al editar el detalle.");
                        result.Mensajes.Add("Por favor, comuníquese con el área de Sistemas para más información.");
                        result.Icono = "error";
                    }
                }
            }

            return result;
        }

        private DataTable ConvertirADatatable(List<DOCS1_E> detalles)
        {
            DataTable table = new DataTable();
            table.Columns.Add("ODCOSId", typeof(int));
            table.Columns.Add("ItemCode", typeof(string));
            table.Columns.Add("ItemName", typeof(string));
            table.Columns.Add("Lote", typeof(string));
            table.Columns.Add("FechaVencimiento", typeof(DateTime));
            table.Columns.Add("RegistroSanitario", typeof(string));
            table.Columns.Add("Fabricante", typeof(string));
            table.Columns.Add("CondicionAlmTrans", typeof(string));
            table.Columns.Add("Almacen", typeof(string));
            table.Columns.Add("CertificadoAnalisis", typeof(string));
            table.Columns.Add("ComentarioOrganoleptico", typeof(string));
            table.Columns.Add("CantidadAprobados", typeof(int));
            table.Columns.Add("CantidadBaja", typeof(int));
            table.Columns.Add("CantidadDevolucion", typeof(int));
            table.Columns.Add("CantidadTotal", typeof(int));
            table.Columns.Add("Liberado", typeof(int));
            table.Columns.Add("Transferido", typeof(int));

            foreach (var detalle in detalles)
            {
                table.Rows.Add(0, detalle.ItemCode, detalle.ItemName, detalle.Lote, detalle.FechaVencimiento, detalle.RegistroSanitario, detalle.Fabricante, detalle.CondicionAlmTrans,
                    detalle.Almacen, detalle.CertificadoAnalisis, detalle.ComentarioOrganoleptico, detalle.CantidadAprobados, detalle.CantidadBaja, detalle.CantidadDevolucion, detalle.CantidadTotal,
                    detalle.Liberado, detalle.Transferido);
            }
            return table;
        }
    }
}
