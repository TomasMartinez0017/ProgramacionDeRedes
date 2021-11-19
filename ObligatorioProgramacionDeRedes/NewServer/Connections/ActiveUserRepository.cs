using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Domain;
using DataAccess;

namespace NewServer
{
    public class ActiveUserRepository
    {
        private readonly List<User> _activeUsers;
        private static ActiveUserRepository _instance;
        private readonly SemaphoreSlim _activeUsersSemaphore;
        private static readonly SemaphoreSlim _instanceSemaphore = new SemaphoreSlim(1);

        private ActiveUserRepository()
        {
            _activeUsers = new List<User>();
            _activeUsersSemaphore = new SemaphoreSlim(1);
        }

        public static ActiveUserRepository GetInstance()
        {
            _instanceSemaphore.Wait();
            
            if (_instance == null)
            {
                _instance = new ActiveUserRepository();
            }
            
            _instanceSemaphore.Release();

            return _instance;
        }
        
        public async Task AddUserAsync(User user)
        {
            await _activeUsersSemaphore.WaitAsync();
            this._activeUsers.Add(user);
            _activeUsersSemaphore.Release();
        }

        public async Task <List<User>> GetUsersAsync()
        {
            await _activeUsersSemaphore.WaitAsync();
            List<User> copy = new List<User>(_activeUsers);
            _activeUsersSemaphore.Release();
            return copy;
        }

        /*public void DisconnectAlUsers()
        {
            lock (_activeUsersLocker)
            {
                this._activeUsers.Clear();
            }
        }*/

        public async Task DisconnectUserAsync(User userToDisconnect)
        {
            await _activeUsersSemaphore.WaitAsync();
            
            for (int i = 0; i < _activeUsers.Count; i++)  
            {
                if (_activeUsers.ElementAt(i).Username.Equals(userToDisconnect.Username))
                {
                    _activeUsers.Remove(_activeUsers.ElementAt(i));
                }
            }

            _activeUsersSemaphore.Release();

        }
    }
}    
    