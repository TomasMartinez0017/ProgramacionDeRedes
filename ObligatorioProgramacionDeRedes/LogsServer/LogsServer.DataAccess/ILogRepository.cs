using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LogsServer.Domain;

namespace LogsServer.DataAccess
{
    public interface ILogRepository
    {
        Task StoreAsync(Log log);
        
        Task<List<Log>> GetLogsByAsync(Func<Log, bool> criteria);
    }
}