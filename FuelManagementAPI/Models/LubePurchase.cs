using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FuelManagementAPI.Models
{
    public class LubePurchase : UserEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PurchaseId { get; set; }
        public DateTime PurchaseDate { get; set; }

        public int ProductCategoryTypeId { get; set; }
        public ProductCategoryType ProductCategoryType { get; set; }
        
        public int ProductId { get; set; }
        public Product Product { get; set; }

        public decimal PurchaseQuantity { get; set; }
        public decimal PurchasePrice { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }

    }
}
