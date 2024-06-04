using System;

namespace Models.DbEntities
{
    public abstract class BaseEntity
    {
        public Guid Id { get; set; }
    }
}
