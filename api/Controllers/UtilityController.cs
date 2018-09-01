using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using damnstack.Data;
using damnstack.Model;

namespace damnstack.Controllers
{
    public class UtilityController : Controller
    {
        private MongoContext _db { get; set; }
        private ApplicationSettings _applicationSettings { get; set; }
        public UtilityController(MongoContext mongoContext, ApplicationSettings applicationSettings)
        {
            _db = mongoContext;
            _applicationSettings = applicationSettings;
        }

        /// <summary>
        /// Use this ping service to the check the status of your server and database.
        /// </summary>
        /// <returns></returns>
        [Route("api/v1/ping")]
        [HttpGet]
        public IActionResult Ping()
        {
            //Create a simple ping object.
            Ping ping = new Ping();
            Ping result;
            try
            {
                _db.Pings.InsertOne(ping);
                result = _db.Pings.Find(p => p.Id == ping.Id).FirstOrDefault();

                //Delete any previous "pings". Don't want to bloat the DB. Remove this line to keep a running log of all pings.
                _db.Pings.DeleteMany(p => p.Id != ping.Id);
            }
            catch
            {
                result = default(Ping);
            }

            return Ok(new {
                Name = _applicationSettings.Name,
                DB = result != null, //Did the database op run? This helps you know if your database connection is up.
                Now = DateTime.UtcNow //What time is it?
            });
        }
    }
}