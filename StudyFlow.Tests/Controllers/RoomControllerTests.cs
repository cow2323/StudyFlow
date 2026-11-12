using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudyFlow.Controllers;
using StudyFlow.Models;

namespace StudyFlow.Tests.Controllers;

public class RoomControllerTests
{
    private StudyFlowDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<StudyFlowDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new StudyFlowDbContext(options);
    }

    [Fact]
    public async Task Index_ReturnsRoomsInView()
    {
        // Arrange
        var context = CreateContext();

        context.Rooms.AddRange(
            new Room
            {
                Id = 1,
                Name = "Room B",
                Location = "Building A",
                Capacity = 4
            },
            new Room
            {
                Id = 2,
                Name = "Room A",
                Location = "Building B",
                Capacity = 6
            }
        );

        await context.SaveChangesAsync();

        var controller = new RoomController(context);

        // Act
        var result = await controller.Index();

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);

        var rooms = Assert.IsAssignableFrom<IEnumerable<Room>>(
            viewResult.Model
        );

        Assert.Equal(2, rooms.Count());
        Assert.Equal("Room A", rooms.First().Name);
    }

    [Fact]
    public async Task Details_WithValidId_ReturnsRoom()
    {
        // Arrange
        var context = CreateContext();

        var room = new Room
        {
            Id = 1,
            Name = "Room 1",
            Location = "Building A",
            Capacity = 8
        };

        context.Rooms.Add(room);
        await context.SaveChangesAsync();

        var controller = new RoomController(context);

        // Act
        var result = await controller.Details(1);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<Room>(viewResult.Model);

        Assert.Equal(1, model.Id);
        Assert.Equal("Room 1", model.Name);
    }

    [Fact]
    public async Task Details_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var context = CreateContext();
        var controller = new RoomController(context);

        // Act
        var result = await controller.Details(0);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }
}