using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FuelManagementAPI.Models
{
    public class LubeSale : UserEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int LubeId { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal Amount { get; set; }
        public int LubeEntryId { get; set; }
        public LubeEntry? LubeEntry { get; set; }
    }

    public class LubeEntry : UserEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int LubeEntryId { get; set; }
        public DateTime Date { get; set; }
        public List<LubeSale> Sales { get; set; } = new List<LubeSale>();
    }
}
