using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FuelManagementAPI.Models
{
    public class Account
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AccountId { get; set; }

        [Required]
        public string CustomerName { get; set; }

        [Required]
        [Phone]
        public string CustomerPhone { get; set; }
        [Required]
        public string CustomerAdress {  get; set; }

        public decimal TotalDebit { get; set; } = 0; // Total amount customer owes
        public decimal TotalCredit { get; set; } = 0; // Total amount customer paid

       public decimal Balance => TotalDebit - TotalCredit; // Remaining balance

        public string? Description { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public List<AccountTransaction> Transactions { get; set; } = new List<AccountTransaction>();
    }
}
