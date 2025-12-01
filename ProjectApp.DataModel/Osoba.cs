namespace ProjectApp.DataModel
{
    public abstract class Osoba
    {
        public string Imie { get; set; }
        public string Nazwisko { get; set; }
        public string Adres { get; set; }
        public string Email { get; set; }
        public string Telefon { get; set; }
        public string Pesel { get; set; }
    }
}