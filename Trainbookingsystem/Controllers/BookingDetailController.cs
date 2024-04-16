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
            await SendCancellationEmail(booking);
            _context.BookingTickets.Remove(booking);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        private async Task SendCancellationEmail(BookingTicket booking)
        {
            try
            {
                string smtpServer = "smtp-mail.outlook.com";
                int smtpPort = 587;
                string smtpUsername = "abhithakur2222@outlook.com";
                string smtpPassword = "Abhishek@123";

                MailMessage mail = new MailMessage();
                mail.From = new MailAddress(smtpUsername);
                mail.To.Add(booking.PassengerEmail);
                mail.Subject = "Booking Cancellation Notification";
                mail.Body = $"Dear {booking.PassengerName},\n\nYour booking with Booking ID {booking.BookingId} has been cancelled.\n\nThank you.";

                SmtpClient smtpClient = new SmtpClient(smtpServer, smtpPort);
                smtpClient.Credentials = new NetworkCredential(smtpUsername, smtpPassword);
                smtpClient.EnableSsl = true;
                await smtpClient.SendMailAsync(mail);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "An error occurred while sending email: " + ex.Message;
            }
        }
    }
}
    
