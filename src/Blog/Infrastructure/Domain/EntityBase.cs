using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Infrastructure.Domain
{
    public abstract class EntityBase<TId>
    {
        public EntityBase()
        {
            LastEdit = DateTime.Now;
        }

        public TId Id { get; set; }

        protected abstract bool Validate();

        public DateTime LastEdit { get; set; }

    }
}
