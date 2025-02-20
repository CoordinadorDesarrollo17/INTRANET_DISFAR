using Capa_Datos.AbastecimientoInterno_DAO.TablasExternas;
using Capa_Datos.AbastecimientoInterno_DAO.TablasSql;
using Capa_Entidad;
using Capa_Entidad.AbastecimientoInterno_ENT.Interfaces;
using Capa_Entidad.AbastecimientoInterno_ENT.TablasSql;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Negocio.AbastecimientoInterno_NEG.TablasSql
{
    public class TransferenciaReserva_N
    {
        TransferenciaReserva_D datosTransferencia = new TransferenciaReserva_D();
        public TransferenciaReserva_E RegistrarTransferenciaReserva(TransferenciaReserva_E transferencia,SqlConnection cn)
        {
            TransferenciaReserva_E transferenciaARegistrar = new TransferenciaReserva_E();
            //Asignar datos de cabecera importantes
            transferenciaARegistrar = transferencia;
            //Limpiar el detalle
            transferenciaARegistrar.Detalle = null;

            foreach (var t in transferencia.Detalle)
            {
                if(!string.IsNullOrWhiteSpace(t.UmAlm)  && t.ValorUmAlm>0 && t.QuantityUnidadesCajas >0 && !string.IsNullOrWhiteSpace(t.CodigoUbicacion) && t.TransferenciaReservaId==0 && t.Id==0) {
                    //Solo se envian los datos nuevos a tranferir
                    transferenciaARegistrar.Detalle.Add(t);
                }
            }
            return datosTransferencia.RegistrarTransferenciaReserva(transferenciaARegistrar, cn);
        }
        public TransferenciaReserva_E ObtenerTransferenciaReserva(int docNum)
        {
            return datosTransferencia.ObtenerTransferenciaReserva(docNum);
        }
    }
 }
