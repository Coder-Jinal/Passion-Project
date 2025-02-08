using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using EventManagementSystem.Models;

namespace EventManagementSystem.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Event> Events { get; set; } 

        public DbSet<Attendee> Attendees { get; set; }

        public DbSet<EventAttendee> EventAttendees { get; set; }
    }
}
