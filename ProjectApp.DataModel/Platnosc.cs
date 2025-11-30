using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectApp.DataModel
{
    public class Platnosc
    {
        private uint idPlatnosci;
        private Kursant kursant;
        private double kwota;
        private DateTime data;
        private bool status;

        public uint IdPlatnosci { get; set; }
        public Kursant Kursant { get; set; }
        public double Kwota { get; set; }
        public DateTime Data { get; set; }
        public bool Status { get; set; }

        public void ZatwierdzPlatnosc() { }
        public void ZwrocPlatnosc() { }
    }
}

