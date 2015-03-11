using System;

namespace DD.Cloud.OctopusDeploy.Powershell.Contracts
{
    public class Feed
    {
        public string Id
        {
            get;
            set;
        }
        public string Name
        {
            get;
            set;
        }

        public Uri FeedUri
        {
            get; 
            set;
        }
    }
}
