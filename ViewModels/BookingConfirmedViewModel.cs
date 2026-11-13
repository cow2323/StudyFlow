namespace StudyFlow.ViewModels;

public class BookingConfirmedViewModel
{
    public int RoomId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public int Capacity { get; set; }
}