using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace FuelManagementAPI.Models
{
    public class FuelEntry
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int FuelEntryId { get; set; } // Primary Key
        public DateTime Date { get; set; }
        public List<FuelSale> Sales { get; set; } = new List<FuelSale>();
    }

}
