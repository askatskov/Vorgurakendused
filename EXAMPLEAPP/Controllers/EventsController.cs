﻿using ITB2203Application.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Route("api/events")]
[ApiController]
public class EventsController : ControllerBase
{
    private readonly DataContext _context;

    public EventsController(DataContext context)
    {
        _context = context;
    }

    [HttpGet]
    public ActionResult<IEnumerable<Event>> GetEvents(string? name = null, string? location = null)
    {
        var query = _context.Events.AsQueryable();

        if (name != null)
            query = query.Where(x => x.Name != null && x.Name.ToUpper().Contains(name.ToUpper()));

        if (location != null)
            query = query.Where(x => x.Location != null && x.Location.ToUpper().Contains(location.ToUpper()));

        return query.ToList();
    }

    [HttpGet("{id}")]
    public ActionResult<Event> GetEvent(int id)
    {
        var @event = _context.Events!.Find(id);

        if (@event == null)
        {
            return NotFound();
        }

        return Ok(@event);
    }

    [HttpPut("{id}")]
    public IActionResult PutEvent(int id, Event @event)
    {
        var dbEvent = _context.Events.AsNoTracking().FirstOrDefault(x => x.Id == @event.Id);
        if (id != @event.Id || dbEvent == null)
        {
            return NotFound();
        }

        _context.Update(@event);
        _context.SaveChanges();

        return NoContent();
    }

    [HttpPost]
    public ActionResult<Event> PostEvent(Event @event)
    {
        var spekaer = _context.Speakers.Find(@event.SpeakerId);
        if (spekaer == null)
        {
            return NotFound();
        }

        var dbEvent = _context.Events.Find(@event.Id);
        if (dbEvent != null)
        {
            return Conflict();

        }
        _context.Add(@event);
        _context.SaveChanges();

        return CreatedAtAction(nameof(GetEvent), new {Id = @event.Id}, @event);
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteEvent(int id)
    {
        var @event = _context.Events.Find(id);
        if (@event == null)
        {
            return NotFound();
        }

        _context.Remove(@event);
        _context.SaveChanges();

        return NoContent();
    }
}
