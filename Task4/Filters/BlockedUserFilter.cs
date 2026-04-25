using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Identity;
using Task4.Models;

public class BlockedUserFilter : IAsyncActionFilter
{
    private readonly UserManager<Users> userManager;
    private readonly SignInManager<Users> signInManager;

    public BlockedUserFilter(UserManager<Users> userManager, SignInManager<Users> signInManager)
    {
        this.userManager = userManager;
        this.signInManager = signInManager;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var user = context.HttpContext.User;

        if (user.Identity != null && user.Identity.IsAuthenticated)
        {
            var dbUser = await userManager.GetUserAsync(user);

            if (dbUser != null && dbUser.Status == "Blocked")
            {
                await signInManager.SignOutAsync();

                context.Result = new RedirectToActionResult("Login", "Account", null);
                return;
            }
        }

        await next();
    }
}