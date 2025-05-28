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
        public string cadSql2;
        public string CadSql3 { get; private set; }
        public string CadSql4 { get; private set; }
        public int CodigoSmtp;
        public string Smtp;
        public string directorioFileServer;
        public string directorioDocumentosRegulatorios;
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
            this.directorioGeneral = Environment.GetEnvironmentVariable("DIRECTORIO_GENERAL", EnvironmentVariableTarget.Machine) ?? @"E:\";
            this.directorioFileServer = Environment.GetEnvironmentVariable("DIRECTORIO_FILESERVER", EnvironmentVariableTarget.Machine) ?? @"E:\COBEFARWEBFILES\";
            this.directorioDocumentosRegulatorios = Environment.GetEnvironmentVariable("DIRECTORIO_DOCUMENTOSREGULATORIOS", EnvironmentVariableTarget.Machine);
            this.directorioLogs = Environment.GetEnvironmentVariable("DIRECTORIO_LOGS", EnvironmentVariableTarget.Machine) ?? @"E:\COBEFARWEBLOGS\";

            // HANA Database Configuration
            this.cadHana = "Server=172.16.55.36:30015;UserName=B1ADMIN;Password=Passw0rd;";
            //this.schemaHana = "\"BASE_01_02_2025\".";
            this.schemaHana = "\"B1H_COBEFAR_2018\".";
            //this.cadHanaOnPremise = Environment.GetEnvironmentVariable("CNX_HANA_ON_PREMISE", EnvironmentVariableTarget.Machine);
            //this.schemaHanaOnPremise = "\"B1H_COBEFAR_2018\".";
            this.BDsql = Environment.GetEnvironmentVariable("BD_PROYECTO_INTRANET", EnvironmentVariableTarget.Machine);
            this.cadSql = Environment.GetEnvironmentVariable("BD_DEVELOP", EnvironmentVariableTarget.Machine);
            //this.cadSql = Environment.GetEnvironmentVariable("PROYECTO_INTRANET", EnvironmentVariableTarget.Machine);
            this.cadSql2 = Environment.GetEnvironmentVariable("PROYECTO_RESERVA_PICKING", EnvironmentVariableTarget.Machine);
            CadSql3 = Environment.GetEnvironmentVariable("BD_DT", EnvironmentVariableTarget.Machine);
            CadSql4 = Environment.GetEnvironmentVariable("BD_DOCUMENTOS_REGULATORIOS", EnvironmentVariableTarget.Machine);
            this.serverSophos = Environment.GetEnvironmentVariable("SOPHOS", EnvironmentVariableTarget.Machine);

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