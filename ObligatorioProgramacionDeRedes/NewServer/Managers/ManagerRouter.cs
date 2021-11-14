using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomExceptions;
using DataAccess;
using Domain;
using Protocol;

namespace NewServer.Managers
{
    public class ManagerRouter
    {
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
        
        private async Task<Frame> CreatePublishGameResponseAsync(Frame frame)
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
            }
            else
            {
                response.Data = Encoding.UTF8.GetBytes("ERROR: The catalog is empty.\n");
                response.DataLength = response.Data.Length;
                response.Status = (int) FrameStatus.Error;
            }
            
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

        private async Task<Frame> CreateSignUpResponseAsync(Frame frame)
        {
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
            }
            else
            {
                response.Data = Encoding.UTF8.GetBytes("ERROR: User already exist.\n");
                response.Status = (int) FrameStatus.Error;
                response.DataLength = response.Data.Length;
            }
            
            return response;
        }

        private async Task<Frame> CreateLogInResponseAsync(Frame frame, List<User> usersConnected)
        {
            Frame response = new Frame();
            response.CreateFrame((int)Header.Response, (int)Command.LogIn);
            
            string username = Encoding.UTF8.GetString(frame.Data);
            UserRepository repository = UserRepository.GetInstance();

            if (UserAlreadyLoggedIn(username, usersConnected))
            {
                response.Data = response.Data = Encoding.UTF8.GetBytes("ERROR: User already has an active session.\n");
                response.DataLength = response.Data.Length;
                response.Status = (int) FrameStatus.Error;
                return response;
            }
            if(await repository.UserExistsAsync(username))
            {
                response.Data = frame.Data;
                response.DataLength = response.Data.Length;
            }
            else
            {
                response.Data = Encoding.UTF8.GetBytes("ERROR: User does not exist.\n");
                response.DataLength = response.Data.Length;
                response.Status = (int) FrameStatus.Error;
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

        private async Task<Frame> CreateUploadImageResponseAsync(Frame frame)
        {
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
                response.Status = (int) FrameStatus.Error;
                response.Data = Encoding.UTF8.GetBytes("ERROR: Game not found.\n");
                response.DataLength = response.Data.Length;
            }
            else
            {
                gameToUploadImage.Image = Directory.GetCurrentDirectory() + "\\" + imageName;
            
                File.WriteAllBytes(Directory.GetCurrentDirectory() + "\\" + imageName, image);

                response.Data = Encoding.UTF8.GetBytes("Image uploaded.\n");
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
                
                    return response;
                }
                catch (InvalidReviewException e)
                {
                    response.Data = Encoding.UTF8.GetBytes(e.Message);
                    response.DataLength = response.Data.Length;
                    response.Status = (int) FrameStatus.Error;
                    return response;
                }
            }
            else
            {
                message = "ERROR: User must login before posting a review.\n";
                response.Data = Encoding.UTF8.GetBytes(message);
                response.DataLength = response.Data.Length;
                response.Status = (int) FrameStatus.Error;
                return response;
            }
        }

        private async Task<Frame> CreateBuyGameResponseAsync(Frame frame, User user)
        {
            Frame response = new Frame();
            response.CreateFrame((int)Header.Response, (int)Command.BuyGame);
            
            string message = null;
            
            if (!String.IsNullOrEmpty(user.Username))
            {
               UserRepository userRepository = UserRepository.GetInstance();
               GameRepository gameRepository = GameRepository.GetInstance();
               
               User userToAddGame = await userRepository.GetUserAsync(user.Username);
               string gameName = Encoding.UTF8.GetString(frame.Data);
               Game gameThatUserWants = await gameRepository.GetGameAsync(gameName);
               
               if (gameThatUserWants != null)
               {
                   if (!userToAddGame.HasGame(gameThatUserWants.Title))
                   {
                       userToAddGame.Games.Add(gameThatUserWants);
                       message = "Game added to your library.\n";
                       frame.Status = (int) FrameStatus.Error;
                   }
                   else
                   {
                       message = $"ERROR: User already has this game: {gameName}.\n";
                       frame.Status = (int) FrameStatus.Error;
                   }
               }
               else
               {
                   message = "ERROR: Game not found.\n";
                   frame.Status = (int) FrameStatus.Error;
               }
                
               response.Data = Encoding.UTF8.GetBytes(message);
               response.DataLength = response.Data.Length;
               return response;

            }
            else
            {
                message = "ERROR: User must login before buying a game.\n";
                response.Data = Encoding.UTF8.GetBytes(message);
                response.DataLength = response.Data.Length;
                return response;
            }
        }

        private async Task<Frame> CreateShowGameReviewsResponseAsync(Frame frame)
        {
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
                }
                else
                {
                    response.Data = Encoding.UTF8.GetBytes("Game has no reviews.\n");
                    response.DataLength = response.Data.Length;
                }
                
            }
            else
            {
                response.Data = Encoding.UTF8.GetBytes("ERROR: Game not found.\n");
                response.DataLength = response.Data.Length;
                response.Status = (int) FrameStatus.Error;
            }
            
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

        private async Task<Frame> CreateDeleteGameResponseAsync(Frame frame)
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
            }

            response.Data = Encoding.UTF8.GetBytes(message);
            response.DataLength = response.Data.Length;
            return response;
        }
        
        private async Task<Frame> CreateUpdateGameResponseAsync(Frame frame)
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
                message = "Game updated successfully.\n";
            }
            else
            {
                message = "ERROR: Could not find game.\n";
            }

            response.Data = Encoding.UTF8.GetBytes(message);
            response.DataLength = response.Data.Length;
            return response;
        }

        private async Task<Frame> CreateDownloadGameCoverResponseAsync(Frame frame)
        {
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
                }
                else
                {
                    response.Data = Encoding.UTF8.GetBytes("ERROR: Games does not contain a cover.\n");
                    response.DataLength = response.Data.Length;
                    response.Status = (int) FrameStatus.Error;
                }
                
            }
            else
            {
                response.Data = Encoding.UTF8.GetBytes("ERROR: Game not found.\n");
                response.DataLength = response.Data.Length;
                response.Status = (int) FrameStatus.Error;
            }
            
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