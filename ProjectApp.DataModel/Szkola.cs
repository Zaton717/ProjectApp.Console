using System.Collections.Generic;

namespace ProjectApp.DataModel
{
    public class Szkola
    {
        public uint IdSzkoly { get; set; }
        public string Nazwa { get; set; }
        public string Adres { get; set; }

        public Menadzer Menadzer { get; set; }
        public List<Instruktor> Instruktorzy { get; set; } = new List<Instruktor>();
        public List<Kursant> Kursanci { get; set; } = new List<Kursant>();
        public List<Kurs> Kursy { get; set; } = new List<Kurs>();
        public List<Pojazd> Pojazdy { get; set; } = new List<Pojazd>();
    }
}