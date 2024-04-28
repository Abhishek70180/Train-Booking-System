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
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var booking = await _context.BookingTickets
                .FirstOrDefaultAsync(b => b.BookingId == id && b.UserId == userId);

            if (booking == null)
            {
                return NotFound();
            }
            var train = await _context.Trains.FirstOrDefaultAsync(t => t.TrainId == booking.TrainId);
            if (train != null)
            {
                ViewBag.TicketPrice = train.Price;
                booking.TotalPrice = booking.NumberOfTickets * train.Price;
            }
            else
            {
                ViewBag.TicketPrice = 0;
            }
            return View(booking);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BookingId,TrainId,PassengerName,PassengerEmail,NumberOfTickets,TotalPrice,BookingTime")] BookingTicket bookingTicket)
        {
            if (id != bookingTicket.BookingId)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                try
                {
                    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Retrieve the current user's ID

                    var existingBooking = await _context.BookingTickets.FindAsync(id);

                    if (existingBooking == null)
                    {
                        return NotFound();
                    }
                    var numberOfPassengersDifference = bookingTicket.NumberOfTickets - existingBooking.NumberOfTickets;
                    bookingTicket.UserId = userId;
                    _context.Entry(existingBooking).CurrentValues.SetValues(bookingTicket);
                    var train = await _context.Trains.FindAsync(existingBooking.TrainId);
                    if (train != null)
                    {
                        train.AvailableSeats -= numberOfPassengersDifference;
                        _context.Update(train);
                    }
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookingExists(bookingTicket.BookingId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index", "BookingDetail");
            }
            return View(bookingTicket);
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
    
