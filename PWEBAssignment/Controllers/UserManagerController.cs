using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PWEBAssignment.Data;
using PWEBAssignment.Models;
using PWEBAssignment.ViewModels;
using System.Collections.ObjectModel;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace PWEBAssignment.Controllers
{
    public class UserManagerController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public UserManagerController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
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
                return RedirectToAction(nameof(Index));
            
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
		        return RedirectToAction(nameof(Index));

	        //TODO: delete certo

	        await _userManager.UpdateAsync(userAsked);

	        return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string type,[Bind("FirstName,LastName,Email,Password")] ManagerCreateUser newUser)
        {


            return RedirectToAction(nameof(Index));
        }
    }
}

