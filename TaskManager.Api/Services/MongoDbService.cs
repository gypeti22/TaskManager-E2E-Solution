using MongoDB.Driver;
using TaskManager.Api.Models;

namespace TaskManager.Api.Services;

public class MongoDbService
{
    private readonly IMongoDatabase _database;
    private IMongoCollection<TaskItem>? _tasks;

    public MongoDbService()
    {
        var client = new MongoClient("mongodb://mongodb:27017");
        _database = client.GetDatabase("TaskManagerDb");
    }

    public IMongoCollection<TaskItem> Tasks
    {
        get
        {
            if (_tasks == null)
                _tasks = _database.GetCollection<TaskItem>("Tasks");

            return _tasks;
        }
    }
}