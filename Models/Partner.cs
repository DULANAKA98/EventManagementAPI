using System.ComponentModel.DataAnnotations;

namespace EventManagementAPI.Models
{
    public class Partner
    {
        [Key]
        public int PartnerId { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string Phone { get; set; }
    }
}
