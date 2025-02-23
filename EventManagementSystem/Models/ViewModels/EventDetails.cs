namespace EventManagementSystem.Models.ViewModels
{
    public class EventDetails
    {
        public EventDto Event { get; set; }
        public IEnumerable<AttendeeDto> RegisteredAttendees { get; set; }
    }
}
