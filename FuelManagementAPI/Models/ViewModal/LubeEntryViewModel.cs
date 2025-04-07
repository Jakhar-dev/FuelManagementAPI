using System.ComponentModel.DataAnnotations;

public class LubeEntryViewModel
{
    [Required]
    public DateTime Date { get; set; }

    [Required]
    public List<LubeSaleViewModel> Sales { get; set; } = new List<LubeSaleViewModel>();
}
