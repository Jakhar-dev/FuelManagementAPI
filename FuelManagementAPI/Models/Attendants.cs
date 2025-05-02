using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FuelManagementAPI.Models
{
    public class Attendant : UserEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AttendantId { get; set; }
        public string AttendantName { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public DateTime JoiningDate { get; set; }
        public DateTime? TerminationDate { get; set; }
        public bool IsActive { get; set; }
        public string Photo { get; set; } // Store as URL or Base64
        public string Description { get; set; }
    }
}
