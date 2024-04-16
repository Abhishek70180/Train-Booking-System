using System.ComponentModel.DataAnnotations;

namespace Trainbookingsystem.Models
{
    public class Train
    {
        [Key]
        public int TrainId { get; set; }

        [Required(ErrorMessage = "Train name is required.")]
        public string Name { get; set; }

        [Display(Name = "Train Photo")]
        public byte[]? Photo { get; set; }
        [Required(ErrorMessage = "Origin is Required")]
        public string Origin { get; set; }

        [Required(ErrorMessage = "Destination is required.")]
        public string Destination { get; set; }

        [Required(ErrorMessage = "Schedule is required.")]
        [DataType(DataType.DateTime, ErrorMessage = "Please enter a valid date and time.")]
        public DateTime Schedule { get; set; }

        [Required(ErrorMessage = "Available seats count is required.")]
        [Range(0, int.MaxValue, ErrorMessage = "Available seats count must be a non-negative number.")]
        public int AvailableSeats { get; set; }
        [Required(ErrorMessage = "Price is required.")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "AC status is required.")]
        public bool IsAC { get; set; }
    }
}
