using System;
using System.Text.RegularExpressions;

namespace Capa_Datos
{
    public class Utilitarios
    {
        public string cadHana;
        public string schemaHana;
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
            //Sophos
            this.serverSophos = @"WIN-TERMINALSAP\\COBEFAR"; this.userSophos = "sa"; this.BDSophossql = "SOPHOS_INT_PROD"; this.passwordSophosSql = "C0B3F@r_2022";
            this.cadSophos = @"Server=" + this.serverSophos + ";database=" + this.BDSophossql + ";user id=" + this.userSophos + ";password=" + this.passwordSophosSql + ";Min Pool Size=0;Max Pool Size=10024;Pooling=true";

            //Hana
            this.cadHana = "Server=192.168.1.37:30015;UserName=B1ADMIN;Password=Passw0rd;";
            //this.cadHana = "Server=172.16.55.36:30015;UserName=B1ADMIN;Password=Passw0rd;"; 
            this.schemaHana = "\"B1H_COBEFAR_2018\".";
            //this.serverSql = @"WIN-DC\DIEGO_BD"; this.passwordSql = "@Ndr@de123"; this.BDsql = "INTRANET_V2"; this.userSql = "sa";
            this.serverSql = @"192.168.1.151"; this.passwordSql = "TICOBE@R789"; this.BDsql = "BASE_03_09_24"; this.userSql = "sa";
            this.directorioGeneral = @"D\";
            this.directorioFileServer = @"D:\COBEFARWEBFILES\";
            this.directorioLogs = @"D:\COBEFARWEBLOGS\";

            this.cadSql = @"Server=" + this.serverSql + ";database=" + this.BDsql + ";user id=" + this.userSql + ";password=" + this.passwordSql + ";Min Pool Size=0;Max Pool Size=10024;Pooling=true";

            //CORREO 
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
