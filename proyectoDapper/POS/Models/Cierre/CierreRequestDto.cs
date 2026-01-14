namespace proyectoDapper.POS.Models.Surtidor.Cliente.Cierre
{
    public class EjecutarCierreRequest
    {
        public string codRecurso { get; set; }
        public List<ValorCierreDto> valores { get; set; }
    }

    public class ValorCierreDto
    {
        public int codigo { get; set; }
        public int idlinea { get; set; }
        public decimal monto { get; set; }
    }
}