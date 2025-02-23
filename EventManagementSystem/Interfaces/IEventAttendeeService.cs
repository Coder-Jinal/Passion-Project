using System.Collections.Generic;
using System.Threading.Tasks;
using EventManagementSystem.Models;

namespace EventManagementSystem.Interfaces
{
    public interface IEventAttendeeService
    {
        Task<IEnumerable<eventattendeeDto>> ListEventAttendees();
        Task<eventattendeeDto> FindEventAttendee(int id);
        Task<eventattendeeDto> AddEventAttendee(eventattendeeDto eventAttendeeDto);
        Task<bool> UpdateEventAttendee(int id, eventattendeeDto eventAttendeeDto);
        Task<bool> DeleteEventAttendee(int id);
    }
}
