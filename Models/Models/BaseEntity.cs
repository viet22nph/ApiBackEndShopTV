using System;

namespace Models.DbEntities
{
    public abstract class BaseEntity
    {
        public Guid Id { get; set; }
        public DateTime DateCreate { get; set; }
        public DateTime? DateUpdate { get; set; }
    }
}
