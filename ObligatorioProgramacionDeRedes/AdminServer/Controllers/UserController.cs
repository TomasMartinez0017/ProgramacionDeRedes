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
                return BadRequest(response.Message);
            }
        }

        [HttpPut("{originalUserName}")]
        public async Task<IActionResult> UpdateUser([FromBody] string newName, string originalUserName)
        {
            UpdateUserRequest request = new UpdateUserRequest()
            {
                Name = originalUserName,
                NewName = newName

            };

            UpdateUserResponse response = await userClient.UpdateUserAsync(request);
            if (response.Ok)
            {
                return Ok(response.Message);
            }
            else
            {
                return BadRequest(response.Message);
            }
        }

        [HttpDelete("{userName}")]
        public async Task<IActionResult> DeleteUser(string userName)
        {
            DeleteUserRequest request = new DeleteUserRequest()
            {
                Name = userName
            };

            DeleteUserResponse response = await userClient.DeleteUserAsync(request);
            
            if (response.Ok)
            {
                return Ok(response.Message);
            }
            else
            {
                return BadRequest(response.Message);
            }
            
        }




    }
}
