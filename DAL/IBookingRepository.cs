
namespace StudyFlow.DAL
{
    public interface IBookingRepository
    {
        Task<bool> HasOverlapAsync(
    int roomId,
    DateTime startTime,
    DateTime endTime);

    }
    
}
