namespace ProjectApp.DataModel
{
    public class Pojazd
    {
        public uint IdPojazdu { get; set; }
        public string Marka { get; set; }
        public string Model { get; set; }
        public string NrRejestracyjny { get; set; }
        public TypPojazdu Typ { get; set; }
        public StatusPojazdu Status { get; set; } = StatusPojazdu.Sprawny;
        public Instruktor PrzypisanyInstruktor { get; set; }
    }
}