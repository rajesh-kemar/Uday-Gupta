using kemar.WHL.Model.Common;
using System.ComponentModel.DataAnnotations;

namespace Kemar.WHL.Model.Request
{
    public class ProductRequest : CommonEntity
    {
        public int? ProductId { get; set; }

        [Required, MinLength(2), MaxLength(100)]
        public string Name { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }

        [Required, Range(0.01, double.MaxValue)]
        public decimal Weight { get; set; }
    }
}