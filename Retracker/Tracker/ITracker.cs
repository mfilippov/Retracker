using System.Collections.Generic;
using System.Threading.Tasks;
using Retracker.Domain;

namespace Retracker.Tracker
{
    public interface ITracker
    {
        int Interval { get; }

        int MinInterval { get; }

        Task<TorrentInfo> ProcessAnnounce(string infoHash, TorrentEvent action, PeerInfo peer);
    }
}