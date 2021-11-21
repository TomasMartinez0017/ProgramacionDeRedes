using System;
using System.Collections.Generic;
using System.Text;

namespace LogsServer.Domain
{
    public class LogInfo
    {
        public string UserName { get; set; }
        public string GameTitle { get; set; }
        public string Message { get; set; }

        public LogInfo(string userName, string title, string message)
        {
            this.UserName = userName;
            this.GameTitle = title;
            this.Message = message;
        }
    }
}
