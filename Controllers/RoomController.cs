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
        public async Task<IActionResult> Index(
    string? searchTerm,
    DateTime? selectedDate,
    TimeSpan? selectedTime)
        {
            var roomsQuery = _context.Rooms.AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var normalizedSearch = searchTerm
                    .Trim()
                    .Replace(" ", "")
                    .ToLower();

                roomsQuery = roomsQuery.Where(r =>
                    r.Name.Replace(" ", "").ToLower().Contains(normalizedSearch) ||
                    r.Location.ToLower().Contains(searchTerm.Trim().ToLower()));
            }
            var rooms = await roomsQuery
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