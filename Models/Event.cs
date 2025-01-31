using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventManagementAPI.Models
{
    public class Event
    {
        [Key]
        public int EventId { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Location { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        public decimal TicketPrice { get; set; }

        [Required]
        public decimal CommissionRate { get; set; }
        [Required]
        public int TotalTickets { get; set; }
        public int TicketsSold { get; set; }

        [ForeignKey("PartnerId")]
        public int PartnerId { get; set; }

    }
}
