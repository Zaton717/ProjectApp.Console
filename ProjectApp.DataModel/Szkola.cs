using System.Collections.Generic;

namespace ProjectApp.DataModel
{
    public class Szkola : Entity
    {
        public string Nazwa { get; set; } = "";
        public string Adres { get; set; } = "";
        public bool CzyAktywna { get; set; } = true;

        public Menadzer Menadzer { get; set; } = new();

        public List<Instruktor> Instruktorzy { get; set; } = new();
        public List<Pojazd> Pojazdy { get; set; } = new();
        public List<Kurs> Kursy { get; set; } = new();
        public List<Kursant> Kursanci { get; set; } = new();
    }
}