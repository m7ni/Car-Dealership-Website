using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PWEBAssignment.Data;
using PWEBAssignment.Models;
using PWEBAssignment.ViewModels;
using System.Data;

namespace PWEBAssignment.Controllers
{
	[Authorize(Roles = "Manager")]
	public class UserManagerController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly INotyfService _toastNotification;
        public UserManagerController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, INotyfService toastNotification)
        {
            _context = context;
            _userManager = userManager;
            _toastNotification = toastNotification;
        }
        public async Task<IActionResult> Index()
        {
            var userSelf = await _userManager.GetUserAsync(User);
            var users = await _userManager.Users.Where(c=>c.CompanyID == userSelf.CompanyID).ToListAsync();
            var userRolesViewModel = new List<UserRolesViewModel>();

            foreach (var user in users)
            {
                var role = await _userManager.GetRolesAsync(user);
           
                if (!role.Contains("Admin") && !role.Contains("Client"))
                {
                    userRolesViewModel.Add(new UserRolesViewModel
                    {
                        UserId = user.Id,
                        FirstName = user.firstName,
                        LastName = user.lastName,
                        UserName = user.UserName,
                        available = user.available,
                        Roles = await _userManager.GetRolesAsync(user),
                        /* Avatar = user.Avatar*/
                    });
                }
            }

            return View(userRolesViewModel);
        }
        public IActionResult Create(string type)
        {

            ViewData["Type"] = type;
            return View();
        }


        public async Task<IActionResult> ActivateUser(string userId)
        {
            var userAsked = await _userManager.FindByIdAsync(userId);
            var userSelf = await _userManager.GetUserAsync(User);

            if (userAsked == null || userAsked.Id == userSelf.Id)
            {
                _toastNotification.Information("You cannot change the status of yourself", 3);
                return RedirectToAction(nameof(Index));
            }
                
            
            if (userAsked.available)
                userAsked.available = false;
            else
                userAsked.available = true;


            await _userManager.UpdateAsync(userAsked);

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(string userId)
        {
	        var userAsked = await _userManager.FindByIdAsync(userId);
	        var userSelf = await _userManager.GetUserAsync(User);

	        if (userAsked == null || userAsked.Id == userSelf.Id)
	        {
		        _toastNotification.Information("You cannot delete yourself", 3);
		        return RedirectToAction(nameof(Index));

			}

			var teste = await _context.Reservations.Where(r => r.ClientUserId.ToString() == userId).FirstOrDefaultAsync();
	        if(teste!=null)
	        {
		        return RedirectToAction(nameof(Index));
			}
	        await _userManager.DeleteAsync(userAsked);

	        return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string type,[Bind("FirstName,LastName,Email,Password")] ManagerCreateUser newUser)
        {
	        if (ModelState.IsValid)
	        {
                var verify = await _userManager.FindByEmailAsync(newUser.Email);

                if (verify != null)
                {
                    _toastNotification.Information("An account with that email already exists in the database", 3);
                    return View(newUser);
				}
					


				var loggedUser = await _userManager.GetUserAsync(User);
		        var companyUser = await _context.Company.Where(c => c.Id == loggedUser.CompanyID).FirstOrDefaultAsync();
		        var defaultUser = new ApplicationUser
		        {
			        UserName = newUser.Email,
			        Email = newUser.Email,
			        firstName = newUser.FirstName,
			        lastName = newUser.LastName,
			        EmailConfirmed = true,
			        PhoneNumberConfirmed = true
		        };
		        var user = await _userManager.FindByEmailAsync(defaultUser.Email);
		        if (user == null)
		        {
			        await _userManager.CreateAsync(defaultUser, newUser.Password);

			        if (type.Equals("Manager"))
			        {
				        await _userManager.AddToRoleAsync(defaultUser,
					        Roles.Manager.ToString());
			        }
			        else
			        {
				        await _userManager.AddToRoleAsync(defaultUser,
					        Roles.Employee.ToString());
			        }


			        user = await _userManager.FindByEmailAsync(defaultUser.Email);
			        companyUser.Workers.Add(user);
			        await _context.SaveChangesAsync();
				}
		       
				return RedirectToAction(nameof(Index));
	        }
	        return View(newUser);
		}

    }
}

