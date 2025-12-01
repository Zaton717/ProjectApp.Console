using System.Collections.Generic;

namespace ProjectApp.DataModel
{
    public class Kurs
    {
        public uint IdKursu { get; set; }
        public KategoriaPrawaJazdy Kategoria { get; set; }
        public Instruktor Instruktor { get; set; }
        public List<Kursant> Uczestnicy { get; set; } = new List<Kursant>();
        public Harmonogram Harmonogram { get; set; }
    }
}