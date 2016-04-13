namespace DD.Cloud.OctopusDeploy.Powershell.Contracts
{
    using System.Collections.Generic;

    class Permission
    {
        public string Name
        {
            get;
            set;
        }
        public List<string> RestrictedToProjectIds
        {
            get;
            set;
        }
        public List<string> RestrictedToEnvironmentIds
        {
            get;
            set;
        }
    }
}
