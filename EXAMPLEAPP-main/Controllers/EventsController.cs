using ITB2203Application.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ITB2203Application.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EventsController : ControllerBase
{
    private readonly DataContext _context;

    public EventsController(DataContext context)
    {
        _context = context;
    }

    [HttpGet]
    public ActionResult<IEnumerable<Event>> GetEvents(string? name = null)
    {
        var query = _context.Events!.AsQueryable();

        if (name == null)
            query = query.Where(x => x.Name != null && x.Name.ToUpper().Contains(name.ToUpper()));

        return query.ToList();
    }

    [HttpGet("{id}")]
    public ActionResult<TextReader> GetEvent(int id)
   {
        var events = _context.Events!.Find(id);

        if (events == null)
        {
            return NotFound();
        }

        return Ok(events);
    }

    [HttpPut("{id}")]
    public IActionResult PutEvent(int id, Event events)
    {
        var dbEvent = _context.Events!.AsNoTracking().FirstOrDefault(x => x.Id == events.Id);
        if (id != events.Id || dbEvent == null)
        {
            return NotFound();
        }

        _context.Update(events);
        _context.SaveChanges();

        return NoContent();
    }

    [HttpPost]
    public ActionResult<Event> PostEvent(Event events)
    {
        var dbExercise = _context.Events!.Find(events.Id);
        if (dbExercise == null)
        {
            _context.Add(events);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetEvent), new { Id = events.Id }, events);
        }
        else
        {
            return Conflict();
        }
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteEvent(int id)
    {
        var events = _context.Events!.Find(id);
        if (events == null)
        {
            return NotFound();
        }
    
        _context.Remove(events);
        _context.SaveChanges();

        return NoContent();
    }
}
