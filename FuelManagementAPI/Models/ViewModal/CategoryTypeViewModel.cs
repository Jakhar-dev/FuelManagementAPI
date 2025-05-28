using System.ComponentModel.DataAnnotations;

namespace FuelManagementAPI.Models.ViewModal
{
    public class CategoryTypeViewModel
    {
        [Required]
        public int CategoryId { get; set; }
        public string CategoryTypeName { get; set; }
        public string? Description { get; set; }
    }
}
