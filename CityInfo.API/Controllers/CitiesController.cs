using CityInfo.API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mail;

namespace CityInfo.API.Controllers
{
    [Route("api/cities")]
    //no need for it if we use this in middleware
    //app.UseEndpoints(endpoints => {  endpoints.MapControllers(); });
    [ApiController]
    public class CitiesController : ControllerBase
    {
        [HttpGet]
        public ActionResult<IEnumerable<CityDto>> GetCities()
        {
            return Ok(CitiesDataStore.Current.Cities);
        }

        [HttpGet("{Id}")]
        public ActionResult<CityDto> GetCity(int Id)
        {
            var result = CitiesDataStore.Current.Cities.Find(x => x.Id == Id);
            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }
    }
}
