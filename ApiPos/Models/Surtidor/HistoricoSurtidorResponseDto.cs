namespace Models.Surtidor
{
    public class HistoricoSurtidorResponseDto
    {
        public string Fecha { get; set; }
        public decimal? Monto { get; set; }
        public decimal? Litros { get; set; }
        public string Producto { get; set; }
        public string Cajero { get; set; }
    }
}
