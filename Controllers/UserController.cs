using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using StudyFlow.Models;
namespace MyShop.Controllers; 



public class UserController : Controller
{

    private readonly StudyFlowDbContext _context;

    public UserController(StudyFlowDbContext context)
    {
        _context = context;
    }


    //public IActionResult Grid()

    //public IActionResultDetails()



 
    public IActionResult Table()
    {

        //Midlertidig løsning for å vise bookings i tabellen.
        var bookings = new List<String>();

        var sampleBooking = "Mandag 1 sept kl 10:00-11:00, Rom 101";
        var sampleBooking2 = "Tirsdag 2 sept kl 12:00-13:00, Rom 102";

        bookings.Add(sampleBooking);
        bookings.Add(sampleBooking2);
        return View(bookings);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Create(User user)
    {
        if (ModelState.IsValid)
        {
            _context.Users.Add(user);
            _context.SaveChanges();
            return RedirectToAction(nameof(Table));

        }

        return View(user);
    }


}