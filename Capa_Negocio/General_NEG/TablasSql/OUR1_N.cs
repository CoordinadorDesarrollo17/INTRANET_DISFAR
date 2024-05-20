using Capa_Datos.General_DAO.TablasSql;
using Capa_Entidad.General_ENT.TablasSql;
using System;
using System.Collections.Generic;

namespace Capa_Negocio.General_NEG.TablasSql
{
    public class OUR1_N
    {
        OUR1_D oD = new OUR1_D();
        public List<OUR1_E> Listar()
        {
            return oD.Listar();
        }
        
        public void Registrar(OUR1_E bean)
        {

            oD.Registrar(bean);
        }
        public void Eliminar(int Id)
        {

            oD.Eliminar(Id);
        }
        public void Validar(OUR1_E bean)
        {
            if (string.IsNullOrEmpty(bean.Calle)) { throw new Exception("Debe ingresar calle"); }
            if (string.IsNullOrEmpty(bean.Distrito)) { throw new Exception("Debe seleccionar distrito"); }
            if (string.IsNullOrEmpty(bean.Provincia)) { throw new Exception("Debe seleccionar provincia"); }
            if (string.IsNullOrEmpty(bean.Departamento)) { throw new Exception("Debe seleccionar departamento"); }
        }
    }

        /*public List<OOFI_E> Buscar (string Departamento)
        {
            int min =0, max=0;
            switch (Departamento) {
                case "Amazonas":
                    min = 10101; max = 10707; break;
                case "Áncash":
                    min = 20101; max = 22008; break;
                case "Apurímac":
                    min = 30101; max = 30714; break;
                case "Arequipa":
                    min = 40101; max = 40811; break;
                case "Ayacucho":
                    min = 50101; max = 51108; break;
                case "Cajamarca":
                    min = 60101; max = 61311; break;
                case "Callao":
                    min = 70101; max = 70107; break;
                case "Cusco":
                    min = 80101; max = 81307; break;
                case "Huancavelica":
                    min = 90101; max = 90720; break;
                case "Huánuco":
                    min = 100101; max = 101108; break;
                case "Ica":
                    min = 110101; max = 110508; break;
                case "Junín":
                    min = 120101; max = 120909; break;
                case "La Libertad":
                    min = 130101; max = 131203; break;
                case "Lambayeque":
                    min = 140101; max = 140312; break;
                case "Lima":
                    min = 150101; max = 151033; break;
                case "Loreto":
                    min = 160101; max = 160804; break;
                case "Madre de Dios":
                    min = 170101; max = 170303; break;
                case "Moquegua":
                    min = 180101; max = 180303; break;
                case "Pasco":
                    min = 190101; max = 190308; break;
                case "Piura":
                    min = 200101; max = 200806; break;
                case "Puno":
                    min = 210101; max = 211307; break;
                case "San Martín":
                    min = 220101; max = 221005; break;
                case "Tacna":
                    min = 230101; max = 230408; break;
                case "Tumbes":
                    min = 240101; max = 240304; break;
                case "Ucayali":
                    min = 250101; max = 250401; break;
            }
            return ofiD.Buscar(min,max);
               
        }*/
        
    }

