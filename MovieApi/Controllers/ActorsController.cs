using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieApi.Models;
using MovieApi.DTOs;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ActorsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ActorsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/actors
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ActorDto>>> GetActors()
        {
            var actors = await _context.Actors
                .Select(a => new ActorDto
                {
                    Id = a.Id,
                    Name = a.Name,
                    BirthYear = a.BirthYear
                })
                .ToListAsync();

            return Ok(actors);
        }

        // GET: api/actors/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<ActorDto>> GetActor(int id)
        {
            var actor = await _context.Actors
                .Where(a => a.Id == id)
                .Select(a => new ActorDto
                {
                    Id = a.Id,
                    Name = a.Name,
                    BirthYear = a.BirthYear
                })
                .FirstOrDefaultAsync();

            if (actor == null)
                return NotFound();

            return Ok(actor);
        }

        // POST: api/actors
        [HttpPost]
        public async Task<ActionResult<ActorDto>> PostActor([FromBody] ActorCreateDto dto)
        {
            // [ApiController] + valideringsattribut ger automatiskt 400 BadRequest vid fel

            var actor = new Actor
            {
                Name = dto.Name,
                BirthYear = dto.BirthYear
            };

            _context.Actors.Add(actor);
            await _context.SaveChangesAsync();

            var result = new ActorDto
            {
                Id = actor.Id,
                Name = actor.Name,
                BirthYear = actor.BirthYear
            };

            return CreatedAtAction(nameof(GetActor), new { id = actor.Id }, result);
        }

        // PUT: api/actors/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> PutActor(int id, [FromBody] ActorUpdateDto dto)
        {
            if (id != dto.Id)
                return BadRequest();

            var actor = await _context.Actors.FindAsync(id);
            if (actor == null)
                return NotFound();

            actor.Name = dto.Name;
            actor.BirthYear = dto.BirthYear;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // POST: api/movies/{movieId}/actors/{actorId}
        [HttpPost("/api/movies/{movieId}/actors/{actorId}")]
        public async Task<IActionResult> AddActorToMovie(int movieId, int actorId)
        {
            var movie = await _context.Movies.Include(m => m.Actors).FirstOrDefaultAsync(m => m.Id == movieId);
            var actor = await _context.Actors.FindAsync(actorId);

            if (movie == null || actor == null)
                return NotFound();

            if (!movie.Actors.Any(a => a.Id == actorId))
                movie.Actors.Add(actor);

            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}