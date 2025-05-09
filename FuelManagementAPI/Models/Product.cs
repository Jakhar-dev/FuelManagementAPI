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
        public string ProductName { get; set; }
        public string? ProductDescription { get; set; }

        public int CategoryTypeId { get; set; }
        public ProductCategoryType ProductCategoryType { get; set; }

        public ICollection<FuelSale> FuelSales { get; set; }
        public ICollection<LubeSale> LubeSales { get; set; }
        public ICollection<Purchase> Purchases { get; set; }

    }
}
