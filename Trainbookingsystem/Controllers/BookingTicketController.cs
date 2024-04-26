using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Net.Mail;
using System.Net;
using Trainbookingsystem.Data;
using Trainbookingsystem.Models;
using Trainbookingsystem.ViewModels;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Trainbookingsystem.Controllers
{
    public class BookingController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BookingController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var bookings = _context.BookingTickets.ToList();
            return View(bookings);
        }

        public IActionResult Create(int trainId)
        {
            var train = _context.Trains.FirstOrDefault(t => t.TrainId == trainId);
            if (train != null)
            {
                ViewBag.TicketPrice = train.Price;
            }
            else
            {
                ViewBag.TicketPrice = 0;
            }

            var viewModel = new BookingCreateViewModel
            {
                TrainId = trainId
            };
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            if (!string.IsNullOrEmpty(userEmail))
            {
                viewModel.PassengerEmail = userEmail;
            }

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Create(BookingCreateViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrWhiteSpace(viewModel.PassengerName))
                {
                    ModelState.AddModelError("PassengerName", "Passenger Name is required.");
                    return View(viewModel);
                }

                var train = await _context.Trains.FindAsync(viewModel.TrainId);
                if (train == null)
                {
                    return NotFound();
                }

                if (train.AvailableSeats < viewModel.NumberOfTickets)
                {
                    ModelState.AddModelError("", "Not enough available seats.");
                    return View(viewModel);
                }

                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var booking = new BookingTicket
                {
                    UserId = userId,
                    TrainId = viewModel.TrainId,
                    PassengerName = viewModel.PassengerName,
                    PassengerEmail = viewModel.PassengerEmail,
                    NumberOfTickets = viewModel.NumberOfTickets,
                    TotalPrice = viewModel.TotalPrice,
                    BookingTime = DateTime.Now
                };
                _context.BookingTickets.Add(booking);
                train.AvailableSeats -= viewModel.NumberOfTickets;
                await _context.SaveChangesAsync();
                await SendBookingNotificationEmailAsync(booking);
                return RedirectToAction("BookingSuccess");
            }
            return View(viewModel);
        }

        public IActionResult BookingSuccess()
        {
            return View();
        }

        private async Task SendBookingNotificationEmailAsync(BookingTicket booking)
        {
            try
            {
                string smtpServer = "smtp-mail.outlook.com";
                int smtpPort = 587;
                string smtpUsername = "abhithakur2222@outlook.com";
                string smtpPassword = "Abhishek@123";

                var userEmail = User.FindFirstValue(ClaimTypes.Email);

                if (string.IsNullOrEmpty(userEmail))
                {
                    throw new ApplicationException("User email address not found.");
                }

                using (MailMessage mail = new MailMessage())
                {
                    mail.From = new MailAddress(smtpUsername);
                    mail.To.Add(userEmail);
                    mail.Subject = "New Booking Created";
                    mail.Body = "A new booking has been created with the following details:\n" +
                                "Passenger Name: " + booking.PassengerName + "\n" +
                                "Passenger Email: " + booking.PassengerEmail + "\n" +
                                "Number of Tickets: " + booking.NumberOfTickets + "\n" +
                                "Total Price: " + booking.TotalPrice.ToString("C2") + "\n" +
                                "Booking Time: " + booking.BookingTime.ToString("yyyy-MM-dd HH:mm");

                    using (SmtpClient smtpClient = new SmtpClient(smtpServer, smtpPort))
                    {
                        smtpClient.Credentials = new NetworkCredential(smtpUsername, smtpPassword);
                        smtpClient.EnableSsl = true;
                        await smtpClient.SendMailAsync(mail);
                    }
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = "An error occurred while sending email: " + ex.Message;
            }
        }
    }
}
