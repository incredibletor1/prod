using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services
{
    public static class AutoMapperService<FROM, TO> where FROM : class where TO : class
    {

        public static List<TO> MapperList(List<FROM> fromList)
        {
            List<TO> toList = new List<TO>();
            foreach (var from in fromList)
            {
                toList.Add(Mapper(from));
            }
            return toList;
        }

        public static TO Mapper(FROM from)
        {
            Mapper mapper = CreateAutoMapper();
            var to = mapper.Map<TO>(from);
            return to;
        }

        public static Mapper CreateAutoMapper()
        {
            MapperConfiguration config = new MapperConfiguration(cfg => cfg.CreateMap<FROM, TO>());
            Mapper mapper = new Mapper(config);
            return mapper;
        }
    }
}
