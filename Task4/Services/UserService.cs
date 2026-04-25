using Task4.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Task4.ViewModels;

namespace Task4.Services;

public interface IUserService
{
    Task<List<UsersViewModel>> GetUsersAsync();
    Task BlockUsersAsync(List<string> ids);
    Task UnblockUsersAsync(List<string> ids);
    Task DeleteUsersAsync(List<string> ids);
    Task<bool> DeleteUnverifiedUsersAsync(string currentUserId);
}
public class UserService:IUserService
{
    private readonly UserManager<Users> userManager;

    public UserService(UserManager<Users> userManager)
    {
        this.userManager = userManager;
    }

    public async Task<List<UsersViewModel>> GetUsersAsync()
    {
        
        return await userManager.Users
            .OrderByDescending(u => u.LastLoginTime)
            .Select(u => new UsersViewModel
            {
                Id = u.Id,
                Name = u.UserName,
                Email = u.Email,
                LastLoginTime = u.LastLoginTime,
                Status = u.Status.ToString()
            })
            .ToListAsync();
    }

    public async Task BlockUsersAsync(List<string> ids)
    {
        var users = await userManager.Users
            .Where(u => ids.Contains(u.Id))
            .ToListAsync();
        foreach (var user in users)
        {
            user.Status = "Blocked";
            await userManager.UpdateAsync(user);
        }
    }
    public async Task UnblockUsersAsync(List<string> ids)
    {
        var users = await userManager.Users
            .Where(u => ids.Contains(u.Id))
            .ToListAsync();
        foreach (var user in users)
        {
            user.Status = "Active";
            await userManager.UpdateAsync(user);
        }
    }
    public async Task DeleteUsersAsync(List<string> ids)
    {
        var users = await userManager.Users
            .Where(u => ids.Contains(u.Id))
            .ToListAsync();
        foreach (var user in users)
        {
            await userManager.DeleteAsync(user);
        }
    }
    public async Task<bool> DeleteUnverifiedUsersAsync(string currentUserId)
    {
        bool deletedCurrentUser = false;

        
        var users = await userManager.Users
            .Where(u => u.Status == "Unverified")
            .ToListAsync();
        foreach (var user in users)
        {
            if (user.Id == currentUserId)
                deletedCurrentUser = true;


            await userManager.DeleteAsync(user);
        }

        return deletedCurrentUser;
    }
}