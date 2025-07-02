namespace MovieApi.Models
{
    public class MovieDetails
    {
        public int Id { get; set; }
        public string Synopsis { get; set; } = null!;
        public string Language { get; set; } = null!;
        public decimal Budget { get; set; }

        public int MovieId { get; set; }
        public Movie Movie { get; set; } = null!;
    }
}