using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FuelManagementAPI.Models
{
    public class PriceHistory : UserEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PriceId { get; set; }
        public DateTime Date { get; set; }

        public int CategoryId { get; set; }
        public ProductCategory Category { get; set; }

        public int CategoryTypeId { get; set; }
        public ProductCategoryType ProductCategoryType { get; set; }

        [Required]
        [ForeignKey("Product")]
        public int ProductId { get; set; }
        public Product Product {  get; set; }

        public string PriceType {  get; set; }
        public decimal Price { get; set; }
        public string? Description { get; set; }
    }
}

