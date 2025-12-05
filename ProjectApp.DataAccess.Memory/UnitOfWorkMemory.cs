using ProjectApp.Abstractions;

namespace ProjectApp.DataAccess.Memory
{
    public class UnitOfWorkMemory : IUnitOfWork
    {
        private readonly MemoryDbContext _db;
        public UnitOfWorkMemory(MemoryDbContext db) => _db = db;
        public int SaveChanges() => _db.SaveChanges();
    }
}