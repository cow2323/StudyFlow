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

    }
}