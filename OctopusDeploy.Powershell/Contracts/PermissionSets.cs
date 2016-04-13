namespace DD.Cloud.OctopusDeploy.Powershell.Contracts
{
    using System.Collections.Generic;

    class PermissionSets
    {
        public string Id
        {
            get;
            set;
        }
        public List<Permission> Permissions
        {
            get;
            set;
        }
        public List<Team> Teams
        {
            get;
            set;
        }
    }
}
