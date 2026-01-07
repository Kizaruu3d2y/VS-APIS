namespace proyectoDapper.POS.Models.Surtidor
{
    public class AnulaSurtidorDto
    {

        public string codRecurso { get; set; }
        public string surtidor { get; set; }
        public long idFactura { get; set; }
        public string tipoFactura { get; set; }
        public string lectura { get; set; }

    }
}
