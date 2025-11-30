namespace ProjectApp.DataModel
{
    public class Osoba
    {
        private string imie;
        private string nazwisko;
        private string adres;
        private string email;
        private string telefon;
        private string pesel;

        public string Imie { get; set; }
        public string Nazwisko { get; set; }
        public string Adres { get; set; }
        public string Email { get; set; }
        public string Telefon { get; set; }
        public string Pesel { get; set; }
    }

    public class Kursant : Osoba
    {
        private uint idKursanta;
        private KategoriaPrawaJazdy kategoria;
        private List<Kurs> oplaconeKursy;

        public uint IdKursanta { get; set; }
        public KategoriaPrawaJazdy Kategoria { get; set; }

        public void ZapiszNaKurs() { }
        public void OplacKurs() { }
    }

    public class Instruktor : Osoba
    {
        private uint idInstruktora;
        private List<KategoriaPrawaJazdy> uprawnienia;

        public uint IdInstruktora { get; set; }
        public List<KategoriaPrawaJazdy> Uprawnienia { get; set; }

        public void ProwadzZajecia() { }
        public void SprawdzHarmonogram() { }
    }

    public class Menadzer : Osoba
    {
        private uint idMenadzera;

        public uint IdMenadzera { get; set; }

        public void TworzKurs() { }
        public void PrzypiszInstruktora() { }
        public void PrzypiszKursanta() { }
    }

    public class Wlasciciel : Osoba
    {
        private uint idWlasciciela;

        public uint IdWlasciciela { get; set; }

        public void ZarzadzajSzkola() { }
        public void PrzegladajRaporty() { }
    }
}

