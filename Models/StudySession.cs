namespace StudyFlow.Models
{
    public class StudySession
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int RoomId { get; set; }
        public string Course { get; set; } = string.Empty;
        public string Topic { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public bool IsPublic { get; set; } = true;

        public User? User { get; set; }
        public Room? Room { get; set; }
    }
}