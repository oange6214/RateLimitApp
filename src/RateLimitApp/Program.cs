using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddRateLimiter(options => 
{
    options.AddFixedWindowLimiter("FixedWindowPolicy", opt =>
    {
        opt.Window = TimeSpan.FromSeconds(5);
        opt.PermitLimit = 5;
        opt.QueueLimit = 10;
        opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
    }).RejectionStatusCode = 429; // Too many request
});

builder.Services.AddRateLimiter(options =>
{
    options.AddSlidingWindowLimiter("SlidingWindowPolicy", opt =>
    {
        opt.Window = TimeSpan.FromSeconds(10);
        opt.PermitLimit = 4;
        opt.QueueLimit = 3;
        opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        opt.SegmentsPerWindow = 3;
    }).RejectionStatusCode = 429;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRateLimiter();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
