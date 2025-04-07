using FuelManagementAPI.Models;
using System.ComponentModel.DataAnnotations;

public class FuelSaleViewModel
{
    [Required]
    public int ProductId { get; set; }

    [Range(0, double.MaxValue)]
    public decimal PreviousReading { get; set; }

    [Range(0, double.MaxValue)]
    public decimal CurrentReading { get; set; }

    [Range(0, double.MaxValue)]
    public decimal Testing { get; set; }

    [Range(0, double.MaxValue)]
    public decimal Price { get; set; }
}
