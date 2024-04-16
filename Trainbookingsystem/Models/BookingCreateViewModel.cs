namespace Trainbookingsystem.ViewModels
{
    public class BookingCreateViewModel
    {
        public int TrainId { get; set; }
        public string PassengerName { get; set; }
        public string PassengerEmail { get; set; }
        public int NumberOfTickets { get; set; }
        public decimal TotalPrice { get; set; }
       
    }
}
