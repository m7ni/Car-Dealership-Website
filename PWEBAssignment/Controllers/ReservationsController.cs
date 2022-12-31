using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Data;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PWEBAssignment.Data;
using PWEBAssignment.Models;

namespace PWEBAssignment.Controllers
{
	[Authorize(Roles = "Client,Admin")]
	public class ReservationsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public ReservationsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Reservations
        public async Task<IActionResult> Index()
        {
            ViewData["ListOfCategorys"] = new SelectList(_context.Category, "Id", "Name");
            ViewData["ListOfDeliveryDate"] = new SelectList(_context.Reservations, "DeliveryDate", "DeliveryDate");
            ViewData["ListOfReturnDate"] = new SelectList(_context.Reservations, "ReturnDate", "ReturnDate");
            ViewData["ListOfModels"] = new SelectList(_context.Car, "Id", "Model");
            ViewData["ListOfClients"] = new SelectList(_context.Reservations, "Id", "Client");

            var userSelf = await _userManager.GetUserAsync(User);
            var applicationDbContext = _context.Reservations.Include(r => r.Car).Include(r => r.Company).Where(r => r.ClientUser == userSelf);
            return View(await applicationDbContext.ToListAsync());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(int CategoryID, DateTime DeliveryDate, DateTime ReturnDate, int ModelID, int ClientID)
        {
            if (CategoryID > 0 && DeliveryDate != null && ReturnDate != null && ModelID > 0 && ClientID > 0)
            {
                var category = await _context.Category.FirstOrDefaultAsync(c => c.Id == CategoryID);
                var car = await _context.Car.FirstOrDefaultAsync(c => c.Id == ModelID);
                var reservation = await _context.Reservations.FirstOrDefaultAsync(c => c.Id == ClientID);
                var user = await _userManager.GetUserAsync(User);

                ViewData["Title"] = "List of Cars with Category: '" + category.Name + "', Model: '" + car.Model + "', Client: '" + reservation.ClientUserId + "', Delivery Date: '" + reservation.DeliveryDate + "', Return Date: '" + reservation.ReturnDate + "'";

                return View(await _context.Reservations.Include("Company")
                    .Where(c => c.Car.Model == car.Model
                    && c.Car.Category == car.Category
                    && c.ClientUserId == ClientID
                    && c.DeliveryDate == DeliveryDate
                    && c.ReturnDate == ReturnDate
                    && c.Company == user.Company ).ToListAsync());
            }
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Return(int id)
        {
	        var reservations = await _context.Reservations.FindAsync(id);
	        reservations.ConfirmReturn = true;
	        _context.Update(reservations);
	        await _context.SaveChangesAsync(); 
			return RedirectToAction(nameof(Index));
        }

		// GET: Reservations/Details/5
		public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Reservations == null)
            {
                return NotFound();
            }

            var reservations = await _context.Reservations
                .Include(r => r.Car)
                .Include(r => r.Company)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (reservations == null)
            {
                return NotFound();
            }

            return View(reservations);
        }

        // GET: Reservations/Create
        public async Task<IActionResult> Create(int id)
        {
            ViewData["CarId"] = new SelectList(_context.Car, "Id", "Id");
            ViewData["CompanyId"] = new SelectList(_context.Company, "Id", "Id");
            var reserv = new Reservations();
            var car = _context.Car.Where(c => c.Id == id).FirstOrDefault();
            var company = await _context.Company.FirstOrDefaultAsync(c => c.Id == car.CompanyID);
            var user = await _userManager.GetUserAsync(User);
			reserv.CarId = car.Id;
            reserv.CompanyId = company.Id;
            return View(reserv);
        }

        // POST: Reservations/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ClientUserId,CompanyId,CarId,DeliveryDate,ReturnDate,ReturnId,DeliveryId,Rejected,ConfirmReturn")] Reservations reservations)
        {
            var dateNow = DateTime.Now;
            var dif1 = DateTime.Compare(reservations.DeliveryDate, reservations.ReturnDate);
            if (dif1 > 0)
            {
                ModelState.AddModelError("DeliveryDate", "The delivery date needs to be before than the return date");
            }
            var dif2 = DateTime.Compare(reservations.DeliveryDate, dateNow);
            
            if (dif2 < 0)
            {
                ModelState.AddModelError("DeliveryDate", "You can only rent a car from tomorrow");
            }
            

            ModelState.Remove(nameof(reservations.DeliveryId));
            ModelState.Remove(nameof(reservations.ReturnId));
            ModelState.Remove(nameof(reservations.Return));
            ModelState.Remove(nameof(reservations.Delivery));
            ModelState.Remove(nameof(reservations.ClientUserId));
            ModelState.Remove(nameof(reservations.ClientUser));
            ModelState.Remove(nameof(reservations.Car));
            ModelState.Remove(nameof(reservations.Company));
            ModelState.Remove(nameof(reservations.Rejected));
            ModelState.Remove(nameof(reservations.ConfirmReturn));
            ModelState.Remove(nameof(reservations.Price));

			if (ModelState.IsValid)
            {

                reservations.ClientUser = await _userManager.GetUserAsync(User);
                var car = await _context.Car.FirstOrDefaultAsync(c => c.Id == reservations.CarId);
                car.Available = false;
                reservations.Car = car;
                reservations.Company = await _context.Company.FirstOrDefaultAsync(c => c.Id == reservations.CompanyId);
                reservations.Id = 0;
                reservations.Rejected = false;
                reservations.ConfirmReturn = false;
                reservations.ReservationDate = DateTime.Now;
                var category = await _context.Category.FirstOrDefaultAsync(c => c.Id == reservations.Car.CategoryID);
				reservations.Price = (reservations.ReturnDate - reservations.DeliveryDate).TotalHours * category.PriceHour;
                _context.Add(reservations);
                await _context.SaveChangesAsync();
                _context.Update(car);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            var errors = ModelState.Where(x => x.Value.Errors.Any())
                .Select(x => new { x.Key, x.Value.Errors });
            ViewData["CarId"] = new SelectList(_context.Car, "Id", "Id", reservations.CarId);
            ViewData["CompanyId"] = new SelectList(_context.Company, "Id", "Id", reservations.CompanyId);
            return View(reservations);
        }

        // GET: Reservations/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Reservations == null)
            {
                return NotFound();
            }

            var reservations = await _context.Reservations.FindAsync(id);
            if (reservations == null)
            {
                return NotFound();
            }
            ViewData["CarId"] = new SelectList(_context.Car, "Id", "Id", reservations.CarId);
            ViewData["CompanyId"] = new SelectList(_context.Company, "Id", "Id", reservations.CompanyId);
            return View(reservations);
        }

        // POST: Reservations/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ClientUserId,CompanyId,CarId,DeliveryDate,ReturnDate,ReturnId,DeliveryId")] Reservations reservations)
        {
            if (id != reservations.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(reservations);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReservationsExists(reservations.Id))
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
            ViewData["CarId"] = new SelectList(_context.Car, "Id", "Id", reservations.CarId);
            ViewData["CompanyId"] = new SelectList(_context.Company, "Id", "Id", reservations.CompanyId);
            return View(reservations);
        }

        // GET: Reservations/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Reservations == null)
            {
                return NotFound();
            }

            var reservations = await _context.Reservations
                .Include(r => r.Car)
                .Include(r => r.Company)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (reservations == null)
            {
                return NotFound();
            }

            return View(reservations);
        }

        // POST: Reservations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Reservations == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Reservations'  is null.");
            }
            var reservations = await _context.Reservations.FindAsync(id);
            if (reservations != null)
            {
                _context.Reservations.Remove(reservations);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ReservationsExists(int id)
        {
          return (_context.Reservations?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
