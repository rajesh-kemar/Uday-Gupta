using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TripApiEF.Models
{
    public class Driver
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        public ICollection<Trip>? Trips { get; set; } // nullable for EF
    }
}
