using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NewServer.Services
{
    public class GameService : GameAdmin.GameAdminBase
    {
        public override Task<CreateGameResponse> CreateGame(CreateGameRequest request, ServerCallContext context)
        {
            return Task.FromResult(new CreateGameResponse
            {
                Message = "created"
            });
        }

        public override Task<UpdateGameResponse> UpdateGame(UpdateGameRequest request, ServerCallContext context)
        {
            return Task.FromResult(new UpdateGameResponse
            {
                Message = "updated"
            });
        }

        public override Task<DeleteGameResponse> DeleteGame(DeleteGameRequest request, ServerCallContext context)
        {
            return Task.FromResult(new DeleteGameResponse
            {
                Message = "deleted"
            });
        }
    }
}
