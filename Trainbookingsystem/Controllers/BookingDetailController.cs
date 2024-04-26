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
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var bookings = await _context.BookingTickets
                .Where(b => b.UserId == userId)
                .ToListAsync();
            return View(bookings);
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
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var booking = await _context.BookingTickets
                .FirstOrDefaultAsync(b => b.BookingId == id && b.UserId == userId);

            if (booking == null)
            {
                return NotFound();

            }

            return View(booking);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, BookingTicket booking)
        {
            if (id != booking.BookingId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(booking);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookingExists(booking.BookingId))
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
            return View(booking);
        }

        private bool BookingExists(int id)
        {
            return _context.BookingTickets.Any(b => b.BookingId == id);
        }
        public async Task<IActionResult> Cancel(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var booking = await _context.BookingTickets
                .FirstOrDefaultAsync(b => b.BookingId == id && b.UserId == userId);

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
    
