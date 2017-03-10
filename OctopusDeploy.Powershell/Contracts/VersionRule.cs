using System;
using System.Collections.Generic;

namespace DD.Cloud.OctopusDeploy.Powershell.Contracts
{
    public class VersionRule
    {
        public string Id
        {
            get;
            set;
        }
        public List<string> Actions
        {
            get;
            set;
        }

        public string Tag
        {
            get;
            set;
        }
    }
}

