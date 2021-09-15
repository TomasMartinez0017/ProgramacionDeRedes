using System;
using System.Linq;
using System.Collections.Generic;
using Domain;
namespace DataAccess
{
    public class GameRepository : IGameRepository
    {
        private readonly List<Game> _games;
        private object _gamesLocker;
        private static GameRepository _instance;
        private static Object _instanceLocker = new Object();

        private GameRepository()
        {
            _games = new List<Game>();
            _gamesLocker = new Object();
        }
        
        public static GameRepository GetInstance()
        {
            lock (_instanceLocker)
            {
                if (_instance == null)
                {
                    _instance = new GameRepository();
                }

                return _instance;
            }
        }
        
        public void AddGame(Game game)
        {
            lock (_gamesLocker)
            {
                this._games.Add(game);    
            }
        }

        public bool GameExists(Game game)
        {
            lock (_gamesLocker)
            {
                foreach (Game gameInList in _games)
                {
                    if (gameInList.Title.Equals(game.Title))
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        public List<Game> GetAllGames()
        {
            lock (_gamesLocker)
            {
                List<Game> copyOfGames = new List<Game>(_games);
                return copyOfGames;
            }
        }
    }
}