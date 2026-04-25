using System.ComponentModel.DataAnnotations;
namespace Task4.ViewModels;

public class ChangePasswordViewModel
{
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress]
    public string Email { get; set; }
    
    [Required(ErrorMessage = "Password is required")]
    [DataType(DataType.Password)]
    [Display(Name = "New Password")]
    [Compare("ConfirmNewPassword", ErrorMessage = "The password and confirmation password do not match")]
    public string NewPassword { get; set; }
    
    [Required(ErrorMessage = "ConfirmPassword  is required")]
    [DataType(DataType.Password)]
    [Display(Name = "Confirm New Password")]
    public string ConfirmNewPassword { get; set; }
}