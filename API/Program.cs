using API.Helpers.Extensions;
using Infrastructure;
using Microsoft.EntityFrameworkCore;
using System.Net;
using API.Helpers.Middleware;
using Core.Entities;
using Infrastructure.Helpers;
using Microsoft.AspNetCore.Identity;
using API.Helpers.Filters;
using Microsoft.Extensions.Options;
using Stripe;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().AddJsonOptions(options => {
  options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
});

builder.Services.AddHttpContextAccessor();

builder.Services.AddIdentityServices(builder.Configuration);
builder.Services.AddApplicationServices(builder.Configuration);

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c => {
  c.SwaggerDoc("ecommerce", new OpenApiInfo {
    Title = "E-Commerce API",
    Version = "v1"
  });

  c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme {
    In = ParameterLocation.Header,
    Description = "Please insert JWT without Bearer into field",
    Name = "Authorization",
    Type = SecuritySchemeType.ApiKey
  });

  c.AddSecurityRequirement(new OpenApiSecurityRequirement
  {
    {
      new OpenApiSecurityScheme
      {
        Reference = new OpenApiReference
        {
          Type = ReferenceType.SecurityScheme,
          Id = "Bearer"
        }
      },
      Array.Empty<string>()
    }
  });
});

var app = builder.Build();

await ApplyMigrationsAndSeedData(app.Services);

// Configure Stripe
var stripeSettings = app.Services.GetRequiredService<IOptions<StripeSettings>>().Value;
StripeConfiguration.ApiKey = stripeSettings.SecretKey;

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI(c => {
  c.SwaggerEndpoint("/swagger/ecommerce/swagger.json", "E-Commerce API v1");

  // Customize the Swagger UI
  c.UseRequestInterceptor("(request) => { request.headers.Authorization = 'Bearer ' + request.headers.Authorization; return request; }");
});

app.UseStaticFiles();

app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("CorsPolicy");

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// app.UseMiddleware<ExceptionHandlerMiddleware>();

await app.RunAsync();
return;

static async Task ApplyMigrationsAndSeedData(IServiceProvider serviceProvider) {
  // Create a scope to resolve scoped services
  using var scope = serviceProvider.CreateScope();
  var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

  // Apply migrations
  await dbContext.Database.MigrateAsync();

  // Seed data
  DbInitializer.SeedData(scope.ServiceProvider);
}
