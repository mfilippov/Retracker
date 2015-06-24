using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Text;
using System.Web.Mvc;
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
            context.HttpContext.Response.ContentType = "text/plain";
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

        public static byte[] Encode(TorrentInfo torrentInfo, int interval, int minInterval)
        {
            var bytes = Encoding.ASCII.GetBytes(new Bencode().StartMap().Append("complete").Append(torrentInfo.GetCompletedPeersCount()).Append("incomplete").Append(torrentInfo.GetIncompletedPeersCount()).Append("interval").Append(interval).Append("min interval").Append(minInterval).Append("peers").ToString() + torrentInfo.Peers.Count * 6);
            var result = new byte[bytes.Length + torrentInfo.Peers.Count * 6 + 2];
            var index = 0;
            for (; index < bytes.Length; index++)
            {
                result[index] = bytes[index];
            }
            result[index] = Encoding.ASCII.GetBytes(":")[0];
            var peersBytes = GetPeersAsBytesArray(torrentInfo.Peers);
            foreach (var t in peersBytes)
            {
                result[index++] = t;
            }
            result[index] = Encoding.ASCII.GetBytes("e")[0];
            
            return result;
        }

        private class Bencode
        {
            private readonly StringBuilder _builder = new StringBuilder();

            public Bencode Append(int value)
            {
                _builder.Append(string.Format(CultureInfo.InvariantCulture, "i{0}e", value));
                return this;
            }

            public override string ToString()
            {
                return _builder.ToString();
            }

            public Bencode Append(string value)
            {
                _builder.Append(string.Format(CultureInfo.InvariantCulture, "{0}:{1}", value.Length, value));
                return this;
            }

            public Bencode Append(byte[] bytes)
            {
                var @string = Encoding.ASCII.GetString(bytes);
                _builder.Append(string.Format(CultureInfo.InvariantCulture, "{0}:{1}", @string.Length, @string));
                return this;
            }

            public Bencode StartList()
            {
                _builder.Append("l");
                return this;
            }

            public Bencode EndList()
            {
                _builder.Append("e");
                return this;
            }

            public Bencode StartMap()
            {
                _builder.Append("d");
                return this;
            }

            public Bencode EndMap()
            {
                _builder.Append("e");
                return this;
            }
        }
    }
}