using System.Text.Json.Serialization;

namespace rest.Helpers
{
    public class PaginatedResult<T>
    {
        [JsonPropertyName("_embedded")]
        public Dictionary<string, IEnumerable<T>> Embedded { get; set; } = [];
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
        [JsonPropertyName("_links")]
        public List<Link> Links { get; set; } = [];
    }
}
