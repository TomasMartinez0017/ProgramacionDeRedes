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

        public Game GetGame(string gameName)
        {
            lock (_gamesLocker)
            {
                foreach (Game game in _games)
                {
                    if (game.Title.Equals(gameName))
                    {
                        return game;
                    }
                }

                return null;
            }
        }

        public void DeleteGame(string gameName)
        {
            lock (_gamesLocker)
            {
                for(int i=0; i<_games.Count; i++)
                {
                    if (_games.ElementAt(i).Title.Equals(gameName))
                    {
                        _games.Remove(_games.ElementAt(i));
                    }
                }
            }
        }

        public void UpdateGame(string gameNameSearched, Game gameUpdated)
        {
            lock (_gamesLocker)
            {
                Game gameToUpdate = this.GetGame(gameNameSearched);
                if(!string.IsNullOrEmpty(gameUpdated.Title)) gameToUpdate.Title = gameUpdated.Title;
                if(!string.IsNullOrEmpty(gameUpdated.Genre)) gameToUpdate.Genre = gameUpdated.Genre;
                if(!string.IsNullOrEmpty(gameUpdated.Rating)) gameToUpdate.Rating = gameUpdated.Rating;
                if (!string.IsNullOrEmpty(gameUpdated.Description)) gameToUpdate.Description = gameUpdated.Description;
            }
        }
    }
}