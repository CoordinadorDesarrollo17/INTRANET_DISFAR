using Capa_Datos.General_DAO.TablasSql;
using Capa_Entidad.General_ENT.TablasSql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Negocio.General_NEG.TablasSql
{
    public class OUR2_N
    {
        OUR2_D oD = new OUR2_D();
        public List<OUR2_E> Visualizar(string filename)
        {
            return oD.Visualizar(filename);
        }
        public List<OUR2_E> Listar(string accion)
        {
            string orderBy = "";

            if (accion == "registrar")
            {
                orderBy = "ORDER BY Id DESC";
            }

            return oD.Listar(orderBy);
        }
        public OUR2_E Obtener(int Id,string Destino)
        {
            return oD.Obtener(Id, Destino);
        }
        public string Registrar(OUR2_E bean)
        {
            var result = Obtener(bean.IdCourier, bean.Destino);
            if (bean.IdCourier==0) { return "Debe seleccionar Agencia"; }
            if (string.IsNullOrWhiteSpace(bean.Destino)) { return "Debe seleccionar Destino"; }
            if (bean.TarifaKg<=0) { return "Debe ingresar Tarifa valida"; }
            if (bean.PrecioBase<=0) { return "Debe ingresar Precio Base valido"; }
            return oD.Registrar(bean); 
        }
        public List<OUR2_E> Buscar(OUR2_E datos)
        {
           
            return oD.Buscar(datos);
        }
        public string Editar(OUR2_E bean)
        {
            if (bean.IdCourier == 0) { return  "Debe seleccionar Agencia"; }
            if (string.IsNullOrWhiteSpace(bean.Destino)) { return "Debe seleccionar Destino"; }
            if (bean.TarifaKg <= 0) { return "Debe ingresar Tarifa valida"; }
            if (bean.PrecioBase <= 0) { return "Debe ingresar Precio Base valido"; }
            return oD.Editar(bean);
        }
        public void Eliminar(int Id)
        {

            oD.Eliminar(Id);
        }
    }

}
    