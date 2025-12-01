using ProjectApp.DataModel;
using System.Collections.Generic;

namespace ProjectApp.Abstractions
{
    // Interfejs do zarządzania całą szkołą (zapis/odczyt)
    public interface ISzkolaRepository
    {
        void Add(Szkola szkola);
        Szkola Get(uint id);
        List<Szkola> GetAll();
        void Delete(uint id);
    }
}