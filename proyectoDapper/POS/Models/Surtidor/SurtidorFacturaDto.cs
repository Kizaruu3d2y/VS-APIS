namespace proyectoDapper.POS.Models.Surtidor
{
    public class SurtidorFacturaDto
    {
        public string IdFactura { get; set; }
        public string Clave { get; set; }
        public string Consecutivo { get; set; }
        public string LecturaSurtidor { get; set; }
        public string TipoFact { get; set; }
        public bool Expiro { get; set; }
        public string CodCliente { get; set; }
        public string NombreCliente { get; set; }
        public string Alias { get; set; }
        public int Credito { get; set; }
        public bool CreditoContado { get; set; }
        public string ActividadEconomica { get; set; }
        public string Correoco { get; set; }
        public string Correocr { get; set; }
        public string Placa { get; set; }
        public int Km { get; set; }
        public string Oc { get; set; }
        public string Observaciones { get; set; }
    }
}