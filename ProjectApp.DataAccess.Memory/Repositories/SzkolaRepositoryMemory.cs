using ProjectApp.Abstractions;
using ProjectApp.DataModel;
using System.Collections.Generic;

namespace ProjectApp.DataAccess.Memory.Repositories
{
    public class SzkolaRepositoryMemory : RepositoryMemoryBase<Szkola>, ISzkolaRepository
    {
        protected override List<Szkola> Entities => _db.Szkoly;
        public SzkolaRepositoryMemory(MemoryDbContext db) : base(db) { }
    }
}