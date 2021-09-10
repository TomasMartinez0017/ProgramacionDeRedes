using System;
using System.Text;

namespace Protocol
{
    public class RequestHandler
    {
        public Frame BuildRequest(int optionSelected)
        {
            Frame requestFrame = new Frame();
            requestFrame.Header =  (int) Header.Request;
            requestFrame.Command = optionSelected;

            switch ((Command) optionSelected)
            {
                case Command.PublishGame:
                    BuildPublishGameRequest(requestFrame);
                    break;
            }

            return requestFrame;
        }
        
        private void BuildPublishGameRequest(Frame requestFrame)
        {
            Console.WriteLine("Name:");
            string gameName = Console.ReadLine();
            Console.WriteLine("Genre:");
            string gameGenre = Console.ReadLine();
            byte[] gameData = Encoding.UTF8.GetBytes($"{gameName}#{gameGenre}");
            requestFrame.Data = gameData;
            requestFrame.DataLength = gameData.Length;

        }
    }
}