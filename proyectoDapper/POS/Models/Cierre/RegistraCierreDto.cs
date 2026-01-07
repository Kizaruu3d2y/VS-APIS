namespace proyectoDapper.POS.Models.Surtidor.Cliente.Cierre
{
    public class RegistraCierreDto
    {
    public int NumeroVersion { get; set; }
        public int idLinea { get; set; }
        public string codigo { get; set; }
        public string descripcion { get; set; }
        public string moneda { get; set; }
        public decimal tipoCambio { get; set; }
        public decimal monto { get; set; }
        public string datafono { get; set; }
    }
}
