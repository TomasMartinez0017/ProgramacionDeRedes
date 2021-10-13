using System;
using System.Collections.Generic;
using System.Linq;
using Domain;
using DataAccess;

namespace Server
{
    public class ActiveUserRepository
    {
        private readonly List<User> _activeUsers;
        private object _activeUsersLocker;
        private static ActiveUserRepository _instance;
        private static Object _instanceLocker = new Object();

        private ActiveUserRepository()
        {
            _activeUsers = new List<User>();
            _activeUsersLocker = new Object();
        }

        public static ActiveUserRepository GetInstance()
        {
            lock (_instanceLocker)
            {
                if (_instance == null)
                {
                    _instance = new ActiveUserRepository();
                }

                return _instance;
            }
        }
        
        public void AddUser(User user)
        {
            lock (_activeUsersLocker)
            {
                this._activeUsers.Add(user);    
            }
        }

        public List<User> GetUsers()
        {
            lock (_activeUsersLocker)
            {
                List<User> copy = new List<User>(_activeUsers);
                return copy;
            }
        }

        public void DisconnectAlUsers()
        {
            lock (_activeUsersLocker)
            {
                this._activeUsers.Clear();
            }
        }

        public void DisconnectUser(User userToDisconnect)
        {
            lock (_activeUsersLocker)
            {
                for (int i = 0; i < _activeUsers.Count; i++)  
                {
                    if (_activeUsers.ElementAt(i).Username.Equals(userToDisconnect.Username))
                    {
                        _activeUsers.Remove(_activeUsers.ElementAt(i));
                    }
                }
            }
        }
    }
}    
    