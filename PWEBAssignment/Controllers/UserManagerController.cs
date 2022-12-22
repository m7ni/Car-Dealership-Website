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
            var users = await _userManager.Users.ToListAsync();
            var userRolesViewModel = new List<UserRolesViewModel>();

            foreach (var user in users)
            {
                var role = await _userManager.GetRolesAsync(user);

                if (!role.Contains("Admin") && !role.Contains("Client"))
                {
                    userRolesViewModel.Add(new UserRolesViewModel
                    {
                        UserId = user.Id,
                        PrimeiroNome = user.firstName,
                        UltimoNome = user.lastName,
                        UserName = user.UserName,
                        available = user.available,
                        Roles = await _userManager.GetRolesAsync(user),
                        /* Avatar = user.Avatar*/
                    });
                }
            }

            return View(userRolesViewModel);
        }
        public IActionResult Create()
        {
            ViewData["CompanyId"] = new SelectList(_context.Company, "Id", "Name");
            return View();
        }

       /* [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FirstName,EmpresaID,Rating,Available")] UserRolesViewModel apUser)
        {
            
           *//* if (ModelState.IsValid)
            {

                company.Available = false;
                company.Rating = 0;
                var defaultUser = new ApplicationUser
                {
                    UserName = company.Name.Replace(" ", "") + "@gmail.com",
                    Email = company.Name.Replace(" ", "") + "@gmail.com",
                    firstName = "Manager",
                    lastName = company.Name.Replace(" ", ""),
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true
                };
                var user = await _userManager.FindByEmailAsync(defaultUser.Email);
                string[] substrings = company.Name.Split(' ');
                var password = string.Join("", substrings).ToUpper() + "." + "0";
                if (user == null)
                {
                    await _userManager.CreateAsync(defaultUser, "Borra1.");
                    await _userManager.AddToRoleAsync(defaultUser,
                        Roles.Manager.ToString());

                    user = await _userManager.FindByEmailAsync(defaultUser.Email);
                    company.Workers = new Collection<ApplicationUser>();
                    company.Workers.Add(user);
                }

                _context.Add(company);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            var errors = ModelState.Where(x => x.Value.Errors.Any())
                .Select(x => new { x.Key, x.Value.Errors });
            return View(company);*//*
        }*/
    }
}
