//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using Microsoft.EntityFrameworkCore;
//using EventManagementSystem;
//using EventManagementSystem.Models;
//using EventManagementSystem.Data;
//using EventManagementSystem.Data.Migrations;

//namespace EventManagementSystem.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class EventAttendeeController : ControllerBase
//    {
//        private readonly ApplicationDbContext _context;
//        public EventAttendeeController(ApplicationDbContext context)
//        {
//            _context = context;
//        }


//        /// <summary>
//        /// Returns a list of all event attendees, including their associated event and attendee details.
//        /// </summary>
//        /// <returns>
//        /// 200 OK  
//        /// [{EventAttendeeDto}, {EventAttendeeDto}, ...]
//        /// </returns>
//        /// <example>
//        /// GET: api/EventAttendee/List -> [{EventAttendeeDto}, {EventAttendeeDto}, ...]
//        /// </example>

//        [HttpGet(template:"List")] 
//        public async Task<ActionResult<IEnumerable<eventattendeeDto>>> List()
//        {
//            List<EventAttendee> EventAttendees = await _context.EventAttendees
//                .Include(ea => ea.Event)
//                .Include(ea => ea.Attendee)
//                .ToListAsync();


//            // empty list of data transfer object OrderItemDto
//            List<eventattendeeDto> EventAttendeeDtos = new List<eventattendeeDto>();
//            foreach (EventAttendee EventAttendee in EventAttendees)
//            {
//                // create new instance of OrderItemDto, add to list
//                EventAttendeeDtos.Add(new eventattendeeDto()
//                {
//                    Event_AttendeeId = EventAttendee.Event_AttendeeId,
//                    AttendeeId = EventAttendee.AttendeeId,
//                    EventId = EventAttendee.EventId,
//                    RegistrationDate = EventAttendee.RegistrationDate
//                });
//            }
//            // return 200 OK with OrderItemDtos
//            return Ok(EventAttendeeDtos);
//        }


//        /// <summary>
//        /// Retrieves a specific event attendee record by its ID.
//        /// </summary>
//        /// <param name="id">The ID of the event attendee</param>
//        /// <returns>
//        /// 200 OK  
//        /// {EventAttendeeDto}  
//        /// or  
//        /// 404 Not Found
//        /// </returns>
//        /// <example>
//        /// GET: api/EventAttendee/Find/5 -> {EventAttendeeDto}
//        /// </example>

//        [HttpGet("Find/{id}")]
//        public async Task<ActionResult<eventattendeeDto>> FindEventAttendee(int id)
//        {
//            var EventAttendee = await _context.EventAttendees
//                .Include(ea => ea.Event)
//                .Include(ea => ea.Attendee)
//                .FirstOrDefaultAsync(ea => ea.Event_AttendeeId == id);

//            if (EventAttendee == null)
//            {
//                return NotFound();
//            }

//            var eventattendeeDto = new eventattendeeDto
//            {
//                Event_AttendeeId = EventAttendee.Event_AttendeeId,
//                EventId = EventAttendee.EventId,
//                AttendeeId = EventAttendee.AttendeeId,
//                RegistrationDate = EventAttendee.RegistrationDate
//            };

//            return Ok(eventattendeeDto);
//        }


//        /// <summary>
//        /// Updates an existing event attendee registration.
//        /// </summary>
//        /// <param name="id">The ID of the event attendee registration to update</param>
//        /// <param name="eventAttendeeDto">The updated event attendee details (Event_AttendeeId, EventId, AttendeeId, RegistrationDate)</param>
//        /// <returns>
//        /// 400 Bad Request if the ID does not match the EventAttendeeDto
//        /// or
//        /// 404 Not Found if no registration exists with the given ID
//        /// or
//        /// 200 OK with the updated event attendee details on successful update
//        /// </returns>
//        /// <example>
//        /// PUT: api/EventAttendee/Update/1
//        /// Body: { "Event_AttendeeId": 1, "EventId": 2, "AttendeeId": 3, "RegistrationDate": "2025-02-01" }
//        /// -> 200 OK { "Event_AttendeeId": 1, "EventId": 2, "AttendeeId": 3, "RegistrationDate": "2025-02-01" }
//        /// </example>

//        [HttpPut("Update/{id}")]
//        public async Task<IActionResult> UpdateEventAttendee(int id, [FromBody] eventattendeeDto eventAttendeeDto)
//        {
//            if (id != eventAttendeeDto.Event_AttendeeId)
//            {
//                return BadRequest("Event_AttendeeId mismatch.");
//            }

//            var existingRegistration = await _context.EventAttendees.FindAsync(id);

//            if (existingRegistration == null)
//            {
//                return NotFound("Registration not found.");
//            }

//            existingRegistration.EventId = eventAttendeeDto.EventId;
//            existingRegistration.AttendeeId = eventAttendeeDto.AttendeeId;
//            existingRegistration.RegistrationDate = eventAttendeeDto.RegistrationDate;

//            _context.Entry(existingRegistration).State = EntityState.Modified;

//            try
//            {
//                await _context.SaveChangesAsync();
//            }
//            catch (DbUpdateConcurrencyException)
//            {
//                if (!_context.EventAttendees.Any(ea => ea.Event_AttendeeId == id))
//                {
//                    return NotFound();
//                }
//                else
//                {
//                    throw;
//                }
//            }

//            // Return only the necessary properties
//            var responseDto = new eventattendeeDto
//            {
//                Event_AttendeeId = existingRegistration.Event_AttendeeId,
//                EventId = existingRegistration.EventId,
//                AttendeeId = existingRegistration.AttendeeId,
//                RegistrationDate = existingRegistration.RegistrationDate
//            };

//            return Ok(responseDto);
//        }


//        /// <summary>
//        /// Registers an attendee for an event.
//        /// </summary>
//        /// <param name="eventAttendeeDto">The event attendee registration details (EventId, AttendeeId, RegistrationDate)</param>
//        /// <returns>
//        /// 201 Created with the created EventAttendee details
//        /// or
//        /// 400 Bad Request if the attendee is already registered for the event
//        /// or
//        /// 404 Not Found if the Event or Attendee does not exist
//        /// </returns>
//        /// <example>
//        /// POST: api/EventAttendee/Add
//        /// Body: { "EventId": 2, "AttendeeId": 3, "RegistrationDate": "2025-01-01" }
//        /// -> 201 Created { "EventAttendeeId": 5, "EventId": 2, "AttendeeId": 3, "RegistrationDate": "2025-01-01" }
//        /// </example>

//        [HttpPost("Add")]
//        public async Task<ActionResult<EventAttendee>> AddEventAttendee([FromBody] eventattendeeDto eventAttendeeDto)
//        {
//            // Ensure event and attendee are retrieved from the database
//            var eventEntity = await _context.Events.FindAsync(eventAttendeeDto.EventId);
//            var attendeeEntity = await _context.Attendees.FindAsync(eventAttendeeDto.AttendeeId);

//            if (eventEntity == null || attendeeEntity == null)
//            {
//                return NotFound("Event or Attendee not found.");
//            }

//            // Check if attendee is already registered for the event
//            var existingRegistration = await _context.EventAttendees
//                .FirstOrDefaultAsync(ea => ea.EventId == eventAttendeeDto.EventId && ea.AttendeeId == eventAttendeeDto.AttendeeId);

//            if (existingRegistration != null)
//            {
//                return BadRequest("Attendee is already registered for this event.");
//            }

//            // Create a new EventAttendee entry
//            var eventAttendee = new EventAttendee
//            {
//                EventId = eventAttendeeDto.EventId,
//                AttendeeId = eventAttendeeDto.AttendeeId,
//                RegistrationDate = eventAttendeeDto.RegistrationDate
//            };

//            _context.EventAttendees.Add(eventAttendee);
//            await _context.SaveChangesAsync();

//            // Return only required fields
//            return CreatedAtAction(nameof(FindEventAttendee),
//                new { eventId = eventAttendee.EventId, attendeeId = eventAttendee.AttendeeId },
//                new
//                {
//                    EventAttendeeId = eventAttendee.Event_AttendeeId, 
//                    EventId = eventAttendee.EventId,
//                    AttendeeId = eventAttendee.AttendeeId,
//                    RegistrationDate = eventAttendee.RegistrationDate
//                });
//        }


//        /// <summary>
//        /// Unregisters an attendee from an event.
//        /// </summary>
//        /// <param name="id">The ID of the event attendee record to delete</param>
//        /// <returns>
//        /// 204 No Content  
//        /// or  
//        /// 404 Not Found if record does not exist  
//        /// </returns>
//        /// <example>
//        /// DELETE: api/EventAttendee/Delete/5
//        /// </example>

//        [HttpDelete("Delete/{id}")]
//        public async Task<IActionResult> UnregisterAttendee(int id)
//        {
//            var eventAttendee = await _context.EventAttendees.FindAsync(id);

//            if (eventAttendee == null)
//            {
//                return NotFound();
//            }

//            _context.EventAttendees.Remove(eventAttendee);
//            await _context.SaveChangesAsync();

//            return NoContent();
//        }


//        private bool EventAttendeeExists(int id)
//        {
//            return _context.EventAttendees.Any(ea => ea.Event_AttendeeId == id);
//        }


//    }
//}


using Microsoft.AspNetCore.Mvc;
using EventManagementSystem.Interfaces;
using EventManagementSystem.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EventManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventAttendeeController : ControllerBase
    {
        private readonly IEventAttendeeService _eventAttendeeService;

        public EventAttendeeController(IEventAttendeeService eventAttendeeService)
        {
            _eventAttendeeService = eventAttendeeService;
        }

        [HttpGet("List")]
        public async Task<ActionResult<IEnumerable<eventattendeeDto>>> List()
        {
            var eventAttendees = await _eventAttendeeService.ListEventAttendees();
            return Ok(eventAttendees);
        }

        [HttpGet("Find/{id}")]
        public async Task<ActionResult<eventattendeeDto>> FindEventAttendee(int id)
        {
            var eventAttendee = await _eventAttendeeService.FindEventAttendee(id);
            if (eventAttendee == null)
            {
                return NotFound();
            }

            return Ok(eventAttendee);
        }

        [HttpPut("Update/{id}")]
        public async Task<IActionResult> UpdateEventAttendee(int id, [FromBody] eventattendeeDto eventAttendeeDto)
        {
            var updatedEventAttendee = await _eventAttendeeService.UpdateEventAttendee(id, eventAttendeeDto);

            if (updatedEventAttendee == null)
            {
                return NotFound();
            }

            return Ok(updatedEventAttendee);
        }

        [HttpPost("Add")]
        public async Task<ActionResult<eventattendeeDto>> AddEventAttendee([FromBody] eventattendeeDto eventAttendeeDto)
        {
            var newEventAttendee = await _eventAttendeeService.AddEventAttendee(eventAttendeeDto);

            if (newEventAttendee == null)
            {
                return NotFound();
            }

            return CreatedAtAction(nameof(FindEventAttendee), new { id = newEventAttendee.Event_AttendeeId }, newEventAttendee);
        }

        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> UnregisterAttendee(int id)
        {
            var success = await _eventAttendeeService.DeleteEventAttendee(id);

            if (!success)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}


