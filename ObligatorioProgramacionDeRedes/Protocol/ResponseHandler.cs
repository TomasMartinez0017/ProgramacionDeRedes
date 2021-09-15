using System.Text;
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
                case Command.PublishGame:
                    response = CreatePublishGameResponse(frame);
                    break;
            }

            return response;
        }

        private Frame CreatePublishGameResponse(Frame frame)
        {
            Game newGame = ExtractGameData(frame);
            GameRepository repository = GameRepository.GetInstance();
            
            Frame response = new Frame();
            response.Command = (int) Command.PublishGame;
            response.Header = (int) Header.Response;

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

        private Game ExtractGameData(Frame gameFrame)
        {
            Game gameExtracted = new Game();
            string[] attributes = Encoding.UTF8.GetString(gameFrame.Data).Split("#");
            gameExtracted.Title = attributes[0];
            gameExtracted.Genre = attributes[1];
            return gameExtracted;
        }
    }
}