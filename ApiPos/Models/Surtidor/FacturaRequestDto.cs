using System.Collections.Generic;

namespace Models.Surtidor
{
    public class FacturaRequestDto
    {
        public string? codRecurso { get; set; }
        public string? codCliente { get; set; }
        public string? actividadEco { get; set; }
        public string? placa { get; set; }
        public string? km { get; set; }
        public string? oc { get; set; }
        public string? observaciones { get; set; }
        public string? creditoContado { get; set; }   // "CR" | "CO"
        public string? formaPago { get; set; }         // "02" | "06" | "09"
        public string? lecturaSurtidor { get; set; }   // null si es aceite
        public List<string>? idArticulos { get; set; } // null si es combustible
        public List<decimal>? cantArticulos { get; set; }
    }
}