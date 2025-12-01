using System.Collections.Generic;

namespace ProjectApp.DataModel
{
    public class Instruktor : Osoba
    {
        public uint IdInstruktora { get; set; }
        public List<KategoriaPrawaJazdy> Uprawnienia { get; set; } = new List<KategoriaPrawaJazdy>();
    }
}