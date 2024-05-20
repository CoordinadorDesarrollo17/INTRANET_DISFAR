using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Capa_Datos.Ventas_DAO;
using Capa_Entidad.Ventas_ENT;

namespace Capa_Negocio.Ventas_NEG
{
    public class LibroSaldo_N
    {
        LibroSaldo_D ld = new LibroSaldo_D();
        public List<LibroSaldo_E> listarLibrosSaldo(LibroSaldo_E li)
        {
            return ld.listarLibrosSaldo(li);
        }
        public LibroSaldo_E obtenerLibroSaldo(string CardCode)
        {
            return ld.obtenerLibroSaldo(CardCode);
        }
        public int crearLibroSaldo(LibroSaldo_E l)
        {
            return ld.crearLibroSaldo(l);
        }
        public List<DetLibroSaldo_E> obtenerDetLibroSaldo(string CardCode)
        {
            return ld.obtenerDetLibroSaldo(CardCode);
        }
        public int agregarDetLibroSaldo(DetLibroSaldo_E d)
        {
            return ld.agregarDetLibroSaldo(d);
        }
    }
}
