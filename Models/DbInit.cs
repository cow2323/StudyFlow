using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Identity;
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

            UserManager<IdentityUser> userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

            // Seed users
            if (!context.Users.Any())
            {
                var users = new List<IdentityUser>
                {
                    new IdentityUser { UserName = "user1@users.com", Email = "user1@users.com", EmailConfirmed = true },
                    new IdentityUser { UserName = "user2@users.com", Email = "user2@users.com", EmailConfirmed = true },
                    new IdentityUser { UserName = "user3@users.com", Email = "user3@users.com", EmailConfirmed = true }
                };

                foreach (var user in users)
                {
                    // CreateAsync hashes the password and saves the user
                    var result = userManager.CreateAsync(user, "Password123!").Result;

                    if (!result.Succeeded)
                    {
                        throw new Exception($"Failed to create user {user.UserName}: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                    }
                }
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
    
    
    

        
        
        