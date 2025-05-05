using Capa_Entidad;
using Capa_Entidad.AbastecimientoInterno_ENT.TablasSql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Datos.AbastecimientoInterno_DAO.TablasSql
{
    public class LotesRegistroSanitario_D
    {
        readonly Utilitarios uti = new Utilitarios();
        public Helper_E ValidarLotesRegistroSanitario(Dictionary<string, DetalleSolicitudesTraslado_E> detalleTraslado,SqlConnection cn)
        {
            var respuesta = new Helper_E();

                    try
                    {
                        if (cn.State != ConnectionState.Open)
                        {
                            cn.Open();
                        }
                        // Crear tabla temporal
                        string createTempTableQuery = @"
                    CREATE TABLE #TempLotes (
                        ItemCode VARCHAR(50),
                        DistNumber VARCHAR(50),
                        ExpDate DATE,
                        InDate DATE
                    );";

                        using (var commandCreateTemp = new SqlCommand(createTempTableQuery, cn))
                        {
                            commandCreateTemp.ExecuteNonQuery();
                        }

                        // Agregar datos a la tabla temporal
                        using (var bulkCopy = new SqlBulkCopy(cn, SqlBulkCopyOptions.Default,null))
                        {
                            bulkCopy.DestinationTableName = "#TempLotes";

                            var dataTable = new DataTable();
                            dataTable.Columns.Add("ItemCode", typeof(string));
                            dataTable.Columns.Add("DistNumber", typeof(string));
                            dataTable.Columns.Add("ExpDate", typeof(DateTime));
                            dataTable.Columns.Add("InDate", typeof(DateTime));

                            foreach (var item in detalleTraslado)
                            {
                                dataTable.Rows.Add(item.Value.ItemCode, item.Value.BatchNum, item.Value.ExpDate, item.Value.InDate);
                            }

                            bulkCopy.WriteToServer(dataTable);
                        }

                        // MERGE para insertar o actualizar registros en LotesRegistroSanitario
                        string mergeQuery = @"
                    MERGE INTO LotesRegistroSanitario AS LRS
                    USING #TempLotes AS source
                    ON LRS.ItemCode = source.ItemCode 
                       AND LRS.DistNumber = source.DistNumber
                    WHEN MATCHED THEN
                    UPDATE SET 
                        LRS.InDate = source.InDate
                    WHEN NOT MATCHED THEN
                    INSERT (ItemCode, DistNumber, ExpDate, InDate)
                    VALUES (source.ItemCode, source.DistNumber, source.ExpDate, source.InDate);";

                        using (var commandMerge = new SqlCommand(mergeQuery, cn))
                        {
                            commandMerge.ExecuteNonQuery();
                        }

                        // Eliminar la tabla temporal
                        string dropTempTableQuery = "DROP TABLE #TempLotes;";
                        using (var commandDropTemp = new SqlCommand(dropTempTableQuery, cn))
                        {
                            commandDropTemp.ExecuteNonQuery();
                        }

                    
                        respuesta.Icono = "success";
                        respuesta.Mensajes = new List<string> { "Validación de lotes completada con éxito." };
                    }
                    catch (SqlException ex)
                    {
                
                        if (ex.Number == 2627 || ex.Number == 2601) // Error de clave duplicada
                        {
                            respuesta.Icono = "error";
                            respuesta.Mensajes = new List<string> { "Error: Se intentó insertar un lote duplicado en LotesRegistroSanitario. Verifique los valores de ItemCode y DistNumber." };
                        }
                        else
                        {
                            respuesta.Icono = "error";
                            respuesta.Mensajes = new List<string> {  "Error en la validación de lotes: " + ex.Message};
                        }
                    }
                    catch (Exception ex)
                    {
                        respuesta.Icono = "error";
                        respuesta.Mensajes = new List<string> { "Error inesperado en la validación de lotes: " + ex.Message };
                    }

            return respuesta;
        }

    }
}
