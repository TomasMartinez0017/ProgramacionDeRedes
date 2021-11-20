using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess;
using Domain;
using Protocol;

namespace NewServer.Managers
{
    public class UserManager
    {
        private UserRepository _userRepository;

        public UserManager()
        {
            _userRepository = UserRepository.GetInstance();
        }

        public async Task<Frame> CreateUserAsync(Frame frame)
        {
            Frame response = new Frame();
            response.CreateFrame((int)Header.Response, (int)Command.SignUp);

            string username = Encoding.UTF8.GetString(frame.Data);

            if (!await _userRepository.UserExistsAsync(username))
            {
                User userToAdd = new User();
                userToAdd.Username = username;
                await _userRepository.AddUserAsync(userToAdd);

                response.Data = frame.Data;
                response.DataLength = response.Data.Length;
            }
            else
            {
                response.Data = Encoding.UTF8.GetBytes("ERROR: User already exist.\n");
                response.Status = (int)FrameStatus.Error;
                response.DataLength = response.Data.Length;
            }
            return response;
        }

        public async Task<Frame> UpdateUserAsync(Frame frame)
        {
            Frame response = new Frame();
            response.CreateFrame((int)Header.Response, (int)Command.UpdateUser);

            string[] names = Encoding.UTF8.GetString(frame.Data).Split('#');

            if (await _userRepository.UserExistsAsync(names[0]) && !await _userRepository.UserExistsAsync(names[1]))
            {
                await _userRepository.UpdateUserAsync(names[0], names[1]);

                response.Data = frame.Data;
                response.DataLength = response.Data.Length;

            }
            else
            {
                response.Data = Encoding.UTF8.GetBytes("ERROR: User already exist.\n");
                response.Status = (int)FrameStatus.Error;
                response.DataLength = response.Data.Length;
            }
            return response;
        }

        public async Task<Frame> DeleteUserAsync(Frame frame)
        {
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
            }
            else
            {
                response.Data = Encoding.UTF8.GetBytes("ERROR: User not found.\n");
                response.Status = (int)FrameStatus.Error;
                response.DataLength = response.Data.Length;
            }
            return response;
            
        } 


        
    }
}
