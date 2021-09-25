using System;
using System.Collections.Generic;
using System.IO;
using Domain;
using System.Text;
using System.Text.RegularExpressions;
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
                case Command.BuyGame:
                    BuildBuyGameRequest(requestFrame);
                    break;
                case Command.ShowGameReviews:
                    BuildShowGameReviewsRequest(requestFrame);
                    break;
                case Command.DeleteGame:
                    BuildDeleteGameRequest(requestFrame);
                    break;
                case Command.UpdateGame:
                    BuildUpdateGameRequest(requestFrame);
                    break;
                case Command.DownLoadImage:
                    BuildDownloadGameImage(requestFrame);
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
            string rating = GetRatingFromConsole((int)Command.PublishGame);
            Console.WriteLine("Description:");
            string gameDescription = Console.ReadLine();
            byte[] gameData = Encoding.UTF8.GetBytes($"{gameName}#{gameGenre}#{rating}#{gameDescription}");
            requestFrame.Data = gameData;
            requestFrame.DataLength = gameData.Length;
        }

        private string GetRatingFromConsole(int requestCommand)
        {
            Console.WriteLine("ESRB:");
            Console.WriteLine("1 - Everyone");
            Console.WriteLine("2 - Teen");
            Console.WriteLine("3 - Mature");
            Console.WriteLine("4 - Adults Only");
            string rating = Console.ReadLine();
            
            if (requestCommand == (int) Command.UpdateGame)
            {
                if (string.IsNullOrEmpty(rating))
                {
                    return rating;
                }
                else
                {
                    ValidateRating(rating);
                    return rating;
                }
            }
            else
            {
                ValidateRating(rating);
                return rating;  
            }
            
        }

        private void ValidateRating(string rating)
        {
            while (!RatingIsNumeric(rating) || Convert.ToInt32(rating) < 0 || Convert.ToInt32(rating) > 4)
            {
                Console.WriteLine("ERROR: Please enter a valid rating");
                rating = Console.ReadLine();
            }
        }

        private bool RatingIsNumeric(string rating)
        {
            Regex onlyNumbers = new Regex("^[0-9]*$");
            return onlyNumbers.IsMatch(rating);
        }

        private void BuildSignUpRequest(Frame requestFrame)
        {
            Console.WriteLine("-----SIGN UP-----");
            Console.WriteLine("Enter username:");
            string username = Console.ReadLine();
            while (String.IsNullOrEmpty(username))
            {
                Console.WriteLine("ERROR: Please, enter a valid username.");
                username = Console.ReadLine();
            }
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
            int lenghtOfDataWithoutImage = gameName.Length + nameOfImage.Length + path.Length + 2; //2 porque son dos '#'
            
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

        private void BuildRateGameRequest(Frame requestFrame)
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

        private void BuildBuyGameRequest(Frame requestFrame)
        {
            Console.WriteLine("Game name:");
            string gameName = Console.ReadLine();
            byte[] data = Encoding.UTF8.GetBytes($"{gameName}");
            requestFrame.Data = data;
            requestFrame.DataLength = requestFrame.Data.Length;
        }

        private void BuildShowGameReviewsRequest(Frame requestFrame)
        {
            Console.WriteLine("Game name:");
            string gameName = Console.ReadLine();
            byte[] data = Encoding.UTF8.GetBytes($"{gameName}");
            requestFrame.Data = data;
            requestFrame.DataLength = requestFrame.Data.Length;
        }

        private void BuildDeleteGameRequest(Frame requestFrame)
        {
            Console.WriteLine("Game name:");
            string gameName = Console.ReadLine();
            byte[] data = Encoding.UTF8.GetBytes($"{gameName}");
            requestFrame.Data = data;
            requestFrame.DataLength = requestFrame.Data.Length;
        }

        private void BuildUpdateGameRequest(Frame requestFrame)
        {
            Console.WriteLine("Enter game name you want to update:");
            string gameName = Console.ReadLine();
            Console.WriteLine("Leave the field empty if you do not want to update attribute");
            Console.WriteLine("New game title:");
            string newGameName = Console.ReadLine();
            Console.WriteLine("New game genre:");
            string newGameGenre = Console.ReadLine();
            Console.WriteLine("New game rating:");
            string newGameRating = GetRatingFromConsole((int)Command.UpdateGame);
            Console.WriteLine("New game description");
            string newGameDescription = Console.ReadLine();
            
            byte[] gameData = Encoding.UTF8.GetBytes($"{gameName}#{newGameName}#{newGameGenre}#{newGameRating}#{newGameDescription}");
            requestFrame.Data = gameData;
            requestFrame.DataLength = gameData.Length;
        }

        private void BuildDownloadGameImage(Frame requestFrame)
        {
            Console.WriteLine("Enter game name:");
            string gameName = Console.ReadLine();
            byte[] data = Encoding.UTF8.GetBytes($"{gameName}");
            requestFrame.Data = data;
            requestFrame.DataLength = requestFrame.Data.Length;
        }
        
    }
}