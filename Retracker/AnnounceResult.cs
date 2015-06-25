using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Bencode;
using Retracker.Domain;

namespace Retracker
{
    public class AnnounceResult : ActionResult
    {
        private readonly TorrentInfo _torrentInfo;
        private readonly int _interval;
        private readonly int _minInterval;

        public AnnounceResult(TorrentInfo torrentInfo, int interval, int minInterval)
        {
            _torrentInfo = torrentInfo;
            _interval = interval;
            _minInterval = minInterval;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            context.HttpContext.Response.ContentType = "text/html";
            var buffer = Encode(_torrentInfo, _interval, _minInterval);
            context.HttpContext.Response.OutputStream.Write(buffer, 0, buffer.Length);
        }

        private static byte[] GetPeersAsBytesArray(IReadOnlyCollection<PeerInfo> peers)
        {
            var result = new byte[peers.Count * 6];
            var index = 0;
            foreach (var peerInfo in peers)
            {
                foreach (var b in IPAddress.Parse(peerInfo.IpAddress).GetAddressBytes())
                    result[index++] = b;
                foreach (var b in BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)peerInfo.Port)))
                    result[index++] = b;
            }
            return result;
        }

        private static byte[] Encode(TorrentInfo torrentInfo, int interval, int minInterval)
        {
            var response = new Dictionary<string, object>
            {
                ["complete"] = torrentInfo.GetCompletedPeersCount(),
                ["incomplete"] = torrentInfo.GetIncompletedPeersCount(),
                ["interval"] = interval,
                ["min interval"] = minInterval,
                ["peers"] = GetPeersAsBytesArray(torrentInfo.Peers)
            };
            
            return BencodeUtility.Encode(response).ToArray();
        }
        
    }
}