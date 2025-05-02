using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FuelManagementAPI.Models
{
    public class Product : UserEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ProductId { get; set; }
        [Required]
        [ForeignKey("ProductCategory")]
        public int CategoryId { get; set; }
        public ProductCategory ProductCategory { get; set; }
        [Required]
        public string ProductName { get; set; }
        public decimal? PurchasePrice { get; set; }
        public string? ProductDescription { get; set; }
        public DateTime Date {  get; set; }
    }
}
