using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Domain;
namespace DataAccess
{
    public class GameRepository
    {
        private readonly List<Game> _games;
        private static GameRepository _instance;
        private readonly SemaphoreSlim _gamesSemaphore;
        private static readonly SemaphoreSlim _instanceSemaphore = new SemaphoreSlim(1);

        private GameRepository()
        {
            _games = new List<Game>();
            _gamesSemaphore = new SemaphoreSlim(1);
        }
        
        public static GameRepository GetInstance()
        {
            _instanceSemaphore.Wait();
            if (_instance == null)
            {
                _instance = new GameRepository();
            }

            _instanceSemaphore.Release();
            return _instance;
            
        }
        
        public async Task AddGameAsync(Game game)
        {
            await _gamesSemaphore.WaitAsync();
            this._games.Add(game);
            _gamesSemaphore.Release();
        }

        public async Task<bool> GameExistsAsync(Game game)
        {
            await _gamesSemaphore.WaitAsync();
            foreach (Game gameInList in _games)
            {
                if (gameInList.Title.Equals(game.Title))
                {
                    _gamesSemaphore.Release();
                    return true;
                }
                
            }
            _gamesSemaphore.Release();
            return false;
        }

        public async Task<List<Game>> GetAllGamesAsync()
        {
            await _gamesSemaphore.WaitAsync();
            List<Game> copyOfGames = new List<Game>(_games);
            _gamesSemaphore.Release();
            return copyOfGames;
        }

        public async Task<Game> GetGameAsync(string gameName)
        {
            await _gamesSemaphore.WaitAsync();
            foreach (Game game in _games)
            {
                if (game.Title.Equals(gameName))
                {
                    _gamesSemaphore.Release();
                    return game;
                }
            }
            _gamesSemaphore.Release();
            return null;
        }

        public async Task DeleteGameAsync(string gameName)
        {
            await _gamesSemaphore.WaitAsync();
            for(int i=0; i<_games.Count; i++)
            {
                if (_games.ElementAt(i).Title.Equals(gameName))
                {
                    _games.Remove(_games.ElementAt(i));
                }
            }
            _gamesSemaphore.Release();
        }

        public async Task UpdateGameAsync(string gameNameSearched, Game gameUpdated)
        {
            Game gameToUpdate = await this.GetGameAsync(gameNameSearched);
            if(!string.IsNullOrEmpty(gameUpdated.Title)) gameToUpdate.Title = gameUpdated.Title;
            if(!string.IsNullOrEmpty(gameUpdated.Genre)) gameToUpdate.Genre = gameUpdated.Genre;
            if(!string.IsNullOrEmpty(gameUpdated.Rating)) gameToUpdate.Rating = gameUpdated.Rating;
            if (!string.IsNullOrEmpty(gameUpdated.Description)) gameToUpdate.Description = gameUpdated.Description;
        }

        public async Task<List<Game>> GetGamesWithGenreAsync(string genre)
        {
            await _gamesSemaphore.WaitAsync();
            List<Game> gamesToReturn = new List<Game>();
            foreach (Game game in _games)
            {
                if (game.Genre.Equals(genre))
                {
                    gamesToReturn.Add(game);
                }
            }
            _gamesSemaphore.Release();
            return gamesToReturn;

        }
        
        public async Task<List<Game>> GetGamesWithRatingAsync(string rating)
        {
            await _gamesSemaphore.WaitAsync();
            List<Game> gamesToReturn = new List<Game>();
            foreach (Game game in _games)
            {
                string ratingNumber = game.ConvertRating();
                if (ratingNumber.Equals(rating))
                {
                    gamesToReturn.Add(game);
                }
            }
            _gamesSemaphore.Release();
            return gamesToReturn;
        }
    }
}