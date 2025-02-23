using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using EventManagementSystem.Data;
using EventManagementSystem.Models;
using EventManagementSystem.Interfaces;

namespace EventManagementSystem.Services
{
    public class AttendeeService : IAttendeeService
    {
        private readonly ApplicationDbContext _context;

        public AttendeeService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<AttendeeDto>> ListAttendees()
        {
            var attendees = await _context.Attendees.ToListAsync();
            return attendees.Select(a => new AttendeeDto
            {
                AttendeeId = a.AttendeeId,
                FirstName = a.FirstName,
                LastName = a.LastName,
                Email = a.Email,
                Phone = a.Phone
            }).ToList();
        }


        public async Task<AttendeeDto?> FindAttendee(int id)
        {
            var attendee = await _context.Attendees.FindAsync(id);
            if (attendee == null) return null;

            return new AttendeeDto
            {
                AttendeeId = attendee.AttendeeId,
                FirstName = attendee.FirstName,
                LastName = attendee.LastName,
                Email = attendee.Email,
                Phone = attendee.Phone
            };
        }

        public async Task<Models.ServiceResponse> AddAttendee(AttendeeDto attendeeDto)
        {
            var serviceResponse = new Models.ServiceResponse();

            try
            {
                var attendee = new Attendee
                {
                    FirstName = attendeeDto.FirstName,
                    LastName = attendeeDto.LastName,
                    Email = attendeeDto.Email,
                    Phone = attendeeDto.Phone
                };

                _context.Attendees.Add(attendee);
                await _context.SaveChangesAsync();
                attendeeDto.AttendeeId = attendee.AttendeeId;

                serviceResponse.Status = Models.ServiceResponse.ServiceStatus.Created;
                serviceResponse.CreatedId = attendee.AttendeeId;
            }
            catch (Exception ex)
            {
                serviceResponse.Status = Models.ServiceResponse.ServiceStatus.Error;
                serviceResponse.Messages.Add("There was an error adding the attendee.");
                serviceResponse.Messages.Add(ex.Message);
            }

            return serviceResponse;
        }

        public async Task<Models.ServiceResponse> UpdateAttendee(int id, AttendeeDto attendeeDto)
        {
            var serviceResponse = new Models.ServiceResponse();

            if (id != attendeeDto.AttendeeId)
            {
                serviceResponse.Status = Models.ServiceResponse.ServiceStatus.Error;
                serviceResponse.Messages.Add("Attendee ID mismatch.");
                return serviceResponse;
            }

            var attendee = await _context.Attendees.FindAsync(id);
            if (attendee == null)
            {
                serviceResponse.Status = Models.ServiceResponse.ServiceStatus.NotFound;
                serviceResponse.Messages.Add("Attendee not found.");
                return serviceResponse;
            }

            attendee.FirstName = attendeeDto.FirstName;
            attendee.LastName = attendeeDto.LastName;
            attendee.Email = attendeeDto.Email;
            attendee.Phone = attendeeDto.Phone;

            _context.Entry(attendee).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                serviceResponse.Status = Models.ServiceResponse.ServiceStatus.Updated;
            }
            catch (DbUpdateConcurrencyException)
            {
                serviceResponse.Status = Models.ServiceResponse.ServiceStatus.Error;
                serviceResponse.Messages.Add("An error occurred while updating the attendee.");
            }

            return serviceResponse;
        }

        public async Task<Models.ServiceResponse> DeleteAttendee(int id)
        {
            var serviceResponse = new Models.ServiceResponse();

            var attendee = await _context.Attendees.FindAsync(id);
            if (attendee == null)
            {
                serviceResponse.Status = Models.ServiceResponse.ServiceStatus.NotFound;
                serviceResponse.Messages.Add("Attendee not found.");
                return serviceResponse;
            }

            try
            {
                _context.Attendees.Remove(attendee);
                await _context.SaveChangesAsync();
                serviceResponse.Status = Models.ServiceResponse.ServiceStatus.Deleted;
            }
            catch (Exception ex)
            {
                serviceResponse.Status = Models.ServiceResponse.ServiceStatus.Error;
                serviceResponse.Messages.Add("Error encountered while deleting the attendee.");
                serviceResponse.Messages.Add(ex.Message);
            }

            return serviceResponse;
        }

        public async Task<IEnumerable<EventDto>> ListEventsForAttendee(int attendeeId)
        {
            var events = await _context.EventAttendees
                .Where(ea => ea.AttendeeId == attendeeId)
                .Select(ea => new EventDto
                {
                    EventId = ea.Event.EventId,
                    EventName = ea.Event.EventName,
                    Description = ea.Event.Description,
                    Location = ea.Event.Location,
                    Date = ea.Event.Date
                })
                .ToListAsync();

            return events;
        }

        public async Task<Models.ServiceResponse> LinkAttendeeToEvent(int attendeeId, int eventId)
        {
            var serviceResponse = new Models.ServiceResponse();

            var attendee = await _context.Attendees.FindAsync(attendeeId);
            var eventEntity = await _context.Events.FindAsync(eventId);

            if (attendee == null || eventEntity == null)
            {
                serviceResponse.Status = Models.ServiceResponse.ServiceStatus.NotFound;
                if (attendee == null) serviceResponse.Messages.Add("Attendee not found.");
                if (eventEntity == null) serviceResponse.Messages.Add("Event not found.");
                return serviceResponse;
            }

            bool alreadyRegistered = await _context.EventAttendees
                .AnyAsync(ea => ea.AttendeeId == attendeeId && ea.EventId == eventId);

            if (alreadyRegistered)
            {
                serviceResponse.Status = Models.ServiceResponse.ServiceStatus.Error;
                serviceResponse.Messages.Add("Attendee is already registered for this event.");
                return serviceResponse;
            }

            _context.EventAttendees.Add(new EventAttendee
            {
                AttendeeId = attendeeId,
                EventId = eventId,
                RegistrationDate = DateTime.UtcNow
            });

            await _context.SaveChangesAsync();

            serviceResponse.Status = Models.ServiceResponse.ServiceStatus.Created;
            return serviceResponse;
        }

        public async Task<Models.ServiceResponse> UnlinkAttendeeFromEvent(int attendeeId, int eventId)
        {
            var serviceResponse = new Models.ServiceResponse();

            var eventAttendee = await _context.EventAttendees
                .FirstOrDefaultAsync(ea => ea.AttendeeId == attendeeId && ea.EventId == eventId);

            if (eventAttendee == null)
            {
                serviceResponse.Status = Models.ServiceResponse.ServiceStatus.NotFound;
                serviceResponse.Messages.Add("Attendee is not registered for this event.");
                return serviceResponse;
            }

            _context.EventAttendees.Remove(eventAttendee);
            await _context.SaveChangesAsync();

            serviceResponse.Status = Models.ServiceResponse.ServiceStatus.Deleted;
            return serviceResponse;
        }

    }
}
