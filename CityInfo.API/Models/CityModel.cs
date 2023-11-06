namespace CityInfo.API.Models
{
    /// <summary>
    /// A DTO for a city without POI
    /// </summary>
    public class CityModel
    {
        /// <summary>
        /// Id of the city
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// name of the city
        /// </summary>
        public string Name { get; set; } = string.Empty;
        /// <summary>
        /// Description of the city
        /// </summary>
        public string? Description { get; set; }
    }
}
