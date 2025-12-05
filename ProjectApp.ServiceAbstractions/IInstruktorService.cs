using ProjectApp.DataModel;
using System;
using System.Collections.Generic;

namespace ProjectApp.ServiceAbstractions
{
    public interface IInstruktorService
    {
        // ZMIANA: Dodano argument List<KategoriaPrawaJazdy> uprawnienia
        bool Zatrudnij(Guid szkolaId, string imie, string nazwisko, List<KategoriaPrawaJazdy> uprawnienia);
        bool Zwolnij(Guid szkolaId, Guid instruktorId);
        IReadOnlyList<Instruktor> GetAll(Guid szkolaId);
    }
}