
using Microsoft.AspNetCore.Mvc;
using StudyFlow.Controllers;
using StudyFlow.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using NSubstitute;
using System.Security.Claims;


namespace StudyFlow.Tests.Controllers;

public class BookingControllerTests
{
    // Her skal vi skrive testene våre
    [Fact]
    public void Create_WhenRoomIdIsMissing_RedirectsToRoomIndex()
    {
        // Arrange
        var controller = new BookingController(null!, null!, null!);

        // Act
        var result = controller.Create((int?)null);

        // Assert
        var redirect = Assert.IsType<RedirectToActionResult>(result);

        Assert.Equal("Index", redirect.ActionName);
        Assert.Equal("Room", redirect.ControllerName);
    }

    [Fact]
    public void Create_WhenRoomIdIsValid_ReturnsViewWithRoomId()
    {
        // Arrange
        var controller = new BookingController(null!, null!, null!);
        int roomId = 101;

        // Act
        var result = controller.Create(roomId);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<BookingCreateViewModel>(
            viewResult.Model);

        Assert.Equal(roomId, model.RoomId);
    }

    [Fact]
    public async Task Create_WhenEndTimeBeforeStartTime_ReturnsViewWithError()

    {
        // Arrange

        var startTime = DateTime.Now.AddDays(1);

        var model = new BookingCreateViewModel
        {
            RoomId = 101,
            StartTime = startTime,
            EndTime = startTime.AddHours(-1)
        };
        var testUser = new ClaimsPrincipal(
              new ClaimsIdentity(
                  new[]
                  {
                new Claim(ClaimTypes.NameIdentifier, "test-user-1")
                  },
                  "TestAuthentication"
              )
          );

        var mockUserStore =
            Substitute.For<IUserStore<IdentityUser>>();

        var userManager = new UserManager<IdentityUser>(
            mockUserStore,
            Options.Create(new IdentityOptions()),
            null!,
            Array.Empty<IUserValidator<IdentityUser>>(),
            Array.Empty<IPasswordValidator<IdentityUser>>(),
            null!,
            new IdentityErrorDescriber(),
            null!,
            null!
        );

        var controller = new BookingController(
            userManager,
            null!,
            null!
        );

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = testUser
            }
        };

        // Act
        var result = await controller.Create(model);

        // Assert
         var viewResult = Assert.IsType<ViewResult>(result);

        Assert.Same(model, viewResult.Model);

        Assert.False(controller.ModelState.IsValid);

        Assert.True(controller.ModelState.ContainsKey(
            nameof(BookingCreateViewModel.EndTime)));
    }


}

