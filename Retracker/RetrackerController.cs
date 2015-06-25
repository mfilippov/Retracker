using System;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Retracker.Domain;
using Retracker.Tracker;

namespace Retracker
{
    public class RetrackerController : Controller
    {
        private readonly ITracker _tracker;

        public RetrackerController(ITracker tracker)
        {
            _tracker = tracker;
        }

        [Route]
        public ActionResult Index()
        {
            return Content("OK");
        }

        [Route("announce")]
        public async Task<ActionResult> Announce(TorrentEvent @event, int port)
        {
            var queryString = Request.RawUrl.Split(new[] {'?'}, 2)[1];
            
            foreach (var param in queryString.Split('&'))
            {
                if (param.StartsWith("info_hash="))
                {
                    var parts = param.Split(new [] { '=' }, 2);
                    if (parts.Length == 2 && parts[1] != null)
                    {
                        var infoHash =
                            // ReSharper disable once AssignNullToNotNullAttribute
                            BitConverter.ToString(HttpUtility.UrlDecodeToBytes(parts[1]))
                                .Replace("-", string.Empty)
                                .ToLower();
                        var peer = new PeerInfo {IpAddress = Request.UserHostAddress, Port = port};
                        return new AnnounceResult(await _tracker.ProcessAnnounce(infoHash, @event, peer), _tracker.Interval, _tracker.MinInterval);
                    }
                }
            }
            return HttpNotFound();
        }
    }
}
