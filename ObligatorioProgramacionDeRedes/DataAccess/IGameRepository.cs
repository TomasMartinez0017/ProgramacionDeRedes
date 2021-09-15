using System;
using Domain;

namespace DataAccess
{
    public interface IGameRepository
    {
        void AddGame(Game game);

        bool GameExists(Game game);
    }
}