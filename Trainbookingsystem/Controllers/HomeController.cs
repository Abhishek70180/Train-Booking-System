using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Trainbookingsystem.Data;
using Trainbookingsystem.Models;

namespace Trainbookingsystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }
        public async Task<IActionResult> Index(string searchString)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Redirect("/Identity/Account/Login");
            }
            var trains = from t in _context.Trains select t;

            if (!string.IsNullOrEmpty(searchString))
            {
                trains = trains.Where(t => t.Name.Contains(searchString)
                                         || t.Origin.Contains(searchString)
                                         || t.Destination.Contains(searchString)
                                         || t.Destination.ToString().Contains(searchString)
                                         || t.Schedule.ToString().Contains(searchString));
            }
            return View(await trains.ToListAsync());
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
