namespace Models.Cierre
{
    public class CierreResumenDto
    {
        public int idLinea { get; set; }
        public string codigo { get; set; }
        public string descripcion { get; set; }
        public string moneda { get; set; }
        public decimal tipoCambio { get; set; }
        public decimal Monto { get; set; }
        public string No_Datafono { get; set; }
    }
}
