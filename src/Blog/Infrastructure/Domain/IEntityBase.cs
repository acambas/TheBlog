using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Domain
{
    public interface IEntityBase<TId>
    {
        bool Active { get; set; }
        TId Id { get; set; }
        bool Validate();
        DateTime LastEdit { get; set; }
    }
}
