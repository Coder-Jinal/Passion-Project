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
    public class AttendeeController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AttendeeController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Returns a list of attendees.
        /// </summary>
        /// <returns>
        /// 200 OK - Returns a list of attendees in JSON format.
        /// </returns>
        /// <example>
        /// GET: api/Attendee/List -> [{AttendeeDto},{AttendeeDto},..] 
        /// </example>
        [HttpGet("List")]
        public async Task<ActionResult<IEnumerable<AttendeeDto>>> ListAttendees()
        {
            var Attendees = await _context.Attendees.ToListAsync();

            var AttendeeDtos = Attendees.Select(a => new AttendeeDto
            {
                AttendeeId = a.AttendeeId,
                FirstName = a.FirstName,
                LastName = a.LastName,
                Email = a.Email,
                Phone = a.Phone
            }).ToList();

            return Ok(AttendeeDtos);
        }


        /// <summary>
        /// Finds a specific attendee by their ID.
        /// </summary>
        /// <param name="id">The ID of the attendee to find.</param>
        /// <returns>
        /// 200 OK - Returns the attendee details if found.  
        /// 404 Not Found - If no attendee exists with the given ID.
        /// </returns>
        /// <example>
        /// GET: api/Attendee/Find/1 -> {AttendeeDto}
        /// </example>
        [HttpGet("Find/{id}")]
        public async Task<ActionResult<AttendeeDto>> FindAttendee(int id)
        {
            var Attendee = await _context.Attendees.FindAsync(id);

            if (Attendee == null)
            {
                return NotFound();
            }

            var attendeeDto = new AttendeeDto
            {
                AttendeeId = Attendee.AttendeeId,
                FirstName = Attendee.FirstName,
                LastName = Attendee.LastName,
                Email = Attendee.Email,
                Phone = Attendee.Phone
            };

            return Ok(attendeeDto);
        }

        /// <summary>
        /// Adds a new attendee.
        /// </summary>
        /// <param name="attendeeDto">The attendee details to add.</param>
        /// <returns>
        /// 201 Created - Returns the newly created attendee details with its assigned ID.
        /// </returns>
        /// <example>
        /// POST: api/Attendee/Add
        /// Body: { "FirstName": "John", "LastName": "Doe", "Email": "johndoe@email.com", "Phone": "1234567890" }
        /// -> 201 Created { AttendeeId: 5, FirstName: "John", ... }
        /// </example>
        [HttpPost("Add")]
        public async Task<ActionResult<AttendeeDto>> AddAttendee(AttendeeDto attendeeDto)
        {
            if (attendeeDto == null)
            {
                return BadRequest("Invalid event data.");
            }

            var attendee = new Attendee
            {
                FirstName = attendeeDto.FirstName,
                LastName = attendeeDto.LastName,
                Email = attendeeDto.Email,
                Phone = attendeeDto.Phone
            };

            _context.Attendees.Add(attendee);
            await _context.SaveChangesAsync();

            attendeeDto.AttendeeId = attendee.AttendeeId; // Return the generated ID

            return CreatedAtAction(nameof(FindAttendee), new { id = attendee.AttendeeId }, attendeeDto);
        }

        /// <summary>
        /// Updates an existing attendee.
        /// </summary>
        /// <param name="id">The ID of the attendee to update.</param>
        /// <param name="attendeeDto">The updated attendee details.</param>
        /// <returns>
        /// 204 No Content - If the update is successful.  
        /// 400 Bad Request - If the ID does not match the provided attendee data.  
        /// 404 Not Found - If the attendee does not exist.
        /// </returns>
        /// <example>
        /// PUT: api/Attendee/Update/1
        /// Body: { "AttendeeId": 1, "FirstName": "Jane", "LastName": "Doe", "Email": "janedoe@email.com", "Phone": "9876543210" }
        /// </example>
        [HttpPut("Update/{id}")]
        public async Task<IActionResult> UpdateAttendee(int id, AttendeeDto attendeeDto)
        {
            if (id != attendeeDto.AttendeeId)
            {
                return BadRequest();
            }

            var Attendee = await _context.Attendees.FindAsync(id);
            if (Attendee == null)
            {
                return NotFound();
            }

            Attendee.FirstName = attendeeDto.FirstName;
            Attendee.LastName = attendeeDto.LastName;
            Attendee.Email = attendeeDto.Email;
            Attendee.Phone = attendeeDto.Phone;

            _context.Entry(Attendee).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AttendeeExists(id))
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
        /// Deletes an attendee by ID.
        /// </summary>
        /// <param name="id">The ID of the attendee to delete.</param>
        /// <returns>
        /// 204 No Content - If the deletion is successful.  
        /// 404 Not Found - If the attendee does not exist.
        /// </returns>
        /// <example>
        /// DELETE: api/Attendee/Delete/1
        /// </example>
        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> DeleteAttendee(int id)
        {
            var Attendee = await _context.Attendees.FindAsync(id);
            if (Attendee == null)
            {
                return NotFound();
            }

            _context.Attendees.Remove(Attendee);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool AttendeeExists(int id)
        {
            return _context.Attendees.Any(a => a.AttendeeId == id);
        }

        /// <summary>
        /// Returns a list of events registered by a specific attendee using their {attendeeId}.
        /// </summary>
        /// <param name="attendeeId">The ID of the attendee.</param>
        /// <returns>
        /// 200 OK: A list of events in the format [{EventDto}, {EventDto}, ...]
        /// 404 Not Found: If no events are found for the given attendee ID.
        /// </returns>
        /// <example>
        /// GET: api/Attendee/ListEventsForAttendee/7 -> [{EventDto}, {EventDto}, ...]
        /// </example>

        [HttpGet("ListEventsForAttendee/{attendeeId}")]
        public async Task<IActionResult> ListEventsForAttendee(int attendeeId)
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

            if (!events.Any())
            {
                return NotFound($"No events found for attendee with ID {attendeeId}.");
            }

            return Ok(events);
        }

        /// <summary>
        /// Links an attendee to an event (Registers the attendee)
        /// </summary>
        /// <param name="attendeeId">The ID of the attendee</param>
        /// <param name="eventId">The ID of the event</param>
        /// <returns>204 No Content or 404 Not Found</returns>
        /// <example>
        /// POST: api/Attendee/Link?attendeeId=2&eventId=5
        /// Response Code: 204 No Content
        /// </example>

        [HttpPost("Link")]
        public async Task<ActionResult> Link(int attendeeId, int eventId)
        {
            // Check if Attendee Exists
            var attendee = await _context.Attendees.FindAsync(attendeeId);
            if (attendee == null)
            {
                return NotFound("Attendee not found.");
            }

            // Check if Event Exists
            var eventEntity = await _context.Events.FindAsync(eventId);
            if (eventEntity == null)
            {
                return NotFound("Event not found.");
            }

            // Check if Attendee is Already Registered for the Event
            bool alreadyRegistered = await _context.EventAttendees
                .AnyAsync(ea => ea.AttendeeId == attendeeId && ea.EventId == eventId);

            if (alreadyRegistered)
            {
                return BadRequest("Attendee is already registered for this event.");
            }

            // Create and Save the New EventAttendee Record
            var eventAttendee = new EventAttendee
            {
                AttendeeId = attendeeId,
                EventId = eventId,
                RegistrationDate = DateTime.UtcNow
            };

            _context.EventAttendees.Add(eventAttendee);
            await _context.SaveChangesAsync();

            return NoContent();  // 204 No Content (Successful, but no response body)
        }

        /// <summary>
        /// Unlinks an attendee from an event (Removes the attendee)
        /// </summary>
        /// <param name="attendeeId">The ID of the attendee</param>
        /// <param name="eventId">The ID of the event</param>
        /// <returns>204 No Content or 404 Not Found</returns>
        /// <example>
        /// DELETE: api/Attendee/Unlink?attendeeId=2&eventId=5
        /// Response Code: 204 No Content
        /// </example>

        [HttpDelete("Unlink")]
        public async Task<ActionResult> Unlink(int attendeeId, int eventId)
        {
        // Find the EventAttendee record
        var eventAttendee = await _context.EventAttendees
            .FirstOrDefaultAsync(ea => ea.AttendeeId == attendeeId && ea.EventId == eventId);

        if (eventAttendee == null)
        {
            return NotFound("Attendee is not registered for this event.");
        }

        // Remove the Attendee from the Event
        _context.EventAttendees.Remove(eventAttendee);
        await _context.SaveChangesAsync();

        return NoContent();
        }
    }
}
