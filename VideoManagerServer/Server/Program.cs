using Microsoft.Extensions.Options;
using VideoManager;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddSingleton<GlobalState>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowVite",
    policy =>
    {
        policy.WithOrigins("http://localhost:5173")
        .AllowAnyHeader()
        .AllowAnyMethod();
    });
});
var app = builder.Build();
app.MapControllers();
app.UseCors("AllowVite");
var globalState = app.Services.GetRequiredService<GlobalState>();
globalState.Repo = await VideoRepository.CreateAsync();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.Run();
