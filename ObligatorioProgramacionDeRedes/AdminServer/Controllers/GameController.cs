using AdminServer.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain;

namespace AdminServer.Controllers
{
    [ApiController]
    [Route("api/games")]
    public class GameController : ControllerBase
    {
        private readonly GameAdmin.GameAdminClient gameClient;

        public GameController(GameAdmin.GameAdminClient client)
        {
            this.gameClient = client;
        }

        [HttpGet("{gameName}", Name = "GetGame")]
        public async Task<IActionResult> GetByName(string gameName)
        {
            ShowGameRequest request = new ShowGameRequest()
            {
                Name = gameName
            };

            ShowGameResponse response = await gameClient.ShowGameAsync(request);

            if (response.Ok)
            {
                return Ok(response);
            }
            else
            {
                return BadRequest(response.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateGame([FromBody] GameRequestDTO newGame)
        {
            CreateGameRequest request = new CreateGameRequest()
            {
                Title = newGame.Title,
                Genre = newGame.Genre,
                Rating = newGame.Rating,
                Description = newGame.Description
            };

            CreateGameResponse response = await gameClient.CreateGameAsync(request);

            if (response.Ok)
            {
                Game gameCreated = new Game();
                gameCreated.Title = request.Title;
                gameCreated.Genre = request.Genre;
                gameCreated.SetRating(request.Rating.ToString());
                gameCreated.Description = request.Description;
                
                return CreatedAtRoute("GetGame", new { gameName = gameCreated.Title }, gameCreated);
            }
            else
            {
                return BadRequest(response.Message);
            }
        }

        [HttpPut("{gameTitle}")]
        public async Task<IActionResult> UpdateGame(string gameTitle, GameRequestDTO gameUpdated)
        {
            UpdateGameRequest request = new UpdateGameRequest()
            {
                OldTitle = gameTitle,
                Title = gameUpdated.Title,
                Genre = gameUpdated.Genre,
                Rating = gameUpdated.Rating, 
                Description = gameUpdated.Description
            };

            UpdateGameResponse response = await gameClient.UpdateGameAsync(request);
            
            if (response.Ok)
            {
                return Ok(response);
            }
            else
            {
                return BadRequest(response.Message);
            }
        }

        [HttpDelete("{gameTitle}")]
        public async Task<IActionResult> DeleteGame(string gameTitle)
        {
            DeleteGameRequest request = new DeleteGameRequest()
            {
                Title = gameTitle
            };

            DeleteGameResponse response = await gameClient.DeleteGameAsync(request);
            
            if (response.Ok)
            {
                return Ok(response);
            }
            else
            {
                return BadRequest(response.Message);
            }
        }

        [HttpPut("{gameTitle}/{userName}")]
        public async Task<IActionResult> AssociateGameToUser(string gameTitle, string userName)
        {
            GameAssociationRequest request = new GameAssociationRequest()
            {
                Title = gameTitle,
                Username = userName
            };

            AssociateGameResponse response = await gameClient.GameAssociationAsync(request);
            if (response.Ok)
            {
                return Ok(response);
            }
            else
            {
                return BadRequest(response.Message);
            }
        }

        [HttpDelete("{gameTitle}/{userName}")]
        public async Task<IActionResult> DisassociateGameToUser(string gameTitle, string userName)
        {
            GameAssociationRequest request = new GameAssociationRequest()
            {
                Title = gameTitle,
                Username = userName
            };

            DissociationGameResponse response = await gameClient.GameDissociationAsync(request);
            
            if (response.Ok)
            {
                return Ok(response);
            }
            else
            {
                return BadRequest(response.Message);
            }
        }
        
    }
}
