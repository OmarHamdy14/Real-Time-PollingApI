using RealTimePollingApI.Models;
using System.Net.WebSockets;

namespace RealTimePollingApI.Services
{
    public interface IPollingService
    {
        Task Handle(WebSocket socket);
        Task<List<Vote>> GetVotesOfPoll(string PollId);
    }
}
