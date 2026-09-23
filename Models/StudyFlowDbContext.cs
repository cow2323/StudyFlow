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

        
        
        public DbSet<User> Users {get; set;}

        public DbSet<Booking> Bookings {get; set;}
        public DbSet<Room> Rooms {get; set;}
        
        }

}