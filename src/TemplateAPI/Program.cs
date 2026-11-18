using TemplateAPI.Data;
using TemplateAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// Provider + connection string come from the "Database" config section.
builder.Services.AddDatabase(builder.Configuration);

builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddControllers();
builder.Services.AddProblemDetails();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Creates the database on first run. Swap for migrations (dotnet ef) when the schema starts evolving.
using (var scope = app.Services.CreateScope())
{
    scope.ServiceProvider.GetRequiredService<AppDbContext>().Database.EnsureCreated();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseExceptionHandler();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
