using StudyFlow.Models;

namespace StudyFlow.DAL
{
    public interface IStudySessionRepository
    {

        Task<StudySession> GetStudySessionById(int id);
        Task<List<StudySession>> GetAllAsync();
    }
}
