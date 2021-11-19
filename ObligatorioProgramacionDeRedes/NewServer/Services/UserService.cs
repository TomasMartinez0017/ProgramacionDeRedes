using Grpc.Core;
using NewServer.Managers;
using Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewServer.Services
{
    public class UserService : UserAdmin.UserAdminBase
    {
        private readonly UserManager userManager;

        public UserService(UserManager manager)
        {
            this.userManager = manager;
        }
        public override async Task<CreateUserResponse> CreateUser(CreateUserRequest request, ServerCallContext context)
        {
            Frame requestFrame = new Frame();
            byte[] userData = Encoding.UTF8.GetBytes(request.Name);
            requestFrame.Data = userData;
            requestFrame.DataLength = userData.Length;

            Frame response = await userManager.CreateUserAsync(requestFrame);

            CreateUserResponse userResponse = new CreateUserResponse();

            if (response.Status == 0)
            {
                userResponse.Ok = true;
                userResponse.Message = "User created";
            }
            else
            {
                userResponse.Ok = false;
                userResponse.Message = "Error creating user";
            }

            return userResponse;
        }

        public override Task<UpdateUserResponse> UpdateUser(UpdateUserRequest request, ServerCallContext context)
        {
            return Task.FromResult(new UpdateUserResponse
            {
                Message = "updated"
            });
        }

        public override Task<DeleteUserResponse> DeleteUser(DeleteUserRequest request, ServerCallContext context)
        {
            return Task.FromResult(new DeleteUserResponse
            {
                Message = "deleted"
            });
        }
    }
}
