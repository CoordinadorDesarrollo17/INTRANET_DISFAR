using Capa_Datos.Ventas_DAO.TablasSql;
using Capa_Entidad.Ventas_ENT.TablasSql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Negocio.Ventas_NEG.TablasSql
{
    public class OCLR_N
    {
        OCLR_D oclrD = new OCLR_D();
        private bool cadenaVacia(string cad)
        {
            if (cad == null || cad.Replace(" ", "").Length == 0) { return true; }
            else return false;
        }
        public List<OCLR_E> listadoRegaloCliente(OCLR_E filtro)
        {
            return oclrD.listadoRegaloCliente(filtro);
        }
        public void registrarClienteRegalo(OCLR_E obj)
        {
            validarNuevoClienteRegalo(obj);
            oclrD.registrarClienteRegalo(obj);
        }
        public OCLR_E buscarClienteRegalo(string CardCode)
        {
            return oclrD.buscarClienteRegalo(CardCode);
        }
        public void editarClienteRegalo(OCLR_E obj)
        {
            validarEditarClienteRegalo(obj);
            oclrD.editarClienteRegalo(obj);
        }
        public bool comprobarDispCliReg(CLR1_E obj)
        {
            return oclrD.ComprobarDispCliReg(obj);
        }
        //validaciones
        public void validarNuevoClienteRegalo(OCLR_E obj)
        {
            OCLR_E bean = buscarClienteRegalo(obj.CardCode);
            if (obj.CardCode == bean.CardCode) { throw new Exception("Cliente ya registrado"); }
            int i = 0;
            foreach (CLR1_E detObj in obj.Det)
            {
                if (cadenaVacia(detObj.Categoria)) { throw new Exception("La categoria en la linea " + (i + 1) + " no puede estar vacia"); }
                if (cadenaVacia(detObj.Tipo)) { throw new Exception("El tipo en la linea " + (i + 1) + " no puede estar vacia"); }
                if (detObj.Cantidad < 0) { throw new Exception("la cantidad debe ser mayor a 0 linea: " + (i + 1)); }
                i++;
            }
        }
        public void validarEditarClienteRegalo(OCLR_E obj)
        {
            int i = 0;
            foreach (CLR1_E detObj in obj.Det)
            {
                if (cadenaVacia(detObj.Categoria)) { throw new Exception("La categoria en la linea " + (i + 1) + " no puede estar vacia"); }
                if (cadenaVacia(detObj.Tipo)) { throw new Exception("El tipo en la linea " + (i + 1) + " no puede estar vacia"); }
                if (detObj.Cantidad < 0) { throw new Exception("la cantidad debe ser mayor a 0 linea: " + (i + 1)); }
                i++;
            }

        }
    }
}
