using System;
using System.Linq;
using System.Threading.Tasks;
using logic.damn;
using Microsoft.AspNetCore.Mvc;
using model.damn;
using MongoDB.Driver;

namespace api.damn.Controllers
{
    public class UtilityController : Controller
    {
        private ApplicationSettings _applicationSettings { get; set; }
        private UtilityLogic _utilityHelper { get; set; }
        public UtilityController(ApplicationSettings applicationSettings, UtilityLogic utilityHelper)
        {
            _applicationSettings = applicationSettings;
            _utilityHelper = utilityHelper;
        }

        /// <summary>
        /// Use this ping service to the check the status of your server and database.
        /// </summary>
        /// <returns></returns>
        [Route("api/v1/ping")]
        [HttpGet]
        async public Task<IActionResult> Ping()
        {
            Ping result = new Ping();
            try
            {
                result = await _utilityHelper.PingAsync(_applicationSettings.Name);
            }
            catch
            {
                result = default(Ping);
            }

            return Ok(result);
        }
    }
}