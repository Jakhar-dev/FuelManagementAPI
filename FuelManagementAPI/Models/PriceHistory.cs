using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FuelManagementAPI.Models
{
    public class PriceHistory : UserEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PriceId { get; set; }
        public DateTime Date { get; set; }
        [Required]
        [ForeignKey("Product")]
        public int ProductId { get; set; }
        public Product Product {  get; set; }
        public PriceType PriceType {  get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
    }

    public enum PriceType
    {
        Sale,
        Purchase
    }

}

