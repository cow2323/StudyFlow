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


                //DEBUG LINE
                Console.WriteLine("Success Reg");
                return RedirectToAction("Index", "Home"); 
            }

            foreach (var error in result.Errors)
            {

            //DEBUG LINE
                Console.WriteLine(" Something wrong in registration: ");
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
        //Default to home page if ReturnUrl is NULL or Empty
        /*string targetUrl = !string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl)
            ? model.ReturnUrl
            : Url.Action("Index","Home" )?? "~/";
*/
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

            //DEBUG LINE
                Console.WriteLine("Success Login");
                return RedirectToAction("Index", "Home"); 
            }}



            ModelState.AddModelError(string.Empty, "Invalid Login Attempt"); 
            
            //DEBUG LINE
            Console.WriteLine("Something wenbt wrong in login ");
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