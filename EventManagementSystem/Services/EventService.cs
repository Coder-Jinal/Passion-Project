using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using EventManagementSystem.Data;
using EventManagementSystem.Models;
using EventManagementSystem.Interfaces;

namespace EventManagementSystem.Services
{
    public class EventService : IEventService
    {
        private readonly ApplicationDbContext _context;

        public EventService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<EventDto>> ListEvents()
        {
            var events = await _context.Events.ToListAsync();
            return events.Select(e => new EventDto
            {
                EventId = e.EventId,
                EventName = e.EventName,
                Description = e.Description,
                Location = e.Location,
                Date = e.Date
            }).ToList();
        }

        public async Task<EventDto?> FindEvent(int id)
        {
            var eventEntity = await _context.Events.FindAsync(id);
            if (eventEntity == null) return null;

            return new EventDto
            {
                EventId = eventEntity.EventId,
                EventName = eventEntity.EventName,
                Description = eventEntity.Description,
                Location = eventEntity.Location,
                Date = eventEntity.Date
            };
        }

        public async Task<Models.ServiceResponse> AddEvent(EventDto eventDto)
        {
            var serviceResponse = new Models.ServiceResponse();

            try
            {
                var eventEntity = new Event
                {
                    EventName = eventDto.EventName,
                    Description = eventDto.Description,
                    Location = eventDto.Location,
                    Date = eventDto.Date
                };

                _context.Events.Add(eventEntity);
                await _context.SaveChangesAsync();
                eventDto.EventId = eventEntity.EventId;

                serviceResponse.Status = Models.ServiceResponse.ServiceStatus.Created;
                serviceResponse.CreatedId = eventEntity.EventId;
            }
            catch (Exception ex)
            {
                serviceResponse.Status = Models.ServiceResponse.ServiceStatus.Error;
                serviceResponse.Messages.Add("There was an error adding the event.");
                serviceResponse.Messages.Add(ex.Message);
            }

            return serviceResponse;
        }

        public async Task<Models.ServiceResponse> UpdateEvent(int id, EventDto eventDto)
        {
            var serviceResponse = new Models.ServiceResponse();

            if (id != eventDto.EventId)
            {
                serviceResponse.Status = Models.ServiceResponse.ServiceStatus.Error;
                serviceResponse.Messages.Add("Event ID mismatch.");
                return serviceResponse;
            }

            var eventEntity = await _context.Events.FindAsync(id);
            if (eventEntity == null)
            {
                serviceResponse.Status = Models.ServiceResponse.ServiceStatus.NotFound;
                serviceResponse.Messages.Add("Event not found.");
                return serviceResponse;
            }

            eventEntity.EventName = eventDto.EventName;
            eventEntity.Description = eventDto.Description;
            eventEntity.Location = eventDto.Location;
            eventEntity.Date = eventDto.Date;

            _context.Entry(eventEntity).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                serviceResponse.Status = Models.ServiceResponse.ServiceStatus.Updated;
            }
            catch (DbUpdateConcurrencyException)
            {
                serviceResponse.Status = Models.ServiceResponse.ServiceStatus.Error;
                serviceResponse.Messages.Add("An error occurred while updating the event.");
            }

            return serviceResponse;
        }

        public async Task<Models.ServiceResponse> DeleteEvent(int id)
        {
            var serviceResponse = new Models.ServiceResponse();

            var eventEntity = await _context.Events.FindAsync(id);
            if (eventEntity == null)
            {
                serviceResponse.Status = Models.ServiceResponse.ServiceStatus.NotFound;
                serviceResponse.Messages.Add("Event not found.");
                return serviceResponse;
            }

            try
            {
                _context.Events.Remove(eventEntity);
                await _context.SaveChangesAsync();
                serviceResponse.Status = Models.ServiceResponse.ServiceStatus.Deleted;
            }
            catch (Exception ex)
            {
                serviceResponse.Status = Models.ServiceResponse.ServiceStatus.Error;
                serviceResponse.Messages.Add("Error encountered while deleting the event.");
                serviceResponse.Messages.Add(ex.Message);
            }

            return serviceResponse;
        }


        public async Task<IEnumerable<AttendeeDto>> ListAttendeesForEvent(int eventId)
        {
            var attendees = await _context.EventAttendees
                .Where(ea => ea.EventId == eventId)
                .Select(ea => new AttendeeDto
                {
                    AttendeeId = ea.Attendee.AttendeeId,
                    FirstName = ea.Attendee.FirstName,
                    LastName = ea.Attendee.LastName,
                    Email = ea.Attendee.Email,
                    Phone = ea.Attendee.Phone
                })
                .ToListAsync();

            return attendees;
        }
    }
}
