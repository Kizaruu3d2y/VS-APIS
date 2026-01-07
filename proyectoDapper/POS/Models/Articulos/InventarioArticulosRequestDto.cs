namespace proyectoDapper.POS.Models.Articulos
{
    public class InventarioArticulosRequestDto
    {
        public string codRecurso { get; set; }
        public string registro { get; set; }
        public string[] idArticulos { get; set; }
        public int[] cantidades { get; set; }
    }
}
