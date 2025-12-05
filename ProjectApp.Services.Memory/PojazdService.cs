using ProjectApp.Abstractions;
using ProjectApp.DataModel;
using ProjectApp.ServiceAbstractions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ProjectApp.Services
{
    public class PojazdService : IPojazdService
    {
        private readonly ISzkolaRepository _szkoly;
        private readonly IUnitOfWork _uow;

        public PojazdService(ISzkolaRepository szkoly, IUnitOfWork uow)
        {
            _szkoly = szkoly;
            _uow = uow;
        }

        public bool DodajPojazd(Guid sId, string marka, string model, string rej, TypPojazdu typ)
        {
            var s = _szkoly.Get(sId);
            if (s == null) return false;

            var p = new Pojazd { Marka = marka, Model = model, NrRejestracyjny = rej, Typ = typ };
            s.Pojazdy.Add(p);
            _uow.SaveChanges();
            return true;
        }

        public bool ZmienStatus(Guid sId, Guid pId, StatusPojazdu status)
        {
            var s = _szkoly.Get(sId);
            var p = s?.Pojazdy.FirstOrDefault(x => x.Id == pId);
            if (p == null) return false;

            p.Status = status;
            _uow.SaveChanges();
            return true;
        }

        public bool PrzypiszInstruktora(Guid sId, Guid pId, Guid iId)
        {
            var s = _szkoly.Get(sId);
            var p = s?.Pojazdy.FirstOrDefault(x => x.Id == pId);
            var exists = s?.Instruktorzy.Any(x => x.Id == iId) ?? false;

            if (p == null || !exists) return false;

            p.PrzypisanyInstruktorId = iId;
            _uow.SaveChanges();
            return true;
        }

        public IReadOnlyList<Pojazd> GetAll(Guid sId)
            => _szkoly.Get(sId)?.Pojazdy.ToList() ?? new List<Pojazd>();
    }
}