using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

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
        public decimal SaleQuantity { get; set; }
        public decimal Price { get; set; }
        public decimal Amount { get; set; }
        public int FuelEntryId { get; set; }
        [JsonIgnore]
        public FuelEntry? FuelEntry { get; set; }
    }
}
