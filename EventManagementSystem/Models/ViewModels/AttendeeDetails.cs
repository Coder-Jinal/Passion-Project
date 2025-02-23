

namespace EventManagementSystem.Models.ViewModels
{
    public class AttendeeDetails 
    {
        public AttendeeDto Attendee { get; set; }
        public IEnumerable<EventDto> RegisteredEvents { get; set; }

        public IEnumerable<EventDto> AvailableEvents { get; set; }
    }
}
