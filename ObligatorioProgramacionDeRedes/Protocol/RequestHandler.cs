using System;
using System.Text;
using Microsoft.VisualBasic.CompilerServices;

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
                case Command.ShowCatalog:
                    break;
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
            Console.WriteLine("Score:");
            string gameScore = Console.ReadLine();
            Console.WriteLine("Description:");
            string gameDescription = Console.ReadLine();
            byte[] gameData = Encoding.UTF8.GetBytes($"{gameName}#{gameGenre}#{gameScore}#{gameDescription}");
            requestFrame.Data = gameData;
            requestFrame.DataLength = gameData.Length;

        }
    }
}