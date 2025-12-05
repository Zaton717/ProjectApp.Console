using System;

namespace ProjectApp.DataModel
{
    public class Kursant : Osoba
    {
        public DateTime DataUrodzenia { get; set; }
        public bool Oplacony { get; set; }
        public Guid? PrzypisanyKursId { get; set; }
    }
}