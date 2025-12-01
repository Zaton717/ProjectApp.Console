using ProjectApp.DataModel;
using System.Collections.Generic;

namespace ProjectApp.DataAccess.Memory
{
    // Nasza "baza danych" w pamięci RAM
    internal class MemoryDbContext
    {
        public List<Szkola> Szkoly { get; set; } = new List<Szkola>();
    }
}