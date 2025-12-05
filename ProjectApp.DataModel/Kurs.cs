using System;
using System.Collections.Generic;

namespace ProjectApp.DataModel
{
    public class Kurs : Entity
    {
        public int Numer { get; set; }
        public decimal Cena { get; set; }

        public KategoriaPrawaJazdy Kategoria { get; set; }
        public Guid? InstruktorId { get; set; }
        public List<string> Harmonogram { get; set; } = new();
        public List<Kursant> Uczestnicy { get; set; } = new();
    }
}