using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using StudyFlow.Models;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
namespace StudyFlow.Controllers;

public class UserController : Controller
{

    private readonly StudyFlowDbContext _dBcontext;

    public UserController(StudyFlowDbContext context)
    {
        _dBcontext = context;
    }



    [HttpPost]
    public async Task <IActionResult> Login(String Email, String PasswordHash)
    {
        //Simple Validation Form handling logins
        List<User> allUsers = await _dBcontext.Users.ToListAsync();

        User selected;

        foreach(var user in allUsers)
        {
            if (user.Email == Email && user.PasswordHash == PasswordHash)
            {

                //add UserID to session

                HttpContext.Session.SetInt32("UserId", user.Id); 

                
                Console.WriteLine($"User Found: \n {user.Email}, \n {user.Name}");

                selected = user; 

                // Logic For User views goes here
                //this needs a hash value / decryption protocol 

                 return RedirectToAction("Index", "Booking");
            }        
        
        }


    Console.WriteLine("User Not found"); 

     return View(nameof(Login));


    }



    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }
  



    [HttpGet]
    public IActionResult Create()
    {
        
        return View(); 
    }
 

    [HttpPost]
    public async Task<IActionResult> Create(User newUser)
    {

        try{

            var allUsers = await _dBcontext.Users.ToListAsync();


            // Check Model is valid and email is unique
            if (ModelState.IsValid && !allUsers.Any(u => u.Email == newUser.Email))
            {
                _dBcontext.Users.Add(newUser);
                _dBcontext.SaveChanges();
                return View(nameof(Login));

            }}
        catch (Exception ex)
        {
            Console.WriteLine($"Error creating user: {ex.Message}");
            ModelState.AddModelError(string.Empty, "An error occurred while creating the user.");
        }

     Console.WriteLine("User Creation failed. Model state is invalid.");
    return View(nameof(Login));
    }




    [HttpGet]
    public IActionResult Update(int id)
    {
        var user = _dBcontext.Users.Find(id);
        if (user == null)
        {
            
            return NotFound();
        }

        return View(user); 
    }
    
    [HttpPost]
    public IActionResult Update(User user)
    {
        if (ModelState.IsValid)
        {
            try{
            _dBcontext.Users.Update(_dBcontext.Users.Find(user.Id));
            _dBcontext.SaveChanges();
            return RedirectToAction(nameof(Login));
            }

            catch(Exception ex)
            {
                Console.WriteLine($"Something went wrong when updating user {user.Id}");
                
            }            
        }

        return View();
        
    }


    [HttpGet]
    public IActionResult Delete(int id)
    {
        var user = _dBcontext.Users.Find(id);
        if (user == null)
        {
            return NotFound();
        }

        return View(user);
    }


    [HttpPost]
    public IActionResult DeleteConfirmed(int id)
    {
        var user = _dBcontext.Users.Find(id);
        if (user == null)
        {
            return NotFound(); 
        }

        _dBcontext.Users.Remove(user);
        _dBcontext.SaveChanges();
        return RedirectToAction(nameof(Login));


    }






}