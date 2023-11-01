using CityInfo.API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CityInfo.API.Controllers
{
    [Route("api/cities/{cityId}/pointsofinterest")]
    [ApiController]
    public class PointsOfInterestController : ControllerBase
    {
        [HttpGet]
        public ActionResult<IEnumerable<PointsOfInterestDto>> GetPointsOfInterest(int cityId) 
        { 
            var city = CitiesDataStore.Current.Cities.Find(x => x.Id == cityId);

            if (city == null) 
            { 
                return NotFound();
            }

            return Ok(city.PointsOfInterest);
        }

        [HttpGet("{pointofinterestid}")]
        public ActionResult<PointsOfInterestDto> GetPointOfInterest(int cityId, int pointofinterestid) 
        {
            var city = CitiesDataStore.Current.Cities.Find(x => x.Id == cityId);

            if (city == null)
            {
                return NotFound();
            }

            var pointOfInterest = city.PointsOfInterest.FirstOrDefault(x => x.Id == pointofinterestid);
            if (pointOfInterest == null) 
            {
                return NotFound();
            }

            return Ok(pointOfInterest);
        }
    }
}
