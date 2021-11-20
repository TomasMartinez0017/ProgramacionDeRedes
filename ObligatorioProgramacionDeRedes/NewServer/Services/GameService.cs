using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NewServer.Managers;
using Protocol;
using System.Text;

namespace NewServer.Services
{
    public class GameService : GameAdmin.GameAdminBase
    {
        private readonly GameManager gameManager;

        public GameService(GameManager manager)
        {
            this.gameManager = manager;
        }

        public override async Task<ShowGameResponse> ShowGame(ShowGameRequest request, ServerCallContext context)
        {
            ShowGameResponse gameResponse = new ShowGameResponse();
            Frame requestFrame = new Frame();
            byte[] gameData = Encoding.UTF8.GetBytes(request.Name);

            requestFrame.Data = gameData;
            requestFrame.DataLength = gameData.Length;

            Frame response = await gameManager.ShowGameAsync(requestFrame);


            if (response.Status == 0)
            {
                
                string[] data = Encoding.UTF8.GetString(response.Data).Split('#');
                gameResponse.Title = data[0];
                gameResponse.Genre = data[1];
                gameResponse.Rating = data[2];
                gameResponse.Description = data[3];
                gameResponse.Image = data[4];
                gameResponse.Ok = true;
            }
            else
            {
                gameResponse.Ok = false;
                gameResponse.Message = "Error showing game";
            }

            return gameResponse;


        }
        

        public override async Task<CreateGameResponse> CreateGame(CreateGameRequest request, ServerCallContext context)
        {
            CreateGameResponse gameResponse = new CreateGameResponse();
            if (Int32.Parse(request.Rating)<1 || Int32.Parse(request.Rating)>4)
            {
                gameResponse.Ok = false;
                gameResponse.Message = "Error creating game";
                return gameResponse;
            }

            Frame requestFrame = new Frame();
            byte[] gameData = Encoding.UTF8.GetBytes(request.Title + "#" + request.Genre + "#" + request.Rating + "#" + request.Description);
            requestFrame.Data = gameData;
            requestFrame.DataLength = gameData.Length;

            Frame response = await gameManager.CreateGameAsync(requestFrame);

            if (response.Status == 0)
            {

                gameResponse.Ok = true;
                gameResponse.Message = "Game created";
            }
            else
            {
                gameResponse.Ok = false;
                gameResponse.Message = "Error creating game";
            }

            return gameResponse;

        }

        public override async Task<UpdateGameResponse> UpdateGame(UpdateGameRequest request, ServerCallContext context)
        {
            UpdateGameResponse gameResponse = new UpdateGameResponse();
            if (Int32.Parse(request.Rating)<0 || Int32.Parse(request.Rating) > 4)
            {
                gameResponse.Ok = false;
                gameResponse.Message = "Error updating game";
                return gameResponse;
            }
            
            Frame requestFrame = new Frame();
            byte[] gameData = Encoding.UTF8.GetBytes(request.OldTitle + "#" + request.Title + "#" + request.Genre + "#" + request.Rating + "#" + request.Description);
            requestFrame.Data = gameData;
            requestFrame.DataLength = gameData.Length;
            
            Frame response = await gameManager.UpdateGameAsync(requestFrame);
            
            if (response.Status == 0)
            {
                
                gameResponse.Ok = true;
                gameResponse.Message = "Game Updated";
            }
            else
            {
                gameResponse.Ok = false;
                gameResponse.Message = "Error updating game";
            }

            return gameResponse;
            
        }

        public override async Task<DeleteGameResponse> DeleteGame(DeleteGameRequest request, ServerCallContext context)
        {
            Frame requestFrame = new Frame();
            byte[] userData = Encoding.UTF8.GetBytes(request.Title);
            requestFrame.Data = userData;
            requestFrame.DataLength = userData.Length;
            
            Frame response = await gameManager.DeleteGameAsync(requestFrame);

            DeleteGameResponse gameResponse = new DeleteGameResponse();
            
            if (response.Status == 0)
            {
                gameResponse.Ok = true;
                gameResponse.Message = "Game Deleted";
            }
            else
            {
                gameResponse.Ok = false;
                gameResponse.Message = "Error deleting game";
            }

            return gameResponse;
        }
        
        public override async Task<AssociateGameResponse> GameAssociation(GameAssociationRequest request, ServerCallContext context)
        {
            Frame requestFrame = new Frame();
            byte[] data = Encoding.UTF8.GetBytes(request.Title + "#" + request.Username);
            requestFrame.Data = data;
            requestFrame.DataLength = data.Length;
            
            Frame response = await gameManager.AssociateGameToUser(requestFrame);

            AssociateGameResponse gameResponse = new AssociateGameResponse();
            
            if (response.Status == 0)
            {
                gameResponse.Ok = true;
                gameResponse.Message = "Game associated to user";
            }
            else
            {
                gameResponse.Ok = false;
                gameResponse.Message = "Error associating game";
            }
            return gameResponse;
        }
        
        public override async Task<DissociationGameResponse> GameDissociation(GameAssociationRequest request, ServerCallContext context)
        {
            Frame requestFrame = new Frame();
            byte[] data = Encoding.UTF8.GetBytes(request.Title + "#" + request.Username);
            requestFrame.Data = data;
            requestFrame.DataLength = data.Length;
            
            Frame response = await gameManager.DissociateGameToUser(requestFrame);

            DissociationGameResponse gameResponse = new DissociationGameResponse();
            
            if (response.Status == 0)
            {
                gameResponse.Ok = true;
                gameResponse.Message = "Game dissociated from user";
            }
            else
            {
                gameResponse.Ok = false;
                gameResponse.Message = "Error dissociating game from user";
            }
            return gameResponse;
        }
    }
}
