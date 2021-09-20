using System;
using System.Collections.Generic;
using System.IO;
using Domain;
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
                case Command.SignUp:
                    BuildSignUpRequest(requestFrame);
                    break;
                case Command.LogIn:
                    BuildLogInRequest(requestFrame);
                    break;
                case Command.UploadImage:
                    BuildUploadImageRequest(requestFrame);
                    break;
                case Command.CreateReview:
                    BuildRateGameRequest(requestFrame);
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
        
        private void BuildLogInRequest(Frame requestFrame)
        {
            Console.WriteLine("-----LOGIN-----");
            Console.WriteLine("Enter username:");
            string username = Console.ReadLine();
            byte[] userData = Encoding.UTF8.GetBytes(username);
            requestFrame.Data = userData;
            requestFrame.DataLength = userData.Length;
        }

        private void BuildUploadImageRequest(Frame requestFrame)
        {
            Console.WriteLine("Game name:");
            string gameName = Console.ReadLine();
            Console.WriteLine("Path were image is located:");
            string path = Console.ReadLine();
            
            while (String.IsNullOrEmpty(path))
            {
                Console.WriteLine("ERROR: Please, enter a valid path");
                path = Console.ReadLine();
            }

            while (!File.Exists(path))
            {
                Console.WriteLine("ERROR: Image file does not exist");
                path = Console.ReadLine();
            }
            
            string nameOfImage = new FileInfo(path).Name;
            byte[] imageData = File.ReadAllBytes(path);
            int lenghtOfDataWithoutImage = gameName.Length + nameOfImage.Length + path.Length + 2; //4 porque son cuatro '#'
            
            List<byte> imageRequestData = new List<byte>();
            imageRequestData.AddRange(BitConverter.GetBytes(lenghtOfDataWithoutImage));
            imageRequestData.AddRange(Encoding.UTF8.GetBytes($"{gameName}#"));
            imageRequestData.AddRange(Encoding.UTF8.GetBytes($"{nameOfImage}#"));
            imageRequestData.AddRange(Encoding.UTF8.GetBytes($"{path}"));
            imageRequestData.AddRange(imageData);

            byte[] data = imageRequestData.ToArray();
            
            requestFrame.Data = data;
            requestFrame.DataLength = data.Length;
        }

        public void BuildRateGameRequest(Frame requestFrame)
        {
            Console.WriteLine("Game name:");
            string gameName = Console.ReadLine();
            Console.WriteLine("Score: (between 0 and 5)");
            string score = Console.ReadLine();
            Console.WriteLine("Review:");
            string comment = Console.ReadLine();
            
            byte[] data = Encoding.UTF8.GetBytes($"{gameName}#{score}#{comment}");
            requestFrame.Data = data;
            requestFrame.DataLength = requestFrame.Data.Length;
        }
    }
}