namespace Trainbookingsystem.Models
{
    public class BookingDetail
    {
        public int BookingDetailId { get; set; }
        public int BookingId { get; set; }
        public string UserId { get; set; } 
        public ApplicationUser User { get; set; }
        public string PassengerName { get; set; }
        public string PassengerEmail { get; set; }
        public int NumberOfTickets { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTime BookingTime { get; set; }
    }
}
