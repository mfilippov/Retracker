using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson;

namespace Retracker.Domain
{
    public class TorrentInfo
    {
        public ObjectId Id { get; set; }
        public string InfoHash { get; set; }
        public List<PeerInfo> Peers { get; set; }
        public DateTime LastUpdateDate { get; set; }

        public TorrentInfo()
        {
            Peers = new List<PeerInfo>();
        }

        public TorrentInfo(string infoHash) : this()
        {
            InfoHash = infoHash;
        }

        public int GetCompletedPeersCount()
        {
            return Peers.Count(p => p.State == State.Downloaded);
        }

        public int GetIncompletedPeersCount()
        {
            return Peers.Count(p => p.State == State.Downloading);
        }
       
    }
}