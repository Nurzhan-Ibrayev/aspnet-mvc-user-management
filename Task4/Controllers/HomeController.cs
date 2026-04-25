using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Task4.Models;
using Task4.Services;
using Task4.ViewModels;

namespace Task4.Controllers;

[Authorize]
public class HomeController : Controller
{
    private readonly UserManager<Users> userManager;
    private readonly SignInManager<Users> signInManager;   
    private readonly IUserService userService;

    public HomeController(SignInManager<Users> signInManager, UserManager<Users> userManager, IUserService userService)
    {
        this.signInManager = signInManager;
        this.userService = userService;
        this.userManager = userManager;
    }
    public async Task<IActionResult> Index()
    {
        var users = await userService.GetUsersAsync();
        return View(users);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Block(List<string> selectedIds)
    {
        var currentUserId = userManager.GetUserId(User);
        await userService.BlockUsersAsync(selectedIds);
        if(selectedIds.Contains(currentUserId))
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }
        return  RedirectToAction("Index");
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Unblock(List<string> selectedIds)
    {
        await userService.UnblockUsersAsync(selectedIds);
        return RedirectToAction("Index");
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(List<string> selectedIds)
    {
        var currentUserId = userManager.GetUserId(User);
        await userService.DeleteUsersAsync(selectedIds);
        if (selectedIds.Contains(currentUserId))
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }
        return RedirectToAction("Index");
        
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteUnverified()
    {
        var currentUserId = userManager.GetUserId(User);
        var deletedCurrentUser = await userService.DeleteUnverifiedUsersAsync(currentUserId);
        if (deletedCurrentUser)
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }
        return RedirectToAction("Index");
        
        
    }
}
