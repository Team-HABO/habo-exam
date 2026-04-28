namespace rest.Models
{
    public class Movie
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public required string ReleaseYear { get; set; }
        public required string Genre { get; set; }
        public int DirectorID { get; set; }
        public Director? Director { get; set; }
        public int ProductionCompanyID { get; set; }
        public ProductionCompany? ProductionCompany { get; set; }
    }
}
