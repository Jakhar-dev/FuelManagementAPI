using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FuelManagementAPI.Models
{
    public class Product
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ProductId { get; set; }
        [Required]
        public string ProductCategory { get; set; }
        [Required]
        public string ProductName { get; set; }
        public decimal? PurchasePrice { get; set; }
        public string? ProductDescription { get; set; }
        public DateTime Date {  get; set; }
    }
}
