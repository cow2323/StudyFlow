using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudyFlow.Models;
using StudyFlow.ViewModels;

namespace StudyFlow.Controllers; 


public class StudySessionsController:Controller
{

    StudyFlowDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;


    public StudySessionsController(UserManager<IdentityUser> userManager, StudyFlowDbContext context)
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

        return View(StudySessions);
        
    }
    


    [HttpGet]
    [Authorize]
    public IActionResult Create(int? roomId)
    {
        if (!roomId.HasValue || roomId.Value <= 0)
        {
            
            return RedirectToAction("Index", "StudySessions");
        }

        return View(new StudySessionCreateViewModel
        {
            RoomId = roomId.Value,
            StartTime = DateTime.Now.AddHours(1),
            EndTime = DateTime.Now.AddHours(2)
        });

    }


    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(StudySessionCreateViewModel model)
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



            StudySession studySession = new StudySession
            {
                RoomId = model.RoomId,
                Course = model.Course,
                Topic = model.Topic,
                Description = model.Description,
                StartTime = model.StartTime,
                EndTime = model.EndTime,
                IsPublic = true // model currently ALWAYS PUBLIC
                
            }; 

            _context.StudySessions.Add(studySession); 
            )









    }





}