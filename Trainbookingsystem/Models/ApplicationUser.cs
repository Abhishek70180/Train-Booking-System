﻿using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Trainbookingsystem.Models
{
    public class ApplicationUser:IdentityUser
    {

        [Required]
        public string Name { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        public string State { get; set; }
        [NotMapped]
        public string Role { get; set; }
        public ICollection<BookingDetail> BookingDetails { get; set; }

    }
}
