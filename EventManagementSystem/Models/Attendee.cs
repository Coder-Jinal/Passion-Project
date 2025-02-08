using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;

namespace EventManagementSystem.Models
{
    public class Attendee
    {
        [Key]
        public int AttendeeId { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Phone]
        public string Phone { get; set; }

        // Navigation property: An attendee can register for many events
        public ICollection<EventAttendee> EventAttendees { get; set; }
    }



        public class AttendeeDto
        {
            public int AttendeeId { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Email { get; set; }
            public string Phone { get; set; }
        }



}
