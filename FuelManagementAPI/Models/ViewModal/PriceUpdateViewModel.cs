namespace FuelManagementAPI.Models
{
    public class PriceUpdateViewModel
    {
        public DateTime Date { get; set; }
        public int CategoryId { get; set; }
        public int CategoryTypeId { get; set; }
        public string PriceType { get; set; } // "Sale" or "Purchase"
        public List<ProductPriceViewModel> Products { get; set; }
    }

    public class ProductPriceViewModel
    {
        public int ProductId { get; set; }
        public decimal Price { get; set; }
    }
        
}
