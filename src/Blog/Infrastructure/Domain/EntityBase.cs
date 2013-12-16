using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Infrastructure.Domain
{
    public abstract class EntityBase<TId> : IEntityBase<TId>
    {
        public EntityBase()
        {
            LastEdit = DateTime.Now;
            Active = true;
        }

        public bool Active { get; set; }

        public TId Id { get; set; }

        public abstract bool Validate();

        public DateTime LastEdit { get; set; }

    }
}
