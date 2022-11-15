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

//Get only one task, by it's id
app.MapGet("/tasks/{id:int}", async (int id, AppDbContext db) => 
    await db.Tasks.FindAsync(id) is Task task ? Results.Ok(task) : Results.NotFound()
);

//Get only finished tasks
app.MapGet("/tasks/finished", async (AppDbContext db) => await db.Tasks.Where(t => t.IsFinished).ToListAsync());

//Create a new task
app.MapPost("/tasks", async(Task task, AppDbContext db) =>
{
    db.Tasks.Add(task);
    await db.SaveChangesAsync();
    return Results.Created($"/tarefas/{task.Id}", task);
});

//Update existent task
app.MapPut("/tasks/{id:int}", async (int id, Task InputTask, AppDbContext db) =>
{
    var task = await db.Tasks.FindAsync(id);
    if (task is null)
        return Results.NotFound("Task not found in the list.");

    task.Name = InputTask.Name;
    task.IsFinished = InputTask.IsFinished;
    
    await db.SaveChangesAsync();

    return Results.NoContent();
});

//Delete existent task
app.MapDelete("/tasks/{id:int}", async (int id, AppDbContext db) =>
{
    Task task = await db.Tasks.FindAsync(id);
    if (task is null)
        return Results.NotFound("Task not found in the list.");

    db.Tasks.Remove(task);
    await db.SaveChangesAsync();

    return Results.Ok(task);
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