using AutoMapper;

namespace CityInfo.API.Profiles
{
    public class CityProfile : Profile
    {
        public CityProfile()
        {
            CreateMap<Entities.City, Models.CityModel>();
            CreateMap<Entities.City, Models.CityDto>();
        }
    }
}
