using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Entidad.Compras_ENT.Reportes
{
    public class TranCuadreContrato_E
    {
        public string SocioName { get; set; }
        public SubTranCuadreContrato SubtranLabo { get; set; }
        public List<SubTranCuadreContrato> SubTranProv { get; set; }
        public List<SubTranCuadreContrato> SubTranProvNC{ get; set; }
        public List<SubTranCuadreContrato> SubTranProvNCArt { get; set; }
        public decimal TotalFacturas { get; set; }
        //metodos
        public decimal CalcularTotalFacturas(string SoloSuma=null)
        {
            decimal total = 0.00M;
            foreach (SubTranCuadreContrato str in SubTranProv)
            { 
                if(SoloSuma!=null)
                {
                    if(str.SoloSuma!=null && str.SoloSuma!="Si")
                    {
                        total += str.DocTotal;
                    }
                }
                else { total += str.DocTotal; }
            }
            return total;
        }
        public decimal CalcularTotalNC()
        {
            decimal total = 0.00M;
            foreach (SubTranCuadreContrato str in SubTranProvNC)
            {
                total += str.DocTotal;
            }
            return total;
        }
        public decimal CalcularTotalNCArt()
        {
            decimal total = 0.00M;
            foreach (SubTranCuadreContrato str in SubTranProvNCArt)
            {
                total += str.DocTotal;
            }
            return total;
        }
        public decimal CalcularRebate(decimal rebate)
        {
            return ((CalcularTotalFacturas("No")-CalcularTotalNCArt())* rebate)/100;
        }
        public decimal CalcularTotalDisplays()
        {
            decimal totalcomprado = 0.00M;
            decimal totaldevuelto = 0.00M;
            foreach (SubTranCuadreContrato str in SubTranProv)
            {
                totalcomprado += str.Displays;
            }
            foreach(SubTranCuadreContrato nc in SubTranProvNCArt)
            {
                totaldevuelto += nc.Displays;
            }
            return (totalcomprado-totaldevuelto);
        }
    }
}
