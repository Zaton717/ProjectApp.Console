using ProjectApp.Abstractions;
using ProjectApp.DataModel;
using System.Collections.Generic;
using System.Linq;

namespace ProjectApp.DataAccess.Memory
{
    internal class SzkolaRepositoryMemory : ISzkolaRepository
    {
        private readonly MemoryDbContext _context;

        public SzkolaRepositoryMemory(MemoryDbContext context)
        {
            _context = context;
        }

        public void Add(Szkola szkola) => _context.Szkoly.Add(szkola);
        public void Delete(uint id)
        {
            var item = Get(id);
            if (item != null) _context.Szkoly.Remove(item);
        }
        public Szkola Get(uint id) => _context.Szkoly.FirstOrDefault(s => s.IdSzkoly == id);
        public List<Szkola> GetAll() => _context.Szkoly;
    }

    public class UnitOfWorkMemory : IUnitOfWork
    {
        private readonly MemoryDbContext _context;
        public ISzkolaRepository SzkolaRepository { get; }

        public UnitOfWorkMemory()
        {
            _context = new MemoryDbContext();
            SzkolaRepository = new SzkolaRepositoryMemory(_context);
        }

        public void Save()
        {
            // W pamięci zmiany są natychmiastowe, więc nic tu nie robimy
            Console.WriteLine("Zapisano stan w pamięci.");
        }
    }
}