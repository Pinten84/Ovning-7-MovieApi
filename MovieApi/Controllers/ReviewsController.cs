using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieApi.Models;
using MovieApi.DTOs;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieApi.Controllers
{
    [Route("api")]
    [ApiController]
    public class ReviewsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ReviewsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/movies/{movieId}/reviews
        [HttpGet("movies/{movieId}/reviews")]
        public async Task<ActionResult<IEnumerable<ReviewDto>>> GetReviewsForMovie(int movieId)
        {
            var reviews = await _context.Reviews
                .Where(r => r.MovieId == movieId)
                .Select(r => new ReviewDto
                {
                    Id = r.Id,
                    ReviewerName = r.ReviewerName,
                    Comment = r.Comment,
                    Rating = r.Rating
                })
                .ToListAsync();

            return Ok(reviews);
        }

        // POST: api/movies/{movieId}/reviews
        [HttpPost("movies/{movieId}/reviews")]
        public async Task<ActionResult<ReviewDto>> PostReview(int movieId, ReviewDto dto)
        {
            var movie = await _context.Movies.FindAsync(movieId);
            if (movie == null)
                return NotFound();

            var review = new Review
            {
                ReviewerName = dto.ReviewerName,
                Comment = dto.Comment,
                Rating = dto.Rating,
                MovieId = movieId
            };

            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();

            dto.Id = review.Id;
            return CreatedAtAction(nameof(GetReviewsForMovie), new { movieId = movieId }, dto);
        }

        // DELETE: api/reviews/{id}
        [HttpDelete("reviews/{id}")]
        public async Task<IActionResult> DeleteReview(int id)
        {
            var review = await _context.Reviews.FindAsync(id);
            if (review == null)
                return NotFound();

            _context.Reviews.Remove(review);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}