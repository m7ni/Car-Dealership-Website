using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PWEBAssignment.Data;
using PWEBAssignment.Models;
using PWEBAssignment.ViewModels;

namespace PWEBAssignment.Controllers
{
    public class ReservationCompanySideController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ReservationCompanySideController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
            _context = context;
        }

        // GET: ReservationCompanySide
        public async Task<IActionResult> Index()
        {
            var userSelf = await _userManager.GetUserAsync(User);
            var applicationDbContext = _context.Reservations.Include(r => r.Car)
                .Include(r => r.Company).Include(r => r.ClientUser).Where(r=> r.CompanyId == userSelf.CompanyID );
            return View(await applicationDbContext.ToListAsync());
        }

        public async Task<IActionResult> Deliver(int id)
        {
            if (_context.Reservations == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Reservations'  is null.");
            }
            
            var userSelf = await _userManager.GetUserAsync(User);
            var delivery = new Deliveries();
            delivery.ReservationId = id;
    
            return View(delivery);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Deliver([Bind("Id,NumberOfKm,VehicleDamage,Observations,EmployeUserId,ReservationId")] Deliveries deliveries)
        {


            ModelState.Remove(nameof(deliveries.EmployeUserId));
            ModelState.Remove(nameof(deliveries.EmployeUser));
            ModelState.Remove(nameof(deliveries.Reservation));
          
            if (ModelState.IsValid)
            {
                var reservations = await _context.Reservations.FindAsync(deliveries.ReservationId);
                var userSelf = await _userManager.GetUserAsync(User);
                deliveries.EmployeUser = userSelf;
                deliveries.Id = 0;
                deliveries.Reservation = reservations;
                _context.Add(deliveries);
                await _context.SaveChangesAsync();

                reservations.DeliveryId = deliveries.Id;
                reservations.Delivery = deliveries;

                _context.Update(reservations);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            
            return View(deliveries);
        }

        public async Task<IActionResult> ReceiveCar(int id)
        {
	        if (_context.Reservations == null)
	        {
		        return Problem("Entity set 'ApplicationDbContext.Reservations'  is null.");
	        }

	        
	        var ret = new ReturnViewModel();
            ret.ReservationId = id;

	        return View(ret);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ReceiveCar([Bind("NumberOfKm,VehicleDamage,PhotoEvidenceFile,Observations,EmployeUserId,ReservationId")] ReturnViewModel returnvm)
        {


	        ModelState.Remove(nameof(returnvm.EmployeUserId));
	        ModelState.Remove(nameof(returnvm.PhotoEvidenceFile));

			if (returnvm.PhotoEvidenceFile != null)
            {
                if (returnvm.PhotoEvidenceFile.Length > (200 * 1024))
                {
                    ModelState.AddModelError("PhotoEvidenceFile", "Error: Ficheiro demasiado grande");
                }

                if (!IsValidFileType(returnvm.PhotoEvidenceFile.FileName))
                {
                    ModelState.AddModelError("PhotoEvidenceFile", "Error: Ficheiro não suportado");
                }

				
			}
			if (ModelState.IsValid)
			{
				var returns = new Returns
					{
                    NumberOfKm = returnvm.NumberOfKm,
                    VehicleDamage = returnvm.VehicleDamage,
                    Observations = returnvm.Observations,
					};

				if (returnvm.PhotoEvidenceFile != null)
				{
					using (var dataStream = new MemoryStream())
					{
						await returnvm.PhotoEvidenceFile.CopyToAsync(dataStream);
						returns.PhotoEvidence = dataStream.ToArray();
					}
				}

				var reservations = await _context.Reservations.FindAsync(returnvm.ReservationId);
		        var userSelf = await _userManager.GetUserAsync(User);
		        returns.EmployeUser = userSelf;
		        returns.Id = 0;
		        returns.ReservationId = reservations.Id;
		        returns.Reservation = reservations;

					_context.Add(returns);
		        await _context.SaveChangesAsync();

		        reservations.ReturnId = returns.Id;
		        reservations.Return = returns;

		        _context.Update(reservations);
		        await _context.SaveChangesAsync();

		        return RedirectToAction(nameof(Index));
	        }

	        return View();
        }


        public bool IsValidFileType(string fileName) // .jpg, .png, .jpeg
        {
	        var valid_extensions = new[] { ".jpg", ".png", ".jpeg" };
	        var ext = Path.GetExtension(fileName);
	        return valid_extensions.Contains(ext);
        }
		// GET: ReservationCompanySide/Details/5
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


        // GET: ReservationCompanySide/Edit/5
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

        // POST: ReservationCompanySide/Edit/5
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

        // GET: ReservationCompanySide/Delete/5
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

        // POST: ReservationCompanySide/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Reservations == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Reservations'  is null.");
            }
            
            var reservations = await _context.Reservations.FindAsync(id);

            if(reservations.Rejected == false)
                return RedirectToAction(nameof(Index));

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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(int id)
        {
            if (_context.Reservations == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Reservations'  is null.");
            }
            var reservations = await _context.Reservations.FindAsync(id);
            if (reservations != null)
            {
                reservations.Rejected = true;
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        
    }
}
