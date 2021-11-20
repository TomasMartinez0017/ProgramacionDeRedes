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

        public override async Task<UpdateUserResponse> UpdateUser(UpdateUserRequest request, ServerCallContext context)
        {
            Frame requestFrame = new Frame();
            byte[] userData = Encoding.UTF8.GetBytes(request.Name + "#" + request.NewName);
            requestFrame.Data = userData;
            requestFrame.DataLength = userData.Length;

            Frame response = await userManager.UpdateUserAsync(requestFrame);

            UpdateUserResponse userResponse = new UpdateUserResponse();

            if (response.Status == 0)
            {
                userResponse.Ok = true;
                userResponse.Message = "User Updated";
            }
            else
            {
                userResponse.Ok = false;
                userResponse.Message = "Error updating user";
            }

            return userResponse;
        }

        public override async Task<DeleteUserResponse> DeleteUser(DeleteUserRequest request, ServerCallContext context)
        {
            Frame requestFrame = new Frame();
            byte[] userData = Encoding.UTF8.GetBytes(request.Name);
            requestFrame.Data = userData;
            requestFrame.DataLength = userData.Length;

            Frame response = await userManager.DeleteUserAsync(requestFrame);

            DeleteUserResponse userResponse = new DeleteUserResponse();
            
            if (response.Status == 0)
            {
                userResponse.Ok = true;
                userResponse.Message = "User Deleted";
            }
            else
            {
                userResponse.Ok = false;
                userResponse.Message = "Error deleting user";
            }

            return userResponse;
        }
    }
}
