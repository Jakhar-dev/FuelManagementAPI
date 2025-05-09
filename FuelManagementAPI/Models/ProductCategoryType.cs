using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FuelManagementAPI.Models
{
    public class ProductCategoryType : UserEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CategoryTypeId { get; set; }
        [Required]
        public string CategoryTypeName { get; set; }
        public string? Description { get; set; }
        public DateTime DateCreated { get; set; } = DateTime.UtcNow;
        public int CategoryId { get; set; }
        public ProductCategory productCategory { get; set; }
        public ICollection<Product> Products { get; set; }
    }
}
