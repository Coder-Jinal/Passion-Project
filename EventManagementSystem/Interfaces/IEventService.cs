using System.Collections.Generic;
using System.Threading.Tasks;
using EventManagementSystem.Models;

namespace EventManagementSystem.Interfaces
{
    public interface IEventService
    {
        Task<IEnumerable<EventDto>> ListEvents();
        Task<EventDto?> FindEvent(int id);
        Task<Models.ServiceResponse> UpdateEvent(int id, EventDto eventDto);

        Task<Models.ServiceResponse> AddEvent(EventDto eventDto);

        Task<Models.ServiceResponse> DeleteEvent(int id);
        Task<IEnumerable<AttendeeDto>> ListAttendeesForEvent(int eventId);


    }
}

