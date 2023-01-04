using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Data;
using System.Linq;
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

namespace PWEBAssignment.Controllers
{

	[Authorize(Roles = "Admin,Employee,Manager")]
	public class ReservationCompanySideController : Controller
    {
	    private readonly string _returnsPath = "wwwroot/ficheiros/Returns/";
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
            var applicationDbContext = _context.Reservations
	            .Include(r => r.Car)
                .Include(r => r.Company)
	            .Include(r => r.ClientUser)
	            .Where(r=> r.CompanyId == userSelf.CompanyID );

            ViewData["ListOfCategorys"] = new SelectList(_context.Category, "Id", "Name");
            ViewData["ListOfDeliveryDate"] = new SelectList(_context.Reservations.Include(r => r.Company).Where(r => r.CompanyId == userSelf.CompanyID), "DeliveryDate", "DeliveryDate");
            ViewData["ListOfReturnDate"] = new SelectList(_context.Reservations.Include(r => r.Company).Where(r => r.CompanyId == userSelf.CompanyID), "ReturnDate", "ReturnDate");
            ViewData["ListOfModels"] = new SelectList(_context.Car, "Id", "Model");
            ViewData["ListOfClients"] = new SelectList(_context.Reservations.Include(r => r.Company).Where(r => r.CompanyId == userSelf.CompanyID), "Id", "ClientUser");

            return View(await applicationDbContext.ToListAsync());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(int CategoryID, DateTime DeliveryDate, DateTime ReturnDate, int ModelID, int ClientID)
        {
            
            var user = await _userManager.GetUserAsync(User);
            
            ViewData["ListOfCategorys"] = new SelectList(_context.Category, "Id", "Name");
            ViewData["ListOfDeliveryDate"] = new SelectList(_context.Reservations.Include(r => r.Company).Where(r => r.CompanyId == user.CompanyID), "DeliveryDate", "DeliveryDate");
            ViewData["ListOfReturnDate"] = new SelectList(_context.Reservations.Include(r => r.Company).Where(r => r.CompanyId == user.CompanyID), "ReturnDate", "ReturnDate");
            ViewData["ListOfModels"] = new SelectList(_context.Car, "Id", "Model");
            ViewData["ListOfClients"] = new SelectList(_context.Reservations.Include(r => r.Company).Where(r => r.CompanyId == user.CompanyID), "Id", "ClientUser");

            if (CategoryID > 0 && DeliveryDate != null && ReturnDate != null && ModelID > 0/* && ClientID > 0*/)
            {
                var category = await _context.Category.FirstOrDefaultAsync(c => c.Id == CategoryID);
                var car = await _context.Car.FirstOrDefaultAsync(c => c.Id == ModelID);
                var reservation = await _context.Reservations.FirstOrDefaultAsync(c => c.DeliveryDate == DeliveryDate);
                

                //ViewData["Title"] = "List of Reservations with Category: '" + category.Name + "', Model: '" + car.Model + "', Client: '" + reservation.ClientUser.firstName + " " + reservation.ClientUser.lastName + "', Delivery Date: '" + reservation.DeliveryDate + "', Return Date: '" + reservation.ReturnDate + "'";

                return View(await _context.Reservations.Include("Company")
                    .Where(c => c.Car.Model == car.Model
                    && c.Car.Category == car.Category
                    //&& c.ClientUserId == ClientID
                    && c.DeliveryDate == DeliveryDate
                    && c.ReturnDate == ReturnDate
                    && c.CompanyId == user.CompanyID).ToListAsync());
            }

            if (CategoryID > 0)
            {
                var category = await _context.Category.FirstOrDefaultAsync(c => c.Id == CategoryID);
                ViewData["Title"] = "List of Reservations with Category: '" + category.Name + "'";

                return View(await _context.Reservations.Include("Company")
                    .Where(c => c.Car.Category == category
                    && c.CompanyId == user.CompanyID).ToListAsync());

            }
            
            

            if (ModelID > 0)
            {
                var car = await _context.Car.FirstOrDefaultAsync(c => c.Id == ModelID);
                ViewData["Title"] = "List of Reservations with Model: '" + car.Model + "'";

                return View(await _context.Reservations.Include("Company")
                    .Where(c => c.Car.Model == car.Model
                    && c.CompanyId == user.CompanyID).ToListAsync());

            }
            
            if (ClientID > 0)
            {
                var reservation = await _context.Reservations.FirstOrDefaultAsync(c => c.ClientUserId == ClientID);
                ViewData["Title"] = "List of Reservations with Client: '" + reservation.ClientUser.firstName + " " + reservation.ClientUser.lastName + "'";

                /*return View(await _context.Reservations.Include("Company")
                    .Where(c => c.Car.Model == car.Model
                    && c.Car.Category == car.Category
                    && c.ClientUserId == ClientID
                    && c.CompanyId == user.CompanyID).ToListAsync());*/

            }

            if (DeliveryDate != null)
            {
                var reservation = await _context.Reservations.FirstOrDefaultAsync(c => c.DeliveryDate == DeliveryDate);
                ViewData["Title"] = "List of Reservations with Delivery Date: '" + reservation.DeliveryDate + "'";

                return View(await _context.Reservations.Include("Company")
                    .Where(c => c.DeliveryDate == DeliveryDate
                    && c.CompanyId == user.CompanyID).ToListAsync());
            }

            if (ReturnDate != null)     //descobrir como ele chegar até este quando não se escolhe para pesquisar o deliveryDate
            {
                var reservation = await _context.Reservations.FirstOrDefaultAsync(c => c.ReturnDate == ReturnDate);
                ViewData["Title"] = "List of Reservations with Return Date: '" + reservation.ReturnDate + "'";

                return View(await _context.Reservations.Include("Company")
                    .Where(c => c.ReturnDate == ReturnDate
                    && c.CompanyId == user.CompanyID).ToListAsync());

            }

            ViewData["Title"] = "There where no reservations that matched those filters";
            return RedirectToAction("Index");
        }


        public async Task<IActionResult> ManagerStatistics()
        {

	        var vm = new StatisticsViewModel();
	        vm.TotalLastSevenDays = new ProfitStat( await Profit(7));
	        vm.TotalLastThirtyDays = new ProfitStat( await Profit(30));
	        vm.AverageLastThirtyDays = new ReservationsStat( await AverageReservations(30));
			return View(vm);
        }
		public async Task<IActionResult> AdminStatistics()
		{

			var vm = new StatisticsViewModel();
			vm.TotalLastSevenDays = new ProfitStat(await Profit(7));
			vm.TotalLastThirtyDays = new ProfitStat(await Profit(30));
			vm.AverageLastThirtyDays = new ReservationsStat(await AverageReservations(30));
			return View(vm);
		}

		private async Task<double> AverageReservations(int days)
        {
	        var userSelf = await _userManager.GetUserAsync(User);
			var profit = 0.0;
	        var reservations = await _context.Reservations
		        .Where(d => d.ReservationDate.Date >= DateTime.Now.AddDays(-days) && d.CompanyId == userSelf.CompanyID).CountAsync();

			return ((double)reservations / (double)days);
        }

        private async Task<double>  Profit(int days)
        {
	        var userSelf = await _userManager.GetUserAsync(User);
			var profit = 0.0;
	        var reservations =  await _context.Reservations
		        .Where(d => d.ReservationDate.Date >= DateTime.Now.AddDays(-days) && d.CompanyId == userSelf.CompanyID).ToListAsync();
	        foreach (var r in reservations)
	        {
		        profit += r.Price;
	        }

	        return profit;
        }


        [HttpPost]
        public async Task<IActionResult> GetDailyReservationsManager()
        {
	        var userSelf = await _userManager.GetUserAsync(User);
			//dados de exemplo
			List<object> dados = new List<object>();
	        DataTable dt = new DataTable();
	        dt.Columns.Add("Day", System.Type.GetType("System.DateTime"));
	        dt.Columns.Add("Quantidade", System.Type.GetType("System.Int32"));
	        
	        for (int i = 30; i > 0; i--)
			{
				DataRow dr = dt.NewRow();
				var reservations = await _context.Reservations
			        .Where(d => d.ReservationDate.Date.DayOfYear == DateTime.Now.AddDays(-i).DayOfYear && d.CompanyId == userSelf.CompanyID).CountAsync();
		        dr["Day"] = DateTime.Now.AddDays(-i).ToString("dd/MM/yyyy");
		        dr["Quantidade"] = reservations;
                dt.Rows.Add(dr);
			}

	        foreach (DataColumn dc in dt.Columns)
	        {
		        List<object> x = new List<object>();
		        x = (from DataRow drr in dt.Rows select drr[dc.ColumnName]).ToList();
		        dados.Add(x);
	        }
	        return Json(dados);
        }
        [HttpPost]
        public async Task<IActionResult> GetDailyReservationsAdmin()
        {
	        var userSelf = await _userManager.GetUserAsync(User);
	        //dados de exemplo
	        List<object> dados = new List<object>();
	        DataTable dt = new DataTable();
	        dt.Columns.Add("Day", System.Type.GetType("System.DateTime"));
	        dt.Columns.Add("Quantidade", System.Type.GetType("System.Int32"));

	        for (int i = 30; i > 0; i--)
	        {
		        DataRow dr = dt.NewRow();
		        var reservations = await _context.Reservations
			        .Where(d => d.ReservationDate.Date.DayOfYear == DateTime.Now.AddDays(-i).DayOfYear).CountAsync();
		        dr["Day"] = DateTime.Now.AddDays(-i).ToString("dd/MM/yyyy");
		        dr["Quantidade"] = reservations;
		        dt.Rows.Add(dr);
	        }

	        foreach (DataColumn dc in dt.Columns)
	        {
		        List<object> x = new List<object>();
		        x = (from DataRow drr in dt.Rows select drr[dc.ColumnName]).ToList();
		        dados.Add(x);
	        }
	        return Json(dados);
        }

		[HttpPost]
        public async Task<IActionResult> GetMonthlyClients()
        {
            var userSelf = await _userManager.GetUserAsync(User);
            //dados de exemplo
            List<object> dados = new List<object>();
            DataTable dt = new DataTable();
            dt.Columns.Add("Month", System.Type.GetType("System.DateTime"));
            dt.Columns.Add("Quantidade", System.Type.GetType("System.Int32"));

            for (int i = 12; i > 0; i--)
            {
                var count = 0;
                DataRow dr = dt.NewRow();
                var user =await _userManager.Users.ToListAsync();
                foreach (var u in user)
                {
                  if(await _userManager.IsInRoleAsync(u,"Client") && u.entryDate.Date.Month == DateTime.Now.AddMonths(-i).Month)
                     count++;
                }
                
                dr["Month"] = DateTime.Now.AddMonths(-i).ToString("dd/MM/yyyy");
                dr["Quantidade"] = count;
                dt.Rows.Add(dr);
            }

            foreach (DataColumn dc in dt.Columns)
            {
                List<object> x = new List<object>();
                x = (from DataRow drr in dt.Rows select drr[dc.ColumnName]).ToList();
                dados.Add(x);
            }
            return Json(dados);
        }
        [HttpPost]
        public async Task<IActionResult> GetMonthlyReservations()
        {
            var userSelf = await _userManager.GetUserAsync(User);
            //dados de exemplo
            List<object> dados = new List<object>();
            DataTable dt = new DataTable();
            dt.Columns.Add("Month", System.Type.GetType("System.DateTime"));
            dt.Columns.Add("Quantidade", System.Type.GetType("System.Int32"));

            for (int i = 12; i > 0; i--)
            {
                DataRow dr = dt.NewRow();
                var reservations = await _context.Reservations
                    .Where(d => d.ReservationDate.Date.Month == DateTime.Now.AddMonths(-i).Month).CountAsync();
                dr["Month"] = DateTime.Now.AddMonths(-i).ToString("dd/MM/yyyy");
                dr["Quantidade"] = reservations;
                dt.Rows.Add(dr);
            }

            foreach (DataColumn dc in dt.Columns)
            {
                List<object> x = new List<object>();
                x = (from DataRow drr in dt.Rows select drr[dc.ColumnName]).ToList();
                dados.Add(x);
            }
            return Json(dados);
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

        public async Task<IActionResult> DeleteImage(int? id, string? image)
        {
	        if (id == null || _context.Returns == null)
	        {
		        return NotFound();
	        }

	        var returns = await _context.Returns.FindAsync(id);
	        if (returns == null)
	        {
		        return NotFound();
	        }

	        var filePath = Path.Combine(Directory.GetCurrentDirectory(), $"{_returnsPath[..7]}/{image}");

	        try
	        {
		        System.IO.File.Delete(filePath);
	        }
	        catch
	        {
		        return NotFound();
	        }

	        return RedirectToAction(nameof(Edit), new { Id = id });
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
        public async Task<IActionResult> ReceiveCar([Bind("NumberOfKm,VehicleDamage,Observations,EmployeUserId,ReservationId")] ReturnViewModel returnvm, [FromForm] List<IFormFile> files)
        {


	        ModelState.Remove(nameof(returnvm.EmployeUserId));
	       

			if (ModelState.IsValid)
			{
				var returns = new Returns
				{
					NumberOfKm = returnvm.NumberOfKm,
                    VehicleDamage = returnvm.VehicleDamage,
                    Observations = returnvm.Observations,
				};

		

				var reservations = await _context.Reservations.FindAsync(returnvm.ReservationId);
			
		        var userSelf = await _userManager.GetUserAsync(User);
		        returns.EmployeUser = userSelf;
		        returns.Id = 0;
		        returns.ReservationId = reservations.Id;
		        returns.Reservation = reservations;
                reservations.ReturnDate = DateTime.Now;
                _context.Add(returns);
		        await _context.SaveChangesAsync();

				var car = await _context.Car.FindAsync(reservations.CarId);
				car.Available = true;
				reservations.ReturnId = returns.Id;
		        reservations.Return = returns;

		        _context.Update(reservations);
		        await _context.SaveChangesAsync();


		        var coursePath = Path.Combine(
			        Directory.GetCurrentDirectory(),
			        Path.Combine(_returnsPath, returns.Id.ToString())
		        );
		        if (!Directory.Exists(coursePath))
			        Directory.CreateDirectory(coursePath);

		        foreach (var formFile in files)
		        {
			        if (formFile.Length <= 0) continue;

			        var filePath = Path.Combine(
				        coursePath,
				        $"{Guid.NewGuid()}{Path.GetExtension(formFile.FileName)}"
			        );
			        while (System.IO.File.Exists(filePath))
			        {
				        filePath = Path.Combine(
					        coursePath,
					        $"{Guid.NewGuid()}{Path.GetExtension(formFile.FileName)}"
				        );
			        }

			        await using var stream = System.IO.File.Create(filePath);
			        await formFile.CopyToAsync(stream);
		        }
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
                .Include(r=>r.ClientUser)
                .Include(r => r.Return.EmployeUser)
                .Include(r => r.Delivery.EmployeUser)
                .Include(r => r.Return)
                .Include(r => r.Delivery)
				.FirstOrDefaultAsync(m => m.Id == id);
            
            
            
            if (reservations == null)
            {
                return NotFound();
            }
            var coursePath = Path.Combine(
	            Directory.GetCurrentDirectory(),
	            Path.Combine(_returnsPath, reservations.ReturnId.GetValueOrDefault().ToString())
            );

            var files = new List<string>();

            if (Directory.Exists(coursePath))
	            files = (
		            from file in Directory.EnumerateFiles(coursePath)
		            select Path.Combine(_returnsPath[7..], $"{reservations.ReturnId}/{Path.GetFileName(file)}")
	            ).ToList();

            ViewData["Files"] = files;
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

      
        public async Task<IActionResult> Reject(int id)
        {
            if (_context.Reservations == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Reservations'  is null.");
            }
            var reservations = await _context.Reservations.FindAsync(id);
            var car = await _context.Car.FindAsync(reservations.CarId);

            if (reservations != null && car!=null)
            {
                car.Available = true;
                reservations.Rejected = true;
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        
    }
}
