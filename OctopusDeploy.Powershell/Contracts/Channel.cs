using System;
using System.Collections.Generic;

namespace DD.Cloud.OctopusDeploy.Powershell.Contracts
{
    public class Channel
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

        public string ProjectId
        {
            get; 
            set;
        }

        public bool IsDefault
        {
            get;
            set;
        }

        public string LifecycleId
        {
            get;
            set;
        }

        public List<VersionRule> Rules
        {
            get;
            set;
        }
    }
}

