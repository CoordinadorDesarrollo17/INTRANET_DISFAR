using Capa_Datos.Almacen_DAO.TablasSql;
using Capa_Entidad.Almacen_ENT.TablasSql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Negocio.Almacen_NEG.TablasSql
{
	public class OIPE_N
	{
		OIPE_D oD = new OIPE_D();

		public List<OIPE_E> listarPeriodosInventario(OIPE_E filtro)
		{
			return oD.ListarPeriodosInventario(filtro);
		}
		public int Registrar(OIPE_E obj)
		{
			validarNuevoPeriodo(obj);
			return oD.Registrar(obj);
		}
		public OIPE_E Buscar(int DocEntry, bool ConArticulos = false)
		{
			return oD.Buscar(DocEntry, ConArticulos);
		}
		public int Seleccionar(OIPE_E obj)
		{
			return oD.Seleccionar(obj);
		}
		public int Editar(OIPE_E obj)
		{
			validarNuevoPeriodo(obj);

			OIPE_E objAux = Buscar(obj.DocEntry);

			if (OIPE_E.PeriodoSeleccionado != null)
			{
				if (string.IsNullOrWhiteSpace(OIPE_E.PeriodoSeleccionado.DocEntry.ToString())) { throw new Exception("No existe un periodo seleccionado"); }
				if (objAux.DocEntry == OIPE_E.PeriodoSeleccionado.DocEntry) { throw new Exception("No se puede editar periodo Seleccionado"); }
			}

			if (objAux.EstadoDatos == "Cargado")
			{
				throw new Exception("No se puede editar periodo con estado datos cargado");
			}

			return oD.Editar(obj);
		}
		public int Cerrar(OIPE_E obj)
		{
			OIPE_E objAux = Buscar(obj.DocEntry);
			if (objAux.Estado != "Abierto") { throw new Exception("Solo se puede cerrar periodo en estado Abierto"); }
			if (objAux.EstadoDatos != "Cargado") { throw new Exception("Sole se puede cerrar con EstadoDatos Cargado"); }
			return oD.Cerrar(obj);
		}
		public int RevertirCerrar(int DocEntry, string Operario)
		{
			OIPE_E objAux = Buscar(DocEntry);
			if (objAux.Estado != "Cerrado") { throw new Exception("Sole se puede revertir periodo cerrado"); }
			return oD.RevertirCerrar(DocEntry, Operario);
		}
		public int MigrarArticulos(OIPE_E obj)
		{
			OIPE_E o = Buscar(obj.DocEntry);
			if (o.Estado == "Cerrado") { throw new Exception("No se puede hacer operaciones en periodo cerrado"); }
			if (o.EstadoDatos == "Cargado") { throw new Exception("No se puede migrar en periodo con estado datos cargado"); }
			return oD.MigrarArticulos(obj);
		}
		public int CargarArticulosMigrados(OIPE_E obj)
		{
			OIPE_E o = Buscar(obj.DocEntry);
			if (o.Estado == "Cerrado") { throw new Exception("No se puede hacer operaciones en periodo cerrado"); }
			if (o.EstadoDatos != "Migrado") { throw new Exception("solo se puede cargar en periodo con estado datos migrado"); }
			return oD.CargarArticulosMigrados(obj);
		}
		// validaciones pre
		public void validarNuevoPeriodo(OIPE_E obj)
		{
			if (string.IsNullOrWhiteSpace(obj.Descripcion)) { throw new Exception("Debe llenar Descripcion"); }
			if (string.IsNullOrWhiteSpace(obj.FecIni)) { throw new Exception("Debe llenar fecha inicio"); }
			if (string.IsNullOrWhiteSpace(obj.FecFin)) { throw new Exception("Debe llenar fecha fin"); }
			if (obj.DetAlmacenes == null || obj.DetAlmacenes.Count == 0) { throw new Exception("El periodo debe tener al menos 1 almacen"); }
		}
	}
}