using Capa_Datos.Rutas_DAO.TablasSql;
using Capa_Entidad.Rutas_ENT.TablasSql;
using System.Collections.Generic;

namespace Capa_Negocio.Rutas_NEG.TablasSql
{
    public class TEMP_RRU01_N
    {
        TEMP_RRU01_D datos = new TEMP_RRU01_D();
        public string Registrar(List<TEMP_RRU01_E> dataList)
        {
            return datos.Registrar(dataList);
        }
        public List<string> Listar(int DocEntryTicket)
        {
            return datos.Listar(DocEntryTicket);
        }
        public List<TEMP_RRU01_E> Obtener(int DocEntryTicket)
        {
            return datos.Obtener(DocEntryTicket);
        }
        public void EditarImpreso(string NumAtCard, int DocEntryTicket)
        {
            datos.EditarImpreso(NumAtCard, DocEntryTicket);
        }
        public string ConsultarImpreso(string NumAtCard)
        {
            string Mensaje = "";
            if (datos.ConsultarImpreso(NumAtCard) == 1)
            {
                Mensaje = "El doc " + NumAtCard + " ya ha sido impreso anteriormente";
            }
            return Mensaje;
        }
        public void Eliminar(int DocEntryTicket)
        {
            datos.Eliminar(DocEntryTicket);
        }
    }
}
