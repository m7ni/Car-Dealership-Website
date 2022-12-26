using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.EntityFrameworkCore;
using PWEBAssignment.Data;
using PWEBAssignment.Models;

namespace PWEBAssignment.Controllers
{
    public class CompaniesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public CompaniesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
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
            if (ModelState.IsValid)
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
            return View(company);
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

            var teste = await _context.Reservations.Where(r => r.CompanyId == id).FirstOrDefaultAsync();
            if (teste != null)
            {
                return View();
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
