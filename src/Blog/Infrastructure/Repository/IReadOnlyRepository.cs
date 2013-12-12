using Infrastructure.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository
{
    public interface IReadOnlyRepository<T, TId> where T : EntityBase<TId>
    {

        T FindBy(TId id, bool hydrate = true);
        IQueryable<T> GetQuery();
        int Count();
        

    }
}
