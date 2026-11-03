using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudyFlow.Models;
using StudyFlow.ViewModels;

namespace StudyFlow.Controllers; 


public class StudySessionController:Controller
{

    StudyFlowDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;


    public StudySessionController(UserManager<IdentityUser> userManager, StudyFlowDbContext context)
    {
        _context = context;
        _userManager = userManager;
    }





    
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> Index()
    {

         var bookings = await _context.StudySessions
            .OrderBy(b => b.StartTime)
            .ToListAsync();

        return View("Index", "StudySessions");
    }
    


    [HttpGet]
    [Authorize]
    public IActionResult Create()
    {
        return View(new StudySessionViewModel
        {
            StartTime = DateTime.Now.AddHours(1),
            EndTime = DateTime.Now.AddHours(2)
        });}



    


    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(StudySessionViewModel model)
    {
        string? userId = _userManager.GetUserId(User); 

        if (string.IsNullOrEmpty(userId))
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


            Booking booking = new Booking
            {
                UserId = userId,
                RoomId = model.RoomId,
                StartTime = model.StartTime,
                EndTime = model.EndTime
            };


            


            StudySession studySession = new StudySession
            {
                RoomId = model.RoomId,
                Course = model.Course,

                BookingReference = booking, 
                Topic = model.Topic,
                Description = model.Description,
                StartTime = model.StartTime,
                EndTime = model.EndTime,
                IsPublic = true // model currently ALWAYS PUBLIC


            }; 


            StudySessionParticipant creator = new StudySessionParticipant 
            {
                UserId = userId,
                Session = studySession


            };

          

            //studySession.Participants.Add(creator);


            _context.Bookings.Add(booking);

            _context.StudySessions.Add(studySession); 

            _context.StudySessionParticipants.Add(creator);
 


            await _context.SaveChangesAsync(); 



            return RedirectToAction("Index", "StudySession");
    }


    [HttpGet]
        [Authorize]
        public async Task<IActionResult> Edit(int id)
        {
            var booking = await _context.StudySessions.FindAsync(id);

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
        [Bind("Id,RoomId,Course,Topic,Description,StartTime,EndTime,IsPublic")] StudySession session)
        {
       
        var booking = session.BookingReference;


        

        // Check id is found by system       
            if (id != session.Id)
            {
                return NotFound();
            }

            // Check end time is after start time
            if (session.EndTime <= session.StartTime)
            {
                ModelState.AddModelError(
                    nameof(session.EndTime),
                    "Sluttid må være etter starttid.");
            }
        // Make sure no existing BOOKINGs overlap
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
                return View(session);
            }

            StudySession existingBooking = await _context.StudySessions.FindAsync(id);

            if (existingBooking == null)
            {
                return NotFound();
            }



            // Check if the user is the host of the session



            StudySessionParticipant hostParticipant = await _context.StudySessionParticipants.FirstOrDefaultAsync(p => p.IsHost && p.Session.Id == existingBooking.Id);


            if (_userManager.GetUserId(User) != hostParticipant?.UserId)
            {
                return Forbid();
            }



//Check if the user has changed times of booking and create new booking if necessary. 
            if (existingBooking.StartTime != session.StartTime || existingBooking.EndTime != session.EndTime)
        {
            

            Booking newBooking = new Booking
            {
                UserId = hostParticipant.UserId,
                RoomId = session.RoomId,
                StartTime = session.StartTime,
                EndTime = session.EndTime
            };


            _context.Bookings.Remove(existingBooking.BookingReference);
            _context.Bookings.Add(newBooking);
        }

            existingBooking.RoomId = session.RoomId;
            existingBooking.StartTime = session.StartTime;
            existingBooking.EndTime = session.EndTime;
            existingBooking.Course = session.Course;
            existingBooking.Topic = session.Topic;
            existingBooking.Description = session.Description;
            existingBooking.IsPublic = session.IsPublic;
             //existingBooking.Participants = session.Participants;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }









    }





