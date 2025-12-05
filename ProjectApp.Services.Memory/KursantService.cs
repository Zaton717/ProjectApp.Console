using ProjectApp.Abstractions;
using ProjectApp.DataModel;
using ProjectApp.ServiceAbstractions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ProjectApp.Services
{
    public class KursantService : IKursantService
    {
        private readonly IKursantRepository _kursanci;
        private readonly IUnitOfWork _uow;

        public KursantService(IKursantRepository k, IUnitOfWork uow)
        {
            _kursanci = k; _uow = uow;
        }

        public Guid Create(string imie, string nazwisko, DateTime dataUr)
        {
            var k = new Kursant { Imie = imie, Nazwisko = nazwisko, DataUrodzenia = dataUr };
            _kursanci.Add(k);
            _uow.SaveChanges();
            return k.Id;
        }

        public Kursant? Get(Guid id) => _kursanci.Get(id);
        public IReadOnlyList<Kursant> GetAll() => _kursanci.Query().OrderBy(k => k.Nazwisko).ToList();

        public bool OplacKurs(Guid id)
        {
            var k = _kursanci.Get(id);
            if (k == null) return false;
            k.Oplacony = true;
            _uow.SaveChanges();
            return true;
        }
    }
}