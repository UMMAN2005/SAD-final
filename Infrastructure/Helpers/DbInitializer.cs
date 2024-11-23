using System.Text.Json;
using Core.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Helpers;

public static class DbInitializer {
  public static async Task SeedData(IServiceProvider serviceProvider) {
    var userManager = serviceProvider.GetRequiredService<UserManager<AppUser>>();
    var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var env = serviceProvider.GetRequiredService<IWebHostEnvironment>();
    var context = serviceProvider.GetRequiredService<AppDbContext>();
  }

  private static async Task<List<T>> ReadData<T>(IWebHostEnvironment env, string filePath) {
    var fullFilePath = Path.Combine(env.WebRootPath, "data", filePath);
    if (!File.Exists(fullFilePath)) return [];

    var json = await File.ReadAllTextAsync(fullFilePath);

    var data = JsonSerializer.Deserialize<List<T>>(json, new JsonSerializerOptions {
      PropertyNameCaseInsensitive = true
    });

    return data ?? [];
  }
}