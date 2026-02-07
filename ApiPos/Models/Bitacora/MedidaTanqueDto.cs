namespace Models.Bitacora
{
    public class MedidaTanqueDto
    {
        public string codRecurso { get; set; }
        
        public decimal cantSuper { get; set; }
        public decimal cantPlus91 { get; set; }
        public decimal cantDiesel { get; set; }

        public DateTime fechaCorte { get; set; }



    }
}
