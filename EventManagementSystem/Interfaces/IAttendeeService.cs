using System.Collections.Generic;
using System.Threading.Tasks;
using EventManagementSystem.Models;


namespace EventManagementSystem.Interfaces
{
    public interface IAttendeeService
    {
        Task<IEnumerable<AttendeeDto>> ListAttendees();

        Task<AttendeeDto?> FindAttendee(int id);

        Task<Models.ServiceResponse> UpdateAttendee(int id, AttendeeDto attendeeDto);

        Task<Models.ServiceResponse> AddAttendee(AttendeeDto attendeeDto);

        Task<Models.ServiceResponse> DeleteAttendee(int id);

        Task<IEnumerable<EventDto>> ListEventsForAttendee(int attendeeId);

        Task<Models.ServiceResponse> LinkAttendeeToEvent(int attendeeId, int eventId);

        Task<Models.ServiceResponse> UnlinkAttendeeFromEvent(int attendeeId, int eventId);
    }
}
