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
        public Szkola Get(uint id) => _context.Szkoly.FirstOrDefault(s => s.IdSzkoly == id);
        public List<Szkola> GetAll() => _context.Szkoly;
        public void Delete(uint id)
        {
            var item = Get(id);
            if (item != null) _context.Szkoly.Remove(item);
        }
    }

    public class UnitOfWorkMemory : IUnitOfWork
    {
        private readonly MemoryDbContext _context;
        public ISzkolaRepository SzkolaRepository { get; }

        public UnitOfWorkMemory()
        {
            _context = new MemoryDbContext();

            // WCZYTANIE DANYCH PRZY STARCIE
            _context.LoadData();

            SzkolaRepository = new SzkolaRepositoryMemory(_context);
        }

        public void Save()
        {
            // ZAPIS DANYCH PRZY KAŻDEJ ZMIANIE
            _context.SaveChanges();
        }
    }
}