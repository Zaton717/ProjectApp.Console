using ProjectApp.DataModel;
using System;
using System.Linq;

namespace ProjectApp.Abstractions
{
    public interface IKursantRepository
    {
        IQueryable<Kursant> Query();
        Kursant? Get(Guid id);
        void Add(Kursant entity);
        void Remove(Kursant entity);
    }
}