using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using StudyFlow.Models;


namespace StudyFlow.Models
{
    public class StudyFlowDbContext : IdentityDbContext
    {
        public StudyFlowDbContext(DbContextOptions<StudyFlowDbContext> options) : base(options)
        {
            
            Database.EnsureCreated(); // early prototyping

     
            
        }

        
        
        //public DbSet<User> Users {get; set;}

        public DbSet<Booking> Bookings {get; set;}
        public DbSet<StudySession> StudySessions { get; set; }

        
        public DbSet<Room> Rooms {get; set;}
        
        }

}