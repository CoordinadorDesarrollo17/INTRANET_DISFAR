using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Capa_Datos;
using Capa_Negocio.Seguridad_NEG;

namespace Capa_Usuario
{
    public class HostedService : IDisposable
    {
        private Timer _timerInactivarUsuario;
        private Timer _timerCambiarEstadoNC;

        public void StartAsync()
        {
            _timerInactivarUsuario = new Timer(InactivarUsuario, null, TimeSpan.Zero, TimeSpan.FromDays(5));
            _timerCambiarEstadoNC = new Timer(CambiarEstadoNCAplicada, null, TimeSpan.Zero, TimeSpan.FromDays(1));
        }

        public void StopAsync()
        {
            _timerInactivarUsuario?.Change(Timeout.Infinite, 0);
            _timerCambiarEstadoNC?.Change(Timeout.Infinite, 0);
        }

        public void InactivarUsuario(object state)
        {
            try
            {
                Usuario_N ousrN = new Usuario_N();
                var listaUsuariosActivos = ousrN.ListaUsuarios(new Capa_Entidad.Seguridad_ENT.Usuario_E { Activo = 1 });
                foreach (var f in listaUsuariosActivos)
                {
                    if (f.DiferenciaDias > 50)
                    {
                        ousrN.Inactivar(f);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.RegistrarError(ex, "Error inesperado en HostedService - InactivarUsuario()");
            }
        }

        public void CambiarEstadoNCAplicada(object state)
        {
            try
            {
                Capa_Negocio.Almacen_NEG.Tablas.ORPD_N orpdN_Hana = new Capa_Negocio.Almacen_NEG.Tablas.ORPD_N();
                Capa_Negocio.Almacen_NEG.TablasSql.ORPD_N orpdN = new Capa_Negocio.Almacen_NEG.TablasSql.ORPD_N();

                List<Capa_Entidad.Almacen_ENT.TablasSql.ORPD_E> listaDev = orpdN
                    .ListarDevoluciones(new Capa_Entidad.Almacen_ENT.TablasSql.ORPD_E { Estado = "RECOGIDO" })
                    .OrderBy(x => x.DocEntry)
                    .ToList();

                foreach (var obj in listaDev)
                {
                    var Dev1 = orpdN.ObtenerDevolucion(obj.DocEntry);
                    if (Dev1.FechaDevolucion == null)
                        continue;

                    bool sonTodasIguales = Dev1.DetalleDevolucion.All(x => x.RefFactura == Dev1.DetalleDevolucion[0].RefFactura);
                    DateTime fechaObjeto = DateTime.ParseExact(Dev1.FechaDevolucion, "dd/MM/yyyy", null);
                    string FechaFormateada = fechaObjeto.ToString("yyyy-MM-dd");

                    if (sonTodasIguales)
                    {
                        orpdN_Hana.BuscarDevolucion(Dev1, FechaFormateada, Dev1.CardCode, Dev1.DetalleDevolucion[0].RefFactura);
                    }
                    else
                    {
                        orpdN_Hana.BuscarDevolucion(Dev1, FechaFormateada, Dev1.CardCode, null);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.RegistrarError(ex, "Error inesperado en HostedService - CambiarEstadoNCAplicada()");
            }
        }

        public void Dispose()
        {
            _timerInactivarUsuario?.Dispose();
            _timerCambiarEstadoNC?.Dispose();
        }
    }
}
