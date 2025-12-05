using ProjectApp.DataModel;
using System;
using System.Collections.Generic;

namespace ProjectApp.ServiceAbstractions
{
    public interface IPojazdService
    {
        bool DodajPojazd(Guid szkolaId, string marka, string model, string rej, TypPojazdu typ);
        bool ZmienStatus(Guid szkolaId, Guid pojazdId, StatusPojazdu status);
        bool PrzypiszInstruktora(Guid szkolaId, Guid pojazdId, Guid instruktorId);
        IReadOnlyList<Pojazd> GetAll(Guid szkolaId);
    }
}