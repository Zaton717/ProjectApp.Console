using ProjectApp.Abstractions;
using ProjectApp.DataModel;
using ProjectApp.ServiceAbstractions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ProjectApp.Services
{
    public class KursService : IKursService
    {
        private readonly ISzkolaRepository _szkoly;
        private readonly IKursantRepository _kursanci;
        private readonly IUnitOfWork _uow;

        public KursService(ISzkolaRepository s, IKursantRepository k, IUnitOfWork uow)
        {
            _szkoly = s; _kursanci = k; _uow = uow;
        }

        public Guid UtworzKurs(Guid sId, KategoriaPrawaJazdy kat, decimal cena)
        {
            var s = _szkoly.Get(sId);
            if (s == null) return Guid.Empty;

            int nowyNumer = 1;
            if (s.Kursy.Any()) nowyNumer = s.Kursy.Max(x => x.Numer) + 1;

            var k = new Kurs { Kategoria = kat, Numer = nowyNumer, Cena = cena };

            s.Kursy.Add(k);
            _uow.SaveChanges();
            return k.Id;
        }

        public bool PrzypiszInstruktora(Guid sId, Guid kId, Guid iId)
        {
            var s = _szkoly.Get(sId);
            var k = s?.Kursy.FirstOrDefault(x => x.Id == kId);
            var i = s?.Instruktorzy.FirstOrDefault(x => x.Id == iId);
            if (k == null || i == null) return false;
            k.InstruktorId = iId;
            _uow.SaveChanges();
            return true;
        }

        public bool DodajTermin(Guid sId, Guid kId, string termin)
        {
            var s = _szkoly.Get(sId);
            var k = s?.Kursy.FirstOrDefault(x => x.Id == kId);
            if (k == null) return false;
            k.Harmonogram.Add(termin);
            _uow.SaveChanges();
            return true;
        }

        public bool ZapiszKursanta(Guid sId, Guid kId, Guid kursantId)
        {
            var s = _szkoly.Get(sId);
            var kurs = s?.Kursy.FirstOrDefault(x => x.Id == kId);
            var kursant = _kursanci.Get(kursantId);
            if (s == null || kurs == null || kursant == null) return false;

            if (!s.Kursanci.Any(x => x.Id == kursantId)) s.Kursanci.Add(kursant);
            if (!kurs.Uczestnicy.Any(x => x.Id == kursantId)) kurs.Uczestnicy.Add(kursant);

            kursant.PrzypisanyKursId = kId;
            _uow.SaveChanges();
            return true;
        }

        public IReadOnlyList<Kurs> GetAll(Guid sId)
            => _szkoly.Get(sId)?.Kursy.ToList() ?? new List<Kurs>();
    }
}