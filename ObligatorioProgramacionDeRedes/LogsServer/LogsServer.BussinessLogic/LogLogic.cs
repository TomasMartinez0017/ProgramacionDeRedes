using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using LogsServer.DataAccess;
using LogsServer.Domain;

namespace LogsServer.BussinessLogic
{
    public class LogLogic
    {
        public async Task<List<Log>> GetLogsByUsername(string username){
            LogRepository repository = LogRepository.GetInstance();
            return await repository.GetLogsByUser(username);
        }

        public async Task<List<Log>> GetLogsByGameTitle(string gameTitle){
            LogRepository repository = LogRepository.GetInstance();
            return await repository.GetLogsByGameTitle(gameTitle);
        }

        public async Task<List<Log>> GetLogsByDate(string date){
            LogRepository repository = LogRepository.GetInstance();
            return await repository.GetLogsByDate(date);
        }
    }
}
