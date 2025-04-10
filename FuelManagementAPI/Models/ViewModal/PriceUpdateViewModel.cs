namespace FuelManagementAPI.Models
{
    public class PriceUpdateViewModel
    {
        public int CategoryId { get; set; }
        public DateTime Date { get; set; }

        public List<PriceProductEntry> Products { get; set; }
    }

    public class PriceProductEntry
    {
        public int ProductId { get; set; }
        public decimal Price { get; set; }
    }
}
