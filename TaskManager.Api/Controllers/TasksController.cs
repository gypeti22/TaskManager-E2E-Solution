using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using TaskManager.Api.Models;
using TaskManager.Api.Services;

namespace TaskManager.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TasksController : ControllerBase
    {
        private readonly MongoDbService _mongoService;

        public TasksController()
        {
            _mongoService = new MongoDbService();
        }

        [HttpGet]
        public ActionResult<List<TaskItem>> Get() =>
            _mongoService.Tasks.Find(task => true).ToList();

        [HttpGet("{id}")]
        public ActionResult<TaskItem> Get(string id) =>
            _mongoService.Tasks.Find(task => task.Id == id).FirstOrDefault();

        [HttpPost]
        public IActionResult Create(TaskItem task)
        {
            _mongoService.Tasks.InsertOne(task);
            return CreatedAtAction(nameof(Get), new { id = task.Id }, task);
        }

        [HttpPut("{id}")]
        public IActionResult Update(string id, TaskItem taskIn)
        {
            var filter = Builders<TaskItem>.Filter.Eq(t => t.Id, id);
            _mongoService.Tasks.ReplaceOne(filter, taskIn);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(string id)
        {
            var filter = Builders<TaskItem>.Filter.Eq(t => t.Id, id);
            _mongoService.Tasks.DeleteOne(filter);
            return NoContent();
        }
    }
}