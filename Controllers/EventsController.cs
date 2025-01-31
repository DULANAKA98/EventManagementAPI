using Microsoft.AspNetCore.Mvc;
using EventManagementAPI.Data;
using EventManagementAPI.Models;

namespace EventManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public EventsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("create")]
        public IActionResult CreateEvent(Event newEvent)
        {
            _context.Events.Add(newEvent);
            _context.SaveChanges();
            return Ok(newEvent);
        }
        [HttpPost("{id}/set-tickets")]
        public IActionResult SetTicketDetails(int id, [FromBody] int totalTickets)
        {
            var eventDetails = _context.Events.FirstOrDefault(e => e.EventId == id);
            if (eventDetails == null) return NotFound();

            eventDetails.TotalTickets = totalTickets;
            _context.SaveChanges();
            return Ok(eventDetails);
        }
        [HttpPost("{id}/sell-tickets")]
        public IActionResult SellTickets(int id, [FromBody] int ticketsToSell)
        {
            var eventDetails = _context.Events.FirstOrDefault(e => e.EventId == id);
            if (eventDetails == null) return NotFound();

            if (eventDetails.TicketsSold + ticketsToSell > eventDetails.TotalTickets)
                return BadRequest("Not enough tickets available.");

            eventDetails.TicketsSold += ticketsToSell;

            var transaction = new TicketTransaction
            {
                EventId = id,
                TicketsSold = ticketsToSell,
                TransactionDate = DateTime.Now
            };

            _context.TicketTransactions.Add(transaction);
            _context.SaveChanges();
            return Ok(new { eventDetails, transaction });
        }
        [HttpGet("partner/{partnerId}/sales-status")]
        public IActionResult GetPartnerSalesStatus(int partnerId)
        {
            var events = _context.Events.Where(e => e.PartnerId == partnerId).ToList();
            if (!events.Any()) return NotFound("No events found for this partner.");

            return Ok(events.Select(e => new
            {
                e.Name,
                e.TicketsSold,
                e.TotalTickets
            }));
        }
        [HttpGet("admin/sales-status")]
        public IActionResult GetAdminSalesStatus()
        {
            var events = _context.Events.ToList();
            var adminView = events.Select(e => new
            {
                e.Name,
                e.TicketsSold,
                e.TotalTickets,
                CommissionEarned = (e.TicketsSold * e.TicketPrice * (e.CommissionRate / 100)).ToString("F2")
            });

            return Ok(adminView);
        }
    }
}
