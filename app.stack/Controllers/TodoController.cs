using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MongoDB.Driver;
using logic.stack;
using model.stack;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace app.stack.Controllers
{
    public class TodoController : Controller
    {
        private UserHelper _userHelper { get; set; }
        private TodoHelper _todoHelper { get; set; }
        public TodoController(UserHelper userHelper, TodoHelper todoHelper)
        {
            _userHelper = userHelper;
            _todoHelper = todoHelper;
        }


        [HttpGet]
        [Route("api/v1/user/{id}/todo")]
        [Authorize]
        async public Task<IActionResult> GetUserTodoItems(string id)
        {
            User user = await _userHelper.GetUserByIdAsync(User.Identity.Name);

            //You must be logged in as the user specified in the URL
            if (id != user?.Id)
                return BadRequest("Invalid Permissions");

            List<Todo> list = await _todoHelper.GetTodosByUserId(user.Id);

            return Ok(list);
        }

        [HttpPost]
        [Authorize]
        [Route("api/v1/user/{id}/todo")]
        async public Task<IActionResult> CreateItem(string id, [FromBody]TodoItemViewModel model)
        {
            if (ModelState.IsValid)
            {
                User user = await _userHelper.GetUserByIdAsync(User.Identity.Name);

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

                await _todoHelper.InsertTodoByUserId(item);
                return Ok(item);
            }
            else
                return BadRequest();
        }

        [HttpPut]
        [Authorize]
        [Route("api/v1/user/{userId}/todo/{itemId}/status/{completed}")]
        async public Task<ActionResult> CompleteItemAsync(string userId, string itemId, bool completed)
        {
            if (ModelState.IsValid)
            {
                User user = await _userHelper.GetUserByIdAsync(User.Identity.Name);

                //You must be logged in as the user specified in the URL
                if (userId != user?.Id)
                    return BadRequest("Invalid Permissions");

                Todo item = await _todoHelper.GetTodoByUserIdItemId(user.Id, itemId);

                if (item == null)
                    return NotFound();

                await _todoHelper.UpdateTodoByUserIdItemId(completed, user.Id, itemId);

                return Ok();
            }
            else
                return BadRequest();
        }

        [HttpDelete]
        [Authorize]
        [Route("api/v1/user/{userId}/todo/{itemId}")]
        async public Task<IActionResult> DeleteItem(string userId, string itemId)
        {
            if (ModelState.IsValid)
            {
                User user = await _userHelper.GetUserByIdAsync(User.Identity.Name);

                //You must be logged in as the user specified in the URL
                if (userId != user?.Id)
                    return BadRequest("Invalid Permissions");

                await _todoHelper.TaskDeleteTodoByUserIdItemId(user.Id, itemId);

                return Ok();
            }
            else
                return BadRequest();
        }

    }
}
