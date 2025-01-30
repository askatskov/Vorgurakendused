using ITB2203Application.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ITB2203Application.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AttendeesController : ControllerBase
{
	private readonly DataContext _context;

	public AttendeesController(DataContext context)
	{
		_context = context;
	}

	[HttpGet]
	public ActionResult<IEnumerable<Attendee>> GetAttendees(string? name = null, int? daysBeforeEvent = null)
	{
		var query = _context.Attendees!.AsQueryable();

		if (name != null)
		{
			query = query.Where(x => x.Name != null && x.Name.ToUpper().Contains(name.ToUpper()));
		}

		if (daysBeforeEvent.HasValue)
		{
			var cutoffDate = DateTime.Now.AddDays(-daysBeforeEvent.Value);
			query = query.Where(x => x.RegistrationTime <= cutoffDate);
		}

		return query.ToList();
	}

	[HttpGet("{id}")]
	public ActionResult<Attendee> GetAttendee(int id)
	{
		var attendee = _context.Attendees!.Find(id);

		if (attendee == null)
		{
			return NotFound();
		}

		return Ok(attendee);
	}

	[HttpPut("{id}")]
	public IActionResult PutAttendee(int id, Attendee attendee)
	{
		if (id != attendee.Id)
		{
			return BadRequest("ID in the URL does not match the ID in the request body.");
		}

		if (!attendee.Email.Contains("@"))
		{
			return BadRequest("Attendee email must contain '@'.");
		}

		var eventExists = _context.Events!.Any(e => e.Id == attendee.EventId);
		if (!eventExists)
		{
			return NotFound("Event not found.");
		}

		var eventTime = _context.Events!.FirstOrDefault(e => e.Id == attendee.EventId)?.Time;
		if (eventTime != null && attendee.RegistrationTime > eventTime)
		{
			return BadRequest("Attendee registration time cannot be later than the event time.");
		}

		var duplicateAttendee = _context.Attendees!.FirstOrDefault(a => a.Email == attendee.Email && a.Id != attendee.Id);
		if (duplicateAttendee != null)
		{
			return BadRequest("Attendee with the same email already exists.");
		}

		var speaker = _context.Speakers!
			.FirstOrDefault(s => s.Id == _context.Events!.FirstOrDefault(e => e.Id == attendee.EventId)!.SpeakerId);
		if (speaker != null && speaker.Email == attendee.Email)
		{
			return BadRequest("Attendee email cannot be the same as the event speaker's email.");
		}

		_context.Entry(attendee).State = EntityState.Modified;
		_context.SaveChanges();

		return NoContent();
	}

	[HttpPost]
	public ActionResult<Attendee> PostAttendee(Attendee attendee)
	{
		if (!attendee.Email.Contains("@"))
		{
			return BadRequest("Attendee email must contain '@'.");
		}

		var eventExists = _context.Events!.Any(e => e.Id == attendee.EventId);
		if (!eventExists)
		{
			return NotFound("Event not found.");
		}

		var eventTime = _context.Events!.FirstOrDefault(e => e.Id == attendee.EventId)?.Time;
		if (eventTime != null && attendee.RegistrationTime > eventTime)
		{
			return BadRequest("Attendee registration time cannot be later than the event time.");
		}

		var duplicateAttendee = _context.Attendees!.FirstOrDefault(a => a.Email == attendee.Email);
		if (duplicateAttendee != null)
		{
			return BadRequest("Attendee with the same email already exists.");
		}

		var speaker = _context.Speakers!
			.FirstOrDefault(s => s.Id == _context.Events!.FirstOrDefault(e => e.Id == attendee.EventId)!.SpeakerId);
		if (speaker != null && speaker.Email == attendee.Email)
		{
			return BadRequest("Attendee email cannot be the same as the event speaker's email.");
		}

		_context.Attendees!.Add(attendee);
		_context.SaveChanges();

		return CreatedAtAction(nameof(GetAttendee), new { id = attendee.Id }, attendee);
	}

	[HttpDelete("{id}")]
	public IActionResult DeleteAttendee(int id)
	{
		var attendee = _context.Attendees!.Find(id);
		if (attendee == null)
		{
			return NotFound();
		}

		_context.Attendees!.Remove(attendee);
		_context.SaveChanges();

		return NoContent();
	}
}