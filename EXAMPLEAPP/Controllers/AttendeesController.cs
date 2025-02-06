using ITB2203Application.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Route("api/attendees")]
[ApiController]
public class AttendeesController : ControllerBase
{
    private readonly DataContext _context;

    public AttendeesController(DataContext context)
    {
        _context = context;
    }

    [HttpGet]
    public ActionResult<IEnumerable<Attendee>> GetAttendees(string? name = null, string? email = null)
    {
        var query = _context.Attendees.AsQueryable();

        if (name != null)
            query = query.Where(x => x.Name != null && x.Name.ToUpper().Contains(name.ToUpper()));


        if (email != null)
            query = query.Where(x => x.Email != null && x.Email.ToUpper().Contains(email.ToUpper()));

        return query.ToList();
    }

    [HttpGet("{id}")]
    public ActionResult<Attendee> GetAttendee(int id)
    {
        var attendee = _context.Attendees.Find(id);

        if (attendee == null)
        {
            return NotFound();
        }

        return Ok(attendee);
    }

    [HttpPut("{id}")]
    public IActionResult PutAttendee(int id, Attendee attendee)
    {
        var dbAttendee = _context.Attendees.AsNoTracking().FirstOrDefault(x => x.ID == attendee.ID);
        if (id != attendee.ID || dbAttendee == null)
        {
            return NotFound();
        }

        _context.Update(attendee);
        _context.SaveChanges();

        return NoContent();
    }

    [HttpPost]
    public ActionResult<Attendee> PostAttendee(Attendee attendee)
    {
        if (!attendee.Email.Contains("@"))
        {
            return BadRequest("Email needs to contain an @");
        }
        var @event = _context.Events.Find(attendee.EventID);
        if (@event == null)
        {
            return NotFound("Sündmust ei leitud.");
        }

        if (attendee.RegistrationTime > @event.Date)
        {
            return BadRequest("Registreerimisaeg ei tohi olla hilisem kui sündmuse toimumisaeg.");
        }
        var existingAttendee = _context.Attendees.FirstOrDefault(a => a.Email == attendee.Email);
        if (existingAttendee != null)
        {
            return BadRequest("Ei saa lisada osalejat, kui sama emailiga osaleja on juba olemas.");
        }
        var speaker = _context.Speakers.Find(@event.SpeakerId);
        if (speaker != null && speaker.Email == attendee.Email)
        {
            return BadRequest("Ei saa lisada osalejat, kelle email on sama mis sündmuse esinejal.");
        }

        _context.Add(attendee);
        _context.SaveChanges();

        return CreatedAtAction(nameof(GetAttendee), new { Id = attendee.ID }, attendee);
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteAttendee(int id)
    {
        var attendee = _context.Attendees.Find(id);
        if (attendee == null)
        {
            return NotFound();
        }

        _context.Remove(attendee);
        _context.SaveChanges();

        return NoContent();
    }
}
