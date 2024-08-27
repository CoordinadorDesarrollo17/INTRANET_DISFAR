using Capa_Datos;
using Capa_Negocio.Seguridad_NEG;
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

        Utilitarios Utilitarios_D = new Utilitarios();

        public void RegistrarLog(string user, string mensaje, int operacion, string ip, string equipo)
        {
            Utilitarios_D.RegistrarLog(user, mensaje, operacion, ip,equipo);
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
            serverSql = Utilitarios_D.serverSql; userSql = Utilitarios_D.userSql; passwordSql = Utilitarios_D.passwordSql;
            schemaSql = Utilitarios_D.BDsql; cadSql = Utilitarios_D.cadSql; directorioFileServer = Utilitarios_D.directorioFileServer;
        }
    }
}
