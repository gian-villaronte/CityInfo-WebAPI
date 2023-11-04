using CityInfo.API.Models;
using CityInfo.API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace CityInfo.API.Controllers
{
    [Route("api/cities/{cityId}/pointsofinterest")]
    [ApiController]
    public class PointsOfInterestController : ControllerBase
    {
        private ILogger<PointsOfInterestController> _logger;
        private LocalMailService _mailService;

        public PointsOfInterestController(ILogger<PointsOfInterestController> logger, LocalMailService mailService)
        {
            _logger = logger;
            _mailService = mailService;
        }
        [HttpGet]
        public ActionResult<IEnumerable<PointsOfInterestDto>> GetPointsOfInterest(int cityId) 
        {
            try
            {
                var city = CitiesDataStore.Current.Cities.Find(x => x.Id == cityId);

                if (city == null)
                {
                    _logger.LogInformation($"City with Id {cityId} wasn't found when accessing points of interest.");
                    return NotFound();
                }

                return Ok(city.PointsOfInterest);
            }
            catch (Exception ex) 
            {
                _logger.LogCritical($"Critical Exception while getting points of interest for city id {cityId}.", ex);
                return StatusCode(500, "A problem happened while handling your request.");
            }
        }

        [HttpGet("{pointofinterestid}", Name = "GetPointOfInterest")]
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

        [HttpPost]
        public ActionResult<PointsOfInterestDto> CreatePointsOfInterest(int cityId, PointsOfInterestForCreationDto pointsOfInterest) 
        {
            var city = CitiesDataStore.Current.Cities.Find(x => x.Id == cityId);

            if (city == null)
            {
                return NotFound();
            }

            //demo ony - will be improved
            var maxPointsOfInterestId = CitiesDataStore.Current.Cities.SelectMany(x => x.PointsOfInterest).Max(y => y.Id);

            var finalPointsOfInterest = new PointsOfInterestDto()
            {
                Id = ++maxPointsOfInterestId,
                Name = pointsOfInterest.Name,
                Description = pointsOfInterest.Description
            };

            city.PointsOfInterest.Add(finalPointsOfInterest);

            return CreatedAtRoute("GetPointOfInterest",
                new
                {
                    cityId = cityId,
                    pointOfInterestId = finalPointsOfInterest.Id
                },
                finalPointsOfInterest
                );//pointofinterestid
        }

        [HttpPut("{pointofinterestid}")]
        public ActionResult UpdatePointsOfInterest(int cityId, int pointofinterestid, PointsOfInterestForUpdateDto pointsOfInterest) 
        {
            //HttpPut is a FULL update
            var city = CitiesDataStore.Current.Cities.Find(x => x.Id == cityId);

            if (city == null)
            {
                return NotFound();
            }

            var pointsOfInterestFromStore = city.PointsOfInterest.FirstOrDefault(x => x.Id == pointofinterestid);

            if (pointsOfInterestFromStore == null)
            {
                return NotFound();
            }

            pointsOfInterestFromStore.Name = pointsOfInterest.Name;
            pointsOfInterestFromStore.Description = pointsOfInterest.Description;

            return NoContent();
        }

        [HttpPatch("{pointofinterestid}")]
        public ActionResult PartiallyUpdatePointsOfInterest (int cityId, int pointofinterestid, 
            JsonPatchDocument<PointsOfInterestForUpdateDto> patchDocument) 
        {
            var city = CitiesDataStore.Current.Cities.Find(x => x.Id == cityId);

            if (city == null)
                return NotFound();

            var pointsOfInterestFromStore = city.PointsOfInterest.FirstOrDefault(x => x.Id == pointofinterestid);

            if (pointsOfInterestFromStore == null)
                return NotFound();

            var pointsOfInterestToPatch = new PointsOfInterestForUpdateDto()
            {
                Name = pointsOfInterestFromStore.Name,
                Description = pointsOfInterestFromStore.Description
            };

            patchDocument.ApplyTo(pointsOfInterestToPatch, ModelState);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!TryValidateModel(pointsOfInterestToPatch)) 
                return BadRequest(ModelState);
            
            pointsOfInterestFromStore.Name = pointsOfInterestToPatch.Name;
            pointsOfInterestFromStore.Description = pointsOfInterestToPatch.Description;
            
            return NoContent();
        }

        [HttpDelete("{pointofinterestid}")]
        public ActionResult DeletePointsOfInterest(int cityId, int pointofinterestid)
        {
            var city = CitiesDataStore.Current.Cities.Find(x => x.Id == cityId);

            if (city == null)
                return NotFound();

            var pointsOfInterestFromStore = city.PointsOfInterest.FirstOrDefault(x => x.Id == pointofinterestid);

            if (pointsOfInterestFromStore == null)
                return NotFound();

            city.PointsOfInterest.Remove(pointsOfInterestFromStore);
            _mailService.Send(
                "Point of Interest deleted", 
                $"Point of Interest {pointsOfInterestFromStore.Name} with id {pointsOfInterestFromStore.Id} is deleted.");
            return NoContent();
        }
    }
}
