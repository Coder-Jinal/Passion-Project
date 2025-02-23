
namespace EventManagementSystem.Models
{
    public class ServiceResponse
    {
        public enum ServiceStatus { NotFound, Created, Updated, Deleted, Error, AlreadyExists, NotLinked }

        public ServiceStatus Status { get; set; }
        public AttendeeDto Payload { get; set; }

        public int? CreatedId { get; set; } 

        public List<string> Messages { get; set; } = new List<string>();
        public EventDto EventData { get; internal set; }

        public static implicit operator bool(ServiceResponse v)
        {
            throw new NotImplementedException();
        }
    }
}
