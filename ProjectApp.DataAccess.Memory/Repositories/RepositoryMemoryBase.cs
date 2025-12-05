using System;
using System.Collections.Generic;
using System.Linq;
using ProjectApp.DataModel;

namespace ProjectApp.DataAccess.Memory.Repositories
{
    public abstract class RepositoryMemoryBase<T> where T : Entity
    {
        protected readonly MemoryDbContext _db;
        protected abstract List<T> Entities { get; }

        protected RepositoryMemoryBase(MemoryDbContext db)
        {
            _db = db;
        }

        public virtual IQueryable<T> Query() => Entities.AsQueryable();
        public virtual T? Get(Guid id) => Entities.FirstOrDefault(e => e.Id == id);
        public virtual void Add(T entity) => Entities.Add(entity);
        public virtual void Remove(T entity) => Entities.Remove(entity);
    }
}