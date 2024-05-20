using Capa_Datos.Almacen_DAO.TablasSql;
using Capa_Entidad.Almacen_ENT.TablasSql;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;

namespace Capa_Negocio.Almacen_NEG.TablasSql
{
    public class ORPD_N
    {
        ORPD_D orpdD = new ORPD_D();

        public List<Capa_Entidad.Almacen_ENT.ReportesSql.RptCorreoDevolucion_E> RptCorreoDevolucion(int DocEntry)
        {
            return orpdD.RptCorreoDevolucion(DocEntry);
        }
        public List<ORPD_E> ListarDevoluciones(ORPD_E Devolucion)
        {
            return orpdD.ListarDevoluciones(Devolucion);
        }
        public ORPD_E ObtenerDevolucion(int DocEntry)
        {
            return orpdD.ObtenerDevolucion(DocEntry);
        }
        public int RegistrarDevolucion(ORPD_E devolucion, List<RPD1_E> detalleDevolucion)
        {
            if (string.IsNullOrEmpty(devolucion.CardCode) || string.IsNullOrEmpty(devolucion.CardName) || string.IsNullOrEmpty(devolucion.WhsCode))
            {
                throw new Exception("El código de cliente, nombre de cliente y el almacén, son campos obligatorios.");
            }

            if (detalleDevolucion == null || detalleDevolucion.Count <= 0)
            {
                throw new Exception("La devolución a crearse debe contener al menos 1 artículo en su detalle.");
            }
            validarCantidadDetalleDevolucion(detalleDevolucion);
            return orpdD.RegistrarDevolucion(devolucion, detalleDevolucion);
        }
        public int EditarDevolucion(ORPD_E devolucion, List<RPD1_E> detalleDevolucion)
        {
            if (detalleDevolucion == null || detalleDevolucion.Count <= 0)
            {
                throw new Exception("La devolución a crearse debe contener al menos 1 artículo en su detalle.");
            }
            validarCantidadDetalleDevolucion(detalleDevolucion);
            return orpdD.EditarDevolucion(devolucion, detalleDevolucion);
        }
        public int CambiarEstadoDevolucion(ORPD_E devolucion, string tipoMantenimiento)
        {
            ORPD_E obj = ObtenerDevolucion(devolucion.DocEntry);
            switch (tipoMantenimiento)
            {
                case "AA":
                    if (!obj.Estado.Equals("PENDIENTE DE RECOJO")) { throw new Exception("La devolución no es apta para la anulacion"); }
                    break;
                case "RR":
                    if (!obj.Estado.Equals("RECOGIDO")) { throw new Exception("La devolución no es apta para revertir recojo"); }
                    break;
                case "NC":
                    if (!obj.Estado.Equals("RECOGIDO")) { throw new Exception("La devolución no es apta para aplicar NC"); }
                    break;
            }
            return orpdD.CambiarEstadoDevolucion(devolucion, tipoMantenimiento);
        }

        public List<Capa_Entidad.Almacen_ENT.ReportesSql.RptHistoricoDevoluciones_E> ExportarExcelDevoluciones(Capa_Entidad.Almacen_ENT.ReportesSql.RptFiltrosHistoricoDevoluciones_E Devolucion)
        {
            return orpdD.ExportarExcelDevoluciones(Devolucion);
        }
        public void validarCantidadDetalleDevolucion(List<RPD1_E> detalleDevolucion)
        {
            //Validacion de cantidades ingresadas por ItemCode y BatchNum

            if (detalleDevolucion != null && detalleDevolucion.Count > 0)
            {  //si el detalle devolucion no esta vacio
                foreach (var obj in detalleDevolucion)
                {
                    if (string.IsNullOrEmpty(obj.RefFactura) || obj.RefFactura.Length!=16) { throw new Exception("El producto " + obj.ItemName.Substring(0, 10) + "-" + obj.BatchNum + " no tiene una factura o esta mal ingresada"); }
                    if (obj.Quantity <= 0) { throw new Exception("El producto " + obj.ItemName.Substring(0, 10) + "-" + obj.BatchNum + " tiene una cantidad no valida"); }
                    if (string.IsNullOrEmpty(obj.BatchNum)) { throw new Exception("El producto " + obj.ItemName.Substring(0, 10) + "tiene un lote no valido"); }
                    if (string.IsNullOrEmpty(obj.ExpDate)) { throw new Exception("El producto " + obj.ItemName.Substring(0, 10) + "-" + obj.BatchNum + " tiene una fec venc no valida"); }
                    //solo para articulos existentes
                    if (obj.MaxQuantity > 0)
                    {
                        decimal MaxQuantityPermitidaPz = obj.MaxQuantity * obj.NumInBuyKey;
                        decimal MaxQuantityPermitidaOIBTPz = obj.MaxQuantityOIBT * obj.NumInBuyKey;
                        decimal CantidadIngresadaPz = obj.Quantity * obj.NumInBuy;
                        if (MaxQuantityPermitidaOIBTPz > 0)
                        {
                            if (CantidadIngresadaPz > MaxQuantityPermitidaPz || CantidadIngresadaPz > MaxQuantityPermitidaOIBTPz) { throw new Exception("El producto " + obj.ItemName.Substring(0, 10) + "-" + obj.BatchNum + " excede la cantidad permitida"); }
                        }
                        else
                        {
                            if (CantidadIngresadaPz > MaxQuantityPermitidaPz) { throw new Exception("El producto " + obj.ItemName.Substring(0, 10) + "-" + obj.BatchNum + " excede la cantidad permitida"); }
                        }
                    }

                }

                // Obtener elementos duplicados agrupados por ItemCode y BatchNum

                var elementosDuplicadosOrdenados = detalleDevolucion
               .GroupBy(x => new { x.ItemCode, x.BatchNum })
               .Where(group => group.Count() > 1)
               .SelectMany(group => group)
               .OrderBy(x => x.ItemCode)
               .ThenBy(x => x.BatchNum)
               .ToList();

                if (elementosDuplicadosOrdenados.Count() > 0)
                {
                    // Separar los elementos duplicados en listas según ItemCode y BatchNum
                    var elementosSeparadosPorGrupo = elementosDuplicadosOrdenados
                    .GroupBy(x => new { x.ItemCode, x.BatchNum })
                    .ToDictionary(group => group.Key, group => group.ToList());
                    // Acceder a las listas por cada grupo
                    foreach (var grupo in elementosSeparadosPorGrupo)
                    {
                        decimal SumCantIngPz = 0; decimal MaxQuantityTotal = 0;

                        foreach (var obj in grupo.Value)
                        {
                            //solo para articulos existentes
                            if (obj.MaxQuantity > 0) { 
                            //suma y convierte la cantidad ingresada en piezas
                            SumCantIngPz += obj.Quantity * obj.NumInBuy;

                            if (obj.MaxQuantityOIBT > 0)
                            {
                                MaxQuantityTotal = obj.MaxQuantityOIBT;
                            }
                            else { MaxQuantityTotal = obj.MaxQuantity; }
                            decimal MaxQuantityPermitidaPz = obj.NumInBuyKey * MaxQuantityTotal;
                            if (SumCantIngPz > MaxQuantityPermitidaPz)
                            {
                                throw new Exception("El producto " + obj.ItemName.Substring(0, 10) + "-" + obj.BatchNum + " supera la cantidad maxima :" + MaxQuantityTotal);
                            }
                            }
                        }
                    }

                }
            }
        }

         public bool VerificarExistenciaDevolucion(Capa_Entidad.Almacen_ENT.ReportesSql.RptFiltrosHistoricoDevoluciones_E filtros)
        {
			return orpdD.VerificarExistenciaDevolucion(filtros);
		}
        
    }
}
