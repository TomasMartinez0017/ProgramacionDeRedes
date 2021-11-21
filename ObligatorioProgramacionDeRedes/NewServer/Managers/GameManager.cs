using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess;
using Domain;
using LogsHelper;
using LogsServer.Domain;
using Newtonsoft.Json;
using Protocol;
namespace NewServer.Managers
{
    public class GameManager
    {
        private ResponseHandler responseHandler;
        private LogEmitter emitter;

        public GameManager()
        {
            responseHandler = new ResponseHandler();
            emitter = new LogEmitter();
        }

        public async Task<Frame> CreateGameAsync(Frame frame)
        {
            return await responseHandler.CreatePublishGameResponseAsync(frame);
        }
        
        public async Task<Frame> UpdateGameAsync(Frame frame)
        {
            return await responseHandler.CreateUpdateGameResponseAsync(frame);
        }
        
        public async Task<Frame> DeleteGameAsync(Frame frame)
        {
            return await responseHandler.CreateDeleteGameResponseAsync(frame);
        }

        public async Task<Frame> AssociateGameToUser(Frame frame)
        {
            Frame response = new Frame();
            response.CreateFrame((int)Header.Response, (int)Command.BuyGame);
            string[] data = Encoding.UTF8.GetString(frame.Data).Split('#');
            
            string message = null;
            UserRepository userRepository = UserRepository.GetInstance();
            GameRepository gameRepository = GameRepository.GetInstance();
               
            User userToAddGame = await userRepository.GetUserAsync(data[1]);
                
            Game gameThatUserWants = await gameRepository.GetGameAsync(data[0]);
               
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
                    message = $"ERROR: User already has this game: {data[0]}.\n";
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
            emitter.EmitLog(JsonConvert.SerializeObject(new LogInfo(userToAddGame.Username, gameThatUserWants.Title, message)), LogTag.BuyGame);

            return response;
        }
        
        public async Task<Frame> DissociateGameToUser(Frame frame)
        {
            Frame response = new Frame();
            response.CreateFrame((int)Header.Response, (int)Command.DeleteGameFromUser);
            string[] data = Encoding.UTF8.GetString(frame.Data).Split('#');
            
            string message = null;
            UserRepository userRepository = UserRepository.GetInstance();
            GameRepository gameRepository = GameRepository.GetInstance();
               
            User userToDeleteGame = await userRepository.GetUserAsync(data[1]);
                
            Game gameThatUserWantsToDelete = await gameRepository.GetGameAsync(data[0]);
               
            if (gameThatUserWantsToDelete != null)
            {
                if (userToDeleteGame.HasGame(gameThatUserWantsToDelete.Title))
                {
                    userToDeleteGame.Games.Remove(gameThatUserWantsToDelete);
                    message = "Game deleted from your library.\n";
                    response.Status = (int) FrameStatus.Ok;
                }
                else
                {
                    message = $"ERROR: User doesn't have this game: {data[0]}.\n";
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
            emitter.EmitLog(JsonConvert.SerializeObject(new LogInfo(userToDeleteGame.Username, gameThatUserWantsToDelete.Title, message)), LogTag.DeleteGameFromUser);

            return response;
        }


        public async Task<Frame> ShowGameAsync(Frame frame)
        {
            string message;

            Frame response = new Frame();
            response.CreateFrame((int)Header.Response, (int)Command.ShowGame);

            GameRepository repository = GameRepository.GetInstance();
            string gameName = Encoding.UTF8.GetString(frame.Data);

            Game gameToReturn = await repository.GetGameAsync(gameName);
            if (gameToReturn != null) {

               byte[] gameData =
               Encoding.UTF8.GetBytes($"{gameToReturn.Title}#{gameToReturn.Genre}#{gameToReturn.Rating}#{gameToReturn.Description}#{gameToReturn.Image}");

                response.Data = gameData;
                response.DataLength = gameData.Length;
                response.Status = (int)FrameStatus.Ok;
                message = "Game showed successfully";
            }
            else
            {
                message = "ERROR: Game not found.\n";
                response.Data = Encoding.UTF8.GetBytes(message);
                response.DataLength = response.Data.Length;
                response.Status = (int)FrameStatus.Error;
            }

             emitter.EmitLog(JsonConvert.SerializeObject(new LogInfo("Not a user related operation", gameName, message)), LogTag.ShowGame);

            return response;

        }
        
        








    }
}