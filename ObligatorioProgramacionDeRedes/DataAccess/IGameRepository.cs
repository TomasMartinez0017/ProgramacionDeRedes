using System;
using System.Collections.Generic;
using Domain;

namespace DataAccess
{
    public interface IGameRepository
    {
        void AddGame(Game game);

        bool GameExists(Game game);

        List<Game> GetAllGames();
    }
}