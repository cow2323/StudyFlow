using System.ComponentModel.DataAnnotations;
namespace StudyFlow.ViewModels; 


class AuthViewModel
{
    public RegisterViewModel Registration {get; set;}
    public LoginViewModel Login {get; set;}

    public string? ReturnUrl {get; set;}

    public AuthViewModel()
    {
        Registration = new RegisterViewModel();
        Login = new LoginViewModel();
    }
    

}