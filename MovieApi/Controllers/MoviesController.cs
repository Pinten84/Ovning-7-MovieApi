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
    public class MoviesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public MoviesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/movies
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MovieDto>>> GetMovies(
            [FromQuery] string? genre,
            [FromQuery] int? year,
            [FromQuery] string? actor)
        {
            var query = _context.Movies
                .Include(m => m.Actors)
                .AsQueryable();

            if (!string.IsNullOrEmpty(genre))
                query = query.Where(m => m.Genre == genre);

            if (year.HasValue)
                query = query.Where(m => m.Year == year.Value);

            if (!string.IsNullOrEmpty(actor))
                query = query.Where(m => m.Actors.Any(a => a.Name == actor));

            var movies = await query
                .Select(m => new MovieDto
                {
                    Id = m.Id,
                    Title = m.Title,
                    Year = m.Year,
                    Genre = m.Genre,
                    Duration = m.Duration
                })
                .ToListAsync();

            return Ok(movies);
        }

        // GET: api/movies/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<MovieDto>> GetMovie(int id, [FromQuery] bool withactors = false)
        {
            var query = _context.Movies.AsQueryable();

            if (withactors)
                query = query.Include(m => m.Actors);

            var movie = await query
                .Where(m => m.Id == id)
                .Select(m => new MovieDto
                {
                    Id = m.Id,
                    Title = m.Title,
                    Year = m.Year,
                    Genre = m.Genre,
                    Duration = m.Duration
                })
                .FirstOrDefaultAsync();

            if (movie == null)
                return NotFound();

            if (withactors)
            {
                var actors = await _context.Movies
                    .Where(m => m.Id == id)
                    .SelectMany(m => m.Actors)
                    .Select(a => new ActorDto
                    {
                        Id = a.Id,
                        Name = a.Name,
                        BirthYear = a.BirthYear
                    })
                    .ToListAsync();

                return Ok(new
                {
                    Movie = movie,
                    Actors = actors
                });
            }

            return Ok(movie);
        }

        // GET: api/movies/{id}/details
        [HttpGet("{id}/details")]
        public async Task<ActionResult<MovieDetailDto>> GetMovieDetails(int id)
        {
            var movie = await _context.Movies
                .Include(m => m.MovieDetails)
                .Include(m => m.Reviews)
                .Include(m => m.Actors)
                .Where(m => m.Id == id)
                .Select(m => new MovieDetailDto
                {
                    Id = m.Id,
                    Title = m.Title,
                    Year = m.Year,
                    Genre = m.Genre,
                    Duration = m.Duration,
                    MovieDetails = m.MovieDetails == null ? null : new MovieDetailsDto
                    {
                        Id = m.MovieDetails.Id,
                        Synopsis = m.MovieDetails.Synopsis,
                        Language = m.MovieDetails.Language,
                        Budget = m.MovieDetails.Budget
                    },
                    Reviews = m.Reviews.Select(r => new ReviewDto
                    {
                        Id = r.Id,
                        ReviewerName = r.ReviewerName,
                        Comment = r.Comment,
                        Rating = r.Rating
                    }).ToList(),
                    Actors = m.Actors.Select(a => new ActorDto
                    {
                        Id = a.Id,
                        Name = a.Name,
                        BirthYear = a.BirthYear
                    }).ToList()
                })
                .FirstOrDefaultAsync();

            if (movie == null)
                return NotFound();

            return Ok(movie);
        }

        // POST: api/movies
        [HttpPost]
        public async Task<ActionResult<MovieDto>> PostMovie(MovieCreateDto dto)
        {
            var movie = new Movie
            {
                Title = dto.Title,
                Year = dto.Year,
                Genre = dto.Genre,
                Duration = dto.Duration
            };

            _context.Movies.Add(movie);
            await _context.SaveChangesAsync();

            var result = new MovieDto
            {
                Id = movie.Id,
                Title = movie.Title,
                Year = movie.Year,
                Genre = movie.Genre,
                Duration = movie.Duration
            };

            return CreatedAtAction(nameof(GetMovie), new { id = movie.Id }, result);
        }

        // PUT: api/movies/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMovie(int id, MovieUpdateDto dto)
        {
            var movie = await _context.Movies.FindAsync(id);
            if (movie == null)
                return NotFound();

            movie.Title = dto.Title;
            movie.Year = dto.Year;
            movie.Genre = dto.Genre;
            movie.Duration = dto.Duration;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/movies/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMovie(int id)
        {
            var movie = await _context.Movies.FindAsync(id);
            if (movie == null)
                return NotFound();

            _context.Movies.Remove(movie);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}