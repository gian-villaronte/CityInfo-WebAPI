using AutoMapper;
using CityInfo.API.Entities;
using CityInfo.API.Models;
using CityInfo.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mail;
using System.Text.Json;

namespace CityInfo.API.Controllers
{
    [Route("api/v{version:apiVersion}/cities")]
    [Authorize]
    [ApiVersion("1.0")]
    [ApiVersion("2.0")]
    //no need for it if we use this in middleware
    //app.UseEndpoints(endpoints => {  endpoints.MapControllers(); });
    [ApiController]
    public class CitiesController : ControllerBase
    {
        private ICityInfoRepository _cityInfoRepository;
        private readonly IMapper _mapper;
        const int maxCitiesPageSize = 20;

        public CitiesController(ICityInfoRepository cityInfoRepository, IMapper mapper)
        {
            _cityInfoRepository = cityInfoRepository ?? throw new ArgumentException(nameof(cityInfoRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CityModel>>> GetCities(
            string? name, 
            string? searchQuery,
            int pageNumber = 1,
            int pageSize = 10)
        {
            if (pageSize > maxCitiesPageSize)
                pageSize = maxCitiesPageSize;

            var (cityEntities, paginationMetadata) = await _cityInfoRepository
                .GetCitiesAsync(name, searchQuery, pageNumber, pageSize);

            Response.Headers.Add("X-Pagination",
                JsonSerializer.Serialize(paginationMetadata));

            if (cityEntities == null) 
                return NotFound();

            return Ok(_mapper.Map<IEnumerable<CityModel>>(cityEntities));
        }
        /// <summary>
        /// Get a city by id
        /// </summary>
        /// <param name="Id">the id of the city</param>
        /// <param name="includePointsOfInterest">Optional to include POI</param>
        /// <returns>An IActionResult</returns>
        /// <response code="200">Returns the requested city</response>
        [HttpGet("{Id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetCity(int Id, bool includePointsOfInterest = false)
        {
            var city = await _cityInfoRepository.GetCityAsync(Id, includePointsOfInterest); 

            if (city == null) 
                return NotFound();

            if(includePointsOfInterest)
                return Ok(_mapper.Map<CityDto>(city));

            return Ok(_mapper.Map<CityModel>(city));

        }
    }
}
