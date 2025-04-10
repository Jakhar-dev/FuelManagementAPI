using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FuelManagementAPI.Models
{
    public class ProductCategory
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CategoryId { get; set; }
        [Required]
        public string CategoryName { get; set; }
        public string? Description { get; set; }
        public DateTime DateCreated { get; set; } = DateTime.UtcNow;
    }
}
