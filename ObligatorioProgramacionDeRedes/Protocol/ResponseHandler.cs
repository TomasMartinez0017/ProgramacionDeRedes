using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Configuration;
using System.Text;
using System.Threading.Tasks;
using CustomExceptions;
using DataAccess;
using Domain;
using LogsHelper;
using LogsServer.Domain;
using Newtonsoft.Json;

namespace Protocol
{
    public class ResponseHandler
    {
        private LogEmitter emitter = new LogEmitter();

        public string ProcessResponse(Frame frame)
        {
            string response = null;
            
            switch ((Command) frame.Command)
            {
                case Command.ShowCatalog:
                    response = ProcessShowCatalogResponse(frame);
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
                    response = ProcessShowGameReviews(frame);
                    break;
                case Command.DeleteGame:
                    response = Encoding.UTF8.GetString(frame.Data);
                    break;
                case Command.UpdateGame:
                    response=Encoding.UTF8.GetString(frame.Data);
                    break;
                case Command.DownLoadImage:
                    response = ProcessDownloadImageResponse(frame);
                    break;
                case Command.SearchGame:
                    response = ProcessSearchGameResponse(frame.Data);
                    break;
            }
            return response;
        }
        private string ProcessShowGameReviews(Frame frame)
        {
            if (frame.Status.Equals(0))
            {
                string[] reviews = Encoding.UTF8.GetString(frame.Data).Split('/');
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
                reviewsResponse = reviewsResponse + "\n" + "Score Average: " + scoreAverage + "\n";
            
                return reviewsResponse;
            }
            else
            {
                return Encoding.UTF8.GetString(frame.Data);
            }
            
        }
        private string ProcessShowCatalogResponse(Frame frame)
        {
            if (frame.Status.Equals(0))
            {
                return ProcessListOfGames(frame.Data);
            }
            else
            {
                return Encoding.UTF8.GetString(frame.Data);
            }
            
        }

        private string ProcessListOfGames(byte[] data)
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
                        catalogResponse = catalogResponse + "Genre: " + gameGenre + "\n";
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

        private string ProcessDownloadImageResponse(Frame frame)
        {
            if (frame.Status.Equals(0))
            {
                byte[] data = frame.Data;
                int imageInformationLength = GetImageInformationLength(data);
                byte[] image = GetImage(data, imageInformationLength);
                byte[] imageInformation = new byte[imageInformationLength];
                Array.Copy(data, 4, imageInformation, 0, imageInformationLength);
                string imageName = Encoding.UTF8.GetString(imageInformation);
                File.WriteAllBytes(ConfigurationManager.AppSettings["DownloadPath"]+imageName,image);
                string response = "Image downloaded successfully.\n";
                return response;
            }
            else
            {
                return Encoding.UTF8.GetString(frame.Data);
            }
            
        }

        private string ProcessSearchGameResponse(byte[] data)
        {
            return ProcessListOfGames(data);
        }

        public async Task<Frame> GetResponseAsync(Frame frame, List<User> usersConnected, User _userConnected)
        {
            Frame response = null;

            switch ((Command) frame.Command)
            {
                case Command.ShowCatalog:
                    response = await CreateShowCatalogResponseAsync(frame);
                    break;
                case Command.PublishGame:
                    response = await CreatePublishGameResponseAsync(frame);
                    break;
                case Command.SignUp:
                    response = await CreateSignUpResponseAsync(frame);
                    break;
                case Command.LogIn:
                    response = await CreateLogInResponseAsync(frame, usersConnected);
                    break;
                case Command.UploadImage:
                    response = await CreateUploadImageResponseAsync(frame);
                    break;
                case Command.CreateReview:
                    response = await CreateReviewResponseAsync(frame, _userConnected);
                    break;
                case Command.BuyGame:
                    response = await CreateBuyGameResponseAsync(frame, _userConnected);
                    break;
                case Command.ShowGameReviews:
                    response = await CreateShowGameReviewsResponseAsync(frame);
                    break;
                case Command.DeleteGame:
                    response = await CreateDeleteGameResponseAsync(frame);
                    break;
                case Command.UpdateGame:
                    response = await CreateUpdateGameResponseAsync(frame);
                    break;
                case Command.DownLoadImage:
                    response = await CreateDownloadGameCoverResponseAsync(frame);
                    break;
                case Command.SearchGame:
                    response = await CreateSearchGameResponseAsync(frame);
                    break;
            }

            return response;
        }

        public async Task<Frame> CreatePublishGameResponseAsync(Frame frame)
        {
            Frame response = new Frame();
            response.CreateFrame((int)Header.Response, (int)Command.PublishGame);
            try
            {
                Game newGame = ExtractGameData(frame);
                newGame.ValidGame();
                GameRepository repository = GameRepository.GetInstance();
                
                string message = null;

                if (!await repository.GameExistsAsync(newGame))
                {
                    await repository.AddGameAsync(newGame);
                    message = "Game published.\n";
                }
                else
                {
                    message = "ERROR: The game you are trying to publish already exists.\n";
                    response.Status = (int) FrameStatus.Error;
                }

                response.Data = Encoding.UTF8.GetBytes(message);
                response.DataLength = response.Data.Length;
                emitter.EmitLog(JsonConvert.SerializeObject(new LogInfo("Not a user related operation", newGame.Title, message)), LogTag.PublishGame);
                return response;
            }
            catch (InvalidGameException e)
            {
                response.Data = Encoding.UTF8.GetBytes(e.Message);
                response.DataLength = response.Data.Length;
                response.Status = (int) FrameStatus.Error;
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

        private async Task<Frame> CreateShowCatalogResponseAsync(Frame frame)
        {
            string message;

            Frame response = new Frame();
            response.CreateFrame((int)Header.Response, (int)Command.ShowCatalog);
            
            GameRepository repository = GameRepository.GetInstance();
            List<Game> gamesInRepository = await repository.GetAllGamesAsync();

            if (gamesInRepository.Count != 0)
            {
                List<byte[]> serializedGames = SerializeGames(gamesInRepository);
                byte[] serializedList = SerializeListOfGames(serializedGames);

                response.Data = serializedList;
                response.DataLength = response.Data.Length;
                message = "Catalog showed successfully";
            }
            else
            {
                message = "ERROR: The catalog is empty.\n";
                response.Data = Encoding.UTF8.GetBytes(message);
                response.DataLength = response.Data.Length;
                response.Status = (int) FrameStatus.Error;
            }
            emitter.EmitLog(JsonConvert.SerializeObject(new LogInfo("Not a user related operation","Not a game related operation", message)), LogTag.ShowCatalog);
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
            
            byte separator = 47; //el 47 representa "/" en bytes
            
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

        public async Task<Frame> CreateSignUpResponseAsync(Frame frame)
        {
            string message;

            Frame response = new Frame();
            response.CreateFrame((int)Header.Response, (int)Command.SignUp);
            
            string username = Encoding.UTF8.GetString(frame.Data);

            UserRepository repository = UserRepository.GetInstance();

            if (!await repository.UserExistsAsync(username))
            {
                User userToAdd = new User();
                userToAdd.Username = username;
                await repository.AddUserAsync(userToAdd);
                
                response.Data = frame.Data;
                response.DataLength = response.Data.Length;
                message = "User added successfully";
            }
            else
            {
                message = "ERROR: User already exist.\n";
                response.Data = Encoding.UTF8.GetBytes(message);
                response.Status = (int) FrameStatus.Error;
                response.DataLength = response.Data.Length;
            }

            emitter.EmitLog(JsonConvert.SerializeObject(new LogInfo(username,"Not a game related operation", message)), LogTag.SignUp);
            return response;
        }

        private async Task<Frame> CreateLogInResponseAsync(Frame frame, List<User> usersConnected)
        {
            string message;

            Frame response = new Frame();
            response.CreateFrame((int)Header.Response, (int)Command.LogIn);
            
            string username = Encoding.UTF8.GetString(frame.Data);
            UserRepository repository = UserRepository.GetInstance();

            if (UserAlreadyLoggedIn(username, usersConnected))
            {
                message = "ERROR: User already has an active session.\n";
                response.Data = response.Data = Encoding.UTF8.GetBytes(message);
                response.DataLength = response.Data.Length;
                response.Status = (int) FrameStatus.Error;
                return response;
            }
            if(await repository.UserExistsAsync(username))
            {
                response.Data = frame.Data;
                response.DataLength = response.Data.Length;
                message = "User logged in successfully";
            }
            else
            {
                message = "ERROR: User does not exist.\n";
                response.Data = Encoding.UTF8.GetBytes(message);
                response.DataLength = response.Data.Length;
                response.Status = (int) FrameStatus.Error;
            }

            emitter.EmitLog(JsonConvert.SerializeObject(new LogInfo(username,"Not a game related operation", message)), LogTag.LogIn);
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

        private async Task<Frame> CreateUploadImageResponseAsync(Frame frame)
        {
            string message;
            
            Frame response = new Frame();
            response.CreateFrame((int)Header.Response, (int)Command.UploadImage);
            
            byte[] data = frame.Data;
            
            int imageInformationLength = GetImageInformationLength(data);

            byte[] image = GetImage(data, imageInformationLength);
            
            byte[] imageInformation = new byte[imageInformationLength];
            
            Array.Copy(data, 4, imageInformation, 0, imageInformationLength);

            string[] imageInformationToString = Encoding.UTF8.GetString(imageInformation).Split('#');
            string gameName = imageInformationToString[0];
            string imageName = imageInformationToString[1];
            
            GameRepository repository = GameRepository.GetInstance();
            Game gameToUploadImage = await repository.GetGameAsync(gameName);

            if (gameToUploadImage == null)
            {
                message = "ERROR: Game not found.\n";
                response.Status = (int) FrameStatus.Error;
                response.Data = Encoding.UTF8.GetBytes(message);
                response.DataLength = response.Data.Length;
            }
            else
            {
                message = "Image uploaded.\n";
                gameToUploadImage.Image = Directory.GetCurrentDirectory() + "\\" + imageName;
            
                File.WriteAllBytes(Directory.GetCurrentDirectory() + "\\" + imageName, image);

                response.Data = Encoding.UTF8.GetBytes(message);
                response.DataLength = response.Data.Length;
            }

            emitter.EmitLog(JsonConvert.SerializeObject(new LogInfo("Not a user related operation", gameName, message)), LogTag.UploadImage);
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

        private async Task<Frame> CreateReviewResponseAsync(Frame frame, User user)
        {
            Frame response = new Frame();
            response.CreateFrame((int)Header.Response, (int)Command.CreateReview);
            
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
                    Game game = await gameRepository.GetGameAsync(gameName);
                
                    if (game == null)
                    {
                        message = "ERROR: Game not found.\n";
                        response.Status = (int) FrameStatus.Error;
                    }
                    else
                    {
                        review.Game = game;
                        ReviewRepository reviewRepository = ReviewRepository.GetInstance();
                        await reviewRepository.AddReviewAsync(review);
                        message = "Review created successfully.\n";
                    }

                    response.Data = Encoding.UTF8.GetBytes(message);
                    response.DataLength = response.Data.Length;
                    emitter.EmitLog(JsonConvert.SerializeObject(new LogInfo(user.Username, gameName, message)), LogTag.CreateReview);

                    return response;
                }
                catch (InvalidReviewException e)
                {
                    response.Data = Encoding.UTF8.GetBytes(e.Message);
                    response.DataLength = response.Data.Length;
                    response.Status = (int) FrameStatus.Error;
                    emitter.EmitLog(JsonConvert.SerializeObject(new LogInfo(user.Username, gameName, e.Message)), LogTag.CreateReview);

                    return response;
                }
            }
            else
            {
                message = "ERROR: User must login before posting a review.\n";
                response.Data = Encoding.UTF8.GetBytes(message);
                response.DataLength = response.Data.Length;
                response.Status = (int) FrameStatus.Error;
                emitter.EmitLog(JsonConvert.SerializeObject(new LogInfo(user.Username, gameName, message)), LogTag.CreateReview);

                return response;
            }
        }

        public async Task<Frame> CreateBuyGameResponseAsync(Frame frame, User user)
        {
            Frame response = new Frame();
            response.CreateFrame((int)Header.Response, (int)Command.BuyGame);
            
            string message = null;
            string gameName = Encoding.UTF8.GetString(frame.Data);
            
            if (!String.IsNullOrEmpty(user.Username))
            {
               UserRepository userRepository = UserRepository.GetInstance();
               GameRepository gameRepository = GameRepository.GetInstance();
               
               User userToAddGame = await userRepository.GetUserAsync(user.Username);
               
               Game gameThatUserWants = await gameRepository.GetGameAsync(gameName);
               
               if (gameThatUserWants != null)
               {
                   if (!userToAddGame.HasGame(gameThatUserWants.Title))
                   {
                       userToAddGame.Games.Add(gameThatUserWants);
                       message = "Game added to your library.\n";
                       response.Status = (int) FrameStatus.Ok;
                   }
                   else
                   {
                       message = $"ERROR: User already has this game: {gameName}.\n";
                       response.Status = (int) FrameStatus.Error;
                   }
               }
               else
               {
                   message = "ERROR: Game not found.\n";
                   response.Status = (int) FrameStatus.Error;
               }
                
               response.Data = Encoding.UTF8.GetBytes(message);
               response.DataLength = response.Data.Length;
               emitter.EmitLog(JsonConvert.SerializeObject(new LogInfo(user.Username, gameName, message)), LogTag.BuyGame);

               return response;

            }
            else
            {
                message = "ERROR: User must login before buying a game.\n";
                response.Data = Encoding.UTF8.GetBytes(message);
                response.DataLength = response.Data.Length;
                emitter.EmitLog(JsonConvert.SerializeObject(new LogInfo(user.Username, gameName, message)), LogTag.BuyGame);

                return response;
            }
        }

        private async Task<Frame> CreateShowGameReviewsResponseAsync(Frame frame)
        {
            string message;

            Frame response = new Frame();
            response.CreateFrame((int)Header.Response, (int)Command.ShowGameReviews);;

            string gameName = Encoding.UTF8.GetString(frame.Data);
            
            GameRepository gameRepository = GameRepository.GetInstance();
            ReviewRepository reviewRepository = ReviewRepository.GetInstance();

            Game gameToShow = await gameRepository.GetGameAsync(gameName);
            
            if (gameToShow != null)
            {
                List<Review> reviewsOfGame = await reviewRepository.GetReviewsAsync(gameToShow);

                if (reviewsOfGame.Count != 0)
                {
                    List<byte[]> serializedReviews = SerializeReviews(reviewsOfGame);
            
                    byte[] serializedList = SerializeListOfReviews(serializedReviews);

                    response.Data = serializedList;
                    response.DataLength = response.Data.Length;
                    message = "Game reviews showed successfully";
                }
                else
                {
                    message = "Game has no reviews.\n";
                    response.Data = Encoding.UTF8.GetBytes(message);
                    response.DataLength = response.Data.Length;
                }
                
            }
            else
            {
                message = "ERROR: Game not found.\n";
                response.Data = Encoding.UTF8.GetBytes(message);
                response.DataLength = response.Data.Length;
                response.Status = (int) FrameStatus.Error;
            }

            emitter.EmitLog(JsonConvert.SerializeObject(new LogInfo("Not a user related operation", gameName, message)), LogTag.ShowGameReviews);

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

        public async Task<Frame> CreateDeleteGameResponseAsync(Frame frame)
        {
            Frame response = new Frame();
            response.CreateFrame((int)Header.Response, (int)Command.DeleteGame);
            string message = null;

            string gameName = Encoding.UTF8.GetString(frame.Data);
            
            GameRepository gameRepository = GameRepository.GetInstance();
            UserRepository userRepository = UserRepository.GetInstance();
            ReviewRepository reviewRepository = ReviewRepository.GetInstance();
            
            Game gameSearched = new Game();
            gameSearched.Title = gameName;
            
            if (await gameRepository.GameExistsAsync(gameSearched))
            {
                await gameRepository.DeleteGameAsync(gameName);
                await userRepository.DeleteBoughtGameAsync(gameName);
                await reviewRepository.DeleteReviewAsync(gameName);
                
                message = "Game deleted successfully.\n";
            }
            else
            {
                message = "ERROR: Could not find game.\n";
                response.Status = (int) FrameStatus.Error;
            }

            response.Data = Encoding.UTF8.GetBytes(message);
            response.DataLength = response.Data.Length;
            emitter.EmitLog(JsonConvert.SerializeObject(new LogInfo("Not a user related operation", gameName, message)), LogTag.DeleteGame);

            return response;
        }
        
        public async Task<Frame> CreateUpdateGameResponseAsync(Frame frame)
        {
            Frame response = new Frame();
            response.CreateFrame((int)Header.Response, (int)Command.UpdateGame);
            string message = null;
            string[] attributes = Encoding.UTF8.GetString(frame.Data).Split("#");

            string gameNameSearched = attributes[0];
            Game gameSearched = new Game();
            gameSearched.Title = gameNameSearched;
            
            GameRepository gameRepository = GameRepository.GetInstance();
            Game gameUpdated = new Game();
            gameUpdated.Title = attributes[1];
            gameUpdated.Genre = attributes[2];
            gameUpdated.Rating = attributes[3];
            gameUpdated.Description = attributes[4];
            
            if (await gameRepository.GameExistsAsync(gameSearched))
            {
                await gameRepository.UpdateGameAsync(gameNameSearched, gameUpdated);
                if (!string.IsNullOrEmpty(gameUpdated.Title))
                {
                    message = $"Game updated name title to {gameUpdated.Title} successfully.\n";
                    
                }
                else
                {
                    message = "Game updated successfully.\n"; 
                }
            }
            else
            {
                message = "ERROR: Could not find game.\n";
            }

            response.Data = Encoding.UTF8.GetBytes(message);
            response.DataLength = response.Data.Length;
            emitter.EmitLog(JsonConvert.SerializeObject(new LogInfo("Not a user related operation", gameSearched.Title, message)), LogTag.UpdateGame);

            return response;
        }

        private async Task<Frame> CreateDownloadGameCoverResponseAsync(Frame frame)
        {
            string message;

            Frame response = new Frame();
            response.CreateFrame((int)Header.Response, (int)Command.DownLoadImage);
            string gameName = Encoding.UTF8.GetString(frame.Data);
            
            GameRepository gameRepository=GameRepository.GetInstance();
            Game gameSearched = await gameRepository.GetGameAsync(gameName);
            
            if (gameSearched != null)
            {
                if (!string.IsNullOrEmpty(gameSearched.Image))
                {
                    string nameOfImage = new FileInfo(gameSearched.Image).Name;
                    byte[] imageData = File.ReadAllBytes(gameSearched.Image);
                    List<byte> imageResponseData = new List<byte>();
                    int lengthOfImageName = nameOfImage.Length;
                
                    imageResponseData.AddRange(BitConverter.GetBytes(lengthOfImageName));
                    imageResponseData.AddRange(Encoding.UTF8.GetBytes($"{nameOfImage}"));
                    imageResponseData.AddRange(imageData);
                
                    response.Data = imageResponseData.ToArray();
                    response.DataLength = response.Data.Length;
                    message = "Image uploaded successfully";
                }
                else
                {
                    message = "ERROR: Games does not contain a cover.\n";
                    response.Data = Encoding.UTF8.GetBytes(message);
                    response.DataLength = response.Data.Length;
                    response.Status = (int) FrameStatus.Error;
                }
                
            }
            else
            {
                message = "ERROR: Game not found.\n";
                response.Data = Encoding.UTF8.GetBytes(message);
                response.DataLength = response.Data.Length;
                response.Status = (int) FrameStatus.Error;
            }

            emitter.EmitLog(JsonConvert.SerializeObject(new LogInfo("Not a user related operation", gameName, message)), LogTag.UploadImage);
            
            return response;
        }

        private async Task<Frame> CreateSearchGameResponseAsync(Frame frame)
        {
            Frame response = new Frame();
            response.CreateFrame((int)Header.Response, (int)Command.SearchGame);

            string[] data = Encoding.UTF8.GetString(frame.Data).Split("#");

            string gameName = data[0];
            string genre = data[1];
            string rating = data[2];

            List<Game> gamesWithGenre = null;
            List<Game> gamesWithRating = null;
            List<Game> joinedGamesWithGenreAndGamesWithRating = null;
            List<Game> joinedList = null;
            GameRepository gameRepository=GameRepository.GetInstance();

            if (!string.IsNullOrEmpty(genre)) gamesWithGenre = await gameRepository.GetGamesWithGenreAsync(genre);
            if (!string.IsNullOrEmpty(rating)) gamesWithRating = await gameRepository.GetGamesWithRatingAsync(rating);
            Game game = await gameRepository.GetGameAsync(gameName);

            if (gamesWithGenre == null && gamesWithRating != null)
            {
                joinedGamesWithGenreAndGamesWithRating = gamesWithRating;
            }
            else if (gamesWithGenre != null && gamesWithRating == null)
            {
                joinedGamesWithGenreAndGamesWithRating = gamesWithGenre;
            }
            else if(gamesWithGenre!=null && gamesWithRating!=null)
            {
                joinedGamesWithGenreAndGamesWithRating = JoinListOfGamesGenreAndRating(gamesWithGenre, gamesWithRating);
            }

            if (game == null && joinedGamesWithGenreAndGamesWithRating == null)
            {
                joinedList = new List<Game>();
            }
            else if(game!=null && joinedGamesWithGenreAndGamesWithRating == null)
            {
                joinedList = new List<Game>();
                joinedList.Add(game);
            }
            else if (game == null && joinedGamesWithGenreAndGamesWithRating != null)
            {
                joinedList = joinedGamesWithGenreAndGamesWithRating;
            }
            else if (game != null && joinedGamesWithGenreAndGamesWithRating != null)
            {
                joinedList = JoinTitleGenreAndRating(gameName, joinedGamesWithGenreAndGamesWithRating);
            }
            
            List<byte[]> serializedGames = SerializeGames(joinedList);
            byte[] serializedList = SerializeListOfGames(serializedGames);
            response.Data = serializedList;
            response.DataLength = response.Data.Length;
            emitter.EmitLog(JsonConvert.SerializeObject(new LogInfo("Not a user related operation",gameName, "Search operation completed")), LogTag.SearchGame);

            return response;
        }

        private List<Game> JoinListOfGamesGenreAndRating(List<Game> gamesWithGenre, List<Game> gamesWithRating)
        {
            List<Game> joinedList = new List<Game>();
            foreach (Game gameGenre in gamesWithGenre)
            {
                foreach (Game gameRating in gamesWithRating)
                {
                    if (gameGenre.Title.Equals(gameRating.Title))
                    {
                        joinedList.Add(gameGenre);
                    }
                }
            }
            return joinedList;
        }
        
        private List<Game> JoinTitleGenreAndRating(string gameName, List<Game> joinedListRatingAndGenre)
        {
            List<Game> joinedList = new List<Game>();
            foreach (Game game in joinedListRatingAndGenre)
            {
                if (game.Title.Equals(gameName))
                {
                    joinedList.Add(game);
                }
            }

            return joinedList;
        }
    }
}