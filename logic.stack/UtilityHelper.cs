using data.stack;
using model.stack;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace logic.stack
{
    public class UtilityHelper
    {
        private MongoContext _db { get; set; }
        public UtilityHelper(MongoContext context)
        {
            _db = context;
        }

        async public Task<Ping> PingAsync(string environment)
        {
            Ping ping = new Ping();
            ping.Environment = environment;

            try
            {
                _db.Pings.InsertOne(ping);
                ping = await _db.Pings.Find(p => p.Id == ping.Id).FirstOrDefaultAsync();

                if (ping != null)
                {
                    ping.DB = true;
                    await _db.Pings.ReplaceOneAsync(p => p.Id == ping.Id, ping);
                }
                else
                    ping.DB = false;

                return ping;
            }
            catch
            {
                ping.DB = false;
                return ping;
            }

            //Delete any previous "pings". Don't want to bloat the DB. Remove this line to keep a running log of all pings.
            //await _db.Pings.DeleteManyAsync(p => p.Id != ping.Id);
        }
    }
}
