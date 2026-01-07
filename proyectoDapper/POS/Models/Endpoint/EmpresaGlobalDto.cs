namespace proyectoDapper.Models.Pos
{
    public static class EmpresaGlobalDto
    {
        public static string Empresa { get; private set; } = string.Empty;

        public static void SetCompania(string empresa)
        {
            if (!string.IsNullOrWhiteSpace(Empresa))
                return; // ya está asignada

            Empresa = empresa;
        }
    }
}
