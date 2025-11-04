using TripApiEF.Data;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// ✅ 1. Configure CORS to allow Angular frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp",
        policy => policy
            .WithOrigins("http://localhost:4200") // Angular dev server
            .AllowAnyMethod()
            .AllowAnyHeader());
});

// ✅ 2. Configure Database Connection
builder.Services.AddDbContext<TripDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ✅ 3. Configure JSON options to handle circular references
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.WriteIndented = true;
    });

// ✅ 4. Enable Swagger (API documentation)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// ✅ 5. Enable Swagger in Development
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// ✅ 6. Enable HTTPS redirection
app.UseHttpsRedirection();

// ✅ 7. Enable CORS (must be before routing)
app.UseCors("AllowAngularApp");

// ✅ 8. Enable Authorization (if any)
app.UseAuthorization();

// ✅ 9. Map Controllers (routes)
app.MapControllers();

// ✅ 10. Run the Application
app.Run();
       