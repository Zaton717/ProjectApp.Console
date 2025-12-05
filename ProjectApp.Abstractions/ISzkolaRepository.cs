using ProjectApp.DataModel;
using System;
using System.Linq;

namespace ProjectApp.Abstractions
{
    public interface ISzkolaRepository
    {
        IQueryable<Szkola> Query();
        Szkola? Get(Guid id);
        void Add(Szkola entity);
        void Remove(Szkola entity);
    }
}