using System.ComponentModel.DataAnnotations;

namespace Task4.ViewModels;

public class RegisterViewModel
{
    [Required(ErrorMessage = "Username is required")]
    [Display(Name = "Username")]
    public string  UserName { get; set; }
    
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress]
    public string Email { get; set; }
    
    [Required(ErrorMessage = "Password is required")]
    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    [Compare("ConfirmPassword", ErrorMessage = "The password and confirmation password do not match")]
    public string Password { get; set; }
    
    [Required(ErrorMessage = "Confirm Password is required")]
    [DataType(DataType.Password)]
    [Display(Name = "Confirm New Password")]
    public string ConfirmPassword { get; set; }
}