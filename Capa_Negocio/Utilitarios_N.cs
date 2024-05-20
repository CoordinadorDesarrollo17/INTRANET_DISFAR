using Capa_Datos;
using CrystalDecisions.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Negocio
{
    public class Utilitarios_N
    {
        private string serverSql { get; set; }
        private string userSql { get; set; }
        private string passwordSql { get; set; }
        private string schemaSql { get; set; }
        private string cadSql { get; set; }
        public string directorioFileServer { get; set; } 

        Utilitarios uti = new Utilitarios();
        public void registrarLog(string user, string mensaje, int operacion, string ip, string equipo)
        {
            uti.registrarLog(user, mensaje, operacion, ip,equipo);
        }
        public ConnectionInfo getConexion()
        {
            ConnectionInfo infocon = new ConnectionInfo();
            infocon.ServerName = serverSql;
            infocon.DatabaseName = schemaSql;
            infocon.UserID = userSql;
            infocon.Password = passwordSql;
            return infocon;
        }
        public Utilitarios_N()
        {
            serverSql = uti.serverSql;userSql = uti.userSql;passwordSql = uti.passwordSql;
            schemaSql = uti.BDsql;cadSql = uti.cadSql;directorioFileServer = uti.directorioFileServer;
        }
    }
}
