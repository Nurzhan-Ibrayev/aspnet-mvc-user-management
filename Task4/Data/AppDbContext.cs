using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Task4.Models;

namespace Task4.Data;

public class AppDbContext : IdentityDbContext<Users>
{	
	public AppDbContext(DbContextOptions options) : base(options)
	{
		
	}
	protected override void OnModelCreating(ModelBuilder builder)
	{
		base.OnModelCreating(builder);

		builder.Entity<Users>()
			.Property(u => u.Email)
			.IsRequired();

		builder.Entity<Users>()
			.HasIndex(u => u.Email)
			.IsUnique();
	}
}