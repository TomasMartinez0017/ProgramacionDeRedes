using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CustomExceptions;
using Domain;
namespace DataAccess
{
    public class UserRepository
    {
        private readonly List<User> _users;
        private static UserRepository _instance;
        private readonly SemaphoreSlim _usersSemaphore;
        private static readonly SemaphoreSlim _instanceSemaphore = new SemaphoreSlim(1);

        private UserRepository()
        {
            _users = new List<User>();
            _usersSemaphore = new SemaphoreSlim(1);
        }
        
        public static UserRepository GetInstance()
        {
            _instanceSemaphore.Wait();
            if (_instance == null)
            {
                _instance = new UserRepository();
            }

            _instanceSemaphore.Release();
            return _instance;
        }

        public async Task AddUserAsync(User user)
        {
            await _usersSemaphore.WaitAsync();
            this._users.Add(user);
            _usersSemaphore.Release();
        }

        public async Task <User> GetUserAsync(string username)
        {
            await _usersSemaphore.WaitAsync();
            User userToReturn = null;
                
            foreach (User user in _users)
            {
                if (user.Username.Equals(username))
                {
                    userToReturn = user;
                }
            }
            _usersSemaphore.Release();
            return userToReturn;
        }

        public async Task<bool> UserExistsAsync(string username)
        {
            await _usersSemaphore.WaitAsync();
            foreach (User user in _users)
            {
                if (user.Username.Equals(username))
                {
                    _usersSemaphore.Release();
                    return true;
                }
            }
            _usersSemaphore.Release();
            return false;
        }

        public async Task DeleteBoughtGameAsync(string gameName)
        {
            await _usersSemaphore.WaitAsync();
            foreach (User user in _users)
            {
                List<Game> games = user.Games;
                for (int i = 0; i < games.Count; i++)
                {
                    if (games.ElementAt(i).Title.Equals(gameName))
                    {
                        games.Remove(games.ElementAt(i));
                    }
                }
            }
            _usersSemaphore.Release();
        }
    }
}