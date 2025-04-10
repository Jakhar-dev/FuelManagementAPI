using FuelManagementAPI.Models;

namespace FuelManagementAPI.ViewModels
{
    public class ProductViewModel
    {       
        public string ProductCategory { get; set; }
        public string ProductName { get; set; }
        public string ProductDescription { get; set; }      
        public DateTime Date {  get; set; } = DateTime.UtcNow;
    }
}
