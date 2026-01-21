namespace proyectoDapper.POS.Models.Surtidor.Cliente.Cierre
{
    public class CierreDto
    {
        public int vCombustible { get; set; }
        public int vAceite { get; set; }
        public int vCredito { get; set; }
        public int Calibracion { get; set; }
        public DateTime? fechaCierre { get; set; }
    }
    public class CierreVersionDto
    {
        public int id_cierre { get; set; }
        public int no_turno { get; set; }
    }
}
