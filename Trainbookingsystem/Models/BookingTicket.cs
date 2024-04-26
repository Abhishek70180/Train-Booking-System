using System;
using System.ComponentModel.DataAnnotations;

namespace Trainbookingsystem.Models
{
    public class BookingTicket
    {
        [Key]
        public int BookingId { get; set; }

        [Required(ErrorMessage = "TrainId is required.")]
        public int TrainId { get; set; }
        [Required]
        public string UserId { get; set; }

        [Required(ErrorMessage = "Passenger name is required.")]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at most {1} characters long.", MinimumLength = 2)]
        public string PassengerName { get; set; }

        [Required(ErrorMessage = "Passenger email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string PassengerEmail { get; set; }

        [Required(ErrorMessage = "Number of tickets is required.")]
        [Range(1, 10, ErrorMessage = "Number of tickets must be between 1 and 10.")]
        public int NumberOfTickets { get; set; }

        [Required(ErrorMessage = "Total price is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Total price must be greater than 0.")]
        public decimal TotalPrice { get; set; } 

        public DateTime BookingTime { get; set; }
       
    }
}
