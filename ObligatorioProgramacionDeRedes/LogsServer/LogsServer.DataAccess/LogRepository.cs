using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LogsServer.Domain;

namespace LogsServer.DataAccess
{
    public class LogRepository
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

        public async Task<List<Log>> GetLogs()
        {
            await _logsSemaphore.WaitAsync();
            List<Log> listToReturn = new List<Log>();
            foreach(Log log in this._logs){
                listToReturn.Add(log);
            }
            _logsSemaphore.Release();
            return listToReturn;
        }

        public async Task<List<Log>> GetLogsByUser(string username){
            await _logsSemaphore.WaitAsync();
            List<Log> listToReturn = new List<Log>();
            foreach(Log log in this._logs){
                if(log.LogInfo.UserName.Equals(username)){
                    listToReturn.Add(log);
                }
            }
            _logsSemaphore.Release();
            return listToReturn;

        }

        public async Task<List<Log>> GetLogsByGameTitle(string title){
            await _logsSemaphore.WaitAsync();
            List<Log> listToReturn = new List<Log>();
            foreach(Log log in this._logs){
                if(log.LogInfo.GameTitle.Equals(title)){
                    listToReturn.Add(log);
                }
            }
            _logsSemaphore.Release();
            return listToReturn;
        }

        public async Task<List<Log>> GetLogsByDate(string stringDate){
            await _logsSemaphore.WaitAsync();
            List<Log> listToReturn = new List<Log>();
            foreach(Log log in this._logs){
                if(log.CreatedAt.ToString("dd/MM/yyyy").Equals(stringDate)){
                    listToReturn.Add(log);
                }
            }
            _logsSemaphore.Release();
            return listToReturn;
        }
        
    }
}