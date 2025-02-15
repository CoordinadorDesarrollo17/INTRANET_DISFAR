using Capa_Entidad;
using Capa_Entidad.Seguridad_ENT;
using Capa_Negocio.Seguridad_NEG;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace Capa_Negocio
{
    public class Helpers
    {
        public string SanitizarTexto(string texto)
        {
            // Escapar comillas simples en SQL
            texto = texto.Replace("'", "''");

            // Texto con espacios, vocales con acentos, apostrofe, guion y punto: áéíóúü'-.
            texto = Regex.Replace(texto, @"[^\w\sáéíóúü'\-.ñ]", string.Empty);

            return texto;
        }

        // Devolverá true si todos los caracteres en la cadena son números y false en caso contrario.
        public bool EsNumero(string cadena)
        {
            foreach (char caracter in cadena)
            {
                if (!char.IsDigit(caracter))
                {
                    return false;
                }
            }
            return true;
        }

        public bool EsCorreo(string correoElectronico)
        {
            const string expresionRegular = @"^[a-zA-Z0-9.!#$%&'*+/=?^_`{|}~-]+@[a-zA-Z0-9-]+(?:\.[a-zA-Z0-9-]+)*$";

            if (Regex.IsMatch(correoElectronico, expresionRegular))
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        public bool ContieneCaracterEspecial(string input)
        {
            // Expresión regular para verificar caracteres especiales
            string pattern = @"[!@#$%^&*(),.?""{}|<>]";

            // Crear la instancia de Regex
            Regex regex = new Regex(pattern);

            // Retorna true si encuentra al menos un carácter especial
            return regex.IsMatch(input);
        }

        public Helper_E CrearRespuestaError(string mensaje)
        {
            return new Helper_E
            {
                Mensaje = mensaje,
                IconoSweetAlert = "error"
            };
        }
    }
}