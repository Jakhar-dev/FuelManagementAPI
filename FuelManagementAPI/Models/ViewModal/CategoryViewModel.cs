using System.ComponentModel.DataAnnotations;

namespace FuelManagementAPI.Models.ViewModal
{
    public class CategoryViewModel
    {
        [Required]
        public string CategoryName { get; set; }

        public string? Description { get; set; }
    }
}
