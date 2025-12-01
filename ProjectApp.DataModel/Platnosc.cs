using System;

namespace ProjectApp.DataModel
{
    public class Platnosc
    {
        public uint IdPlatnosci { get; set; }
        public Kursant Kursant { get; set; }
        public double Kwota { get; set; }
        public DateTime Data { get; set; }
        public bool Status { get; set; } // true = zatwierdzona
    }
}