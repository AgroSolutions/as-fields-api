using AS.Fields.Application.Publishers;
using AS.Fields.Application.Publishers.Interfaces;
using AS.Fields.Application.Services;
using AS.Fields.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace AS.Fields.Application
{
    public static class DependecyInjection
    {
        public static IServiceCollection AddApplicationModules(this IServiceCollection services)
        {
            // Publishers
            services.AddTransient<ISensorPublisher, SensorPublisher>();

            // Services
            services.AddTransient<IFieldService, FieldService>();
            services.AddTransient<IPropertyService, PropertyService>();

            return services;
        }

        public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            var jwtSettings = configuration.GetSection("JwtSettings");
            var chaveSecreta = jwtSettings["Key"];

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings["Issuer"],
                    ValidAudience = jwtSettings["Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(chaveSecreta!)),
                    ClockSkew = TimeSpan.Zero
                };
            });

            services.AddAuthorizationBuilder()
                .AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"))
                .AddPolicy("Authenticated", policy => policy.RequireAuthenticatedUser());

            return services;
        }
    }
}
