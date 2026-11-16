using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudyFlow.DAL;
using StudyFlow.Models;
using StudyFlow.ViewModels;

namespace StudyFlow.Controllers; 


public class StudySessionController:Controller
{

    StudyFlowDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IStudySessionRepository _sessionRepository;


    public StudySessionController(
        UserManager<IdentityUser> userManager,
        StudyFlowDbContext context,
        IStudySessionRepository sessionRepository)
    {
        _context = context;
        _userManager = userManager;
        _sessionRepository = sessionRepository;
    }





    
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> Index()
    {

        var sessions = await _sessionRepository.GetAllAsync();

        return View(sessions);
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
                Session = studySession,
                IsHost = true


            };

          

            //studySession.Participants.Add(creator);


            _context.Bookings.Add(booking);

            studySession.Participants.Add(creator);

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
       public async Task<IActionResult> Edit(int id,
    [Bind("Id,RoomId,Course,Topic,Description,StartTime,EndTime,IsPublic,Participants")] StudySession session)
{
    if (id != session.Id) return NotFound();

    // BookingReference is `required` but not bound, so don't let it invalidate the model
    ModelState.Remove(nameof(StudySession.BookingReference));

    if (session.EndTime <= session.StartTime)
        ModelState.AddModelError(nameof(session.EndTime), "Sluttid må være etter starttid.");

    var existing = await _context.StudySessions
        .Include(s => s.BookingReference)
        .FirstOrDefaultAsync(s => s.Id == id);
    if (existing == null) return NotFound();

    var host = await _context.StudySessionParticipants
        .FirstOrDefaultAsync(p => p.IsHost && p.Session.Id == existing.Id);
    if (_userManager.GetUserId(User) != host?.UserId) return Forbid();

    bool overlap = await _context.Bookings.AnyAsync(b =>
        b.Id != existing.BookingReference.Id &&
        b.RoomId == session.RoomId &&
        b.StartTime < session.EndTime &&
        session.StartTime < b.EndTime);
    if (overlap)
        ModelState.AddModelError(nameof(session.RoomId), "Rommet er allerede booket i dette tidsrommet.");

    if (!ModelState.IsValid) return View(session);

    // Update the booking in place 
    existing.BookingReference.RoomId = session.RoomId;
    existing.BookingReference.StartTime = session.StartTime;
    existing.BookingReference.EndTime = session.EndTime;

    existing.RoomId = session.RoomId;
    existing.StartTime = session.StartTime;
    existing.EndTime = session.EndTime;
    existing.Course = session.Course;
    existing.Topic = session.Topic;
    existing.Description = session.Description;
    existing.IsPublic = session.IsPublic;

    await _context.SaveChangesAsync();
    return RedirectToAction(nameof(Index));
}




    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> AddUser(int id)
    {

        //Add user method for additional participants

        StudySession session = await _sessionRepository.GetStudySessionById(id); 
        string? userId = _userManager.GetUserId(User); 
    if (id != session.Id || userId == null) return BadRequest();

    foreach (StudySessionParticipant participant in _context.StudySessionParticipants)
    //Check that participant NOT in list from before
        {
            if (participant.UserId.Equals(userId) && session == participant.Session) return RedirectToAction(nameof(Index));
        }
    

    //Check capacity

    Room room = await _context.Rooms.FindAsync(session.RoomId);
    
    if (room == null || room.Capacity <= session.Participants.Count ){
        
        ModelState.AddModelError(nameof(room.Capacity), "Session at full capacity");
        return RedirectToAction(nameof(Index));}
    
    
    //Cannot join if session is not public
    if (!session.IsPublic){

        ModelState.AddModelError(nameof(session.IsPublic), "Session is private"); 
    
        return RedirectToAction(nameof(Index));}

    //Create new Participant
    StudySessionParticipant newParticipant = new StudySessionParticipant
        {
            Session = session, 
            UserId = userId,
            IsHost = false 
            
        };
    //Add Participant
    session.Participants.Add(newParticipant
        ); 

    //Add participant to DB

    _context.StudySessionParticipants.Add(newParticipant); 
    await _context.SaveChangesAsync(); 

        
        return RedirectToAction(nameof(Index));
    }

    }





