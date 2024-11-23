using API.Helpers.Extensions;
using Infrastructure;
using Microsoft.EntityFrameworkCore;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(serverOptions => {
  serverOptions.Listen(IPAddress.Any, 5000);
  serverOptions.Listen(IPAddress.Any, 5001, listenOptions => {
    listenOptions.UseHttps();
  });
});

builder.Services.AddControllers().AddJsonOptions(options =>
{
  options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
});

builder.Services.AddHttpContextAccessor();

builder.Services.AddIdentityServices(builder.Configuration);
builder.Services.AddApplicationServices(builder.Configuration);

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
  c.SwaggerDoc("ecommerce", new OpenApiInfo
  {
    Title = "E-Commerce API",
    Version = "v1"
  });

  c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
  {
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

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI(c =>
  {
    c.SwaggerEndpoint("/swagger/ecommerce/swagger.json", "E-Commerce API v1");

    // Customize the Swagger UI
    c.UseRequestInterceptor("(request) => { request.headers.Authorization = 'Bearer ' + request.headers.Authorization; return request; }");
  });
}

app.UseStaticFiles();

app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("CorsPolicy");

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

using var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;
var context = services.GetRequiredService<AppDbContext>();
var userManager = services.GetRequiredService<ILogger<Program>>();
try
{
  await context.Database.MigrateAsync();
  // await identityContext.Database.MigrateAsync();
  //  await StoreContextSeed.SeedAsync(context);
  //await AppIdentityDbContextSeed.SeedUsersAsync(userManager);
}
catch (Exception ex)
{
  Console.WriteLine("An error occured during migration");
  Console.WriteLine(ex.Message);
}

app.Run();

