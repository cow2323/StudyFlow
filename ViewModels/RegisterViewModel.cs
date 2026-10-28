using System.ComponentModel.DataAnnotations; 

namespace StudyFlow.ViewModels; 

public class RegisterViewModel
{
[Required]
[EmailAddress]
public string Email {get; set;} = string.Empty;

[Required]
public String UserName {get; set;} = string.Empty;

[Required]
[DataType(DataType.Password)]
public string Password {get; set;} = string.Empty;


[DataType(DataType.Password)]
[Display(Name = "Confirm Password")]
[Compare("Password", ErrorMessage = "Passwords must match!")]
public string ConfirmPassword {get; set;} = string.Empty;



}