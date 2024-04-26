using System.ComponentModel.DataAnnotations;

namespace Trainbookingsystem.Models.ViewModel
{
    public class BookingCreateVM
    {
        [Required(ErrorMessage = "Passenger Name is required.")]
        [RegularExpression("^[a-zA-Z]*$", ErrorMessage = "Passenger Name must contain only alphabets.")]
        public string PassengerName { get; set; }

        [Required(ErrorMessage = "Passenger Email is required.")]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "Invalid email format. Please enter a valid email address.")]
        public string PassengerEmail { get; set; }
    }
}
