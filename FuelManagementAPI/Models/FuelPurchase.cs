namespace FuelManagementAPI.Models
{
    public class FuelPurchase
    {
        public int FuelPurchaseId { get; set; }
        public DateTime PurchaseDate { get; set; }

        public int CategoryTypeId { get; set; }
        public ProductCategoryType CategoryType { get; set; }

        public decimal PurchaseQuantity { get; set; }
        public decimal PurchasePrice { get; set; }
        public decimal Amount { get; set; }

        public string Description { get; set; }

    }
}
