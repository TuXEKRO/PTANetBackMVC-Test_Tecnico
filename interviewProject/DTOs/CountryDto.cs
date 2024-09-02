using System.ComponentModel.DataAnnotations;

namespace interviewProject.DTOs
{
    public class CountryDto
    {
        public string? Country { get; set; }

        [Required]
        [StringLength(2, MinimumLength = 2, ErrorMessage = "Country Code must be exactly 2 characters long.")]
        public string? CountryCode { get; set; }

        public List<MbaDto>? Mbas { get; set; }
        // public List<MbaDto>? Mbas { get; set; } = new List<MbaDto>();
    }

    public class MbaDto
    {
        public string Code { get; set; }
        public string Name { get; set; }
    }
}
