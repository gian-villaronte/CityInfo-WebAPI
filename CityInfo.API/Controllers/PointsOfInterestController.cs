using AutoMapper;
using CityInfo.API.Entities;
using CityInfo.API.Models;
using CityInfo.API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace CityInfo.API.Controllers
{
    [Route("api/cities/{cityId}/pointsofinterest")]
    [ApiController]
    public class PointsOfInterestController : ControllerBase
    {
        private ILogger<PointsOfInterestController> _logger;
        private IMailService _mailService;
        private readonly ICityInfoRepository _cityInfoRepository;
        private readonly IMapper _mapper;

        public PointsOfInterestController(
            ILogger<PointsOfInterestController> logger, 
            IMailService mailService,
            ICityInfoRepository cityInfoRepository, 
            IMapper mapper)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mailService = mailService ?? throw new ArgumentNullException(nameof(mailService));
            _cityInfoRepository = cityInfoRepository ?? throw new ArgumentNullException(nameof(cityInfoRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PointsOfInterestDto>>> GetPointsOfInterest(int cityId) 
        {
            try
            {
                if (!await _cityInfoRepository.CityExistsAsync(cityId))
                { 
                    _logger.LogInformation($"City with Id {cityId} wasn't found when accessing points of interest.");
                    return NotFound();
                }
                
                var pointsOfInterestForCity = await _cityInfoRepository.GetPointsOfInterestForCityAsync(cityId);

                return Ok(_mapper.Map<IEnumerable<PointsOfInterestDto>>(pointsOfInterestForCity));
            }
            catch (Exception ex) 
            {
                _logger.LogCritical($"Critical Exception while getting points of interest for city id {cityId}.", ex);
                return StatusCode(500, "A problem happened while handling your request.");
            }
        }

        [HttpGet("{pointofinterestid}", Name = "GetPointOfInterest")]
        public async Task<ActionResult<PointsOfInterestDto>> GetPointOfInterest(int cityId, int pointofinterestid) 
        {
            if (!await _cityInfoRepository.CityExistsAsync(cityId))
            {
                _logger.LogInformation($"City with Id {cityId} wasn't found when accessing points of interest.");
                return NotFound();
            }

            var pointOfInterest = await _cityInfoRepository.GetPointOfInterestForCityAsync (cityId, pointofinterestid);
                //city.PointsOfInterest.FirstOrDefault(x => x.Id == pointofinterestid);
            if (pointOfInterest == null) 
            {
                _logger.LogInformation($"Point of interest id {pointofinterestid} in City with Id {cityId} wasn't found.");
                return NotFound();
            }

            return Ok(_mapper.Map<PointsOfInterestDto>(pointOfInterest));
        }

        [HttpPost]
        public async Task<ActionResult<PointsOfInterestDto>> CreatePointsOfInterest(int cityId, PointsOfInterestForCreationDto pointsOfInterest)
        {
            if (!await _cityInfoRepository.CityExistsAsync(cityId))
            {
                _logger.LogInformation($"City with Id {cityId} wasn't found when trying to add a point of interest.");
                return NotFound();
            }

            var mappedToDB = _mapper.Map<Entities.PointsOfInterest>(pointsOfInterest);

            await _cityInfoRepository.AddPointOfInterestForCityAsync(cityId, mappedToDB);

            await _cityInfoRepository.SaveChangesAsync();

            var mappedBack = _mapper.Map<Models.PointsOfInterestDto>(mappedToDB);


            return CreatedAtRoute("GetPointOfInterest",
                new
                {
                    cityId = cityId,
                    pointOfInterestId = mappedBack.Id
                },
                mappedBack
                );//pointofinterestid
        }

        [HttpPut("{pointofinterestid}")]
        public async Task<ActionResult> UpdatePointsOfInterest(int cityId, int pointofinterestid, PointsOfInterestForUpdateDto pointsOfInterest)
        {
            //HttpPut is a FULL update
            if (!await _cityInfoRepository.CityExistsAsync(cityId))
            {
                return NotFound();
            }


            var pointOfInterest = await _cityInfoRepository.GetPointOfInterestForCityAsync(cityId, pointofinterestid);

            if (pointOfInterest == null)
            {
                return NotFound();
            }

            _mapper.Map(pointsOfInterest,pointOfInterest);

            await _cityInfoRepository.SaveChangesAsync();
            

            return NoContent();
        }

        [HttpPatch("{pointofinterestid}")]
        public async Task<ActionResult> PartiallyUpdatePointsOfInterest(int cityId, int pointofinterestid,
            JsonPatchDocument<PointsOfInterestForUpdateDto> patchDocument)
        {
            if (!await _cityInfoRepository.CityExistsAsync(cityId))
                return NotFound();

            var pointsOfInterestEntity = await _cityInfoRepository.GetPointOfInterestForCityAsync(cityId,pointofinterestid);

            if (pointsOfInterestEntity == null)
                return NotFound();

            var pointsOfInterestToPatch = _mapper.Map<PointsOfInterestForUpdateDto>(pointsOfInterestEntity);

            patchDocument.ApplyTo(pointsOfInterestToPatch, ModelState);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!TryValidateModel(pointsOfInterestToPatch))
                return BadRequest(ModelState);

            _mapper.Map(pointsOfInterestToPatch, pointsOfInterestEntity);

            await _cityInfoRepository.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{pointofinterestid}")]
        public async Task<ActionResult> DeletePointOfInterest(
            int cityId, int pointOfInterestId)
        {
            if (!await _cityInfoRepository.CityExistsAsync(cityId))
            {
                return NotFound();
            }

            var pointOfInterestEntity = await _cityInfoRepository
                .GetPointOfInterestForCityAsync(cityId, pointOfInterestId);
            if (pointOfInterestEntity == null)
            {
                return NotFound();
            }

            _cityInfoRepository.DeletePointOfInterest(pointOfInterestEntity);
            await _cityInfoRepository.SaveChangesAsync();

            _mailService.Send(
                "Point of interest deleted.",
                $"Point of interest {pointOfInterestEntity.Name} with id {pointOfInterestEntity.Id} was deleted.");

            return NoContent();
        }
    }
}
