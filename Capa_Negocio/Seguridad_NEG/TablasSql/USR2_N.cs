using Capa_Datos.Seguridad_DAO;
using Capa_Entidad.Seguridad_ENT;
using System;
namespace Capa_Negocio.Seguridad_NEG.TablasSql
{
    public class USR2_N
    {
        USR2_D usr2D = new USR2_D();
        public void IntentosUsoOp(int DocEntry,int IdOperacion)
        {
            USR2_E obj = usr2D.buscarIntenRolUsu(DocEntry, IdOperacion);
            if (!DateTime.Now.ToString("yyyy-MM-dd").Equals(obj.Dia.ToString("yyyy-MM-dd")))
            {
                usr2D.reiniciarUsos(DocEntry, IdOperacion);
            }
            if (usr2D.buscarIntenRolUsu(DocEntry, IdOperacion).UsosDia <= 0) { throw new Exception("Ya no tiene intentos disponibles"); }
            usr2D.UsarIntento(DocEntry, IdOperacion);
        }
    }
}
