using System;
using System.Text.RegularExpressions;

namespace Capa_Datos
{
    public class Utilitarios
    {
        public string cadHana;
        public string cadHanaOnPremise;
        public string schemaHana;
        public string schemaHanaOnPremise;
        public string cadSqlite;
        public string cadAccess;
        public string serverSql;
        public string userSql;
        public string passwordSql;
        public string BDsql;
        public string cadSql;
        public int CodigoSmtp;
        public string Smtp;
        public string directorioFileServer;
        public string directorioGeneral;
        public string directorioLogs;
        public string cadSophos;
        public string serverSophos;
        public string userSophos;
        public string BDSophossql;
        public string passwordSophosSql;
        public Utilitarios()
        {
            // File Directories
            this.directorioGeneral = @"D:\";
            this.directorioFileServer = @"D:\COBEFARWEBFILES\";
            this.directorioLogs = @"D:\COBEFARWEBLOGS\";

            // HANA Database Configuration
            this.cadHana = "Server=172.16.55.36:30015;UserName=B1ADMIN;Password=Passw0rd;";
            this.schemaHana = "\"BASE_28_09_2024\".";

            //Hana On Premise 
            this.cadHanaOnPremise = "Server=192.168.1.37:30015;UserName=B1ADMIN;Password=Passw0rd;";
            this.schemaHanaOnPremise = "\"B1H_COBEFAR_2018\".";

            // SQL Server Configuration
            this.serverSql = @"WIN-DC\DIEGO_BD";
            this.userSql = "sa";
            this.passwordSql = "@Ndr@de123";
            this.BDsql = "INTRANET_V2";
            //this.BDsql = "BASE_131224";

            this.BDsql = "BASE_110924";
            this.cadSql = $@"Server={this.serverSql};" +
                          $"Database={this.BDsql};" +
                          $"User Id={this.userSql};" +
                          $"Password={this.passwordSql};" +
                          "Min Pool Size=0;" +
                          "Max Pool Size=10024;" +
                          "Pooling=true;";


            // SOPHOS Database Configuration
            this.serverSophos = @"WIN-TERMINALSAP\COBEFAR";
            this.userSophos = "sa";
            this.BDSophossql = "SOPHOS_INT_PROD";
            this.passwordSophosSql = "C0B3F@r_2022";

            this.cadSophos = $@"Server={this.serverSophos};" +
                             $"Database={this.BDSophossql};" +
                             $"User Id={this.userSophos};" +
                             $"Password={this.passwordSophosSql};" +
                             "Min Pool Size=0;" +
                             "Max Pool Size=10024;" +
                             "Pooling=true;";

            // CORREO 
            this.Smtp = "smtp.gmail.com";
            this.CodigoSmtp = 25;
        }
        public void RegistrarLog(string user, string mensaje, int operacion, string ip, string equipo)
        {
            try
            {
                string nombre = directorioLogs + BDsql + "_log_" + DateTime.Now.ToString("yyyyMMdd") + ".txt";
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(nombre, true))
                {
                    file.WriteLine("Usuario:" + user + ";Accion:" + mensaje + ";Operacion:" + operacion + ";Ip:" + ip + ";Equipo:" + equipo + ";Fecha:" + DateTime.Now.ToString());
                }
            }
            catch { }
        }
        public string msjError(int HResult)
        {
            string msj = "";
            if (HResult == -2146232060) { msj = "El valor ya se encuentra registrado"; }
            return msj;
        }
    }
}
