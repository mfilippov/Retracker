namespace Retracker.Domain
{
    public class PeerInfo
    {
        public State State { get; set; }
        public string IpAddress { get; set; }
        public int Port { get; set; }
    }
}