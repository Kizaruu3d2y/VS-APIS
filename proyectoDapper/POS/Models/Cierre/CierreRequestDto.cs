namespace proyectoDapper.POS.Models.Surtidor.Cliente.Cierre
{
    public class EjecutarCierreRequest
    {
        public string codRecurso { get; set; }
        public List<ValorCierreDto> valores { get; set; }
    }

    public class ValorCierreDto
    {
        public int idLinea { get; set; }
        public string codigo { get; set; }
        public string moneda { get; set; }
        public double monto { get; set; }

 
        public double tipoCambio { get; set; }
        public string no_datafono { get; set; }
    }
}