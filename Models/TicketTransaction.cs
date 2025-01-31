using System.ComponentModel.DataAnnotations;

namespace EventManagementAPI.Models
{
    public class TicketTransaction
    {
        [Key]
        public int TransactionId { get; set; }
        public int EventId { get; set; }
        public int TicketsSold { get; set; }
        public DateTime TransactionDate { get; set; }
    }
}
