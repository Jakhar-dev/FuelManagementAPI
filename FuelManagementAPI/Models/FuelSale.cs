using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FuelManagementAPI.Models
{
    public class FuelSale
    {        
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int FuelSaleId { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }
        public decimal PreviousReading { get; set; } = 0;
        public decimal CurrentReading { get; set; }
        public decimal Testing { get; set; }
        public decimal SaleQuantity => CurrentReading - PreviousReading - Testing;
        public decimal Price { get; set; }
        public decimal Amount => SaleQuantity * Price;
        public int FuelEntryId { get; set; }
        public FuelEntry? FuelEntry { get; set; }
    }
}
