using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectApp.DataModel
{
    public class Kurs
    {
        private uint idKursu;
        private KategoriaPrawaJazdy kategoria;
        private List<Instruktor> instruktorzy;
        private List<Kursant> uczestnicy;
        private Harmonogram harmonogram;

        public uint IdKursu { get; set; }
        public KategoriaPrawaJazdy Kategoria { get; set; }
        public Harmonogram Harmonogram { get; set; }

        public void DodajKursanta() { }
        public void UsunKursanta() { }
    }

    public class Harmonogram
    {
        private uint idHarmonogramu;
        private List<string> terminy;
        private Kurs kurs;

        public uint IdHarmonogramu { get; set; }
        public List<string> Terminy { get; set; }

        public void DodajTermin() { }
        public void UsunTermin() { }
    }
}


