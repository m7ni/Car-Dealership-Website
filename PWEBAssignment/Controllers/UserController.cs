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
	[Authorize(Roles = "Admin")]
	public class UserController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;

        public UserController(UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var users = await _userManager.Users.ToListAsync();
            var userRolesViewModel = new List<UserRolesViewModel>();

            foreach (var user in users)
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

            return View(userRolesViewModel);
        }
          private async Task<List<string>> GetUserRoles(ApplicationUser user)
        {
            return new List<string>(await _userManager.GetRolesAsync(user));
        }

        public async Task<IActionResult> Details(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            var manageUserRolesViewModels = new List<ManageUserRolesViewModel>();

            foreach (var role in await _roleManager.Roles.ToListAsync())
            {
                manageUserRolesViewModels.Add(new ManageUserRolesViewModel
                {
                    RoleId = role.Id,
                    RoleName = role.Name,
                    Selected = await _userManager.IsInRoleAsync(user, role.Name)
                });
            }

            ViewBag.UserName = user.UserName;
            /*ViewBag.Avatar = user.Avatar;*/
            return View(manageUserRolesViewModels);
        }
        [HttpPost]
        public async Task<IActionResult> Details(List<ManageUserRolesViewModel> model,
            string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return View();
            }

            var roles = await _userManager.GetRolesAsync(user);
            var result = await _userManager.RemoveFromRolesAsync(user, roles);

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Cannot remove user existing roles");
                return View(model);
            }

            result = await _userManager.AddToRolesAsync(
                user, 
                model
                    .Where(r => r.Selected)
                    .Select(r => r.RoleName)
                );

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Cannot add selected roles to user");
                return View(model);
            }

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> ActivateUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

           if(user==null)
               return RedirectToAction(nameof(Index));

            if (user.available)
                user.available = false;
             else
                user.available = true;


            await _userManager.UpdateAsync(user);

            return RedirectToAction(nameof(Index));
        }

    }
}

