using API.Dtos;
using API.Helpers.Profiles;
using AutoMapper;
using Core.Entities;
using Core.Interfaces.Repositories;
using Core.Interfaces.Services;
using FluentValidation;
using FluentValidation.AspNetCore;
using Infrastructure;
using Infrastructure.Helpers;
using Infrastructure.Implementations.Repositories;
using Infrastructure.Implementations.Services;
using MicroElements.Swashbuckle.FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;

namespace API.Helpers.Extensions;

public static class ApplicationServicesExtensions {
  public static IServiceCollection AddApplicationServices(this IServiceCollection services,
      IConfiguration config) {
    services.AddDbContext<AppDbContext>(opt => {
      opt.UseNpgsql(config.GetConnectionString("DefaultConnection"), b => b.MigrationsAssembly("API"));
    });

    services.AddIdentity<AppUser, IdentityRole>(opt => {
      opt.Password.RequireNonAlphanumeric = false;
      opt.Password.RequiredLength = 8;
      opt.Password.RequireUppercase = false;
      opt.Password.RequireLowercase = false;
      opt.Password.RequireDigit = false;
      opt.User.RequireUniqueEmail = true;
    }).AddDefaultTokenProviders().AddEntityFrameworkStores<AppDbContext>();

    services.AddSingleton<IConnectionMultiplexer>(c => {
      var options = ConfigurationOptions.Parse(config.GetConnectionString("RedisConnection") ?? string.Empty);
      return ConnectionMultiplexer.Connect(options);
    });

    //Fluent Validation
    services.AddFluentValidationAutoValidation();
    services.AddFluentValidationClientsideAdapters();
    services.AddValidatorsFromAssemblyContaining<CategoryPostDtoValid>();

    //Custom Services
    services.AddScoped<ICategoryRepository, CategoryRepository>();
    services.AddScoped<IProductRepository, ProductRepository>();
    services.AddScoped<IReviewRepository, ReviewRepository>();
    services.AddScoped<IOrderItemRepository, OrderItemRepository>();
    services.AddScoped<IOrderRepository, OrderRepository>();
    services.AddScoped<ICardRepository, CardRepository>();
    services.AddScoped<IPaymentRepository, PaymentRepository>();

    services.AddScoped<IEmailSenderService, EmailSenderService>();

    services.AddLogging();

    // Micro-elements
    services.AddFluentValidationRulesToSwagger();

    services.AddSingleton(provider => new MapperConfiguration(cfg => {
      var accessor = provider.GetRequiredService<IHttpContextAccessor>();
      var configuration = provider.GetRequiredService<IConfiguration>();

      cfg.AddProfile(new MappingProfile(accessor, configuration));
    }).CreateMapper());

    services.Configure<ApiBehaviorOptions>(options => {
      options.InvalidModelStateResponseFactory = actionContext => {
        var errors = actionContext.ModelState.Where(x => x.Value!.Errors.Count > 0)
          .Select(x => new RestExceptionError(x.Key, x.Value!.Errors.First().ErrorMessage)).ToList();
        return new BadRequestObjectResult(new { message = "", errors });
      };
    });

    services.AddCors(opt => {
      opt.AddPolicy("CorsPolicy", policy => {
        policy.AllowAnyOrigin()
          .AllowAnyMethod()
          .AllowAnyHeader();
      });
    });

    return services;
  }
}