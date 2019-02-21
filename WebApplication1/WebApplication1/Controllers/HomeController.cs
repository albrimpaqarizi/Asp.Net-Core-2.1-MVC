using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;
using PagedList.Mvc;
using PagedList;


namespace WebApplication1.Controllers
{

    public class HomeController : Controller
    {
        private ApplicationDbContext _dbContext;

        public HomeController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Cars(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["NameSortParm"] = string.IsNullOrEmpty(sortOrder) ? "nameUp" : "";
            //ViewData["DateSortParm"] = sortOrder == "Date" ? "date_desc" : "Date";

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            var cars = from s in _dbContext.Cars
                       select s;
            

            if (!string.IsNullOrEmpty(searchString))
            {
                cars = cars.Where(s => s.Name.ToLower().Contains(searchString.ToLower()));
            }

            if (sortOrder != null)
            {
                // Sort by is given...
                switch (sortOrder)
                {
                    case "priceUp":
                        cars = _dbContext.Cars
                                    .OrderBy(s => s.Price);
                        break;
                    case "priceDown":
                        cars = _dbContext.Cars
                                    .OrderByDescending(s => s.Price);
                        break;
                    case "nameUp":
                        cars = _dbContext.Cars
                                    .OrderBy(s => s.Name);
                        break;
                    default:
                        cars = _dbContext.Cars
                                    .OrderByDescending(s => s.Name);
                        break;
                }
            }
            else if (sortOrder == null)
            {
                cars = _dbContext.Cars.OrderByDescending(s => s.PostedDate);
            }

            int pageSize = 3;
            return View(await PaginatedList<Cars>.CreateAsync(cars.AsNoTracking(), page ?? 1, pageSize));
        }





        //heloooo
        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
    }
}
