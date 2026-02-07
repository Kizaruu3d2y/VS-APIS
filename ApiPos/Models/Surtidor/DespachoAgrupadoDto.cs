namespace Models.Surtidor
{
    public class DespachoAgrupadoDto
    {
        public string Fecha { get; set; }
        public decimal Monto { get; set; }
        public decimal Litros { get; set; }
        public string Producto { get; set; }
        public string Recurso { get; set; }
        public string Lectura { get; set; }
    }
}