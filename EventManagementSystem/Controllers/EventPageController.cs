using EventManagementSystem.Interfaces;
using EventManagementSystem.Models;
using EventManagementSystem.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventManagementSystem.Controllers
{
    public class EventPageController : Controller
    {
        private readonly IEventService _eventService;
        private readonly IAttendeeService _attendeeService;

        // Dependency injection for event service and attendee service
        public EventPageController(IEventService EventService, IAttendeeService AttendeeService)
        {
            _eventService = EventService;
            _attendeeService = AttendeeService;
        }

        // Show List of Events on Index page
        public IActionResult Index()
        {
            return RedirectToAction("List");
        }

        // GET: EventPage/List
        public async Task<IActionResult> List()
        {
            IEnumerable<EventDto?> eventDtos = await _eventService.ListEvents();
            return View(eventDtos);
        }

        // GET: EventPage/EventDetails/{id}
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            EventDto? EventDto = await _eventService.FindEvent(id);
            IEnumerable<AttendeeDto> RegisteredAttendees = await _eventService.ListAttendeesForEvent(id);

            if (EventDto == null)
            {
                return View("Error", new ErrorViewModel() { Errors = ["Could not find category"] });
            }
            else
            {
                // information which drives a category page
                EventDetails EventInfo = new EventDetails()
                {
                    Event = EventDto,
                    RegisteredAttendees = RegisteredAttendees
                };
                return View(EventInfo);
            }
        }

        // GET AttendeePage/New
        public ActionResult New()
        {
            return View();
        }


        // POST EventPage/Add
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(EventDto EventDto)
        {
            Models.ServiceResponse response = await _eventService.AddEvent(EventDto);

            if (response.Status == Models.ServiceResponse.ServiceStatus.Created)
            {
                return RedirectToAction("Details", "EventPage", new { id = response.CreatedId });
            }
            else
            {
                return View("Error", new ErrorViewModel() { Errors = response.Messages });
            }
        }

        //GET EventPage/Edit/{id}
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Edit(int id)
        {
            EventDto? EventDto = await _eventService.FindEvent(id);
            if (EventDto == null)
            {
                return View("Error");
            }
            else
            {
                return View(EventDto);
            }
        }

        //POST AttendeePage/Update/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Update(int id, EventDto eventDto)
        {
            if (id != eventDto.EventId)
            {
                return BadRequest("Attendee ID mismatch.");
            }

            Models.ServiceResponse response = await _eventService.UpdateEvent(id, eventDto);

            if (response.Status == Models.ServiceResponse.ServiceStatus.Updated)
            {
                return RedirectToAction("Details", "EventPage", new { id = id });
            }
            else
            {
                return View("Error", new ErrorViewModel() { Errors = response.Messages });
            }
        }

        //GET CategoryPage/ConfirmDelete/{id}
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> ConfirmDelete(int id)
        {
            EventDto? EventDto = await _eventService.FindEvent(id);
            if (EventDto == null)
            {
                return View("Error");
            }
            else
            {
                return View(EventDto);
            }
        }

        //POST EventPage/Delete/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            Models.ServiceResponse response = await _eventService.DeleteEvent(id);

            if (response.Status == Models.ServiceResponse.ServiceStatus.Deleted)
            {
                return RedirectToAction("List", "EventPage");
            }
            else
            {
                return View("Error", new ErrorViewModel() { Errors = response.Messages });
            }
        }

    }
}
