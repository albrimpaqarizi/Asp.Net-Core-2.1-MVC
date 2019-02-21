using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication1.Areas.Admin.Controllers
{
    public class DashboardController : Controller
    {
        // GET: Dashboard
        [Area("Admin")]
        [Authorize(Roles = "Administrator")]
        public IActionResult Index()
        {
            return View();
        }

        
    }
}