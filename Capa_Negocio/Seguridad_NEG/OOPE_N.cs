using Capa_Datos.Seguridad_DAO;
using Capa_Entidad.Seguridad_ENT;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Negocio.Seguridad_NEG
{
    public class OOPE_N
    {
        OOPE_D opeD = new OOPE_D();

        public Dictionary<string, List<OOPE_E>> ListarOperaciones(OOPE_E filtros)
        {
            var lista = opeD.ListarOperaciones(filtros);

            var registrosPorControlador = new Dictionary<string, List<OOPE_E>>();

            foreach (var registro in lista)
            {
                if (!registrosPorControlador.ContainsKey(registro.Controlador))
                {
                    registrosPorControlador[registro.Controlador] = new List<OOPE_E>();
                }
                registrosPorControlador[registro.Controlador].Add(registro);
            }

            return registrosPorControlador;
        }
    }
}