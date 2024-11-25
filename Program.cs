// using Microsoft.AspNetCore.Builder;
// using Microsoft.Extensions.Configuration;
// using Microsoft.Extensions.DependencyInjection;
// using Microsoft.Extensions.Hosting;
// using Microsoft.EntityFrameworkCore;
// using MyUni.Data;
// using Microsoft.OpenApi.Models;

// var builder = WebApplication.CreateBuilder(args);

// // Add services to the container.
// builder.Services.AddControllers();
// builder.Services.AddEndpointsApiExplorer();
// builder.Services.AddSwaggerGen(c =>
// {
//     c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });

//     // JWT authorization section, if needed
//     c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
//     {
//         In = ParameterLocation.Header,
//         Description = "Please enter admin credentials in the format **'Bearer {your token}'**",
//         Name = "Authorization",
//         Type = SecuritySchemeType.ApiKey
//     });

//     c.AddSecurityRequirement(new OpenApiSecurityRequirement
//     {
//         {
//             new OpenApiSecurityScheme
//             {
//                 Reference = new OpenApiReference
//                 {
//                     Type = ReferenceType.SecurityScheme,
//                     Id = "Bearer"
//                 },
//             },
//             new string[] {}
//         }
//     });
// });

// builder.Services.AddDbContext<ApplicationDbContext>(options =>
//     options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// // Register the Emailcs service
// builder.Services.AddScoped<Emailcs>(); // Use AddSingleton or AddTransient based on your needs

// // Configure CORS
// builder.Services.AddCors(options =>
// {
//     options.AddPolicy("AllowAllOrigins",
//         policyBuilder =>
//         {
//             policyBuilder.AllowAnyOrigin()
//                          .AllowAnyMethod()
//                          .AllowAnyHeader();
//         });
// });

// // Add Authorization (optional, based on your app needs)
// builder.Services.AddAuthorization();

// // Build the app
// var app = builder.Build();

// // Configure the HTTP request pipeline
// if (app.Environment.IsDevelopment())
// {
//     app.UseSwagger();
//     app.UseSwaggerUI(c =>
//     {
//         c.InjectJavascript("/swagger-ui/custom.js"); // Inject custom JS for login
//     });
// }

// app.UseCors("AllowAllOrigins");

// app.UseHttpsRedirection();

// app.UseAuthorization();

// app.MapControllers();

// app.Run();


using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using MyUni.Data;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();

// Add Swagger configuration
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });

    // Add JWT authentication to Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Enter JWT Bearer token in the format **'Bearer {your token}'**",
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
                },
            },
            new string[] {}
        }
    });
});

// Configure Entity Framework and database connection
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Retrieve JWT settings from configuration
var jwtSettings = builder.Configuration.GetSection("Jwt"); // Access the "Jwt" section from appsettings.json

// Configure JWT authentication
builder.Services.AddAuthentication(options =>
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
        ValidateLifetime = true, // Validate token expiration
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"], // Access the "Issuer" setting
        ValidAudience = jwtSettings["Audience"], // Access the "Audience" setting
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"])), // Access the "Key" setting
        ClockSkew = TimeSpan.Zero // Prevent small timing issues on token expiration
    };
});

// Register your custom Email service (if needed)
builder.Services.AddScoped<Emailcs>(); // Use AddSingleton or AddTransient based on your service's requirements

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        policyBuilder =>
        {
            policyBuilder.AllowAnyOrigin()
                         .AllowAnyMethod()
                         .AllowAnyHeader();
        });
});

// Add authorization
builder.Services.AddAuthorization();

// Build the app
var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.InjectJavascript("/swagger-ui/custom.js"); // Optional custom JS for Swagger
    });
}

app.UseCors("AllowAllOrigins");

app.UseHttpsRedirection();

// Enable authentication and authorization middlewares
app.UseAuthentication(); // Add this before UseAuthorization
app.UseAuthorization();

app.MapControllers();

app.Run();

