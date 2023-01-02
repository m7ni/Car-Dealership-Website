using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.EntityFrameworkCore;
using PWEBAssignment.Data;
using PWEBAssignment.Models;

namespace PWEBAssignment.Controllers
{
	[Authorize(Roles = "Admin,Employee,Manager")]
	public class CompaniesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly INotyfService _toastNotification;
        public CompaniesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, INotyfService toastNotification)
        {
            _context = context;
            _userManager = userManager;
            _toastNotification = toastNotification;
        }

        // GET: Companies
        public async Task<IActionResult> Index()
        {
              return _context.Company != null ? 
                          View(await _context.Company.ToListAsync()) :
                          Problem("Entity set 'ApplicationDbContext.Company'  is null.");
        }

        // GET: Companies/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Company == null)
            {
                return NotFound();
            }

            var company = await _context.Company
                .FirstOrDefaultAsync(m => m.Id == id);
            if (company == null)
            {
                return NotFound();
            }

            return View(company);
        }

        // GET: Companies/Create
        public IActionResult Create()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Address,Rating,Available")] Company company)
        {
            ModelState.Remove(nameof(company.Available));
            ModelState.Remove(nameof(company.Rating));
            ModelState.Remove(nameof(company.Workers));
            ModelState.Remove(nameof(company.Cars));



            var equals = await _context.Company.FirstOrDefaultAsync(c =>
                c.Name.ToUpper().Equals(company.Name.ToUpper()));
            if (equals != null)
                ModelState.AddModelError("Name", "A Company with that name already exists");

            if (!ModelState.IsValid)
                return View(company);

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

            if (user == null)
            {
                await _userManager.CreateAsync(defaultUser, "CompanyManager.0");
                await _userManager.AddToRoleAsync(defaultUser,
                    Roles.Manager.ToString());
                user = await _userManager.FindByEmailAsync(defaultUser.Email);
                company.Workers = new Collection<ApplicationUser>();
                company.Cars = new Collection<Car>();
                company.Workers.Add(user);
            }

            _context.Add(company);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public static string FormatString(string input)
		{
			input = input.ToLowerInvariant();

			string[] words = input.Split(' ');

			string firstLetter = words[0].Substring(0, 1).ToUpperInvariant();

			string output = string.Concat(firstLetter, words[0].Substring(1));

			for (int i = 1; i < words.Length; i++)
			{
				output = string.Concat(output, words[i]);
			}

			output = output.TrimEnd();

			output = string.Concat(output, ".0");

			return output;
		}
		



		// GET: Companies/Edit/5
		public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Company == null)
            {
                return NotFound();
            }

            var company = await _context.Company.FindAsync(id);
            if (company == null)
            {
                return NotFound();
            }
            return View(company);
        }

        // POST: Companies/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Address,Rating,Available")] Company company)
        {
            if (id != company.Id)
            {
                return NotFound();
            }

            ModelState.Remove(nameof(company.Rating));
            ModelState.Remove(nameof(company.Workers));
            ModelState.Remove(nameof(company.Cars));
            if (ModelState.IsValid)
            {


                try
                {
                    _context.Update(company);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CompanyExists(company.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(company);
        }

        // GET: Companies/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Company == null)
            {
                return NotFound();
            }

            var company = await _context.Company
                .FirstOrDefaultAsync(m => m.Id == id);
            if (company == null)
            {
                return NotFound();
            }

            return View(company);
        }

        // POST: Companies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Company == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Company'  is null.");
            }

            var teste = await _context.Reservations.Where(r => r.CompanyId == id && r.ReturnId==null).FirstOrDefaultAsync();
            if (teste != null)
            {
                _toastNotification.Error("This company has reservations");
                return RedirectToAction(nameof(Index));
            }


            var users = await _userManager.Users.Where(u => u.CompanyID == id).ToListAsync();
            foreach (var u in users)
            {
                await _userManager.DeleteAsync(u);
            }

            var cars = await _context.Car.Where(u => u.CompanyID == id).ToListAsync();

            foreach (var c in cars)
            {
                _context.Car.Remove(c);

                await _context.SaveChangesAsync();
            }


            var company = await _context.Company.FindAsync(id);
            if (company != null)
            {
                

                _context.Company.Remove(company);
                await _context.SaveChangesAsync();
            }
            
          /*  await _context.SaveChangesAsync();*/
            return RedirectToAction(nameof(Index));
        }

        private bool CompanyExists(int id)
        {
          return (_context.Company?.Any(e => e.Id == id)).GetValueOrDefault();
        }
        public async Task<IActionResult> ActivateCompany(int id)
        {
            var company = await _context.Company.Where(c => c.Id == id).FirstOrDefaultAsync();

            if (company == null)
                return RedirectToAction(nameof(Index));

            if (company.Available)
                company.Available = false;
            else
                company.Available = true;

            _context.Update(company);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

     
    }
}
