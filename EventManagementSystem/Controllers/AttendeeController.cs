using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using EventManagementSystem.Models;
using EventManagementSystem.Interfaces;

namespace EventManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AttendeeController : ControllerBase
    {
        private readonly IAttendeeService _attendeeService;

        // Dependency injection for attendee service
        public AttendeeController(IAttendeeService attendeeService)
        {
            _attendeeService = attendeeService;
        }

        /// <summary>
        /// Returns a list of all attendees
        /// </summary>
        /// <returns>
        /// 200 OK
        /// [{AttendeeDto}, {AttendeeDto}, ...]
        /// </returns>
        /// <example>
        /// GET: api/Attendee/List -> [{AttendeeDto}, {AttendeeDto}, ...]
        /// </example>
        [HttpGet("List")]
        public async Task<ActionResult<IEnumerable<AttendeeDto>>> ListAttendees()
        {
            var attendees = await _attendeeService.ListAttendees();
            return Ok(attendees);
        }

        /// <summary>
        /// Returns a single attendee specified by its {id}
        /// </summary>
        /// <param name="id">The attendee id</param>
        /// <returns>
        /// 200 OK
        /// {AttendeeDto}
        /// or
        /// 404 Not Found
        /// </returns>
        /// <example>
        /// GET: api/Attendee/Find/1 -> {AttendeeDto}
        /// </example>
        [HttpGet("Find/{id}")]
        public async Task<ActionResult<AttendeeDto>> FindAttendee(int id)
        {
            var attendee = await _attendeeService.FindAttendee(id);
            if (attendee == null) return NotFound();
            return Ok(attendee);
        }

        /// <summary>
        /// Adds a new attendee
        /// </summary>
        /// <param name="attendeeDto">The required information to add the attendee (e.g., Name, Email)</param>
        /// <returns>
        /// 201 Created
        /// Location: api/Attendee/Find/{AttendeeId}
        /// {AttendeeDto}
        /// </returns>
        /// <example>
        /// POST: api/Attendee/Add
        /// Request Headers: Content-Type: application/json
        /// Request Body: {AttendeeDto}
        /// -> Response Code: 201 Created
        /// Response Headers: Location: api/Attendee/Find/{AttendeeId}
        /// </example>
        [HttpPost("Add")]
        public async Task<ActionResult<AttendeeDto>> AddAttendee(AttendeeDto attendeeDto)
        {
            if (attendeeDto == null) return BadRequest("Invalid attendee data.");

            var serviceResponse = await _attendeeService.AddAttendee(attendeeDto);

            // Check if the service response or its Payload is null
            if (serviceResponse == null || serviceResponse.Payload == null)
            {
                return BadRequest("Failed to create attendee.");
            }

            // Access the actual AttendeeDto from Payload
            var createdAttendee = serviceResponse.Payload;

            // Return Created response with the created attendee
            return CreatedAtAction(nameof(FindAttendee), new { id = createdAttendee.AttendeeId }, createdAttendee);
        }


        /// <summary>
        /// Updates an existing attendee
        /// </summary>
        /// <param name="id">The ID of the attendee to update</param>
        /// <param name="attendeeDto">The information to update the attendee (e.g., Name, Email)</param>
        /// <returns>
        /// 400 Bad Request
        /// or
        /// 404 Not Found
        /// or
        /// 204 No Content
        /// </returns>
        /// <example>
        /// PUT: api/Attendee/Update/5
        /// Request Headers: Content-Type: application/json
        /// Request Body: {AttendeeDto}
        /// -> Response Code: 204 No Content
        /// </example>
        [HttpPut("Update/{id}")]
        public async Task<IActionResult> UpdateAttendee(int id, AttendeeDto attendeeDto)
        {
            if (id != attendeeDto.AttendeeId)
            {
                return BadRequest("Attendee ID mismatch.");
            }

            bool updated = await _attendeeService.UpdateAttendee(id, attendeeDto);
            if (!updated) return NotFound();

            return NoContent();
        }

        /// <summary>
        /// Deletes an attendee by its ID
        /// </summary>
        /// <param name="id">The ID of the attendee to delete</param>
        /// <returns>
        /// 204 No Content
        /// or
        /// 404 Not Found
        /// </returns>
        /// <example>
        /// DELETE: api/Attendee/Delete/7
        /// -> Response Code: 204 No Content
        /// </example>
        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> DeleteAttendee(int id)
        {
            bool deleted = await _attendeeService.DeleteAttendee(id);
            if (!deleted) return NotFound();

            return NoContent();
        }

        /// <summary>
        /// Lists all events for a specific attendee
        /// </summary>
        /// <param name="attendeeId">The ID of the attendee</param>
        /// <returns>
        /// 200 OK
        /// [{EventDto}, {EventDto}, ...]
        /// </returns>
        /// <example>
        /// GET: api/Attendee/ListEventsForAttendee/3 -> [{EventDto}, {EventDto}, ...]
        /// </example>
        [HttpGet("ListEventsForAttendee/{attendeeId}")]
        public async Task<IActionResult> ListEventsForAttendee(int attendeeId)
        {
            var events = await _attendeeService.ListEventsForAttendee(attendeeId);
            if (events == null || events.Count() == 0)  // Ensure Count() is invoked
            {
                return NotFound($"No events found for attendee with ID {attendeeId}.");
            }
            return Ok(events);
        }


        /// <summary>
        /// Links an attendee to an event
        /// </summary>
        /// <param name="attendeeId">The ID of the attendee</param>
        /// <param name="eventId">The ID of the event</param>
        /// <returns>
        /// 204 No Content
        /// or
        /// 404 Not Found
        /// </returns>
        /// <example>
        /// POST: api/Attendee/Link?attendeeId=4&eventId=12
        /// -> Response Code: 204 No Content
        /// </example>
        [HttpPost("Link")]
        public async Task<IActionResult> LinkAttendeeToEvent(int attendeeId, int eventId)
        {
            var response = await _attendeeService.LinkAttendeeToEvent(attendeeId, eventId);

            if (response.Status == Models.ServiceResponse.ServiceStatus.NotFound)
                return NotFound(response.Messages);

            if (response.Status == Models.ServiceResponse.ServiceStatus.Error)
                return BadRequest(response.Messages);

            return NoContent(); 
        }


        /// <summary>
        /// Unlinks an attendee from an event
        /// </summary>
        /// <param name="attendeeId">The ID of the attendee</param>
        /// <param name="eventId">The ID of the event</param>
        /// <returns>
        /// 204 No Content
        /// or
        /// 404 Not Found
        /// </returns>
        /// <example>
        /// DELETE: api/Attendee/Unlink?attendeeId=4&eventId=12
        /// -> Response Code: 204 No Content
        /// </example>
        [HttpDelete("Unlink")]
        public async Task<ActionResult> UnlinkAttendeeFromEvent(int attendeeId, int eventId)
        {
            var response = await _attendeeService.UnlinkAttendeeFromEvent(attendeeId, eventId);

            if (response.Status == Models.ServiceResponse.ServiceStatus.NotFound)
                return NotFound(response.Messages);

            return NoContent();
        }

    }
}

