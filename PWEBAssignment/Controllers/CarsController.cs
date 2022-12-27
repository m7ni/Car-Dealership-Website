using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Runtime.ConstrainedExecution;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PWEBAssignment.Data;
using PWEBAssignment.Models;
using PWEBAssignment.ViewModels;
//using Microsoft.AspNetCore.Identity;

namespace PWEBAssignment.Controllers
{
    
    public class CarsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public CarsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public enum Orders
        {
            All,
            PriceLow2High,
            PriceHigh2Low,
            RatingLow2High,
            RatingHigh2Low,
            Available,
            NotAvailable
        }

        // GET: Cars
        public async Task<IActionResult> Index(Orders? order)
        {
            ViewData["ListOfCompanys"] = new SelectList(_context.Company, "Id", "Name");
            ViewData["ListOfCategorys"] = new SelectList(_context.Category, "Id", "Name");
            ViewData["ListOfAddresses"] = new SelectList(_context.Company, "Address", "Address");
            var listCar = new List<Car>();
            if (User.IsInRole("Employee") || User.IsInRole("Manager"))
            {
                var user =await _userManager.GetUserAsync(User);
                var carsUser = await _context.Car.Include("Category").Include("Company").Where(c=> c.CompanyID == user.CompanyID).ToListAsync();
                listCar = carsUser;
                
                if (order == Orders.PriceLow2High)
                {
                    ViewData["Title"] = "Price: Low to High";
                    return View(listCar.OrderBy(c => c.Category.PriceHour));
                }
                if (order == Orders.PriceHigh2Low)
                {
                    ViewData["Title"] = "Price: High to Low";
                    return View(listCar.OrderByDescending(c => c.Category.PriceHour));
                }
                if (order == Orders.Available)
                {
                    ViewData["Title"] = "Available Cars";
                    return View(await _context.Car.Include("Category").Include("Company").Where(c => c.Available == true).ToListAsync());
                }
                if (order == Orders.NotAvailable)
                {
                    ViewData["Title"] = "Not Available Cars";
                    return View(await _context.Car.Include("Category").Include("Company").Where(c => c.Available == false).ToListAsync());
                }

                return View(listCar);
            }
            if (User.IsInRole("Admin"))
            {
                var cars = _context.Car.Include(c => c.Company).Include(d => d.Category);
                listCar = await cars.ToListAsync();
            }
            else { 
                var cars = _context.Car.Include(c => c.Company).Include(d => d.Category).Where(c => c.Available == true && c.Company.Available == true);
                listCar = await cars.ToListAsync();
            }
            
            if (order == Orders.PriceLow2High)
            {
                ViewData["Title"] = "Price: Low to High";
                return View(listCar.OrderBy(c => c.Category.PriceHour));
            }
            if (order == Orders.PriceHigh2Low)
            {
                ViewData["Title"] = "Price: High to Low";
                return View(listCar.OrderByDescending(c => c.Category.PriceHour));
            }
            if (order == Orders.RatingLow2High)
            {
                ViewData["Title"] = "Rating: Low to High";
                return View(listCar.OrderBy(c => c.Company.Rating));
            }
            if (order == Orders.RatingHigh2Low)
            {
                ViewData["Title"] = "Rating: High to Low";
                return View(listCar.OrderByDescending(c => c.Company.Rating));
            }
            return View(listCar);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(int CompanyID, int CategoryID, string Address)
        {
            

            if (CompanyID > 0 && CategoryID > 0 && Address != "Select Option")
            {
                var category = await _context.Category.FirstOrDefaultAsync(c => c.Id == CategoryID);
                var company = await _context.Company.FirstOrDefaultAsync(c => c.Id == CompanyID);

                ViewData["Title"] = "List of Cars with Company: '" + company.Name + "', Category: '" + category.Name + "' and Location: '" + Address + "'";
                ViewData["ListOfCategorys"] = new SelectList(_context.Category.ToList(), "Id", "Name");
                ViewData["ListOfCompanys"] = new SelectList(_context.Company.ToList(), "Id", "Name");
                ViewData["ListOfAddresses"] = new SelectList(_context.Company.ToList(), "Address", "Address");
                return View(await _context.Car.Include("Company")
                    .Where(c => c.CompanyID == CompanyID
                    && c.CategoryID == CategoryID
                    && c.Company.Address == Address).ToListAsync());
            }
            
            if(CompanyID > 0 || CategoryID > 0 || Address != "Select Option") 
            {
                var listCar = _context.Car;
                if (CompanyID > 0)
                {
                    return View(await _context.Car.Include("Company")
                    .Where(c => c.CompanyID == CompanyID).ToListAsync());
                    //listCar = await listCar.Where(c => c.CompanyID == CompanyID).ToListAsync();       //estar a acrescentar na pes
                }
                if (CategoryID > 0)
                {
                    return View(await _context.Car.Include("Company")
                    .Where(c => c.CategoryID == CategoryID).ToListAsync());
                    //listCar.Where(c => c.CategoryID == CompanyID);
                }
                if (Address != "Select Option")
                {
                    return View(await _context.Car.Include("Company")
                    .Where(c => c.Company.Address == Address).ToListAsync());
                }
                return View(await listCar.ToListAsync());
            
            }
            return RedirectToAction("Index");
            
        }

		// GET: Cars/Create
		[Authorize(Roles = "Admin,Employee,Manager")]
		public async Task<IActionResult> Create()
        {
            if (User.IsInRole("Employee") || User.IsInRole("Manager"))
            {
                var user = await _userManager.GetUserAsync(User);
                ViewData["CompanyID"] = new SelectList(_context.Company.Where(c=> c.Id == user.CompanyID), "Id", "Name");
            }
            else
            {
                ViewData["CompanyID"] = new SelectList(_context.Company, "Id", "Name");
            }
          
            ViewData["Category"] = new SelectList(_context.Category, "Id", "Name");
            return View();
        }

		// POST: Cars/Create
		// To protect from overposting attacks, enable the specific properties you want to bind to.
		// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
		[Authorize(Roles = "Admin,Employee,Manager")]
		[HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Model,LicencePlate,Damage,Available,CategoryID,CompanyID")] Car car)
        {
            ModelState.Remove(nameof(car.Category));
            ModelState.Remove(nameof(car.Company));

            var testCar = await _context.Car.FirstOrDefaultAsync(c => c.LicencePlate == car.LicencePlate);
            if(testCar != null)
                return RedirectToAction(nameof(Create));
            if (ModelState.IsValid)
            {
	            var company = await _context.Company.FindAsync(car.CategoryID);
	            car.Company = company;
                _context.Add(car);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return RedirectToAction(nameof(Create));
        }

		// GET: Cars/Edit/5

		[Authorize(Roles = "Admin,Employee,Manager")]
		public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Car == null)
            {
                return NotFound();
            }

            var car = await _context.Car.FindAsync(id);
            if (car == null)
            {
                return NotFound();
            }
            ViewData["Category"] = new SelectList(_context.Category, "Id", "Name");
            ViewData["CompanyID"] = new SelectList(_context.Company, "Id", "Name", car.CompanyID);
            return View(car);
        }

        // GET: Cars/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Car == null)
            {
                return NotFound();
            }

            var car = await _context.Car
                .Include(c => c.Company).Include(
                    d => d.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (car == null)
            {
                return NotFound();
            }

            return View(car);
        }

        // POST: Cars/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Employee,Manager")]
		public async Task<IActionResult> Edit(int id, [Bind("Id,Model,LicencePlate,Damage,Available,CategoryID,CompanyID")] Car car)
        {
            if (id != car.Id)
            {
                return NotFound();
            }

            ModelState.Remove(nameof(car.Category));
            ModelState.Remove(nameof(car.Company));
            if (ModelState.IsValid)
            {
                try
                {
                    car.Company = await _context.Company.FindAsync(car.CompanyID);
                    car.Category = await _context.Category.FindAsync(car.CategoryID);

                    _context.Update(car);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CarExists(car.Id))
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
            ViewData["CompanyID"] = new SelectList(_context.Company, "Id", "Id", car.CompanyID);
            return View(car);
        }

		// GET: Cars/Delete/5
		[Authorize(Roles = "Admin,Employee,Manager")]
		public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Car == null)
            {
                return NotFound();
            }


            var car = await _context.Car
                .Include(c => c.Company)
                .FirstOrDefaultAsync(m => m.Id == id);

            var teste = await _context.Reservations.Where(r => r.CarId == id).FirstOrDefaultAsync();

            if (teste!=null)
                return View(car);

            if (car == null)
            {
                return NotFound();
            }

            return View(car);
        }

        // POST: Cars/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
		[Authorize(Roles = "Admin,Employee,Manager")]
		public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Car == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Car'  is null.");
            }
            var car = await _context.Car.FindAsync(id);
            if (car != null)
            {
                _context.Car.Remove(car);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CarExists(int id)
        {
          return (_context.Car?.Any(e => e.Id == id)).GetValueOrDefault();
        }




		/*
        public async Task<IActionResult> Reservation(int? id)
        {
            if (id == null || _context.Car == null)
            {
                return NotFound();
            }

            //var car = RedirectToAction(nameof(CarExists(id)));
            //if (car == false)
            //{
            //    return NotFound();
            //}

            //return View(Reservations);
            return RedirectToAction("Index");
        }*/


		[Authorize(Roles = "Admin,Employee,Manager")]
		public async Task<IActionResult> ActivateCar(int id)
        {
            var car = await _context.Car.Where(c => c.Id == id).FirstOrDefaultAsync();

            if (car == null)
                return RedirectToAction(nameof(Index));

            if (car.Available)
                car.Available = false;
            else
                car.Available = true;

            _context.Update(car);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
