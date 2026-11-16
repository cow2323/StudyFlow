using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using StudyFlow.Controllers;
using StudyFlow.DAL;
using StudyFlow.Models;
using StudyFlow.ViewModels;


namespace StudyFlow.Tests.Controllers;



public class StudySessionsControllerTests
{


    private const string HostId = "host-user";
    private const string OtherId = "other-user";

    private static readonly DateTime Start = DateTime.Now.AddDays(1);
    private static readonly DateTime End = Start.AddHours(2);


//Helper Methods


    private static DbContextOptions<StudyFlowDbContext> NewDbOptions() =>
        new DbContextOptionsBuilder<StudyFlowDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

    private static StudyFlowDbContext Db(DbContextOptions<StudyFlowDbContext> options) => new(options); 


    private static StudySessionController CreateController (

        
        DbContextOptions<StudyFlowDbContext> options, string? currentUserId)
    {
        //Create a controller instance
        var store = Substitute.For<IUserStore<IdentityUser>>();
        var userManager = Substitute.For<UserManager<IdentityUser>>(
            store, null, null, null, null, null, null, null, null);
        userManager.GetUserId(Arg.Any<ClaimsPrincipal>()).Returns(currentUserId);

     var context = Db(options);

    return new StudySessionController(userManager, context, new StudySessionRepository(context))

        {
            ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() }
        };
    }


     private static Booking NewBooking(
        int id = 0, int roomId = 1, DateTime? start = null, DateTime? end = null, string userId = HostId) => new()
    {
        Id = id,
        UserId = userId,
        RoomId = roomId,
        StartTime = start ?? Start,
        EndTime = end ?? End
    };


  private static StudySession NewSession(
        Booking booking, int id = 0, string topic = "Old topic", string description = "Old") => new()
    {
        Id = id,
        BookingReference = booking,
        RoomId = booking.RoomId,
        Course = "IT",
        Topic = topic,
        Description = description,
        StartTime = booking.StartTime,
        EndTime = booking.EndTime,
        IsPublic = true
    };



    private static StudySessionViewModel ValidViewModel() => new()
    {
        RoomId = 1,
        Course = "IT",
        Topic = "Algorithms",
        Description = "Exam prep",
        StartTime = Start,
        EndTime = End
    };
    


    [Fact]
    public void CreateGet_ReturnsViewWithDefaultTimesInTheFuture()
    {
        //Arrange

        var controller = CreateController(NewDbOptions(), HostId);


        //Act
        var result = controller.Create();

        //Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<StudySessionViewModel>(viewResult.Model);
        Assert.True(model.StartTime > DateTime.Now);
        Assert.True(model.EndTime > model.StartTime);
    }


[Fact]
    public async Task CreatePost_WhenStartTimeInPast_ReturnsViewWithErrorAndSavesNothing()
    {

        //Arrange
        var options = NewDbOptions();
        var controller = CreateController(options, HostId);
        var model = ValidViewModel();
        model.StartTime = DateTime.Now.AddHours(-2);
        model.EndTime = DateTime.Now.AddHours(-1);

        //Act
        var result = await controller.Create(model);

        //Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Same(model, viewResult.Model);
        Assert.True(controller.ModelState.ContainsKey(nameof(model.StartTime)));

        using var db = Db(options);
        Assert.Empty(db.Bookings);
        Assert.Empty(db.StudySessions);
    }



     [Fact]
    public async Task EditGet_WhenSessionDoesNotExist_ReturnsNotFound()
    {

        //Arrange
        var controller = CreateController(NewDbOptions(), HostId);
        //Act
        var result = await controller.Edit(123);
        //Assert
        Assert.IsType<NotFoundResult>(result);
    }


    [Fact]
    public async Task AddUser_WhenSessionIsFull_NotAllowed()
    {   //arrange 

    var options = NewDbOptions();
    var seed = Db(options);

    var room = new Room {Id = 1, Name="TestRoom", Capacity = 4, Location = "P32", Description = "Small"}; 

    seed.Rooms.Add(room);

    var session = NewSession(NewBooking(roomId: room.Id)); 

    seed.StudySessions.Add(session); 

    seed.SaveChanges();

    for (int i = 1; i <= room.Capacity + 6; i++)
        {
            
            var controller = CreateController(options, $"user{i}");
            await controller.AddUser(session.Id);
        }


        using var db = Db(options);

        Assert.Equal(room.Capacity, db.StudySessionParticipants.Count(p => p.Session.Id == session.Id)); 

        
    }

}