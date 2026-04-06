using Microsoft.EntityFrameworkCore;
using UrbanX.Services.Identity.Data;
using UrbanX.Services.Identity.Models;

namespace UrbanX.Services.Identity.UnitTests;

public class ApplicationDbContextTests
{
    private static DbContextOptions<ApplicationDbContext> CreateOptions(string dbName) =>
        new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: dbName)
            .Options;

    [Fact]
    public void ApplicationDbContext_ShouldConfigureApplicationUserEntity()
    {
        using var context = new ApplicationDbContext(CreateOptions("IdentityTestDb_" + Guid.NewGuid()));

        var entityType = context.Model.FindEntityType(typeof(ApplicationUser));

        Assert.NotNull(entityType);
    }

    [Fact]
    public async Task ApplicationDbContext_ShouldAddAndRetrieveUser()
    {
        var dbName = "IdentityTestDb_" + Guid.NewGuid();
        var userId = Guid.NewGuid().ToString();

        var user = new ApplicationUser
        {
            Id = userId,
            UserName = "testuser@example.com",
            NormalizedUserName = "TESTUSER@EXAMPLE.COM",
            Email = "testuser@example.com",
            NormalizedEmail = "TESTUSER@EXAMPLE.COM",
            EmailConfirmed = true,
            FullName = "Test User",
            Role = "customer",
            CreatedAt = DateTime.UtcNow
        };

        using (var context = new ApplicationDbContext(CreateOptions(dbName)))
        {
            context.Users.Add(user);
            await context.SaveChangesAsync();
        }

        using (var context = new ApplicationDbContext(CreateOptions(dbName)))
        {
            var saved = await context.Users.FindAsync(userId);
            Assert.NotNull(saved);
            Assert.Equal("testuser@example.com", saved!.Email);
            Assert.Equal("Test User", saved.FullName);
            Assert.Equal("customer", saved.Role);
        }
    }

    [Fact]
    public async Task ApplicationDbContext_ShouldRetrieveUserByEmail()
    {
        var dbName = "IdentityTestDb_" + Guid.NewGuid();
        var userId = Guid.NewGuid().ToString();
        const string email = "merchant@example.com";

        using (var context = new ApplicationDbContext(CreateOptions(dbName)))
        {
            context.Users.Add(new ApplicationUser
            {
                Id = userId,
                UserName = email,
                NormalizedUserName = email.ToUpperInvariant(),
                Email = email,
                NormalizedEmail = email.ToUpperInvariant(),
                EmailConfirmed = true,
                FullName = "Merchant User",
                Role = "merchant",
                CreatedAt = DateTime.UtcNow
            });
            await context.SaveChangesAsync();
        }

        using (var context = new ApplicationDbContext(CreateOptions(dbName)))
        {
            var found = await context.Users
                .FirstOrDefaultAsync(u => u.NormalizedEmail == email.ToUpperInvariant());

            Assert.NotNull(found);
            Assert.Equal("merchant", found!.Role);
        }
    }

    [Fact]
    public async Task ApplicationDbContext_ShouldSupportMultipleUsers()
    {
        var dbName = "IdentityTestDb_" + Guid.NewGuid();

        using (var context = new ApplicationDbContext(CreateOptions(dbName)))
        {
            context.Users.AddRange(
                new ApplicationUser
                {
                    Id = Guid.NewGuid().ToString(), UserName = "a@test.com",
                    NormalizedUserName = "A@TEST.COM", Email = "a@test.com",
                    NormalizedEmail = "A@TEST.COM", Role = "customer"
                },
                new ApplicationUser
                {
                    Id = Guid.NewGuid().ToString(), UserName = "b@test.com",
                    NormalizedUserName = "B@TEST.COM", Email = "b@test.com",
                    NormalizedEmail = "B@TEST.COM", Role = "merchant"
                }
            );
            await context.SaveChangesAsync();
        }

        using (var context = new ApplicationDbContext(CreateOptions(dbName)))
        {
            var count = await context.Users.CountAsync();
            Assert.Equal(2, count);
        }
    }
}
