using Microsoft.EntityFrameworkCore;
using MovieApi.Models;
using Bogus;

namespace MovieApi.Extensions
{
    public static class SeedDataExtension
    {
        public static void SeedData(this IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            if (!context.Movies.Any())
            {
                var actorFaker = new Faker<Actor>()
                    .RuleFor(a => a.Name, f => f.Name.FullName())
                    .RuleFor(a => a.BirthYear, f => f.Date.Past(60, DateTime.Now.AddYears(-20)).Year);

                var actors = actorFaker.Generate(10);

                var genreList = new[] { "Drama", "Comedy", "Action", "Thriller", "Sci-Fi", "Romance" };
                var movieFaker = new Faker<Movie>()
                    .RuleFor(m => m.Title, f => f.Lorem.Sentence(2, 2))
                    .RuleFor(m => m.Year, f => f.Date.Past(40, DateTime.Now).Year)
                    .RuleFor(m => m.Genre, f => f.PickRandom(genreList))
                    .RuleFor(m => m.Duration, f => f.Random.Int(80, 180))
                    .RuleFor(m => m.MovieDetails, f => new MovieDetails
                    {
                        Synopsis = f.Lorem.Paragraph(),
                        Language = f.PickRandom("English", "Swedish", "French", "Spanish"),
                        Budget = f.Random.Decimal(1_000_000, 200_000_000)
                    })
                    .RuleFor(m => m.Reviews, f => new List<Review>());

                var movies = movieFaker.Generate(10);

                var reviewFaker = new Faker<Review>()
                    .RuleFor(r => r.ReviewerName, f => f.Name.FirstName())
                    .RuleFor(r => r.Comment, f => f.Lorem.Sentence())
                    .RuleFor(r => r.Rating, f => f.Random.Int(1, 5));

                foreach (var movie in movies)
                {
                    var reviews = reviewFaker.GenerateBetween(1, 4);
                    foreach (var review in reviews)
                    {
                        review.Movie = movie;
                    }
                    movie.Reviews = reviews;
                }

                var random = new Random();
                foreach (var movie in movies)
                {
                    var actorCount = random.Next(2, 5);
                    var selectedActors = actors.OrderBy(x => random.Next()).Take(actorCount).ToList();
                    foreach (var actor in selectedActors)
                    {
                        movie.Actors.Add(actor);
                        actor.Movies.Add(movie);
                    }
                }

                context.Actors.AddRange(actors);
                context.Movies.AddRange(movies);
                context.SaveChanges();
            }
        }
    }
}