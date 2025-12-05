using ProjectApp.Abstractions;
using ProjectApp.DataModel;
using ProjectApp.ServiceAbstractions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ProjectApp.Services
{
    public class InstruktorService : IInstruktorService
    {
        private readonly ISzkolaRepository _szkoly;
        private readonly IUnitOfWork _uow;

        public InstruktorService(ISzkolaRepository szkoly, IUnitOfWork uow)
        {
            _szkoly = szkoly;
            _uow = uow;
        }

        // ZMIANA: Obsługa listy uprawnień
        public bool Zatrudnij(Guid sId, string imie, string nazwisko, List<KategoriaPrawaJazdy> uprawnienia)
        {
            var s = _szkoly.Get(sId);
            if (s == null) return false;

            var i = new Instruktor
            {
                Imie = imie,
                Nazwisko = nazwisko,
                Uprawnienia = uprawnienia ?? new List<KategoriaPrawaJazdy>() // Zabezpieczenie przed nullem
            };

            s.Instruktorzy.Add(i);
            _uow.SaveChanges();
            return true;
        }

        public bool Zwolnij(Guid sId, Guid iId)
        {
            var s = _szkoly.Get(sId);
            if (s == null) return false;

            var instr = s.Instruktorzy.FirstOrDefault(x => x.Id == iId);
            if (instr == null) return false;

            foreach (var p in s.Pojazdy.Where(p => p.PrzypisanyInstruktorId == iId))
                p.PrzypisanyInstruktorId = null;

            foreach (var k in s.Kursy.Where(k => k.InstruktorId == iId))
                k.InstruktorId = null;

            s.Instruktorzy.Remove(instr);
            _uow.SaveChanges();
            return true;
        }

        public IReadOnlyList<Instruktor> GetAll(Guid sId)
            => _szkoly.Get(sId)?.Instruktorzy.ToList() ?? new List<Instruktor>();
    }
}