﻿using System;
using System.Collections.Generic;
using System.IO;
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
                case Command.SignUp:
                    response = Encoding.UTF8.GetString(frame.Data);
                    break;
                case Command.LogIn:
                    response = Encoding.UTF8.GetString(frame.Data);
                    break;
                case Command.UploadImage:
                    response = Encoding.UTF8.GetString(frame.Data);
                    break;
                case Command.CreateReview:
                    response = Encoding.UTF8.GetString(frame.Data);
                    break;
                case Command.BuyGame:
                    response = Encoding.UTF8.GetString(frame.Data);
                    break;
                case Command.ShowGameReviews:
                    response = ProcessShowGameReviews(frame.Data);
                    break;
            }

            return response;
        }

        public Frame GetResponse(Frame frame, List<User> usersConnected, User _userConnected)
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
                case Command.SignUp:
                    response = CreateSignUpResponse(frame);
                    break;
                case Command.LogIn:
                    response = CreateLogInResponse(frame, usersConnected);
                    break;
                case Command.UploadImage:
                    response = CreateUploadImageResponse(frame);
                    break;
                case Command.CreateReview:
                    response = CreateReviewResponse(frame, _userConnected);
                    break;
                case Command.BuyGame:
                    response = CreateBuyGameResponse(frame, _userConnected);
                    break;
                case Command.ShowGameReviews:
                    response = CreateShowGameReviewsResponse(frame);
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
                    response.Status = (int) Status.Ok;
                    message = "Game published";
                }
                else
                {
                    message = "ERROR: The game you are trying to publish already exists";
                    response.Status = (int) Status.Error;
                }

                response.Data = Encoding.UTF8.GetBytes(message);
                response.DataLength = response.Data.Length;
                return response;
            }
            catch (InvalidGameException e)
            {
                response.Data = Encoding.UTF8.GetBytes(e.Message);
                response.DataLength = response.Data.Length;
                response.Status = (int) Status.Error;
                return response;
            }
            
        }

        private Game ExtractGameData(Frame gameFrame)
        {
            Game gameExtracted = new Game();
            string[] attributes = Encoding.UTF8.GetString(gameFrame.Data).Split("#");
            gameExtracted.Title = attributes[0];
            gameExtracted.Genre = attributes[1];
            string rating = attributes[2];
            gameExtracted.SetRating(rating);
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
            response.Status = (int) Status.Ok;
            return response;
        }

        private List<byte[]> SerializeGames(List<Game> games)
        {
            List<byte[]> serializedGames = new List<byte[]>();
            foreach (Game game in games)
            {
                byte[] gameData =
                    Encoding.UTF8.GetBytes($"{game.Title}#{game.Genre}#{game.Rating}#{game.Description}#");
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

        private string ProcessShowCatalogResponse(byte[] data)
        {
            string[] games = Encoding.UTF8.GetString(data).Split('/');
            string joinedGames = string.Join("", games);
            string[] gamesSeparated = joinedGames.Split('#');

            int gameCounter = 0;
            string catalogResponse = "";

            for (int i = 0; i < gamesSeparated.Length - 1; i++)
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
                        string gameRating = gamesSeparated[i];
                        catalogResponse = catalogResponse + "Rating: " + gameRating + "\n";
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

        private string ProcessShowGameReviews(byte[] data)
        {
            string[] reviews = Encoding.UTF8.GetString(data).Split('/');
            string joinedReviews = string.Join("", reviews);
            string[] reviewsSeparated = joinedReviews.Split('#');

            int reviewCounter = 0;
            string reviewsResponse = "";
            int scoreTotal = 0;
            
            for (int i = 0; i < reviewsSeparated.Length - 1; i++)
            {
                switch (reviewCounter)
                {
                    case 0:
                        string reviewUser = reviewsSeparated[i];
                        reviewsResponse = reviewsResponse + "User: " + reviewUser + "\n";
                        reviewCounter++;
                        break;
                    case 1:
                        string reviewScore = reviewsSeparated[i];
                        reviewsResponse = reviewsResponse +  "Score: " + reviewScore + "\n";
                        reviewCounter++;
                        scoreTotal += Convert.ToInt32(reviewScore); 
                        break;
                    case 2:
                        string reviewComment = reviewsSeparated[i];
                        reviewsResponse = reviewsResponse + "Comment: " + reviewComment + "\n";
                        reviewCounter = 0;
                        break;
                }
            }

            int scoreAverage = scoreTotal / reviews.Length;
            reviewsResponse = reviewsResponse + scoreAverage.ToString() + "\n";
            
            return reviewsResponse;
        }

        private Frame CreateSignUpResponse(Frame frame)
        {
            Frame response = new Frame();
            response.Command = (int) Command.SignUp;
            response.Header = (int) Header.Response;
            response.Status = (int) Status.Ok;
            
            string username = Encoding.UTF8.GetString(frame.Data);

            UserRepository repository = UserRepository.GetInstance();

            if (!repository.UserExists(username))
            {
                User userToAdd = new User();
                userToAdd.Username = username;
                repository.AddUser(userToAdd);
                
                response.Data = frame.Data;
                response.DataLength = response.Data.Length;
            }
            else
            {
                response.Data = Encoding.UTF8.GetBytes("ERROR: User already exist.");
                response.Status = (int) Status.Error;
                response.DataLength = response.Data.Length;
            }
            
            return response;
        }

        private Frame CreateLogInResponse(Frame frame, List<User> usersConnected)
        {
            Frame response = new Frame();
            response.Command = (int) Command.LogIn;
            response.Header = (int) Header.Response;
            response.Status = (int) Status.Ok;
            string username = Encoding.UTF8.GetString(frame.Data);
            UserRepository repository = UserRepository.GetInstance();

            if (UserAlreadyLoggedIn(username, usersConnected))
            {
                response.Data = response.Data = Encoding.UTF8.GetBytes("ERROR: User already has an active session");
                response.DataLength = response.Data.Length;
                response.Status = (int) Status.Error;
                return response;
            }
            if(repository.UserExists(username))
            {
                response.Data = frame.Data;
                response.DataLength = response.Data.Length;
            }
            else
            {
                response.Data = Encoding.UTF8.GetBytes("ERROR: User does not exist.");
                response.DataLength = response.Data.Length;
                response.Status = (int) Status.Error;
            }
            
            return response;
        }

        private bool UserAlreadyLoggedIn(string username, List<User> usersConnected)
        {
            foreach (User user in usersConnected)
            {
                if (user.Username.Equals(username))
                {
                    return true;
                }
            }

            return false;
        }

        private Frame CreateUploadImageResponse(Frame frame)
        {
            Frame response = new Frame();
            response.Command = (int) Command.UploadImage;
            response.Header = (int) Header.Response;
            response.Status = (int) Status.Ok;
            
            byte[] data = frame.Data;
            
            int imageInformationLength = GetImageInformationLength(data);

            byte[] image = GetImage(data, imageInformationLength);
            
            byte[] imageInformation = new byte[imageInformationLength];
            
            Array.Copy(data, 4, imageInformation, 0, imageInformationLength);

            string[] imageInformationToString = Encoding.UTF8.GetString(imageInformation).Split('#');
            string gameName = imageInformationToString[0];
            string imageName = imageInformationToString[1];
            string path = imageInformationToString[2];
            
            GameRepository repository = GameRepository.GetInstance();
            Game gameToUploadImage = repository.GetGame(gameName);

            if (gameToUploadImage == null)
            {
                response.Status = (int) Status.Error;
                response.Data = Encoding.UTF8.GetBytes("ERROR: Game not found.");
                response.DataLength = response.Data.Length;
            }
            else
            {
                gameToUploadImage.Image = Directory.GetCurrentDirectory() + "\\" + imageName;
            
                File.WriteAllBytes(Directory.GetCurrentDirectory() + "\\" + imageName, image);

                response.Data = Encoding.UTF8.GetBytes("Image uploaded.");
                response.DataLength = response.Data.Length;
            }
            
            return response;
        }

        private byte[] GetImage(byte[] data, int imageInformationLength)
        {
            int imageLength = data.Length - (imageInformationLength + 4);
            byte[] image = new byte[imageLength];
            int positionWhereImageStarts = imageInformationLength + 4;
            Array.Copy(data, positionWhereImageStarts, image, 0, imageLength);
            return image;
        }

        private int GetImageInformationLength(byte[] data)
        {
            byte[] imageDataLength = new byte[4];
            Array.Copy(data, imageDataLength, 4);
            int dataLength = BitConverter.ToInt32(imageDataLength);
            return dataLength;
        }

        private Frame CreateReviewResponse(Frame frame, User user)
        {
            Frame response = new Frame();
            response.Command = (int) Command.CreateReview;
            response.Header = (int) Header.Response;
            response.Status = (int) Status.Ok;
            
            string[] attributes = Encoding.UTF8.GetString(frame.Data).Split("#");
            string gameName = attributes[0];
            string score = attributes[1];
            string comment = attributes[2];

            string message = null;

            Review review = new Review();
            
            if (!String.IsNullOrEmpty(user.Username))
            {
                try
                {
                    review.Comment = comment;
                    review.Score = score;
                    review.User = user;
                    review.ValidReview();
                    GameRepository gameRepository = GameRepository.GetInstance();
                    Game game = gameRepository.GetGame(gameName);
                
                    if (game == null)
                    {
                        message = "ERROR: Game not found.";
                        response.Status = (int) Status.Error;
                    }
                    else
                    {
                        review.Game = game;
                        ReviewRepository reviewRepository = ReviewRepository.GetInstance();
                        reviewRepository.AddReview(review);
                        message = "Review created successfully.";
                    }

                    response.Data = Encoding.UTF8.GetBytes(message);
                    response.DataLength = response.Data.Length;
                
                    return response;
                }
                catch (InvalidReviewException e)
                {
                    response.Data = Encoding.UTF8.GetBytes(e.Message);
                    response.DataLength = response.Data.Length;
                    response.Status = (int) Status.Error;
                    return response;
                }
            }
            else
            {
                message = "ERROR: User must login before posting a review.";
                response.Data = Encoding.UTF8.GetBytes(message);
                response.DataLength = response.Data.Length;
                response.Status = (int) Status.Error;
                return response;
            }
            
            
        }

        private Frame CreateBuyGameResponse(Frame frame, User user)
        {
            Frame response = new Frame();
            response.Command = (int) Command.CreateReview;
            response.Header = (int) Header.Response;
            response.Status = (int) Status.Ok;
            string message = null;
            
            if (!String.IsNullOrEmpty(user.Username))
            {
               UserRepository userRepository = UserRepository.GetInstance();
               GameRepository gameRepository = GameRepository.GetInstance();
               
               User userToAddGame = userRepository.GetUser(user.Username);
               string gameName = Encoding.UTF8.GetString(frame.Data);
               Game gameThatUserWants = gameRepository.GetGame(gameName);
               if (gameThatUserWants != null)
               {
                   if (!userToAddGame.HasGame(gameThatUserWants.Title))
                   {
                       userToAddGame.Games.Add(gameThatUserWants);
                       message = "Game added to your library";
                       frame.Status = (int) Status.Error;
                   }
                   else
                   {
                       message = $"ERROR: User already has this game: {gameName}.";
                       frame.Status = (int) Status.Error;
                   }
               }
               else
               {
                   message = "ERROR: Game not found.";
                   frame.Status = (int) Status.Error;
               }
                
               response.Data = Encoding.UTF8.GetBytes(message);
               response.DataLength = response.Data.Length;
               return response;

            }
            else
            {
                message = "ERROR: User must login before buying a game.";
                response.Data = Encoding.UTF8.GetBytes(message);
                response.DataLength = response.Data.Length;
                return response;
            }
        }

        private Frame CreateShowGameReviewsResponse(Frame frame)
        {
            Frame response = new Frame();
            response.Command = (int) Command.ShowGameReviews;
            response.Header = (int) Header.Response;
            response.Status = (int) Status.Ok;

            string gameName = Encoding.UTF8.GetString(frame.Data);
            
            GameRepository gameRepository = GameRepository.GetInstance();
            ReviewRepository reviewRepository = ReviewRepository.GetInstance();

            Game gameToShow = gameRepository.GetGame(gameName);
            List<Review> reviewsOfGame = reviewRepository.GetReviews(gameToShow);
            List<byte[]> serializedReviews = SerializeReviews(reviewsOfGame);
            
            byte[] serializedList = SerializeListOfReviews(serializedReviews);

            response.Data = serializedList;
            response.DataLength = response.Data.Length;
            response.Status = (int) Status.Ok;
            return response;

        }
        
        private List<byte[]> SerializeReviews(List<Review> reviews)
        {
            List<byte[]> serializedReviews = new List<byte[]>();
            foreach (Review review in reviews)
            {
                byte[] data =
                    Encoding.UTF8.GetBytes($"{review.User.Username}#{review.Score}#{review.Comment}#");
                serializedReviews.Add(data);
            }

            return serializedReviews;
        }
        
        private byte[] SerializeListOfReviews(List<byte[]> reviewsSerialized)
        {
            List<byte> dataToReturn = new List<byte>();
            
            byte separator = 47;
            
            for (int i = 0; i < reviewsSerialized.Count; i++)
            {
                byte[] review = reviewsSerialized.ElementAt(i);
                dataToReturn.AddRange(review);

                if (i != reviewsSerialized.Count() - 1)
                {
                    dataToReturn.Add(separator);
                }
            }

            return dataToReturn.ToArray();
        }
    }
}