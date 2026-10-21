using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudyFlow.Models;
using StudyFlow.ViewModels;

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
        [Authorize]
        public async Task<IActionResult> Index()
        {
            var bookings = await _context.Bookings
                .OrderBy(b => b.StartTime)
                .ToListAsync();

            return View(bookings);
        }

        [HttpGet]
        [Authorize]
        public IActionResult Create()
        {
            return View(new BookingCreateViewModel
            {
                StartTime = DateTime.Now.AddHours(1),
                EndTime = DateTime.Now.AddHours(2)
            });
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BookingCreateViewModel model)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return RedirectToAction("Login", "User");
            }
            if (model.StartTime < DateTime.Now)
            {
                ModelState.AddModelError(
                    nameof(model.StartTime),
                    "Du kan ikke booke et rom i fortiden.");
            }
            
            if (model.EndTime <= model.StartTime)
            {
                ModelState.AddModelError(
                    nameof(model.EndTime),
                    "Sluttid må være etter starttid.");
            }

            if (ModelState.IsValid)
            {
                bool overlap = await _context.Bookings.AnyAsync(b =>
                    b.RoomId == model.RoomId &&
                    b.StartTime < model.EndTime &&
                    model.StartTime < b.EndTime);

                if (overlap)
                {
                    ModelState.AddModelError(
                        nameof(model.RoomId),
                        "Rommet er allerede booket.");
                }
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var booking = new Booking
            {
                UserId = userId.Value,
                RoomId = model.RoomId,
                StartTime = model.StartTime,
                EndTime = model.EndTime
            };

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        [Authorize]
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
        [Authorize]
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
        [Authorize]
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