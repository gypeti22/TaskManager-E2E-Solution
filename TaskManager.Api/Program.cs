using TaskManager.Api.Models;
using TaskManager.Api.Services;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

// Services
builder.Services.AddSingleton<MongoDbService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Swagger
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "TaskManager API v1");
    c.RoutePrefix = string.Empty;
});

// =====================
// CRUD ENDPOINTS
// =====================

// GET ALL
app.MapGet("/tasks", async (MongoDbService mongo) =>
{
    var tasks = await mongo.Tasks.Find(Builders<TaskItem>.Filter.Empty).ToListAsync();
    return Results.Ok(tasks);
});

// GET BY ID
app.MapGet("/tasks/{id}", async (MongoDbService mongo, string id) =>
{
    var filter = Builders<TaskItem>.Filter.Eq(t => t.Id, id);
    var task = await mongo.Tasks.Find(filter).FirstOrDefaultAsync();

    return task is null ? Results.NotFound() : Results.Ok(task);
});

// CREATE
app.MapPost("/tasks", async (MongoDbService mongo, TaskItem task) =>
{
    await mongo.Tasks.InsertOneAsync(task);
    return Results.Created($"/tasks/{task.Id}", task);
});

// UPDATE
app.MapPut("/tasks/{id}", async (MongoDbService mongo, string id, TaskItem updatedTask) =>
{
    updatedTask.Id = id;

    var filter = Builders<TaskItem>.Filter.Eq(t => t.Id, id);
    var result = await mongo.Tasks.ReplaceOneAsync(filter, updatedTask);

    return result.MatchedCount == 0 
        ? Results.NotFound() 
        : Results.Ok(updatedTask);
});

// DELETE
app.MapDelete("/tasks/{id}", async (MongoDbService mongo, string id) =>
{
    var filter = Builders<TaskItem>.Filter.Eq(t => t.Id, id);
    var result = await mongo.Tasks.DeleteOneAsync(filter);

    return result.DeletedCount == 0 
        ? Results.NotFound() 
        : Results.Ok();
});

app.Run();