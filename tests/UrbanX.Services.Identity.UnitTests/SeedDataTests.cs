using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using UrbanX.Services.Identity.Data;
using UrbanX.Services.Identity.Models;

namespace UrbanX.Services.Identity.UnitTests;

public class SeedDataTests
{
    private static IServiceProvider BuildServiceProvider(string dbName)
    {
        var services = new ServiceCollection();

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseInMemoryDatabase(dbName));

        services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Password.RequiredLength = 6;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireDigit = false;
                options.Password.RequireNonAlphanumeric = false;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        services.AddLogging(b => b.AddDebug());

        return services.BuildServiceProvider();
    }

    [Fact]
    public async Task EnsureSeedDataAsync_CreatesDefaultRoles()
    {
        var provider = BuildServiceProvider("SeedTest_Roles_" + Guid.NewGuid());

        await SeedData.EnsureSeedDataAsync(provider);

        using var scope = provider.CreateScope();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        Assert.True(await roleManager.RoleExistsAsync("admin"));
        Assert.True(await roleManager.RoleExistsAsync("customer"));
        Assert.True(await roleManager.RoleExistsAsync("merchant"));
    }

    [Fact]
    public async Task EnsureSeedDataAsync_CreatesDefaultUsers()
    {
        var provider = BuildServiceProvider("SeedTest_Users_" + Guid.NewGuid());

        await SeedData.EnsureSeedDataAsync(provider);

        using var scope = provider.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        var admin = await userManager.FindByEmailAsync("admin@urbanx.com");
        var customer = await userManager.FindByEmailAsync("customer@urbanx.com");
        var merchant = await userManager.FindByEmailAsync("merchant@urbanx.com");

        Assert.NotNull(admin);
        Assert.NotNull(customer);
        Assert.NotNull(merchant);
    }

    [Fact]
    public async Task EnsureSeedDataAsync_AssignsCorrectRolesToUsers()
    {
        var provider = BuildServiceProvider("SeedTest_Assignments_" + Guid.NewGuid());

        await SeedData.EnsureSeedDataAsync(provider);

        using var scope = provider.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        var admin = await userManager.FindByEmailAsync("admin@urbanx.com");
        var customer = await userManager.FindByEmailAsync("customer@urbanx.com");
        var merchant = await userManager.FindByEmailAsync("merchant@urbanx.com");

        Assert.Contains("admin", await userManager.GetRolesAsync(admin!));
        Assert.Contains("customer", await userManager.GetRolesAsync(customer!));
        Assert.Contains("merchant", await userManager.GetRolesAsync(merchant!));
    }

    [Fact]
    public async Task EnsureSeedDataAsync_IsIdempotent()
    {
        var provider = BuildServiceProvider("SeedTest_Idempotent_" + Guid.NewGuid());

        // Run twice – should not throw or create duplicates
        await SeedData.EnsureSeedDataAsync(provider);
        await SeedData.EnsureSeedDataAsync(provider);

        using var scope = provider.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var userCount = await context.Users.CountAsync();
        Assert.Equal(3, userCount);
    }

    [Fact]
    public async Task EnsureSeedDataAsync_SeedUsersHaveEmailConfirmed()
    {
        var provider = BuildServiceProvider("SeedTest_EmailConfirmed_" + Guid.NewGuid());

        await SeedData.EnsureSeedDataAsync(provider);

        using var scope = provider.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        var admin = await userManager.FindByEmailAsync("admin@urbanx.com");
        Assert.NotNull(admin);
        Assert.True(admin!.EmailConfirmed);
    }

    [Fact]
    public async Task EnsureSeedDataAsync_SeedUsersHaveCorrectRole()
    {
        var provider = BuildServiceProvider("SeedTest_RoleField_" + Guid.NewGuid());

        await SeedData.EnsureSeedDataAsync(provider);

        using var scope = provider.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        var admin = await userManager.FindByEmailAsync("admin@urbanx.com");
        var customer = await userManager.FindByEmailAsync("customer@urbanx.com");
        var merchant = await userManager.FindByEmailAsync("merchant@urbanx.com");

        Assert.Equal("admin", admin!.Role);
        Assert.Equal("customer", customer!.Role);
        Assert.Equal("merchant", merchant!.Role);
    }
}
