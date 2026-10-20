using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudyFlow.Models;

namespace StudyFlow.Controllers
{
    public class RoomController : Controller
    {
        private readonly StudyFlowDbContext _context;

        public RoomController(StudyFlowDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var rooms = await _context.Rooms
                .OrderBy(r => r.Name)
                .ToListAsync();

            return View(rooms);
        }
    }
}