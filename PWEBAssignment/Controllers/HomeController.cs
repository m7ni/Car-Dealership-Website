using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using PWEBAssignment.Data;
using PWEBAssignment.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace PWEB_P6.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            ViewData["ListOfCompanys"] = new SelectList(_context.Company, "Id", "Name");
            ViewData["ListOfCategorys"] = new SelectList(_context.Category, "Id", "Name");


            if (User.IsInRole("Employee") || User.IsInRole("Manager"))
            {

                var user = await _userManager.GetUserAsync(User);

                var carsUser = _context.Car.Include(c => c.Company).Include(d => d.Category).Where(c => c.CompanyID == user.CompanyID);
                return View(await carsUser.ToListAsync());
            }

            var applicationDbContext = _context.Car.Include(c => c.Company).Include(d => d.Category).Where(c => c.Available == true);
            return View(await applicationDbContext.ToListAsync());
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    } 

}