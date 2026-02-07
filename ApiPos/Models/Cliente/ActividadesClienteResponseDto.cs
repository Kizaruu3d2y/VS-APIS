namespace Models.Cliente
{
    public class ActividadCliente
    {
        public string indPrincipal { get; set; }
        public string actividadEco { get; set; }

        public ActividadCliente() { }

        public ActividadCliente(string indPrincipal, string actividadEco)
        {
            this.indPrincipal = indPrincipal;
            this.actividadEco = actividadEco;
        }
    }
}