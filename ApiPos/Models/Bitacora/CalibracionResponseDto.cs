namespace Models.Bitacora
{
    public class CalibracionResponseDto
    {
        public List<CalibracionDto> Diesel { get; set; } = new();
        public List<CalibracionDto> SuperGasolina { get; set; } = new();
        public List<CalibracionDto> Plus91Gasolina { get; set; } = new();
    }
}
