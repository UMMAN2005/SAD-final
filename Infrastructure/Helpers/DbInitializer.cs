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

    // Seed default data
    var categories = await ReadData<Category>(env, "categories.json");
    var products = await ReadData<Product>(env, "products.json");
    var users = await ReadData<AppUser>(env, "users.json");
    var reviews = await ReadData<Review>(env, "reviews.json");

    if (!context.Categories.Any()) {
      context.Categories.AddRange(categories);
      await context.SaveChangesAsync();
    }

    if (!context.Products.Any()) {
      context.Products.AddRange(products);
      await context.SaveChangesAsync();
    }

    if (!context.Users.Any()) {
      foreach (var user in users) {
        await userManager.CreateAsync(user, "Pa$$w0rd");
      }
    }

    if (!context.Reviews.Any()) {
      context.Reviews.AddRange(reviews);
      await context.SaveChangesAsync();
    }
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