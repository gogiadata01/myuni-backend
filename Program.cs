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

// // No need for JWT Bearer Authentication

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

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });

    // JWT authorization section, if needed
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter admin credentials in the format **'Bearer {your token}'**",
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

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register the Emailcs service
builder.Services.AddScoped<Emailcs>(); // Use AddSingleton or AddTransient based on your needs

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

// Add Authorization (optional, based on your app needs)
builder.Services.AddAuthorization();

// Build the app
var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.InjectJavascript("/swagger-ui/custom.js"); // Inject custom JS for login
    });
}

app.UseCors("AllowAllOrigins");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
