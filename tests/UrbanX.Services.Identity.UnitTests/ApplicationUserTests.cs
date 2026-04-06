using UrbanX.Services.Identity.Models;

namespace UrbanX.Services.Identity.UnitTests;

public class ApplicationUserTests
{
    [Fact]
    public void ApplicationUser_DefaultCreatedAt_IsUtcNow()
    {
        var before = DateTime.UtcNow;
        var user = new ApplicationUser();
        var after = DateTime.UtcNow;

        Assert.InRange(user.CreatedAt, before, after);
    }

    [Fact]
    public void ApplicationUser_CanSetFullName()
    {
        var user = new ApplicationUser { FullName = "John Doe" };

        Assert.Equal("John Doe", user.FullName);
    }

    [Fact]
    public void ApplicationUser_CanSetRole_Customer()
    {
        var user = new ApplicationUser { Role = "customer" };

        Assert.Equal("customer", user.Role);
    }

    [Fact]
    public void ApplicationUser_CanSetRole_Merchant()
    {
        var user = new ApplicationUser { Role = "merchant" };

        Assert.Equal("merchant", user.Role);
    }

    [Fact]
    public void ApplicationUser_CanSetRole_Admin()
    {
        var user = new ApplicationUser { Role = "admin" };

        Assert.Equal("admin", user.Role);
    }

    [Fact]
    public void ApplicationUser_RoleIsNullByDefault()
    {
        var user = new ApplicationUser();

        Assert.Null(user.Role);
    }

    [Fact]
    public void ApplicationUser_FullNameIsNullByDefault()
    {
        var user = new ApplicationUser();

        Assert.Null(user.FullName);
    }

    [Fact]
    public void ApplicationUser_InheritsFromIdentityUser()
    {
        var user = new ApplicationUser
        {
            UserName = "test@example.com",
            Email = "test@example.com"
        };

        Assert.Equal("test@example.com", user.UserName);
        Assert.Equal("test@example.com", user.Email);
    }
}
