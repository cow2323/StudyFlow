using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace StudyFlow.Models
{
    public static class DbInit
    {
        public static void Seed(IApplicationBuilder app)
        {
            
            using var serviceScope = app.ApplicationServices.CreateScope(); 
            StudyFlowDbContext context = serviceScope.ServiceProvider.GetRequiredService<StudyFlowDbContext>();
            context.Database.EnsureDeleted(); 
            context.Database.EnsureCreated();

        


        if(!context.Users.Any()){

            var users = new List<User>
            {
                new User { Name = "user1", Email = "user1@users.com", PasswordHash="password" },
                 new User { Name = "user2", Email = "user2@users.com", PasswordHash="password" },
                 new User { Name = "user3", Email = "user3@users.com", PasswordHash="password" }            
                 }; 

            context.Users.AddRange(users); 
            context.SaveChanges();


        }


        if (!context.Rooms.Any())
            {
                
            
                var rooms = new List<Room>(); 

                for (int i = 1; i <= 10; i++)
                {
                    
                    int roomSize = RandomNumberGenerator.GetInt32(4);
                    int size = 0;
                    string sizeDescription = string.Empty; 

                    switch (roomSize)

                    {

                        case 0: 
                        size = 4;
                        sizeDescription = "Small Study Room";
                        break; 

                        case 1: 
                        size = 8; 
                        sizeDescription = "Medium Study Room";
                        break;

                        case 2:
                        size = 16;
                        sizeDescription = "Large Study Room";
                        break;

                        case 3: 
                        size = 32;
                        sizeDescription = "Classroom";
                        break;
                        

                    }


                    rooms.Add(new Room{
                        Name = $"Room {i}",
                        Capacity = size,
                        Location = "Pilestredet 32, Oslo",
                        Description = sizeDescription

                    });
                    
                    }

                    context.Rooms.AddRange(rooms);
                    context.SaveChanges();




                }

            
    
            }

        }
    }
        
        
        