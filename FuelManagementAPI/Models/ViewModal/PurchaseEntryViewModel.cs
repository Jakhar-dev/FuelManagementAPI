using System.ComponentModel.DataAnnotations;

namespace FuelManagementAPI.Models.ViewModal
{
    // ViewModels/PurchaseEntryViewModel.cs
    public class PurchaseEntryViewModel
    {
        [Required]
        public DateTime PurchaseDate { get; set; }

        [Required]
        public List<FuelPurchaseViewModel> FuelPurchases { get; set; }
        public List<LubePurchaseViewModel> LubePurchases { get; set; }
    }

    public class LubePurchaseViewModel
    {
        public int ProductCategoryTypeId { get; set; }
        public int ProductId { get; set; }
        public decimal PurchaseQuantity { get; set; }
        public decimal PurchasePrice { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }
    }

    public class FuelPurchaseViewModel
    {
        public int ProductCategoryTypeId { get; set; }
        public decimal PurchaseQuantity { get; set; }
        public decimal PurchasePrice { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }
    }



}
