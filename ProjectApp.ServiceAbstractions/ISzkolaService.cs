using ProjectApp.DataModel;
using System;
using System.Collections.Generic;

namespace ProjectApp.ServiceAbstractions
{
    public interface ISzkolaService
    {
        Guid Create(string nazwa, string adres, string menadzerImie, string menadzerNazwisko);
        Szkola? Get(Guid id);
        IReadOnlyList<Szkola> GetAll();
        bool ZamknijSzkole(Guid id);
        bool OtworzSzkole(Guid id);
    }
}