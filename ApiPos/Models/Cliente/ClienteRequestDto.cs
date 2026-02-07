namespace Models.Cliente
{
    public class ClienteRequestDto
    {
        public string codRecurso { get; set; }
        public string codCliente { get; set; }
        public string nombre { get; set; }
        public string telefono { get; set; }
        public string correo { get; set; }
        public string tipoCedula { get; set; }
        public string numeroCedula { get; set; }
    }
}