using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LogsServer.Domain;

namespace LogsServer.DataAccess
{
    public class LogRepository: ILogRepository
    {
        private List<Log> _logs;
        private readonly SemaphoreSlim _logsSemaphore;
        private static LogRepository _instance;
        private static readonly SemaphoreSlim _instanceSemaphore = new SemaphoreSlim(1);

        private LogRepository()
        {
            _logs = new List<Log>();
            _logsSemaphore = new SemaphoreSlim(1);
        }
        
        public static LogRepository GetInstance()
        {
            _instanceSemaphore.Wait();
            if (_instance == null)
                _instance = new LogRepository();

            _instanceSemaphore.Release();
            return _instance;
        }
        
        public async Task StoreAsync(Log log)
        {
            await _logsSemaphore.WaitAsync();
            _logs.Add(log);
            _logsSemaphore.Release();
        }
        
        public async Task<List<Log>> GetLogsByAsync(Func<Log, bool> criteria)
        {
            await _logsSemaphore.WaitAsync();
            List<Log> retrievedLogs = _logs.Where(criteria).ToList();
            _logsSemaphore.Release();
            return retrievedLogs;
        }
    }
}