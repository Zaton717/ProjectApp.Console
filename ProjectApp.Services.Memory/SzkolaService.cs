using ProjectApp.Abstractions;
using ProjectApp.DataModel;
using ProjectApp.ServiceAbstractions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ProjectApp.Services
{
    public class SzkolaService : ISzkolaService
    {
        private readonly ISzkolaRepository _szkoly;
        private readonly IUnitOfWork _uow;

        public SzkolaService(ISzkolaRepository szkoly, IUnitOfWork uow)
        {
            _szkoly = szkoly;
            _uow = uow;
        }

        // Ta metoda jest wywoływana, gdy Właściciel otwiera nową placówkę
        public Guid Create(string nazwa, string adres, string mImie, string mNazw)
        {
            var s = new Szkola
            {
                Nazwa = nazwa,
                Adres = adres,
                // Tworzymy Menadżera od razu przy tworzeniu szkoły
                Menadzer = new Menadzer { Imie = mImie, Nazwisko = mNazw }
            };

            _szkoly.Add(s);
            _uow.SaveChanges();
            return s.Id;
        }

        public Szkola? Get(Guid id) => _szkoly.Get(id);

        public IReadOnlyList<Szkola> GetAll() => _szkoly.Query().OrderBy(s => s.Nazwa).ToList();

        public bool ZamknijSzkole(Guid id)
        {
            var s = Get(id); if (s == null) return false;
            s.CzyAktywna = false;
            _uow.SaveChanges(); return true;
        }

        public bool OtworzSzkole(Guid id)
        {
            var s = Get(id); if (s == null) return false;
            s.CzyAktywna = true;
            _uow.SaveChanges(); return true;
        }
    }
}