using Microsoft.AspNetCore.Identity; 
using Microsoft.AspNetCore.Mvc;
using StudyFlow.ViewModels;

namespace StudyFlow.Controllers; 

public class UserController:Controller
{
    private readonly UserManager<IdentityUser> _userManager; 

    private readonly SignInManager<IdentityUser> _signInManager;


    public UserController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    //Get: /User/Register

    [HttpGet]
    public IActionResult Register() => View("Login", new AuthViewModel());

    //POST: /User/Register
    [HttpPost]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        
        if(ModelState.IsValid)
        {
            
            var user = new IdentityUser {UserName = model.UserName, Email= model.Email };
            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(user, isPersistent:false);


                
            }

            foreach (var error in result.Errors)
            {

                ModelState.AddModelError(string.Empty, error.Description); 
            }

        }


            return View("Login", new AuthViewModel {Registration = model} ); 

    }


    // GET: /User/Login

    [HttpGet]
    public IActionResult Login(string? returnUrl = null) => View(new AuthViewModel {ReturnUrl = returnUrl});



    //POST: /User/Login
    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
    {
       
            if (ModelState.IsValid)
        {
            var user = await  _userManager.FindByEmailAsync(model.Email);

            if (user != null){


            var result = await _signInManager.PasswordSignInAsync(
                user.UserName!, 
                //model.Email, 
                model.Password, 
                model.RememberMe, 
                lockoutOnFailure: false
            );
            if (result.Succeeded)
            {

            
                return RedirectToAction("Index", "Home"); 
            }}



            ModelState.AddModelError(string.Empty, "Invalid Login Attempt"); 
            

        }

        return View(new AuthViewModel {Login= model, ReturnUrl = returnUrl}); 
    }



    //POST: /Account/Logout 

    [HttpPost]
    public async Task <IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home"); 
    }
}