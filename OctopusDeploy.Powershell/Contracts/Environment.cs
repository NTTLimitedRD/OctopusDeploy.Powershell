namespace DD.Cloud.OctopusDeploy.Powershell.Contracts
{
    using System;

    /// <summary>
    /// The data contract for an environment
    /// </summary>
    public class Environment
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

        public int SortOrder
        {
            get;
            set;
        }

        public bool UseGuidedFailure
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