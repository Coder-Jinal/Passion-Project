using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using EventManagementSystem;
using EventManagementSystem.Models;
using EventManagementSystem.Data;
using EventManagementSystem.Data.Migrations;

namespace EventManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public EventController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Returns a list of all events, each represented by an EventDto.
        /// </summary>
        /// <returns>
        /// 200 OK
        /// [{EventDto},{EventDto},..]
        /// </returns>
        /// <example>
        /// GET: api/Event/List -> [{EventDto},{EventDto},..]
        /// </example>

        [HttpGet("List")]
        public async Task<ActionResult<IEnumerable<EventDto>>> ListEvents()
        {
            var Events = await _context.Events.ToListAsync();

            var EventDtos = Events.Select(e => new EventDto
            {
                EventId = e.EventId,
                EventName = e.EventName,
                Description = e.Description,
                Location = e.Location,
                Date = e.Date
            }).ToList();

            return Ok(EventDtos);
        }

        /// <summary>
        /// Finds an event by its ID.
        /// </summary>
        /// <param name="id">The ID of the event to find</param>
        /// <returns>
        /// 200 OK with the found EventDto
        /// or
        /// 404 Not Found if no event exists with the given ID
        /// </returns>
        /// <example>
        /// GET: api/Event/Find/1 -> { EventId: 1, EventName: "Sample Event", ... }
        /// </example>

        [HttpGet("Find/{id}")]
        public async Task<ActionResult<EventDto>> FindEvent(int id)
        {
            var EventEntity = await _context.Events.FindAsync(id);

            if (EventEntity == null)
            {
                return NotFound();
            }

            var eventDto = new EventDto
            {
                EventId = EventEntity.EventId,
                EventName = EventEntity.EventName,
                Description = EventEntity.Description,
                Location = EventEntity.Location,
                Date = EventEntity.Date
            };

            return Ok(eventDto);
        }



        /// <summary>
        /// Adds a new event.
        /// </summary>
        /// <param name="eventDto">The event details to add (EventName, Description, Location, Date)</param>
        /// <returns>
        /// 201 Created with the created EventDto
        /// or
        /// 400 Bad Request if the request is invalid
        /// </returns>
        /// <example>
        /// POST: api/Event/Add
        /// Body: { "EventName": "New Event", "Description": "Corncert", "Location": "Hamilton", "Date": "2025-01-01" }
        /// -> 201 Created { EventId: 5, EventName: "New Event", ... }
        /// </example>

        [HttpPost("Add")]
        public async Task<ActionResult<EventDto>> AddEvent(EventDto eventDto)
        {
            if (eventDto == null)
            {
                return BadRequest("Invalid event data.");
            }

            var EventEntity = new Event
            {
                EventName = eventDto.EventName,
                Description = eventDto.Description,
                Location = eventDto.Location,
                Date = eventDto.Date
            };

            _context.Events.Add(EventEntity);
            await _context.SaveChangesAsync();

            eventDto.EventId = EventEntity.EventId; // Return the generated EventId

            return CreatedAtAction(nameof(FindEvent), new { id = EventEntity.EventId }, eventDto);
        }


        /// <summary>
        /// Updates an existing event.
        /// </summary>
        /// <param name="id">The ID of the event to update</param>
        /// <param name="eventDto">The updated event details (EventId, EventName, Description, Location, Date)</param>
        /// <returns>
        /// 400 Bad Request if the ID does not match the EventDto
        /// or
        /// 404 Not Found if no event exists with the given ID
        /// or
        /// 204 No Content on successful update
        /// </returns>
        /// <example>
        /// PUT: api/Event/Update/1
        /// Body: { "EventId": 1, "EventName": "Updated Event", "Description": "Updated Details", "Location": "New Venue", "Date": "2025-02-01" }
        /// -> 204 No Content
        /// </example>

        [HttpPut("Update/{id}")]
        public async Task<IActionResult> UpdateEvent(int id, EventDto eventDto)
        {
            if (id != eventDto.EventId)
            {
                return BadRequest();
            }

            var EventEntity = await _context.Events.FindAsync(id);
            if (EventEntity == null)
            {
                return NotFound();
            }

            EventEntity.EventName = eventDto.EventName;
            EventEntity.Description = eventDto.Description;
            EventEntity.Location = eventDto.Location;
            EventEntity.Date = eventDto.Date;

            _context.Entry(EventEntity).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EventExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }


        /// <summary>
        /// Deletes an event by its ID.
        /// </summary>
        /// <param name="id">The ID of the event to delete</param>
        /// <returns>
        /// 404 Not Found if no event exists with the given ID
        /// or
        /// 204 No Content on successful deletion
        /// </returns>
        /// <example>
        /// DELETE: api/Event/Delete/1 -> 204 No Content
        /// </example>

        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> DeleteEvent(int id)
        {
            var EventEntity = await _context.Events.FindAsync(id);
            if (EventEntity == null)
            {
                return NotFound();
            }

            _context.Events.Remove(EventEntity);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool EventExists(int id)
        {
            return _context.Events.Any(e => e.EventId == id);
        }


        /// <summary>
        /// Returns a list of attendees registered for a specific event by its {eventId}.
        /// </summary>
        /// <param name="eventId">The ID of the event.</param>
        /// <returns>
        /// 200 OK: A list of attendees in the format [{AttendeeDto}, {AttendeeDto}, ...]
        /// 404 Not Found: If no attendees are found for the given event ID.
        /// </returns>
        /// <example>
        /// GET: api/Event/ListAttendeesForEvent/1 -> [{AttendeeDto}, {AttendeeDto}, ...]
        /// </example>

        [HttpGet("ListAttendeesForEvent/{eventId}")]
        public async Task<IActionResult> ListAttendeesForEvent(int eventId)
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

            if (!attendees.Any())
            {
                return NotFound($"No attendees found for event with ID {eventId}.");
            }

            return Ok(attendees);
        }

    }
}
