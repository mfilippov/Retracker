using Microsoft.Owin;
using Owin;
using Retracker;

[assembly: OwinStartup(typeof(Startup))]
namespace Retracker
{
    public class Startup
    {
        public void Configuration(IAppBuilder app) {}
    }
}