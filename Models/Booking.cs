namespace StudyFlow.Models
{
    public class Booking
    {
        public int Id { get; set; }

        // Kobler bookingen til brukeren som opprettet den.
        public int UserId { get; set; }

        // Kobler bookingen til rommet som er booket.
        public int RoomId { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Kan være null dersom brukerdata ikke er lastet inn fra databasen.
        public User? User { get; set; }

        // Kan være null dersom romdata ikke er lastet inn fra databasen.
        public Room? Room { get; set; }
    }
}