using EventManagementSystem.Data.Migrations;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;

namespace EventManagementSystem.Models
{
    public class Event
    {
        [Key]
        public int EventId { get; set; }

        [Required]
        public string EventName { get; set; }

        public string Description { get; set; }

        [Required]
        public DateTime Date { get; set; }

        public string Location { get; set; }

        // Navigation property: An event can have many attendees
        public ICollection<EventAttendee> EventAttendees { get; set; }

    }


  
        public class EventDto
        {
            public int EventId { get; set; }
            public string EventName { get; set; }
            public string Description { get; set; }
            public string Location { get; set; }
            public DateTime Date { get; set; }
        }

}

