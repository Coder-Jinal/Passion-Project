using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using EventManagementSystem.Services;
using EventManagementSystem.Interfaces;
using EventManagementSystem.Models;

[Route("api/[controller]")]
[ApiController]
public class EventController : ControllerBase
{
    private readonly IEventService _eventService;

    public EventController(IEventService eventService)
    {
        _eventService = eventService;
    }

    /// <summary>
    /// Retrieves a list of all events.
    /// </summary>
    /// <returns>
    /// 200 OK
    /// [{EventDto}, {EventDto}, ...]
    /// </returns>
    [HttpGet("List")]
    public async Task<ActionResult<IEnumerable<EventDto>>> ListEvents()
    {
        var events = await _eventService.ListEvents();
        return Ok(events);
    }

    /// <summary>
    /// Retrieves a specific event by ID.
    /// </summary>
    /// <param name="id">The event ID</param>
    /// <returns>
    /// 200 OK
    /// {EventDto}
    /// or
    /// 404 Not Found
    /// </returns>
    [HttpGet("Find/{id}")]
    public async Task<ActionResult<EventDto>> FindEvent(int id)
    {
        var eventDto = await _eventService.FindEvent(id);
        if (eventDto == null)
        {
            return NotFound();
        }
        return Ok(eventDto);
    }

    /// <summary>
    /// Creates a new event.
    /// </summary>
    /// <param name="eventDto">The event details</param>
    /// <returns>
    /// 201 Created
    /// Location: api/Event/Find/{EventId}
    /// {EventDto}
    /// </returns>
    [HttpPost("Add")]
    public async Task<ActionResult<EventDto>> AddEvent(EventDto eventDto)
    {
        if (eventDto == null)
        {
            return BadRequest("Invalid event data.");
        }

        var serviceResponse = await _eventService.AddEvent(eventDto);

        // Ensure response is valid
        if (serviceResponse == null || serviceResponse.EventData == null)
        {
            return BadRequest("Failed to create event.");
        }

        var createdEvent = serviceResponse.EventData;

        return CreatedAtAction(nameof(FindEvent), new { id = createdEvent.EventId }, createdEvent);
    }



    /// <summary>
    /// Updates an existing event.
    /// </summary>
    /// <param name="id">The ID of the event</param>
    /// <param name="eventDto">Updated event details</param>
    /// <returns>
    /// 400 Bad Request
    /// or
    /// 404 Not Found
    /// or
    /// 204 No Content
    /// </returns>
    [HttpPut("Update/{id}")]
    public async Task<IActionResult> UpdateEvent(int id, EventDto eventDto)
    {
        if (eventDto == null || id != eventDto.EventId)
        {
            return BadRequest("Invalid request.");
        }
        var updated = await _eventService.UpdateEvent(id, eventDto);
        if (!updated)
        {
            return NotFound();
        }
        return NoContent();
    }

    /// <summary>
    /// Deletes an event by its ID.
    /// </summary>
    /// <param name="id">The ID of the event</param>
    /// <returns>
    /// 204 No Content
    /// or
    /// 404 Not Found
    /// </returns>
    [HttpDelete("Delete/{id}")]
    public async Task<IActionResult> DeleteEvent(int id)
    {
        var deleted = await _eventService.DeleteEvent(id);
        if (!deleted)
        {
            return NotFound();
        }
        return NoContent();
    }

    /// <summary>
    /// Lists all attendees for a specific event.
    /// </summary>
    /// <param name="eventId">The ID of the event</param>
    /// <returns>
    /// 200 OK
    /// [{AttendeeDto}, {AttendeeDto}, ...]
    /// or
    /// 404 Not Found
    /// </returns>
    [HttpGet("ListAttendeesForEvent/{eventId}")]
    public async Task<IActionResult> ListAttendeesForEvent(int eventId)
    {
        var attendees = await _eventService.ListAttendeesForEvent(eventId);
        if (attendees == null || attendees.Count() == 0)
        {
            return NotFound($"No attendees found for event with ID {eventId}.");
        }
        return Ok(attendees);
    }
}




