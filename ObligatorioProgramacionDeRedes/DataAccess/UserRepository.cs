using System;
using System.Collections.Generic;
using System.Linq;
using CustomExceptions;
using Domain;
namespace DataAccess
{
    public class UserRepository : IUserRepository
    {
        private readonly List<User> _users;
        private object _usersLocker;
        private static UserRepository _instance;
        private static Object _instanceLocker = new Object();

        private UserRepository()
        {
            _users = new List<User>();
            _usersLocker = new Object();
        }
        
        public static UserRepository GetInstance()
        {
            lock (_instanceLocker)
            {
                if (_instance == null)
                {
                    _instance = new UserRepository();
                }

                return _instance;
            }
        }

        public void AddUser(User user)
        {
            lock (_usersLocker)
            {
                this._users.Add(user);    
            }
        }

        public User GetUser(string username)
        {
            lock (_usersLocker)
            {
                User userToReturn = null;
                
                foreach (User user in _users)
                {
                    if (user.Username.Equals(username))
                    {
                        userToReturn = user;
                    }
                }
                return userToReturn;
            }
        }

        public bool UserExists(string username)
        {
            lock (_usersLocker)
            {
                foreach (User user in _users)
                {
                    if (user.Username.Equals(username))
                    {
                        return true;
                    }
                }
                
                return false;
            }
        }

        public void DeleteBoughtGame(string gameName)
        {
            lock (_usersLocker)
            {
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
            }
        }
    }
}