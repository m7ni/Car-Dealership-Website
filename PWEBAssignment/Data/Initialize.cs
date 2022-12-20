using Microsoft.AspNetCore.Identity;
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
			userManager, RoleManager<IdentityRole> roleManager)
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
		}
	}
}
