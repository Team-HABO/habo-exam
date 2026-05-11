using System.ComponentModel.DataAnnotations;

namespace rest.DTOs
{
    /// <summary>
    /// DTO for update and create movie endpoints
    /// This class validates the input in the body
    /// </summary>
    public class MovieDto
    {
        [StringLength(200)]
        public required string Title { get; set; }
        [RegularExpression(@"^\d{4}$", ErrorMessage = "Release Year must be exactly 4 digits.")]
        public required string ReleaseYear { get; set; }
        [StringLength(60)]
        public required string Genre { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "DirectorID must be a positive number.")]
        public int DirectorID { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "ProductionCompanyID must be a positive number.")]
        public int ProductionCompanyID { get; set; }
    }
}
