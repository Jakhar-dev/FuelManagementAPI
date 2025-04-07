using System.ComponentModel.DataAnnotations;

public class FuelEntryViewModel
{
    [Required]
    public DateTime Date { get; set; }

    [Required]
    public List<FuelSaleViewModel> Sales { get; set; } = new List<FuelSaleViewModel>();
}
