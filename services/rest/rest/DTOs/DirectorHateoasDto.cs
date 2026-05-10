using rest.Helpers;
using rest.Models;
using System.Text.Json.Serialization;

namespace rest.DTOs
{
    public class DirectorHateoasDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        [JsonPropertyName("_links")]
        public List<Link> Links { get; set; } = [];

        public DirectorHateoasDto(Director director)
        {
            Id = director.Id;
            FirstName = director.FirstName;
            LastName = director.LastName;
        }
    }
}
