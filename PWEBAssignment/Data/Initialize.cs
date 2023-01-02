using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PWEBAssignment.Models;

namespace PWEBAssignment.Data
{
	public enum Roles
	{
		Admin,
		Employee,
		Manager,
		Client
	}
	public static class Initialize
	{
		public static async Task CriaDadosIniciais(UserManager<ApplicationUser>
			userManager, RoleManager<IdentityRole> roleManager, ApplicationDbContext context)
		{
			//Adicionar default Roles
			await roleManager.CreateAsync(new IdentityRole(Roles.Admin.ToString()));
			await roleManager.CreateAsync(new IdentityRole(Roles.Manager.ToString()));
			await roleManager.CreateAsync(new IdentityRole(Roles.Client.ToString()));
			await roleManager.CreateAsync(new IdentityRole(Roles.Employee.ToString()));
			//Adicionar Default User - Admin
			var defaultUser = new ApplicationUser
			{
				UserName = "admin@localhost.com", 
				Email = "admin@localhost.com",
				firstName = "Administrador",
				lastName = "Local",
				EmailConfirmed = true,
				PhoneNumberConfirmed = true
			};
			var user = await userManager.FindByEmailAsync(defaultUser.Email);
			if (user == null)
			{
				await userManager.CreateAsync(defaultUser, "Is3C..00");
				await userManager.AddToRoleAsync(defaultUser,
					Roles.Admin.ToString());
			}


            if (context.Category.IsNullOrEmpty())
            {
                var categoria = new Category();
                categoria.Name = "SUV";
                categoria.PriceHour = 30;
                context.Category.Add(categoria);
                context.SaveChanges();

                categoria = new Category();
                categoria.Name = "SPORT";
                categoria.PriceHour = 50;
                context.Category.Add(categoria);
                await context.SaveChangesAsync();

                categoria = new Category();
                categoria.Name = "MICRO";
                categoria.PriceHour = 20;
                context.Category.Add(categoria);
                await context.SaveChangesAsync();


                categoria = new Category();
                categoria.Name = "LUXURY";
                categoria.PriceHour = 50;
                context.Category.Add(categoria);
                await context.SaveChangesAsync();

                categoria = new Category();
                categoria.Name = "LIGHT";
                categoria.PriceHour = 25;
                context.Category.Add(categoria);
                await context.SaveChangesAsync();

            }
        }
	}
}
