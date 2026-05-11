using rest.Helpers;
using rest.Models;
using System.Text.Json.Serialization;

namespace rest.DTOs
{
    public class ProductionCompanyHateoasDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        [JsonPropertyName("_links")]
        public List<Link> Links { get; set; } = [];
        public ProductionCompanyHateoasDto(ProductionCompany pc)
        {
            Id = pc.Id;
            Name = pc.Name;
        }
    }
}
