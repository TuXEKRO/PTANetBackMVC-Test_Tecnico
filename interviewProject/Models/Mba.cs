using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace interviewProject.Models
{
    public class Mba
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Code { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        public int CountryId { get; set; }

        [JsonIgnore] // Ignore serialization to avoid circular reference
        public Country Country { get; set; }
    }
}
