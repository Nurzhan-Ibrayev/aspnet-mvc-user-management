using Microsoft.AspNetCore.Mvc;
using Task4.ViewModels;
using Task4.Models;
using Task4.Services;
using Microsoft.AspNetCore.Identity;
using System.Text;
using Microsoft.AspNetCore.WebUtilities;

namespace Task4.Controllers;




public class AccountController : Controller
{
    private readonly SignInManager<Users> signInManager;   
    private readonly UserManager<Users> userManager;
    private readonly IEmailService emailService;

    public AccountController(SignInManager<Users> signInManager, UserManager<Users> userManager, IEmailService emailService)
    {
        this.signInManager = signInManager;
        this.userManager = userManager;
        this.emailService = emailService;
    }
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }
    
        var userCheck = await userManager.FindByEmailAsync(model.Email);
        if (userCheck == null)
        {
            ModelState.AddModelError("", "Invalid login attempt");
            return View(model);
        }
        if (userCheck.Status == "Blocked")
        {
            ModelState.AddModelError("", "User is blocked");
            return View(model);
        }
        var result = await signInManager.PasswordSignInAsync(userCheck.UserName, model.Password, model.RememberMe, false);
        if (!result.Succeeded)
        {
            ModelState.AddModelError("", "Invalid login attempt");
            return View(model);
        }
        userCheck.LastLoginTime = DateTime.UtcNow;
        await userManager.UpdateAsync(userCheck);
        return RedirectToAction("Index", "Home");
        
}
    
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }
        Users users = new Users()
        {
            UserName = model.UserName,
            Email = model.Email,

        };
        var result = await userManager.CreateAsync(users, model.Password);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
            return View(model);
        }
        var token = await userManager.GenerateEmailConfirmationTokenAsync(users);
        var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
        var confirmUrl = Url.Action(
            "ConfirmEmail",
            "Account",
            new { email = users.Email, token = encodedToken },
            protocol: Request.Scheme
        );
        try
        {
            await emailService.SendEmailAsync(users.Email, "Confirm", $"Link: {confirmUrl}");
        }
        catch
        {
        
        }
        
        
        return RedirectToAction("Login", "Account");
        
    }
    public async Task<IActionResult> Logout()
    {
        await signInManager.SignOutAsync();
        return RedirectToAction("Login", "Account");
    }
    

    public async Task<IActionResult> ConfirmEmail(string email, string token)
    {
        if (email == null || token == null)
            return BadRequest();

        var user = await userManager.FindByEmailAsync(email);
        if (user == null)
            return NotFound();

        var decodedToken = Encoding.UTF8.GetString(
            WebEncoders.Base64UrlDecode(token)
        );

        var result = await userManager.ConfirmEmailAsync(user, decodedToken);

        if (!result.Succeeded)
        {
            return View(false);
        }
        user.Status = "Active";
        await userManager.UpdateAsync(user);
        return View(true);
    }
}