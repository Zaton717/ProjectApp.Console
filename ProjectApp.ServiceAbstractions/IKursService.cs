using ProjectApp.DataModel;
using System;
using System.Collections.Generic;

namespace ProjectApp.ServiceAbstractions
{
    public interface IKursService
    {
        Guid UtworzKurs(Guid szkolaId, KategoriaPrawaJazdy kat, decimal cena);
        bool PrzypiszInstruktora(Guid szkolaId, Guid kursId, Guid instruktorId);
        bool DodajTermin(Guid szkolaId, Guid kursId, string termin);
        bool ZapiszKursanta(Guid szkolaId, Guid kursId, Guid kursantId);
        IReadOnlyList<Kurs> GetAll(Guid szkolaId);
    }
}