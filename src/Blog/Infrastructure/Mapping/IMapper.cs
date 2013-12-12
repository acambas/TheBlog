using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Mapping
{
    public interface IMapper
    {
        void CreateMap<TSource, TDestination>();
        TDestination Map<TSource, TDestination>(TSource source);

        TDestination Map<TSource, TDestination>(TSource source, TDestination destination);

        IEnumerable<TDestination> Map<TSource, TDestination>(IEnumerable<TSource> source);
    }
}
