using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess;
using Domain;
using LogsHelper;
using LogsServer.Domain;
using Newtonsoft.Json;
using Protocol;

namespace NewServer.Managers
{
    public class UserManager
    {
        private UserRepository _userRepository;
        private ResponseHandler _responseHandler;
        private LogEmitter _emitter;

        public UserManager()
        {
            _userRepository = UserRepository.GetInstance();
            _responseHandler = new ResponseHandler();
            _emitter = new LogEmitter();
        }

        public async Task<Frame> CreateUserAsync(Frame frame)
        {
            return await _responseHandler.CreateSignUpResponseAsync(frame);
        }

        public async Task<Frame> UpdateUserAsync(Frame frame)
        {
            string message;

            Frame response = new Frame();
            response.CreateFrame((int)Header.Response, (int)Command.UpdateUser);

            string[] names = Encoding.UTF8.GetString(frame.Data).Split('#');

            if (await _userRepository.UserExistsAsync(names[0]) && !await _userRepository.UserExistsAsync(names[1]))
            {
                await _userRepository.UpdateUserAsync(names[0], names[1]);

                response.Data = frame.Data;
                response.DataLength = response.Data.Length;
                message = "User updated successfully";

            }
            else
            {
                message = "ERROR: User already exist.\n";
                response.Data = Encoding.UTF8.GetBytes(message);
                response.Status = (int)FrameStatus.Error;
                response.DataLength = response.Data.Length;
               
            }

            _emitter.EmitLog(JsonConvert.SerializeObject(new LogInfo(names[0], "", message)), LogTag.UpdateUser);

            return response;
        }

        public async Task<Frame> DeleteUserAsync(Frame frame)
        {
            string message;
            
            Frame response = new Frame();
            response.CreateFrame((int)Header.Response, (int)Command.DeleteUser);
            string username = Encoding.UTF8.GetString(frame.Data);
            
            if (await _userRepository.UserExistsAsync(username))
            {
                User userToDelete = new User();
                userToDelete.Username = username;
                await _userRepository.DeleteUserAsync(userToDelete);

                response.Data = frame.Data;
                response.DataLength = response.Data.Length;
                message = "User deleted successfully";
            }
            else
            {
                message = "ERROR: User not found.\n";
                response.Data = Encoding.UTF8.GetBytes(message);
                response.Status = (int)FrameStatus.Error;
                response.DataLength = response.Data.Length;
            }

            _emitter.EmitLog(JsonConvert.SerializeObject(new LogInfo(username, "", message)), LogTag.DeleteUser);

            return response;
            
        } 


        
    }
}
