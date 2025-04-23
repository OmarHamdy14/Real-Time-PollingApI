using Microsoft.EntityFrameworkCore;
using RealTimePollingApI.Models;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

namespace RealTimePollingApI.Services
{
    public class PollingService : IPollingService
    {
        private readonly AppDbContext _appDbContext;
        public static readonly Dictionary<string,List<WebSocket>> _Pollsockets = new ();
        public PollingService(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task Handle(WebSocket socket)
        {
            var buffer = new byte[1024 * 4];
            string? PollId = null;
            while(socket.State == WebSocketState.Open)
            {
                var msgByte = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                if(msgByte.MessageType == WebSocketMessageType.Close)
                {
                    if(PollId != null)
                    {
                        _Pollsockets[PollId].Remove(socket);
                    }
                    await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closed", CancellationToken.None);
                    return;
                }
                var msgTostring = Encoding.UTF8.GetString(buffer,0, msgByte.Count);
                var VoteDetails = JsonSerializer.Deserialize<VoteMessage>(msgTostring);
                if (VoteDetails == null) continue;

                if(VoteDetails.What == "Join")
                {
                    PollId = VoteDetails.PollId;
                    if(!_Pollsockets.ContainsKey(PollId)) _Pollsockets[PollId] = new List<WebSocket>();   
                    _Pollsockets[PollId].Add(socket);
                    continue;
                }
                if(VoteDetails.What == "Vote")
                {
                    var Vote = new Vote()
                    {
                        Option = msgTostring,
                        PollId = PollId
                    };
                    _appDbContext.Votes.Add(Vote);

                    var VoteResult = JsonSerializer.Serialize(new
                    {
                        PollId = VoteDetails.PollId,
                        Option = VoteDetails.Option,
                        TimeStamp = Vote.TimeStamp
                    });

                    foreach(var ws in _Pollsockets[VoteDetails.PollId].Where(ws => ws.State == WebSocketState.Open))
                    {
                        await ws.SendAsync(Encoding.UTF8.GetBytes(VoteResult), WebSocketMessageType.Text, true, CancellationToken.None);
                    }
                }

            }
        }
        public async Task<List<Vote>> GetVotesOfPoll(string PollId)
        {
            return await _appDbContext.Votes.Where(ws => ws.PollId == PollId).ToListAsync();
        }
    }
}
