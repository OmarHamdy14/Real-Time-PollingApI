namespace RealTimePollingApI.Models
{
    public class Vote
    {
        public int Id { get; set; }
        public string Option { get; set; }
        public DateTime TimeStamp { get; set; } = DateTime.UtcNow;
        public string PollId { get; set; } = string.Empty;
    }
}
