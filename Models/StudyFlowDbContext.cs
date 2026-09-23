using Microsoft.EntityFrameworkCore;

namespace StudyFlow.Models
{
    public class StudyFlowDbContext : DbContext
    {
        public StudyFlowDbContext(DbContextOptions<StudyFlowDbContext> options) : base(options)
        {
            //  REMOVE WHEN SWITCHING TO EF CORE MIGRATIONS
            Database.EnsureCreated(); //For early prototyping
            
        }

        
        
        //public DbSet<Users> Users {get; set;}
        
        }

}