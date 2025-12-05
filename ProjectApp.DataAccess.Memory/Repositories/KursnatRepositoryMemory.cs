using ProjectApp.Abstractions;
using ProjectApp.DataModel;
using System.Collections.Generic;

namespace ProjectApp.DataAccess.Memory.Repositories
{
    public class KursantRepositoryMemory : RepositoryMemoryBase<Kursant>, IKursantRepository
    {
        protected override List<Kursant> Entities => _db.Kursanci;
        public KursantRepositoryMemory(MemoryDbContext db) : base(db) { }
    }
}