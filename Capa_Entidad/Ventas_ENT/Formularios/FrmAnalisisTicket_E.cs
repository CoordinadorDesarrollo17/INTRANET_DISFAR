namespace Capa_Entidad.Ventas_ENT.Formularios
{
    public class FrmAnalisisTicket_E
    {
        public string AlmacenSalida { get; set; }
        public string AlmIni { get; set; }
        public string AlmFin { get; set; }
        public string Estado { get; set; }
        public string FecIni { get; set; }
        public string FecFin { get; set; }
        public string CardCode { get; set; }
        public string LugarDestino { get; set; }
        public string OpSeparo { get; set; }
        public string OpSacar { get; set; }
        public string OpRecibir { get; set; }
        public string OpSacando { get; set; }
        public string OpEmpacado { get; set; }
        public string OpEnvio { get; set; }
        public string OpEntrega { get; set; }
        public string OpAnulacion { get; set; }
        public string OpFacturacion { get; set; }
        public string OpSacando2 { get; set; }
        public string OpChequeador { get; set; }
        public string OpEmpacado2 { get; set; }
        public decimal MontoFinalIni { get; set; }
        public decimal MontoFinalFin { get; set; }
        //
        public string TipoOperario { get; set; }
        public string Operario { get; set; }
        //metodo
        public void inicializarOperario()
        {
            if (TipoOperario == "OpSeparo")
            {
                OpSeparo = Operario;
            }
            else if (TipoOperario == "OpSacar")
            {
                OpSacar = Operario;
            }
            else if (TipoOperario == "OpRecibir")
            {
                OpRecibir = Operario;
            }
            else if (TipoOperario == "OpSacando")
            {
                OpSacando = Operario;
            }
            else if (TipoOperario == "OpEmpacado")
            {
                OpEmpacado = Operario;
            }
            else if (TipoOperario == "OpEnvio")
            {
                OpEnvio = Operario;
            }
            else if (TipoOperario == "OpEntrega")
            {
                OpEntrega = Operario;
            }
            else if (TipoOperario == "OpAnulacion")
            {
                OpAnulacion = Operario;
            }
            else if (TipoOperario == "OpFacturacion")
            {
                OpFacturacion = Operario;
            }
            else if (TipoOperario == "OpSacando2")
            {
                OpSacando2 = Operario;
            }
            else if (TipoOperario == "OpChequeador")
            {
                OpChequeador = Operario;
            }
            else if (TipoOperario == "OpEmpacado2")
            {
                OpEmpacado2 = Operario;
            }

        }
    }
}