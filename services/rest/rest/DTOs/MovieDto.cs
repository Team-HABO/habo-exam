namespace rest.DTOs
{
    public class MovieDto
    {
        public required string Title { get; set; }
        public required string ReleaseYear { get; set; }
        public required string Genre { get; set; }
        public int DirectorID { get; set; }
        public int ProductionCompanyID { get; set; }
    }
}
