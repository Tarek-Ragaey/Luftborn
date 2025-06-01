using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Luftborn.Domain;
using Luftborn.Application.IServices.Interfaces;
using Luftborn.Application.Settings;
using Luftborn.Application.Services.Services;
using Luftborn.API.Middleware;
using Luftborn.Application.Extensions;
using Luftborn.Domain.Repositories;
using Microsoft.OpenApi.Models;
using System.Reflection;
using Luftborn.Task.Extensions;

namespace Luftborn
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            // Configure CORS Settings
            var corsSettings = Configuration.GetSection("CorsSettings").Get<CorsSettings>();
            services.Configure<CorsSettings>(Configuration.GetSection("CorsSettings"));

            // Add CORS before other service configurations
            services.AddCors(options =>
            {
                options.AddPolicy("ReactAppPolicy",
                    builder =>
                    {
                        builder
                            .WithOrigins(corsSettings.AllowedOrigins)
                            .AllowAnyMethod()
                            .AllowAnyHeader()
                            .AllowCredentials();
                    });
            });

            services.AddControllers();
            
            // Add Swagger configuration
            services.AddSwaggerConfiguration();

            // Configure DbContext
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            // Configure Identity
            services.AddIdentity<IdentityUser, IdentityRole>(options =>
            {
                // Password settings
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequiredLength = 8;

                // Lockout settings
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;

                // User settings
                options.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

            // Register repositories and services
            services.AddScoped<IArticleRepository, ArticleRepository>();
            services.AddApplicationServices();

            // Register services
            services.AddScoped<IJwtService, JwtService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<IAuthenticationService, AuthenticationService>();

            // Configure JWT Settings
            services.Configure<JwtSettings>(Configuration.GetSection("JwtSettings"));

            // Configure JWT Authentication
            var jwtSettings = Configuration.GetSection("JwtSettings").Get<JwtSettings>();
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidAudience = jwtSettings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey))
                };
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwaggerConfiguration();
            }

            app.UseHttpsRedirection();
            app.UseRouting();

            // Add CORS middleware before authentication and authorization
            app.UseCors("ReactAppPolicy");

            // Add language middleware before authentication and authorization
            app.UseLanguageMiddleware();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}