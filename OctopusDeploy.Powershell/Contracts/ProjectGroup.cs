namespace DD.Cloud.OctopusDeploy.Powershell.Contracts
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// The data contract for a project group
    /// </summary>
    public class ProjectGroup
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

        public string Description
        {
            get;
            set;
        }

        public List<string> EnvironmentIds
        {
            get;
            set;
        }

        public string RetentionPolicyId
        {
            get;
            set;
        }

        public DateTime LastModifiedOn
        {
            get;
            set;
        }

        public string LastModifiedBy
        {
            get;
            set;
        }
    }
}