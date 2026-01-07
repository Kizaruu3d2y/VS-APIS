namespace proyectoDapper.POS.Models.Surtidor.Cliente
{
    public class SolicitarTagRequestDto
    {
        public string codRecurso { get; set; }
        public string codCliente { get; set; }

        public string placa { get; set; }
        public string tag { get; set; }
    }


}