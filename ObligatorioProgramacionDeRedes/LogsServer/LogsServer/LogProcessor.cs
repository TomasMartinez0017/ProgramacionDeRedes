using LogsServer.Domain;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LogsServer
{
    public class LogProcessor
    {
        public Log ProcessLog(string message){

            string[] messageComponents = message.Split("#");

            return new Log()
            {
                CreatedAt = new DateTime(Int64.Parse(messageComponents[0])),
                LogTag = messageComponents[1],
                LogInfo = JsonConvert.DeserializeObject<LogInfo>(messageComponents[2]),
            };
        }

    }
}
