using ProjectApp.DataModel;
using System.Collections.Generic;

namespace ProjectApp.DataAccess.Memory
{
    public class MemoryDbContext
    {
        public List<Szkola> Szkoly { get; } = new();
        public List<Kursant> Kursanci { get; } = new();
        public int SaveChanges() => 0;
    }
}