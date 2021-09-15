using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CustomExceptions;
using DataAccess;
using Domain;
namespace Protocol
{
    public class ResponseHandler
    {
        public string ProcessResponse(Frame frame)
        {
            string response = null;
            switch ((Command) frame.Command)
            {
                case Command.ShowCatalog:
                    response = ProcessShowCatalogResponse(frame.Data);
                    break;
                case Command.PublishGame:
                    response = Encoding.UTF8.GetString(frame.Data);
                    break;
            }

            return response;
        }

        public Frame GetResponse(Frame frame)
        {
            Frame response = null;

            switch ((Command) frame.Command)
            {
                case Command.ShowCatalog:
                    response = CreateShowCatalogResponse(frame);
                    break;
                case Command.PublishGame:
                    response = CreatePublishGameResponse(frame);
                    break;
            }

            return response;
        }

        private Frame CreatePublishGameResponse(Frame frame)
        {
            Frame response = new Frame();
            response.Command = (int) Command.PublishGame;
            response.Header = (int) Header.Response;
            try
            {
                Game newGame = ExtractGameData(frame);
                newGame.ValidGame();
                GameRepository repository = GameRepository.GetInstance();
                
                string message = null;

                if (!repository.GameExists(newGame))
                {
                    repository.AddGame(newGame);
                    message = "Game published";
                }
                else
                {
                    message = "ERROR: The game you are trying to publish already exists";
                }

                response.Data = Encoding.UTF8.GetBytes(message);
                response.DataLength = response.Data.Length;
                return response;
            }
            catch (InvalidGameException e)
            {
                response.Data = Encoding.UTF8.GetBytes(e.Message);
                response.DataLength = response.Data.Length;
                return response;
            }
            
        }

        private Game ExtractGameData(Frame gameFrame)
        {
            Game gameExtracted = new Game();
            string[] attributes = Encoding.UTF8.GetString(gameFrame.Data).Split("#");
            gameExtracted.Title = attributes[0];
            gameExtracted.Genre = attributes[1];
            gameExtracted.ScoreAverage = attributes[2];
            gameExtracted.Description = attributes[3];
            return gameExtracted;
        }

        private Frame CreateShowCatalogResponse(Frame frame)
        {
            Frame response = new Frame();
            response.Command = (int) Command.ShowCatalog;
            response.Header = (int) Header.Response;
            
            GameRepository repository = GameRepository.GetInstance();
            List<Game> gamesInRepository = repository.GetAllGames();

            List<byte[]> serializedGames = SerializeGames(gamesInRepository);
            byte[] serializedList = SerializeListOfGames(serializedGames);

            response.Data = serializedList;
            response.DataLength = response.Data.Length;

            return response;
        }

        private List<byte[]> SerializeGames(List<Game> games)
        {
            List<byte[]> serializedGames = new List<byte[]>();
            foreach (Game game in games)
            {
                byte[] gameData =
                    Encoding.UTF8.GetBytes($"{game.Title}#{game.Genre}#{game.ScoreAverage}#{game.Description}#");
                serializedGames.Add(gameData);
            }

            return serializedGames;
        }

        private byte[] SerializeListOfGames(List<byte[]> gamesSerialized)
        {
            List<byte> dataToReturn = new List<byte>();
            
            byte separator = 47;
            
            for (int i = 0; i < gamesSerialized.Count; i++)
            {
                byte[] game = gamesSerialized.ElementAt(i);
                dataToReturn.AddRange(game);

                if (i != gamesSerialized.Count() - 1)
                {
                    dataToReturn.Add(separator);
                }
            }

            return dataToReturn.ToArray();
        }

        string ProcessShowCatalogResponse(byte[] data)
        {
            string[] games = Encoding.UTF8.GetString(data).Split('/');
            string joinedGames = string.Join("", games);
            string[] gamesSeparated = joinedGames.Split('#');

            int gameCounter = 0;
            string catalogResponse = "";

            for (int i = 0; i < gamesSeparated.Length; i++)
            {
                switch (gameCounter)
                {
                    case 0:
                        string gameTitle = gamesSeparated[i];
                        catalogResponse = catalogResponse + "Title: " + gameTitle + "\n";
                        gameCounter++;
                        break;
                    case 1:
                        string gameGenre = gamesSeparated[i];
                        catalogResponse = catalogResponse +  "Genre: " + gameGenre + "\n";
                        gameCounter++;
                        break;
                    case 2:
                        string gameScore = gamesSeparated[i];
                        catalogResponse = catalogResponse + "Score: " + gameScore + "\n";
                        gameCounter++;
                        break;
                    case 3:
                        string gameDescription = gamesSeparated[i];
                        catalogResponse = catalogResponse + "Description: " + gameDescription + "\n";
                        gameCounter = 0;
                        break;
                }
            }
            
            return catalogResponse;

        }
    }
}