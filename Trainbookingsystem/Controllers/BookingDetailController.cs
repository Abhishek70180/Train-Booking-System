using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Trainbookingsystem.Data;
using Trainbookingsystem.Models;
using Microsoft.AspNetCore.Authorization;
using OfficeOpenXml;
using System.Security.Claims;
using System.Net.Mail;
using System.Net;

namespace Trainbookingsystem.Controllers
{
    
    public class BookingDetailController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BookingDetailController(ApplicationDbContext context)
        {
            _context = context;            
        }
        public async Task<IActionResult> Index()
        {
            if (User.IsInRole("Admin"))
            {
                var bookings = await _context.BookingTickets.ToListAsync();
                return View(bookings);
            }
            else
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var bookings = await _context.BookingTickets
                    .Where(b => b.UserId == userId)
                    .ToListAsync();
                return View(bookings);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var booking = await _context.BookingTickets
                .FirstOrDefaultAsync(b => b.BookingId == id && b.UserId == userId);
            if (booking == null)
            {
                return NotFound();
            }
            _context.BookingTickets.Remove(booking);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }     


        private bool BookingExists(int id)
        {
            return _context.BookingTickets.Any(b => b.BookingId == id);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int id)
        {
            var booking = await _context.BookingTickets.FindAsync(id);

            if (booking == null)
            {
                return NotFound();
            }

            var train = await _context.Trains.FindAsync(booking.TrainId);
            if (train != null)
            {
                train.AvailableSeats += booking.NumberOfTickets;
                _context.Update(train);
            }

            _context.BookingTickets.Remove(booking);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

    }
}
    
