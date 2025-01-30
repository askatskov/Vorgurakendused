using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WorkoutApplication.Model;

namespace WorkoutApplication.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExercisesController : ControllerBase
    {
        private readonly DataContext _context;

        public ExercisesController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetExercises()
        {
            return Ok(_context.ExerciseList);
        }
    }
}