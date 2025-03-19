using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Datos
{
    public static class LogHelper
    {
        public static void RegistrarError(Exception ex, string nombreArchivo)
        {
            string directorioLogs = new Utilitarios().directorioLogs;
            File.AppendAllText(directorioLogs + nombreArchivo + ".txt", $"{DateTime.Now}: {ex.Message}\n{ex.StackTrace}\n");
        }
    }

}
