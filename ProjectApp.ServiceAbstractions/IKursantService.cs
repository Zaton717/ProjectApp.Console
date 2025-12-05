using ProjectApp.DataModel;
using System;
using System.Collections.Generic;

namespace ProjectApp.ServiceAbstractions
{
    public interface IKursantService
    {
        Guid Create(string imie, string nazwisko, DateTime dataUr);
        Kursant? Get(Guid id);
        IReadOnlyList<Kursant> GetAll();
        bool OplacKurs(Guid kursantId); // <--- NOWE
    }
}