using System;
using Domain;
using System.Text;
using Microsoft.VisualBasic.CompilerServices;

namespace Protocol
{
    public class RequestHandler
    {
        public Frame BuildRequest(int optionSelected, User user)
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
                case Command.SignUp:
                    BuildSignUpRequest(requestFrame);
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
        
        private void BuildSignUpRequest(Frame requestFrame)
        {
            Console.WriteLine("-----SIGN UP-----");
            Console.WriteLine("Enter username:");
            string username = Console.ReadLine();
            byte[] userData = Encoding.UTF8.GetBytes(username);
            requestFrame.Data = userData;
            requestFrame.DataLength = userData.Length;

        }
    }
}