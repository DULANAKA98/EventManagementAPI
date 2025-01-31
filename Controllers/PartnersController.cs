using Microsoft.AspNetCore.Mvc;
using EventManagementAPI.Data;
using EventManagementAPI.Models;

namespace EventManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PartnersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PartnersController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("register")]
        public IActionResult RegisterPartner(Partner partner)
        {
            _context.Partners.Add(partner);
            _context.SaveChanges();
            return Ok(partner);
        }
    }
}
