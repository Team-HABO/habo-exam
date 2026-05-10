using rest.Helpers;
using rest.Models;
using System.Text.Json.Serialization;

namespace rest.DTOs
{
    public class MovieHateoasDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string ReleaseYear { get; set; }
        public string Genre { get; set; }
        public int DirectorID { get; set; }
        public int ProductionCompanyID { get; set; }
        [JsonPropertyName("_links")]
        public List<Link> Links { get; set; } = [];
        public MovieHateoasDto(Movie movie)
        {
            Id = movie.Id;
            Title = movie.Title;
            ReleaseYear = movie.ReleaseYear;
            Genre = movie.Genre;
            DirectorID = movie.DirectorID;
            ProductionCompanyID = movie.ProductionCompanyID;
        }
    }
}
