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

        [HttpGet]
        public async Task<IActionResult> Index(DateTime? selectedDate, TimeSpan? selectedTime)
        {
            var rooms = await _context.Rooms
                .OrderBy(r => r.Name)
                .ToListAsync();

            var unavailableRoomIds = new List<int>();

            if (selectedDate.HasValue && selectedTime.HasValue)
            {
                var selectedDateTime = selectedDate.Value.Date + selectedTime.Value;

                unavailableRoomIds = await _context.Bookings
                    .Where(b =>
                        b.StartTime <= selectedDateTime &&
                        b.EndTime > selectedDateTime)
                    .Select(b => b.RoomId)
                    .Distinct()
                    .ToListAsync();
            }

            ViewBag.UnavailableRoomIds = unavailableRoomIds;

            return View(rooms);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            if (id <= 0)
            {
                return NotFound();
            }

            var room = await _context.Rooms.FindAsync(id);

            if (room == null)
            {
                return NotFound();
            }

            return View(room);
        }

    }
}