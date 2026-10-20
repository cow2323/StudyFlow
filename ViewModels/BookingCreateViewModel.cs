using Microsoft.AspNetCore.Mvc.Rendering;

namespace StudyFlow.ViewModels
{
    public class BookingCreateViewModel
    {
        public int RoomId { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public List<SelectListItem> Rooms { get; set; } = new();
    }
}