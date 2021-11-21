using LogsServer.BussinessLogic;
using LogsServer.Domain;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LogsServer.Controllers
{
    [Route("api/logs")]
    [ApiController]
    public class LogController : ControllerBase
    {
        private readonly LogLogic _logLogic;

        public LogController(LogLogic logic)
        {
            this._logLogic = logic;
        }

        [HttpGet("users/{userName}")]
        public async Task<IActionResult> GetLogsByUser(string userName)
        {
            List<Log> logs =  await this._logLogic.GetLogsByUsername(userName);
            return Ok(logs);
        }

        [HttpGet("games/{gameTitle}")]
        public async Task<IActionResult> GetLogsByGameTitleAsync(string gameTitle)
        {
            List<Log> logs = await this._logLogic.GetLogsByGameTitle(gameTitle);
            return Ok(logs);
        }

        [HttpGet("dates")]
        public async Task<IActionResult> GetLogsByDateAsync([FromBody] string someDate)
        {
            List<Log> logs = await this._logLogic.GetLogsByDate(someDate);
            return Ok(logs);
        }
    }
}
