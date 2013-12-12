using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
namespace Infrastructure.Mapping
{
    public class AutoMapperAdapter:IMapper
    {
        public void CreateMap<TSource, TDestination>()
        {
            Mapper.CreateMap<TSource, TDestination>();
        }
        public TDestination Map<TSource, TDestination>(TSource source)
        {
            return Mapper.Map<TSource, TDestination>(source);
        }

        public TDestination Map<TSource, TDestination>(TSource source, TDestination destination)
        {
            return Mapper.Map<TSource, TDestination>(source, destination);
        }

        public IEnumerable<TDestination> Map<TSource, TDestination>(IEnumerable<TSource> source)
        {
            return source.Select(m => Map<TSource, TDestination>(m));
        }
    }
}
