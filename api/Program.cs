using API.Context;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption.ConfigurationModel;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Text;
using Microsoft.AspNetCore.DataProtection;

namespace API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddDataProtection().UseCryptographicAlgorithms(
                new AuthenticatedEncryptorConfiguration
                {
                    EncryptionAlgorithm = EncryptionAlgorithm.AES_256_CBC,
                    ValidationAlgorithm = ValidationAlgorithm.HMACSHA256
                });

            IConfiguration Configuration = builder.Configuration;
            var Connection = Configuration.GetConnectionString("DefaultConnection") ?? Environment.GetEnvironmentVariable("DefaultConnection");
            var issuer = Configuration["JwtSettings:Issuer"] ?? Environment.GetEnvironmentVariable("Issuer");
            var audience = Configuration["JwtSettings:Audience"] ?? Environment.GetEnvironmentVariable("Audience");
            var key = Configuration["JwtSettings:Key"] ?? Environment.GetEnvironmentVariable("Key");

            string connectionString = Configuration.GetConnectionString("DefaultConnection")
            ?? Environment.GetEnvironmentVariable("DefaultConnection");

            builder.Services.AddDbContext<AppDBContext>(options =>
                    options.UseNpgsql(connectionString));

            // Configure CORS policy
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll",
                    policyBuilder =>
                    {
                       
                        policyBuilder.AllowAnyOrigin() 
                                     .AllowAnyMethod()
                                     .AllowAnyHeader();
                    });
            });

            // Configure JWT Authentication
            builder.Services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(x =>
            {
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidIssuer = Configuration["JwtSettings:Issuer"] ?? Environment.GetEnvironmentVariable("Issuer"),
                    ValidAudience = Configuration["JwtSettings:Audience"] ?? Environment.GetEnvironmentVariable("Audience"),
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JwtSettings:Key"] ?? Environment.GetEnvironmentVariable("Key"))),
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true
                };
            });

            var app = builder.Build();

            app.UseHttpsRedirection();
            app.UseCors("AllowAll");

            // Configure the HTTP request pipeline.
            app.UseSwagger();
            app.UseSwaggerUI();


            // Apply Authentication & Authorization
            app.UseAuthentication();
            app.UseAuthorization();

            // Map endpoints
            app.MapGet("/", () => Results.Ok("API is running"));
            app.MapControllers();

            app.Run();
        }
    }
}
