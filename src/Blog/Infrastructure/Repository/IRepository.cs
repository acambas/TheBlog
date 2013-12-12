using Infrastructure.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository
{
    public interface IRepository<T, TId> : IReadOnlyRepository<T, TId>
        where T : EntityBase<TId>
    {
        void Create(T entity);

        void Update(T entity);

        void Delete(TId Id);
    }
}
