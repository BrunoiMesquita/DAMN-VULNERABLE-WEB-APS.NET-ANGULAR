using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using glubfish.Data;
using Microsoft.AspNetCore.Authorization;
using glubfish.Model;
using MongoDB.Driver;
using glubfish.Helpers;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace glubfish.Controllers
{
    public class TodoController : Controller
    {
        private MongoContext _db { get; set; }
        private UserHelper _userHelper { get; set; }
        public TodoController(MongoContext mongoContext, UserHelper userHelper)
        {
            _db = mongoContext;
            _userHelper = userHelper;
        }


        [HttpGet]
        [Route("api/v1/users/{id}/todo")]
        [Authorize]
        public IActionResult GetUserTodoItems(string id)
        {
            User user = _userHelper.GetUserById(User.Identity.Name);

            //You must be logged in as the user specified in the URL
            if (id != user?.Id)
                return BadRequest("Invalid Permissions");

            List<Todo> list = _db.Todos.Find(t => t.Owner == user.Id).ToList();

            return Ok(list);
        }

        [HttpPost]
        [Authorize]
        [Route("api/v1/users/{id}/todo")]
        public IActionResult CreateItem(string id, [FromBody]TodoItemViewModel model)
        {
            if (ModelState.IsValid)
            {
                User user = _userHelper.GetUserById(User.Identity.Name);

                //You must be logged in as the user specified in the URL
                if (id != user?.Id)
                    return BadRequest("Invalid Permissions");

                if (model.Task == null || model.Task.Trim() == "")
                  return BadRequest();

                Todo item = new Todo()
                {
                    Owner = user.Id,
                    Completed = false,
                    Task = model.Task
                };

                _db.Todos.InsertOne(item);
                return Ok(item);
            }
            else
                return BadRequest();
        }

        [HttpPut]
        [Authorize]
        [Route("api/v1/users/{userId}/todo/{itemId}/status/{completed}")]
        public IActionResult CompleteItem(string userId, string itemId, bool completed)
        {
            if (ModelState.IsValid)
            {
                User user = _userHelper.GetUserById(User.Identity.Name);

                //You must be logged in as the user specified in the URL
                if (userId != user?.Id)
                    return BadRequest("Invalid Permissions");

                Todo item = _db.Todos.Find(t => t.Owner == userId && t.Id == itemId).FirstOrDefault();

                if (item == null)
                    return NotFound();

                var update = Builders<Todo>.Update.Set(t => t.Completed, completed);
                _db.Todos.UpdateOne(t => t.Id == itemId, update);

                return Ok();
            }
            else
                return BadRequest();
        }

        [HttpDelete]
        [Authorize]
        [Route("api/v1/users/{userId}/todo/{itemId}")]
        public IActionResult DeleteItem(string userId, string itemId)
        {
            if (ModelState.IsValid)
            {
                User user = _userHelper.GetUserById(User.Identity.Name);

                //You must be logged in as the user specified in the URL
                if (userId != user?.Id)
                    return BadRequest("Invalid Permissions");

                _db.Todos.DeleteOne(t => t.Id == itemId && t.Owner == userId);

                return Ok();
            }
            else
                return BadRequest();
        }

    }
}
