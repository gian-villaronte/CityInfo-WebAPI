using AutoMapper;
using CityInfo.API.Models;

namespace CityInfo.API.Profiles
{
    public class PointsOfInterestProfile : Profile
    {
        public PointsOfInterestProfile()
        {
            CreateMap<Entities.PointsOfInterest, Models.PointsOfInterestDto>();
            CreateMap<Models.PointsOfInterestForCreationDto, Entities.PointsOfInterest>();
            CreateMap<Models.PointsOfInterestForUpdateDto, Entities.PointsOfInterest>();
            CreateMap<Entities.PointsOfInterest, Models.PointsOfInterestForUpdateDto>();

        }
    }
}
