using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectApp.DataModel
{
    public class Szkola
    {
        private uint idSzkoly;
        private string nazwa;
        private string adres;
        private Menadzer menadzer;
        private List<Instruktor> instruktorzy;
        private List<Kursant> kursanci;
        private List<Kurs> kursy;
        private List<Pojazd> pojazdy;

        public uint IdSzkoly { get; set; }
        public string Nazwa { get; set; }
        public string Adres { get; set; }
        public Menadzer Menadzer { get; set; }

        public void DodajKurs() { }
        public void DodajInstruktora() { }
        public void DodajPojazd() { }
    }

    public class Pojazd
    {
        private uint idPojazdu;
        private string marka;
        private string model;
        private string nrRejestracyjny;
        private TypPojazdu typ;
        private Instruktor przypisanyInstruktor;

        public uint IdPojazdu { get; set; }
        public string Marka { get; set; }
        public string Model { get; set; }
        public string NrRejestracyjny { get; set; }
        public TypPojazdu Typ { get; set; }

        public void PrzypiszInstruktora() { }
    }
}

