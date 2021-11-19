using Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdminServer.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UserController : ControllerBase
    {
        private readonly UserAdmin.UserAdminClient userClient;

        public UserController(UserAdmin.UserAdminClient client)
        {
            this.userClient = client;
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] string userName)
        {
            CreateUserRequest request = new CreateUserRequest()
            {
                Name = userName
            };

            CreateUserResponse response = await userClient.CreateUserAsync(request);

            if (response.Ok)
            {
                return Ok(response.Message);
            }
            else
            {
                return BadRequest();
            }       
        } 



    }
}
