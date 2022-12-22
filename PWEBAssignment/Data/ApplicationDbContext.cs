using PWEBAssignment.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace PWEBAssignment.Data
{
	public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
	{
        public DbSet<Car> Car { get; set; }
        public DbSet<Company> Company { get; set; }
        public DbSet<Reservations> Reservations { get; set; }
        public DbSet<Deliveries> Deliveries { get; set; } 
        public DbSet<Returns> Returns { get; set; }
        public DbSet<Category> Category { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
			: base(options)
		{
		}
	}
}