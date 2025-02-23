using EventManagementSystem.Interfaces;
using EventManagementSystem.Models;
using EventManagementSystem.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventManagementSystem.Services
{
    public class EventAttendeeService : IEventAttendeeService
    {
        private readonly ApplicationDbContext _context;

        public EventAttendeeService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<eventattendeeDto>> ListEventAttendees()
        {
            var eventAttendees = await _context.EventAttendees
                .Include(ea => ea.Event)
                .Include(ea => ea.Attendee)
                .ToListAsync();

            return eventAttendees.Select(ea => new eventattendeeDto
            {
                Event_AttendeeId = ea.Event_AttendeeId,
                AttendeeId = ea.AttendeeId,
                EventId = ea.EventId,
                RegistrationDate = ea.RegistrationDate
            }).ToList();
        }

        public async Task<eventattendeeDto> FindEventAttendee(int id)
        {
            var eventAttendee = await _context.EventAttendees
                .Include(ea => ea.Event)
                .Include(ea => ea.Attendee)
                .FirstOrDefaultAsync(ea => ea.Event_AttendeeId == id);

            if (eventAttendee == null)
            {
                return null;
            }

            return new eventattendeeDto
            {
                Event_AttendeeId = eventAttendee.Event_AttendeeId,
                EventId = eventAttendee.EventId,
                AttendeeId = eventAttendee.AttendeeId,
                RegistrationDate = eventAttendee.RegistrationDate
            };
        }

        public async Task<eventattendeeDto> AddEventAttendee(eventattendeeDto eventAttendeeDto)
        {
            var eventEntity = await _context.Events.FindAsync(eventAttendeeDto.EventId);
            var attendeeEntity = await _context.Attendees.FindAsync(eventAttendeeDto.AttendeeId);

            if (eventEntity == null || attendeeEntity == null)
            {
                return null; 
            }

            var eventAttendee = new EventAttendee
            {
                EventId = eventAttendeeDto.EventId,
                AttendeeId = eventAttendeeDto.AttendeeId,
                RegistrationDate = eventAttendeeDto.RegistrationDate
            };

            _context.EventAttendees.Add(eventAttendee);
            await _context.SaveChangesAsync();

            return new eventattendeeDto
            {
                Event_AttendeeId = eventAttendee.Event_AttendeeId,
                EventId = eventAttendee.EventId,
                AttendeeId = eventAttendee.AttendeeId,
                RegistrationDate = eventAttendee.RegistrationDate
            };
        }

        public async Task<bool> UpdateEventAttendee(int id, eventattendeeDto eventAttendeeDto)
        {
            var existingRegistration = await _context.EventAttendees.FindAsync(id);

            if (existingRegistration == null)
            {
                return false; 
            }

            existingRegistration.EventId = eventAttendeeDto.EventId;
            existingRegistration.AttendeeId = eventAttendeeDto.AttendeeId;
            existingRegistration.RegistrationDate = eventAttendeeDto.RegistrationDate;

            _context.Entry(existingRegistration).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return false; 
            }

            return true;
        }

        public async Task<bool> DeleteEventAttendee(int id)
        {
            var eventAttendee = await _context.EventAttendees.FindAsync(id);
            if (eventAttendee == null)
            {
                return false;
            }

            _context.EventAttendees.Remove(eventAttendee);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
