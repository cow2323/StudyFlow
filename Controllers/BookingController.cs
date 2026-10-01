using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudyFlow.Models;

namespace StudyFlow.Controllers
{
    public class BookingController : Controller
    {
        private readonly StudyFlowDbContext _context;

        public BookingController(StudyFlowDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var bookings = await _context.Bookings
                .OrderBy(b => b.StartTime)
                .ToListAsync();

            return View(bookings);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new Booking
            {
                StartTime = DateTime.Now.AddHours(1),
                EndTime = DateTime.Now.AddHours(2)
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("RoomId,Subject,Topic,StartTime,EndTime")] Booking booking)
        {


            int? sessionUserId = HttpContext.Session.GetInt32("UserId");

            if (sessionUserId == null)
            {
                return RedirectToAction("Login", "User");
            }

            booking.UserId = sessionUserId.Value;


            try{if (booking.EndTime <= booking.StartTime)
            {
                ModelState.AddModelError(
                    nameof(Booking.EndTime),
                    "Sluttid må være etter starttid.");
            }}
            catch (Exception ex){Console.WriteLine($"Error: {ex.Message}");}

            var overlap = await _context.Bookings.AnyAsync(b =>
                b.RoomId == booking.RoomId &&
                b.StartTime < booking.EndTime &&
                booking.StartTime < b.EndTime);

            if (overlap)
            {
                ModelState.AddModelError(
                    nameof(Booking.RoomId),
                    "Rommet er allerede booket i dette tidsrommet.");
            }

            if (!ModelState.IsValid)
            {
                return View(booking);
            }

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);

            if (booking != null)
            {
                _context.Bookings.Remove(booking);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudyFlow.Models;

namespace StudyFlow.Controllers
{
    public class BookingController : Controller
    {
        private readonly StudyFlowDbContext _context;

        public BookingController(StudyFlowDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var bookings = await _context.Bookings
                .OrderBy(b => b.StartTime)
                .ToListAsync();

            return View(bookings);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new Booking
            {
                StartTime = DateTime.Now.AddHours(1),
                EndTime = DateTime.Now.AddHours(2)
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("RoomId,Subject,Topic,StartTime,EndTime")] Booking booking)
        {
            if (booking.EndTime <= booking.StartTime)
            {
                ModelState.AddModelError(
                    nameof(Booking.EndTime),
                    "Sluttid må være etter starttid.");
            }

            var overlap = await _context.Bookings.AnyAsync(b =>
                b.RoomId == booking.RoomId &&
                b.StartTime < booking.EndTime &&
                booking.StartTime < b.EndTime);

            if (overlap)
            {
                ModelState.AddModelError(
                    nameof(Booking.RoomId),
                    "Rommet er allerede booket i dette tidsrommet.");
            }

            if (!ModelState.IsValid)
            {
                return View(booking);
            }

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);

            if (booking == null)
            {
                return NotFound();
            }

            return View(booking);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            [Bind("Id,RoomId,StartTime,EndTime")] Booking booking)
        {
            if (id != booking.Id)
            {
                return NotFound();
            }

            if (booking.EndTime <= booking.StartTime)
            {
                ModelState.AddModelError(
                    nameof(Booking.EndTime),
                    "Sluttid må være etter starttid.");
            }

            var overlap = await _context.Bookings.AnyAsync(b =>
                b.Id != booking.Id &&
                b.RoomId == booking.RoomId &&
                b.StartTime < booking.EndTime &&
                booking.StartTime < b.EndTime);

            if (overlap)
            {
                ModelState.AddModelError(
                    nameof(Booking.RoomId),
                    "Rommet er allerede booket i dette tidsrommet.");
            }

            if (!ModelState.IsValid)
            {
                return View(booking);
            }

            var existingBooking = await _context.Bookings.FindAsync(id);

            if (existingBooking == null)
            {
                return NotFound();
            }

            existingBooking.RoomId = booking.RoomId;
            existingBooking.StartTime = booking.StartTime;
            existingBooking.EndTime = booking.EndTime;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);

            if (booking != null)
            {
                _context.Bookings.Remove(booking);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}