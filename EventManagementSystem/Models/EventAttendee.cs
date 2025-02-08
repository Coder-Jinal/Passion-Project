using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventManagementSystem.Models
{
    public class EventAttendee
    {
        [Key]
        public int Event_AttendeeId { get; set; }


        [Required]
        public DateTime RegistrationDate { get; set; }

        // Navigation properties
        [ForeignKey("Events")]
        public int EventId { get; set; }
        public Event Event { get; set; }

        [ForeignKey("Attendees")]
        public int AttendeeId { get; set; }
        public Attendee Attendee { get; set; }
    }


    public class eventattendeeDto
    {
        public int Event_AttendeeId { get; set; }
        public int EventId { get; set; }
        public int AttendeeId { get; set; }

        public DateTime RegistrationDate { get; set; }

    }



    //public class EventAttendeeDto
    //{
    //    public int Event_AttendeeId { get; set; }

    //    //public int EventId { get; set; }
    //    //public int AttendeeId { get; set; }
    //    public string FirstName { get; set; }
    //    public string LastName { get; set; }
    //    public DateTime RegistrationDate { get; set; }
    //    public string EventName { get; set; }
    //    public string AttendeeName { get; set; }

    //}
}
