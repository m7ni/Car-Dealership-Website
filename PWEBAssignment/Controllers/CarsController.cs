using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.ConstrainedExecution;
using System.Threading.Tasks;
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
            RatingHigh2Low
        }

        // GET: Cars
        public async Task<IActionResult> Index(Orders? order)
        {
            
            /*
            if (order == null)
            {
                ViewData["Title"] = "All cars";
            }*/

            ViewData["ListOfCompanys"] = new SelectList(_context.Company, "Id", "Name");
            ViewData["ListOfCategorys"] = new SelectList(_context.Category, "Id", "Name");
            ViewData["ListOfAddresses"] = new SelectList(_context.Company, "Id", "Address");
            var listCar = new List<Car>();
            if (User.IsInRole("Employee") || User.IsInRole("Manager"))
            {
                var user =await _userManager.GetUserAsync(User);
                var carsUser = await _context.Car.Include("Categoria").ToListAsync();
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
                return View(listCar);
            }

            var cars = _context.Car.Include(c => c.Company).Include(d => d.Category).Where(c => c.Available == true);
            listCar = await cars.ToListAsync();
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
        public async Task<IActionResult> Index(int CompanyID, int CategoryID/*, int AddressId*/)
        {
            ViewData["Title"] = "List of Cars with Company: '" + CompanyID + "', Category: '" + CategoryID + "' and Location: '" +/* AddressId + */"'";

            if (CompanyID > 0 && CategoryID > 0 /*&& AddressId > 0*/)
            {
                ViewData["ListOfCategorys"] = new SelectList(_context.Category.ToList(), "Id", "Name");
                ViewData["ListOfCompanys"] = new SelectList(_context.Company.ToList(), "Id", "Name");
                ViewData["ListOfAddresses"] = new SelectList(_context.Company.ToList(), "Id", "Address");
                return View(await _context.Car.Include("Company")
                    .Where(c => c.CompanyID == CompanyID
                    && c.CategoryID == CategoryID
                    /*&& c.Company.Address == Address*/).ToListAsync());
            }
            /*
            if(CompanyID > 0 || CategoryID > 0 || AddressId > 0) 
            {
                var listCar = _context.Car;
                if (CompanyID > 0)
                {
                    //listCar = await listCar.Where(c => c.CompanyID == CompanyID).ToListAsync();       //estar a acrescentar na pes
                }
                if (CategoryID > 0)
                {
                    //listCar.Where(c => c.CategoryID == CompanyID);
                }
                if (AddressId > 0)
                {
                    
                }
                return View(await listCar.ToListAsync());
            
            }*/
            return RedirectToAction("Index");
            
        }

        //GET
        public async Task<IActionResult> Search(int CompanyId, int CategoryId, int AddressId)
        {

            var searchVM = new CarSearchViewModel();
            ViewData["Title"] = "Cars´ list with '" + searchVM + "'";

            /*
            if (string.IsNullOrEmpty(textToSearch))
            {
                searchVM.ListOfCars = await _context.Car.ToListAsync();
            } else
            {*/
            searchVM.ListOfCars = await _context.Car.Include("Category").
                Where(c => c.Company.Address.Contains(searchVM.textToSearch)).ToListAsync();


            searchVM.NumResults = searchVM.ListOfCars.Count;

            return View(searchVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Search([Bind("Category,Company,Address")] CarSearchViewModel carSearch)
        {
            ViewData["Title"] = "Cars´ list with '" + carSearch.textToSearch + "'";

            if (carSearch.Category == null)
            {
            }
            if (carSearch.Company == null)
            {

            }
            if (carSearch.Address == null)
            {

            }
            if (carSearch.Category != null && carSearch.Company != null && carSearch.Address != null)
            {
                carSearch.ListOfCars = await _context.Car.Include("Category")
                    .Where(c => c.Company == carSearch.Company
                    && c.Category == carSearch.Category
                    && c.Company.Address == carSearch.Address).ToListAsync();
            }

            carSearch.NumResults = carSearch.ListOfCars.Count;
            return View(carSearch);
        }

        // GET: Cars/Create
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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Model,LicencePlate,Damage,Available,CategoryID,CompanyID")] Car car)
        {
            ModelState.Remove(nameof(car.Category));
            ModelState.Remove(nameof(car.Company));
            if (ModelState.IsValid)
            {
                _context.Add(car);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return RedirectToAction(nameof(Create));
        }

        // GET: Cars/Edit/5
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
        //[Authorize(Roles = "Employe")]
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
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Car == null)
            {
                return NotFound();
            }

            var car = await _context.Car
                .Include(c => c.Company)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (car == null)
            {
                return NotFound();
            }

            return View(car);
        }

        // POST: Cars/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        //[Authorize(Roles = "Employe")]
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
