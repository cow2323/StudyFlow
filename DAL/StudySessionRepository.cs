using Microsoft.EntityFrameworkCore;
using StudyFlow.Models;

namespace StudyFlow.DAL
{
    public class StudySessionRepository : IStudySessionRepository
    {
        private readonly StudyFlowDbContext _context;

        public StudySessionRepository(StudyFlowDbContext context)
        {
            _context = context;
        }


        public async Task<StudySession> GetStudySessionById(int id)
        {
            return await _context.StudySessions.FindAsync(id); 
        }

        public async Task<List<StudySession>> GetAllAsync()
        {

            return await _context.StudySessions
                .Include(s => s.Participants) //loads participants
                .Include(s => s.Room) // loads the Room so Name/Capacity are available in the view
                .OrderBy(s => s.StartTime)
                .ToListAsync();
        }
    }
}
