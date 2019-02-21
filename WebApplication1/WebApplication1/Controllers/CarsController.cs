using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Areas.Admin.Models;
using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    [Authorize(Roles = "Administrator , NormalUser")]
    public class CarsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public string CurrentUserId
        {
            get
            {
                return User.FindFirst(ClaimTypes.NameIdentifier).Value;
            }
        }

        public CarsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Cars
        public async Task<IActionResult> Index()
        {
            if (User.IsInRole("Administrator"))
            {
                var cars = await _context.Cars.ToListAsync();
                return View(cars);
            }
            else
            {
                var cars = await _context.Cars
                                .Where(a => a.CarUsers.Exists(au => au.UserId == CurrentUserId))
                                .ToListAsync();

                return View(cars);
            }
        }

        [AllowAnonymous]
        // GET: Admin/Articles/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }


            var cars = await _context.Cars
                .FirstOrDefaultAsync(m => m.Id == id);
            if (cars == null)
            {
                return NotFound();
            }

            return View(cars);
        }

        // GET: Cars/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Cars/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Description,Image, Location,CompanyName,PostedDate,Price,Fuel,Transmission,Doors,Seats,AirConditioning,ABS,FullToFull,Casco")] Cars cars, IFormFile imageFile)

        {
            if (ModelState.IsValid && imageFile  != null ) 
            {
                var fileName = Path.GetRandomFileName() + Path.GetExtension(imageFile.FileName);
                var fileDirectory = "wwwroot/images";

                if (!Directory.Exists(fileDirectory))
                {
                    Directory.CreateDirectory(fileDirectory);
                }
                var filePath = fileDirectory + "/" + fileName;

                // Copy file to path...
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }
                cars.Image = fileName;

                // Add object to db
                cars.PostedDate = DateTime.Now;
                _context.Add(cars);

                _context.CarUsers.Add(new CarUser
                {
                    UserId = CurrentUserId,
                    Cars = cars
                });

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(cars);
        }

        

        // GET: Cars/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            if (!await HasAccessToCars(id.Value))
            {
                return View("_Error");
            }

            var cars = await _context.Cars.FindAsync(id);
            if (cars == null)
            {
                return NotFound();
            }
            return View(cars);
        }

        // POST: Cars/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,Image, Location,CompanyName ,PostedDate,Price,Fuel,Transmission,Doors,Seats,AirConditioning,ABS,FullToFull,Casco")] Cars cars, IFormFile imageFile)
        {
            if (id != cars.Id)
            {
                return NotFound();
            }

            if (!await HasAccessToCars(id))
            {
                return View("_Error");
            }

            if (ModelState.IsValid)
            {

                
                    if (imageFile != null)
                    {
                        var fileName = Path.GetRandomFileName() + Path.GetExtension(imageFile.FileName);
                        var fileDirectory = "wwwroot/images";

                        if (!Directory.Exists(fileDirectory))
                        {
                            Directory.CreateDirectory(fileDirectory);
                        }
                        var filePath = fileDirectory + "/" + fileName;

                        // Copy file to path...

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await imageFile.CopyToAsync(stream);
                        }

                        cars.Image = fileName;
                        
                    }

                    _context.Update(cars);
                    await _context.SaveChangesAsync();

              
                return RedirectToAction(nameof(Index));
            }
            return View(cars);
        }

        // GET: Cars/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            if (!await HasAccessToCars(id.Value))
            {
                return View("_Error");
            }

            var cars = await _context.Cars
                .FirstOrDefaultAsync(m => m.Id == id);
            if (cars == null)
            {
                return NotFound();
            }

            return View(cars);
        }

        // POST: Cars/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (!await HasAccessToCars(id))
            {
                return View("_Error");
            }

            var cars = await _context.Cars.FindAsync(id);
            _context.Cars.Remove(cars);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }



        [HttpGet]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> ManageUsers(int id)
        {
            var cars = await _context.Cars
                                .Include(a => a.CarUsers)
                                .ThenInclude(au => au.User)
                                .SingleOrDefaultAsync(a => a.Id == id);

            var allUsers = await _userManager.GetUsersInRoleAsync("NormalUser");

            var vm = new ManageUsersViewModel();
            vm.Id = id;
            vm.AssignedUsers = cars.CarUsers.Select(au => au.User);
            vm.AvailableUsers = allUsers.Where(u => !vm.AssignedUsers.Any(x => x.Id == u.Id));

            return View(vm);
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> AddUserToCars(int id, string userId)
        {
            _context.CarUsers.Add(new CarUser
            {
                CarsId = id,
                UserId = userId
            });

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(ManageUsers), new { id = id });
        }

        [HttpGet]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> RemoveUserFromCars(int id, string userId)
        {
            var au = await _context.CarUsers.SingleOrDefaultAsync(a => a.UserId == userId && a.CarsId == id);

            if (au != null)
            {
                _context.CarUsers.Remove(au);

                await _context.SaveChangesAsync();
            }
            else
            {
                return NotFound();
            }

            return RedirectToAction(nameof(ManageUsers), new { id = id });
        }



        private async Task<bool> HasAccessToCars(int carsId)
        {
            if (User.IsInRole("Administrator"))
                return true;

            var cars = await _context.Cars
                                .Where(a => a.Id == carsId)
                                .Where(a => a.CarUsers.Exists(au => au.UserId == CurrentUserId))
                                .SingleOrDefaultAsync();

            return cars != null;
        }

        private bool CarsExists(int id)
        {
            return _context.Cars.Any(e => e.Id == id);
        }
    }
}
