using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Entidad.ReportesDigemid_ENT.Reportes
{
    public class PreciosOpm_E
    {
        public int Orden { get; set; }
        public string ItemCode { get; set; }
        public string Dscription { get; set; }
        public decimal PrecioCajas { get; set; }
        public string MnfSerial { get; set; }
        public int DocEntry { get; set; }
        public string ObjType { get; set; }
        // parametros calculados
        public decimal PreMinimo { get; set; }
        public decimal PreMaximo { get; set; }
        public decimal PreMediano { get; set; }
        public decimal PrePromedio { get; set; }
        // metodos
        public static decimal PrecioMinimo(List<PreciosOpm_E> lista)
        {
            return lista.Min(x => x.PrecioCajas);
        }
        public static decimal PrecioMaximo(List<PreciosOpm_E> lista)
        {
            return lista.Max(x => x.PrecioCajas);
        }
        public static decimal CalculoMediana(List<PreciosOpm_E> listaItemRegistro)
        { 
            //asumo que la lista tiene el mismo producto y mismo registro
            decimal med = 0.00M;
            int totalPre = listaItemRegistro.Count();
            int posIni = 0;int posFin = 0;
            if(totalPre%2==0 && totalPre>0)
            {
                posIni = (totalPre / 2) - 1;
                posFin = posIni + 1;
                med = (listaItemRegistro[posIni].PrecioCajas + listaItemRegistro[posFin].PrecioCajas) / 2;
            }
            else
            {
                posIni = (totalPre - 1) / 2;
                med = listaItemRegistro[posIni].PrecioCajas;
            }
            return med;
        }
        public static decimal CalculoPromedio(List<PreciosOpm_E> listaItemRegistro)
        {
            //asumo que la lista tiene el mismo producto y mismo registro
            decimal prom = 0.00M;
            int totalPre = listaItemRegistro.Count();
            if (totalPre == 0) { return 0.00M; }
            decimal sumaPre = listaItemRegistro.Sum(x => x.PrecioCajas);
            prom = sumaPre / totalPre;
            return prom;
        }
        public static List<PreciosOpm_E> ListaConPreciosFinales(List<PreciosOpm_E> lista)
        {
            List<PreciosOpm_E> l = new List<PreciosOpm_E>();
            string auxItemCode = "";string auxMnfSerial = "";
            List<PreciosOpm_E> sublista = new List<PreciosOpm_E>();
            for(int i = 0;i<lista.Count();i++)
            {
                try
                {
                    if (i == 0)
                    {
                        auxItemCode = lista[i].ItemCode; auxMnfSerial = lista[i].MnfSerial;
                    }
                    if (lista[i].ItemCode == auxItemCode && lista[i].MnfSerial == auxMnfSerial)
                    {
                        sublista.Add(lista[i]);
                    }
                    else
                    {
                        PreciosOpm_E auxPre = new PreciosOpm_E();
                        auxPre = sublista[0];
                        auxPre.PreMinimo = PrecioMinimo(sublista);
                        auxPre.PreMaximo = PrecioMaximo(sublista);
                        auxPre.PreMediano = CalculoMediana(sublista);
                        auxPre.PrePromedio = CalculoPromedio(sublista);
                        l.Add(auxPre);

                        sublista = new List<PreciosOpm_E>();
                        auxPre = new PreciosOpm_E();

                        auxItemCode = lista[i].ItemCode; auxMnfSerial = lista[i].MnfSerial;
                        if (lista[i].ItemCode == auxItemCode && lista[i].MnfSerial == auxMnfSerial)
                        {
                            sublista.Add(lista[i]);
                        }
                    }
                    if (i == (lista.Count() - 1))
                    {
                        if (sublista.Count > 0)
                        {
                            PreciosOpm_E auxPre = new PreciosOpm_E();
                            auxPre = sublista[0];
                            auxPre.PreMinimo = PrecioMinimo(sublista);
                            auxPre.PreMaximo = PrecioMaximo(sublista);
                            auxPre.PreMediano = CalculoMediana(sublista);
                            l.Add(auxPre);
                            sublista = new List<PreciosOpm_E>();
                            auxPre = new PreciosOpm_E();
                        }
                        else
                        {
                            sublista.Add(lista[i]);
                            PreciosOpm_E auxPre = new PreciosOpm_E();
                            auxPre = sublista[0];
                            auxPre.PreMinimo = PrecioMinimo(sublista);
                            auxPre.PreMaximo = PrecioMaximo(sublista);
                            auxPre.PreMediano = CalculoMediana(sublista);
                            l.Add(auxPre);
                            sublista = new List<PreciosOpm_E>();
                            auxPre = new PreciosOpm_E();
                        }
                    }
                }
                catch { }
            }
            return l;
        }
    }
}
