namespace ProjectApp.DataModel
{
    public abstract class Osoba : Entity
    {
        public string Imie { get; set; } = "";
        public string Nazwisko { get; set; } = "";
        public string PelneImie => $"{Imie} {Nazwisko}";
    }
}