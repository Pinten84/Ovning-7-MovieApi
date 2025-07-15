namespace MovieApi.DTOs
{
    public class MovieActorCreateDto
    {
        public int ActorId { get; set; }
        public string Role { get; set; } = null!;
    }
}