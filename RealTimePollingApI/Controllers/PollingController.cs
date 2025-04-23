using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RealTimePollingApI.Services;

namespace RealTimePollingApI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PollingController : ControllerBase
    {
        private readonly IPollingService _pollingService;
        public PollingController(IPollingService pollingService)
        {
            _pollingService = pollingService;   
        }
        [HttpPost("Handle")]
        public async Task<IActionResult> Handle(HttpContext context)
        {
            if (context.WebSockets.IsWebSocketRequest)
            {
                var socket = await context.WebSockets.AcceptWebSocketAsync();
                await _pollingService.Handle(socket);
                return Ok();
            }
            else
            {
                return BadRequest();
            }
        }
        [HttpGet("GetVotesOfPoll/{PollId}")]
        public async Task<IActionResult> GetVotesOfPoll(string PollId)
        {
            return Ok(await _pollingService.GetVotesOfPoll(PollId));
        }
    }
}
