using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Unit_Of_Work
{
    public interface IUnitOfWork
    {
        void Save();
        Task SaveAsync();
    }
}
