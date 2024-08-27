using System.Threading;
using System;
using Capa_Datos;
using Capa_Datos.SocioNegocios_DAO.Tablas;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;
using Capa_Negocio.Seguridad_NEG;

namespace Capa_Usuario
{
    public class HostedService
    {
        private Timer _timer;
        int status = -1;

        public void StartAsync()
        {
            _timer = new Timer(migracion, null, TimeSpan.Zero, TimeSpan.FromDays(1));
            _timer = new Timer(inactivarUsuario, null, TimeSpan.Zero, TimeSpan.FromDays(5));
            _timer = new Timer(cambiarEstadoNCAplicada, null, TimeSpan.Zero, TimeSpan.FromDays(1));
        }

        public void StopAsync()
        {
            _timer?.Change(Timeout.Infinite, 0);
        }

        public void migracion(object state)
        {
            OCRD_D ocrdD = new OCRD_D();
            ocrdD.Migrar();
        }
        public void inactivarUsuario(object state)
        {
            Usuario_N ousrN = new Usuario_N();
            var listaUsuariosActivos = ousrN.ListaUsuarios(new Capa_Entidad.Seguridad_ENT.Usuario_E { Activo = 1 });
            foreach (var f in listaUsuariosActivos)
            {
                if (f.DiferenciaDias > 50) { ousrN.Inactivar(f); }
            }
        }

        public void cambiarEstadoNCAplicada(object state)
        {
                Capa_Negocio.Almacen_NEG.Tablas.ORPD_N orpdN_Hana = new Capa_Negocio.Almacen_NEG.Tablas.ORPD_N();
                Capa_Negocio.Almacen_NEG.TablasSql.ORPD_N orpdN = new Capa_Negocio.Almacen_NEG.TablasSql.ORPD_N();
                List<Capa_Entidad.Almacen_ENT.TablasSql.ORPD_E> listaDev = orpdN.ListarDevoluciones(new Capa_Entidad.Almacen_ENT.TablasSql.ORPD_E { Estado = "RECOGIDO" }).OrderBy(x =>x.DocEntry).ToList();
                foreach (Capa_Entidad.Almacen_ENT.TablasSql.ORPD_E obj in listaDev)
                {

                    Capa_Entidad.Almacen_ENT.TablasSql.ORPD_E Dev1 = orpdN.ObtenerDevolucion(obj.DocEntry);
                    bool sonTodasIguales = Dev1.DetalleDevolucion.All(x => x.RefFactura == Dev1.DetalleDevolucion[0].RefFactura);
                    DateTime fechaObjeto = DateTime.ParseExact(Dev1.FechaDevolucion, "dd/MM/yyyy", null);
                    string FechaFormateada = fechaObjeto.ToString("yyyy-MM-dd");

                    if (sonTodasIguales)
                    {
                        //si la lista de devoluciones tiene en su detallado factura unica, buscamos el archivo en estado Cerrado a nivel de SAP y la devolucion pasa a NC APLICADA
                        orpdN_Hana.BuscarDevolucion(Dev1, FechaFormateada, Dev1.CardCode, Dev1.DetalleDevolucion[0].RefFactura);

                    }
                    else
                    {
                        //si la lista de devoluciones tiene en su detallado facturas diferentes:

                        orpdN_Hana.BuscarDevolucion(Dev1, FechaFormateada, Dev1.CardCode, null);
                    }


                }

    }

        public void Dispose()
        {
            _timer.Dispose();
        }
    }
}

