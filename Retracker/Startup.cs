using Microsoft.Owin;
using Owin;
using Retracker;

[assembly: OwinStartup(typeof(Startup))]
namespace Retracker
{
    public class Startup
    {
        // ReSharper disable once UnusedMember.Global
        // ReSharper disable once UnusedParameter.Global
        public void Configuration(IAppBuilder app) {}
    }
}