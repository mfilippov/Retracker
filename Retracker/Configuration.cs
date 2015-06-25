using System.Configuration;

namespace Retracker
{
    public static class Configuration
    {
        public static string MongoDbConnectionString => ConfigurationManager.ConnectionStrings["MongoDbConnection"].ConnectionString;
        public static int AnnounceInterval => int.Parse(ConfigurationManager.AppSettings["Interval"]);
        public static int AnnounceMinimumInterval => int.Parse(ConfigurationManager.AppSettings["MinimumInterval"]);
    }
}