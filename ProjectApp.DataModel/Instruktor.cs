using System.Collections.Generic;

namespace ProjectApp.DataModel
{
    public class Instruktor : Osoba
    {
        public List<KategoriaPrawaJazdy> Uprawnienia { get; set; } = new();
    }
}