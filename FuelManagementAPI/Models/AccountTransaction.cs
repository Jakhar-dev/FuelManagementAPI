using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace FuelManagementAPI.Models
{
    public class AccountTransaction
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TransactionId { get; set; }

        [Required]
        [ForeignKey("Account")]
        public int AccountId { get; set; } // Link to Customer Account
        [JsonIgnore]
        public Account Account { get; set; }

        public decimal Amount { get; set; } // Amount of transaction
        public string TransactionType { get; set; } = "Debit"; // Debit or Credit
        public string? Description { get; set; }
        public DateTime TransactionDate { get; set; } = DateTime.UtcNow;
    }
}

                                                                 