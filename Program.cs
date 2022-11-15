using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(option => option.UseInMemoryDatabase("taskdb"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//Get all tasks
app.MapGet("/tasks", async (AppDbContext db) => await db.Tasks.ToListAsync());
//Create a new task
app.MapPost("/tasks", async(Task task, AppDbContext db) =>
{
    db.Tasks.Add(task);
    await db.SaveChangesAsync();
    return Results.Created($"/tarefas/{task.Id}", task);
});

app.UseHttpsRedirection();

app.Run();

class Task
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public Boolean IsFinished { get; set; }
}

class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Task> Tasks => Set<Task>();
}