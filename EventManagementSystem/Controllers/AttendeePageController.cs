using EventManagementSystem.Interfaces;
using EventManagementSystem.Models;
using EventManagementSystem.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EventManagementSystem.Controllers
{
    public class AttendeePageController : Controller
    {
        private readonly IAttendeeService _attendeeService;
        private readonly IEventService _eventService;

        // Dependency injection for attendee service and event service
        public AttendeePageController(IAttendeeService AttendeeService, IEventService EventService)
        {
            _attendeeService = AttendeeService;
            _eventService = EventService;
        }

        // Show List of Attendees on Index page 
        public IActionResult Index()
        {
            return RedirectToAction("List");
        }

        // GET: AttendeePage/List
        public async Task<IActionResult> List()
        {
            IEnumerable<AttendeeDto?> AttendeeDtos = await _attendeeService.ListAttendees();
            return View(AttendeeDtos);
        }


        // GET: AttendeePage/AttendeeDetails/{id}
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            AttendeeDto? attendeeDto = await _attendeeService.FindAttendee(id);
            if (attendeeDto == null)
            {
                return View("Error", new ErrorViewModel { Errors = ["Could not find attendee"] });
            }

            IEnumerable<EventDto> registeredEvents = await _attendeeService.ListEventsForAttendee(id);
            IEnumerable<EventDto> allEvents = await _eventService.ListEvents();

            IEnumerable<EventDto> availableEvents = allEvents
                .Where(e => !registeredEvents.Any(re => re.EventId == e.EventId));

            if (!availableEvents.Any())
            {
                // Log to the console or debug output
                Console.WriteLine("No available events to display.");
            }

            var attendeeDetails = new AttendeeDetails
            {
                Attendee = attendeeDto,
                RegisteredEvents = registeredEvents,
                AvailableEvents = availableEvents
            };

            ViewBag.EventDropdown = new SelectList(availableEvents, "EventId", "EventName");

            return View(attendeeDetails);
        }

        // POST: AttendeePage/LinkForEvent
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LinkEvent(int attendeeId, int eventId)
        {
            var response = await _attendeeService.LinkAttendeeToEvent(attendeeId, eventId);

            if (response.Status == Models.ServiceResponse.ServiceStatus.Created)
            {
                return RedirectToAction("Details", new { id = attendeeId });
            }

            return View("Error", new ErrorViewModel { Errors = response.Messages });
        }


        // POST: AttendeePage/UnlinkEvent
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UnlinkEvent(int attendeeId, int eventId)
        {
            var response = await _attendeeService.UnlinkAttendeeFromEvent(attendeeId, eventId);

            if (response.Status == Models.ServiceResponse.ServiceStatus.Deleted)
            {
                return RedirectToAction("Details", new { id = attendeeId });
            }

            return View("Error", new ErrorViewModel { Errors = response.Messages });
        }

        // GET AttendeePage/New
        public ActionResult New()
        {
            return View();
        }


        // POST AttendeePage/Add
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(AttendeeDto AttendeeDto)
        {
            Models.ServiceResponse response = await _attendeeService.AddAttendee(AttendeeDto);

            if (response.Status == Models.ServiceResponse.ServiceStatus.Created)
            {
                return RedirectToAction("Details", "AttendeePage", new { id = response.CreatedId });
            }
            else
            {
                return View("Error", new ErrorViewModel() { Errors = response.Messages });
            }
        }

        //GET AttendeePage/Edit/{id}
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Edit(int id)
        {
            AttendeeDto? AttendeeDto = await _attendeeService.FindAttendee(id);
            if (AttendeeDto == null)
            {
                return View("Error");
            }
            else
            {
                return View(AttendeeDto);
            }
        }

        //POST AttendeePage/Update/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Update(int id, AttendeeDto attendeeDto)
        {
            if (id != attendeeDto.AttendeeId)
            {
                return BadRequest("Attendee ID mismatch.");
            }

            Models.ServiceResponse response = await _attendeeService.UpdateAttendee(id, attendeeDto);

            if (response.Status == Models.ServiceResponse.ServiceStatus.Updated)
            {
                return RedirectToAction("Details", "AttendeePage", new { id = id });
            }
            else
            {
                return View("Error", new ErrorViewModel() { Errors = response.Messages });
            }
        }


        //GET AttendeePage/ConfirmDelete/{id}
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> ConfirmDelete(int id)
        {
            AttendeeDto? AttendeeDto = await _attendeeService.FindAttendee(id);
            if (AttendeeDto == null)
            {
                return View("Error");
            }
            else
            {
                return View(AttendeeDto);
            }
        }

        //POST AttendeePage/Delete/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            Models.ServiceResponse response = await _attendeeService.DeleteAttendee(id);

            if (response.Status == Models.ServiceResponse.ServiceStatus.Deleted)
            {
                return RedirectToAction("List", "AttendeePage");
            }
            else
            {
                return View("Error", new ErrorViewModel() { Errors = response.Messages });
            }
        }

        
    }
}


