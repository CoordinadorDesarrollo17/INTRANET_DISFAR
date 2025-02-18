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
        public void ValidarLotesRegistroSanitario(List<DetTransferenciaStock_E> detalleTransferencia)
        {
            //validar que en la tabla LotesRegistroSanitario existan todos los ItemCode-BatchNum, se inserta si no existe alguno de detalleTransferencia los nombres de columnas tienen los mismos valores
            
                using (var connection = new SqlConnection(uti.cadSql2))
                {
                    connection.Open();
                    using (var transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            // En una tabla temporal se almacenaran todas las combinaciones de ItemCode y BatchNum (Producto y Lote)
                            string createTempTableQuery = @"
                                        CREATE TABLE #TempLotes (
                                            ItemCode VARCHAR(50),
                                            BatchNum VARCHAR(50),
                                            ExpDate DATE,
                                            InDate DATE
                                        );";

                            using (var commandCreateTemp = new SqlCommand(createTempTableQuery, connection, transaction))
                            {
                                commandCreateTemp.ExecuteNonQuery();
                            }

                            // Agrega esos datos
                            using (var bulkCopy = new SqlBulkCopy(connection, SqlBulkCopyOptions.Default, transaction))
                            {
                                bulkCopy.DestinationTableName = "#TempLotes";

                                var dataTable = new DataTable();
                                dataTable.Columns.Add("ItemCode", typeof(string));
                                dataTable.Columns.Add("BatchNum", typeof(string));

                                foreach (var item in detalleTransferencia)
                                {
                                    dataTable.Rows.Add(item.ItemCode, item.BatchNum);
                                }

                                bulkCopy.WriteToServer(dataTable);
                            }

                            // Con MERGE se insertara solo los registros que no existen a la tabla LotesRegistroSanitario
                            string mergeQuery = @"
                                    MERGE INTO LotesRegistroSanitario AS target
                                    USING #TempLotes AS source
                                    ON target.ItemCode = source.ItemCode AND target.BatchNum = source.BatchNum
                                    WHEN NOT MATCHED THEN
                                        INSERT (ItemCode, BatchNum,ExpDate,InDate)
                                        VALUES (source.ItemCode, source.BatchNum,source.ExpDate,source.InDate);";

                            using (var commandMerge = new SqlCommand(mergeQuery, connection, transaction))
                            {
                                commandMerge.ExecuteNonQuery();
                            }

                            // Eliminar la tabla temporal
                            string dropTempTableQuery = "DROP TABLE #TempLotes;";
                            using (var commandDropTemp = new SqlCommand(dropTempTableQuery, connection, transaction))
                            {
                                commandDropTemp.ExecuteNonQuery();
                            }

                            transaction.Commit();
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            throw new Exception("Error al validar los lotes en LotesRegistroSanitario: " + ex.Message);
                        }
                    }
                }
         }
    }
}
