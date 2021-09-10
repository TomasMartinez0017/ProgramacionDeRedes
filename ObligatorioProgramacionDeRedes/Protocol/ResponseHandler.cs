using System.Text;
using Domain;
namespace Protocol
{
    public class ResponseHandler
    {
        public string ProcessResponse(Frame frame)
        {
            return null;
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
            Frame response = new Frame();
            response.Command = (int) Command.PublishGame;
            response.Header = (int) Header.Response;
            //TO BE CONTINUED
            //response.Data
            return null;
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