using Capa_Datos.Rutas_DAO.TablasSql;
using Capa_Entidad.Rutas_ENT.TablasSql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Negocio.Rutas_NEG.TablasSql
{
    public class RRU1_N
    {
        RRU1_D rru1D = new RRU1_D(); 
        RRU11_D rru11D = new RRU11_D(); 
        ORRU_N orruN = new ORRU_N();

        public void ActualizarNroCajas(int DocEntry, int BaseLinea)
        {
            rru1D.ActualizarNroCajas(DocEntry, BaseLinea);
        }

        public RRU1_E buscarRRU1(int DocEntry, int Linea)
        {
            return rru1D.buscarRRU1(DocEntry, Linea);
        }
        public void entregarRRU1(RRU1_E o)
        {
            validarEntDetReparto(o);
            rru1D.entregarRRU1(o);
        }

        public void validarEntDetReparto(RRU1_E o)
        {
            RRU1_E rru1E = buscarRRU1(o.DocEntry, o.Linea);
            ORRU_E orruE = orruN.obtenerOrdenDeRuta(o.DocEntry);
            if (orruE.Estado != "ENVIADO") { throw new Exception("El reparto no esta enviado"); }
            if (rru1E.Estado != "ENVIADO") { throw new Exception("Ya no puedes entregar este item"); }

            if (!(o.TempF1 >= 15 && o.TempF1 <= 25)) { throw new Exception("Temp1 Final no cumple con el rango valido"); }
            if (!(o.HumedF1 >= 50 && o.HumedF1 <= 80)) { throw new Exception("Humed1 Final no cumple con el rango valido"); }
            if (!(o.TempF2 >= 15 && o.TempF2 <= 25)) { throw new Exception("Temp2 Final no cumple con el rango valido"); }
            if (!(o.HumedF2 >= 50 && o.HumedF2 <= 80)) { throw new Exception("Humed2 Final no cumple con el rango valido"); }

            if (orruE.TipoRuta == "VG")
            {
                if (o.Archivo == null)
                {
                    throw new Exception("Debe subir foto Evidencia");
                }
            }
            if (o.Archivo != null)
            {
                if (!(o.Archivo.ContentType == "image/gif" ||
                    o.Archivo.ContentType == "image/png" ||
                    o.Archivo.ContentType == "image/jpeg"))
                {
                    throw new Exception("Formatos válidos: .gif, .png, .jpeg");
                }
                if (o.Archivo.ContentLength > 15485760) { throw new Exception("No puedes cargar un archivo superior a 15 MB"); }
            }
            o.NroSap = rru1E.NroSap;
        }
    }
}
