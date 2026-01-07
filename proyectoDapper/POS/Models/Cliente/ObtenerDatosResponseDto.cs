namespace proyectoDapper.POS.Models.Surtidor.Cliente
{
    public class ObtenerDatosResponseDto
    {
        public string Codigo { get; set; }
        public string Nombre { get; set; }
        public string Alias { get; set; }
        public int Credito { get; set; }
        public bool FactCredito { get; set; }
        public string? CorreoCo { get; set; }
        public string? CorreoCr { get; set; }
        public string? Placa { get; set; }
        public bool TagUser { get; set; }
        public bool RestCorreo { get; set; }
        public bool RestPlaca { get; set; }
        public bool RestKm { get; set; }
        public bool RestOc { get; set; }
        public bool DespMult { get; set; }
        public string? ActividadEco { get; set; }
    }
}
