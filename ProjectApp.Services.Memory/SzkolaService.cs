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
        private readonly IKursantRepository _kursanci;
        private readonly IUnitOfWork _uow;

        public SzkolaService(ISzkolaRepository szkoly, IKursantRepository kursanci, IUnitOfWork uow)
        {
            _szkoly = szkoly;
            _kursanci = kursanci;
            _uow = uow;
        }

        public Guid Create(string nazwa, string adres, string mImie, string mNazw)
        {
            var s = new Szkola
            {
                Nazwa = nazwa,
                Adres = adres,
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

        public bool AddExistingKursant(Guid szkolaId, Guid kursantId)
        {
            var szkola = _szkoly.Get(szkolaId);
            var kursant = _kursanci.Get(kursantId);

            if (szkola == null || kursant == null) return false;

            if (!szkola.Kursanci.Any(k => k.Id == kursantId))
            {
                szkola.Kursanci.Add(kursant);
                _uow.SaveChanges();
            }
            return true;
        }

        public bool Update(Guid id, string nowaNazwa, string nowyAdres)
        {
            var s = _szkoly.Get(id);
            if (s == null) return false;

            s.Nazwa = nowaNazwa;
            s.Adres = nowyAdres;
            _uow.SaveChanges();
            return true;
        }

        public bool Delete(Guid id)
        {
            var s = _szkoly.Get(id);
            if (s == null) return false;

            _szkoly.Remove(s);
            _uow.SaveChanges();
            return true;
        }
    }

}