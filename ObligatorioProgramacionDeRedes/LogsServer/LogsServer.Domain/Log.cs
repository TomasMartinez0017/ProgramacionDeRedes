using System;
using System.Collections;
using System.Collections.Generic;

namespace LogsServer.Domain
{
    public class Log
    {
        public DateTime CreatedAt { get; set; }
        public string LogTag { get; set; }
        public LogInfo LogInfo { get; set; }
    }
}