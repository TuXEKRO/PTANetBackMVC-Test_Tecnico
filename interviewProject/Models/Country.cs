using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace interviewProject.Models
{
    public class Country
    {
        public int Id { get; set; }

        [Required]
        public string CountryName { get; set; }

        [Required]
        [MaxLength(2)]
        public string CountryCode { get; set; }

        public ICollection<Mba> Mbas { get; set; }
    }
}
