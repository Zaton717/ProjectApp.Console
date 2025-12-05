using System;

namespace ProjectApp.DataModel
{
    public abstract class Entity
    {
        public Guid Id { get; init; } = Guid.NewGuid();
    }
}