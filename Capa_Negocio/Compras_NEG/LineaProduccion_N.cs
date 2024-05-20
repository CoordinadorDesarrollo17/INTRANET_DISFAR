using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Capa_Datos.Compras_DAO;
using Capa_Entidad.Compras_ENT;
using Capa_Entidad.Almacen_ENT;
using Capa_Entidad.Almacen_ENT.Tablas;
using Capa_Entidad.SocioNegocios_ENT.Tablas;

namespace Capa_Negocio.Compras_NEG
{
    public class LineaProduccion_N
    {
        LineaProduccion_D lpD = new LineaProduccion_D();
        public List<LineaProduccion_E> listarLineasProduccion(LineaProduccion_E lf)
        {
            return lpD.listarLineasProduccion(lf);
        }
        public List<OMRC_E> listarFabricantes()
        {
            return lpD.listarFabricantes();
        }
        public List<OCRD_E> listarProveedores()
        {
            return lpD.listarProveedores();
        }
        public List<OITM_E> listarArticulos(int idLabVal, string idTipoVal)
        {
            return lpD.listarArticulos(idLabVal, idTipoVal);
        }
        public int CrearLineaProduccion(LineaProduccion_E l)
        {
            if (l.Descripcion == null) { throw new Exception("No lleno descripcion"); }
            if (l.Fabricante == null || l.Fabricante.FirmCode <= 0) { throw new Exception("No eligio Fabricante"); }
            if (l.Det.Count(p => p.ItemCode != null && p.ItemCode != "") <= 0) { throw new Exception("Debe elegir un grupo de articulos"); }
            return lpD.CrearLineaProduccion(l);
        }
        public int EditarLineaProduccion(LineaProduccion_E l)
        {
            return lpD.EditarLineaProduccion(l);
        }
        public LineaProduccion_E obtenerLineaProduccion(int id)
        {
            return lpD.obtenerLineaProduccion(id);
        }
        public int EliminarLineaProduccion(int id)
        {
            return lpD.EliminarLineaProduccion(id);
        }
        //*******infos
        public string infoListarArticulos(List<OITM_E> lista)
        {
            return lpD.infoListarArticulos(lista);
        }
        public string infoListarArticulos2(List<OITM_E> lista)
        {
            return lpD.infoListarArticulos2(lista);
        }
        public string infoListarArticulosManual(List<OITM_E> lista)
        {
            return lpD.infoListarArticulosManual(lista);
        }
        //*********validaciones*********
    }
}
