using Microsoft.EntityFrameworkCore;
using StudyFlow.Models;

namespace StudyFlow.DAL
{
    public class BookingRepository : IBookingRepository
    {
        private readonly StudyFlowDbContext _context;

        public BookingRepository(StudyFlowDbContext context)
        {
            _context = context;
        }

        public async Task<bool> HasOverlapAsync(
            int roomId,
            DateTime startTime,
            DateTime endTime)
        {
            return await _context.Bookings.AnyAsync(b =>
                b.RoomId == roomId &&
                b.StartTime < endTime &&
                startTime < b.EndTime);
        }
    }
}
