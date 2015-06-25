using System;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using Retracker.Domain;

namespace Retracker.Tracker
{
    public class MongoDbTracker : ITracker
    {
        private readonly IMongoCollection<TorrentInfo> _torrents;

        public int Interval { get; }

        public int MinInterval { get; }

        public MongoDbTracker(string connectionString, int interval, int minInterval)
        {
            _torrents = new MongoClient(connectionString).GetDatabase("tracker").GetCollection<TorrentInfo>("torrents");
            Interval = interval;
            MinInterval = minInterval;
        }

        public async Task<TorrentInfo> ProcessAnnounce(string infoHash, TorrentEvent @event, PeerInfo peer)
        {
            var torrent = await _torrents.Find(t => t.InfoHash == infoHash).FirstOrDefaultAsync();
            if (torrent != null)
            {
                var currentPeer = torrent.Peers.SingleOrDefault(p => p.IpAddress == peer.IpAddress);
                switch (@event)
                {
                    case TorrentEvent.Started:

                        if (currentPeer != null)
                        {
                            currentPeer.State = State.Downloading;
                            break;
                        }
                        torrent.Peers.Add(peer);
                        break;
                    case TorrentEvent.Stopped:
                        if (currentPeer != null)
                        {
                            torrent.Peers.Remove(currentPeer);
                        }
                        break;
                    case TorrentEvent.Completed:
                        if (currentPeer != null)
                        {
                            currentPeer.State = State.Downloaded;
                        }
                        break;
                    case TorrentEvent.None:
                        if (currentPeer != null)
                        {
                            torrent.Peers.Add(peer);
                        }
                        break;
                }
                torrent.LastUpdateDate = DateTime.Now;
                await _torrents.ReplaceOneAsync(t => t.Id == torrent.Id, torrent);
            }
            else
            {

                torrent = new TorrentInfo(infoHash);
                torrent.Peers.Add(peer);
                torrent.LastUpdateDate = DateTime.Now;
                await _torrents.InsertOneAsync(torrent);
            }

            return torrent;
        }
        
    }
}