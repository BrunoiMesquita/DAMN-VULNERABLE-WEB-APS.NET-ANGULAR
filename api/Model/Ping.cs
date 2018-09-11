using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Model
{
    public class Ping
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public DateTime PingDateTime { get; set; } = DateTime.UtcNow;
    }
}
