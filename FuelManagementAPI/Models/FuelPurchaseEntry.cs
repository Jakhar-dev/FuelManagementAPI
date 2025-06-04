using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FuelManagementAPI.Models
{
    public class PurchaseEntry : UserEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PurchaseEntryId { get; set; }
        [Required]
        public DateTime? PurchaseDate { get; set;}

        public ICollection<Purchase> Purchases { get; set; }
    }
}
