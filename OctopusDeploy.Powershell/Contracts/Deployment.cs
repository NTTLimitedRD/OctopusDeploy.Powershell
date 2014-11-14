namespace DD.Cloud.OctopusDeploy.Powershell.Contracts
{
    using System;


    /// <summary>
    /// The data contract for a release
    /// </summary>
    public class Deployment
    {
        public string Id
        {
            get;
            set;
        }

        public string ReleaseId 
        {
            get;
            set;
        }

        public string EnvironmentId
        {
            get;
            set;
        }

        public bool ForcePackageDownload
        {
            get;
            set;
        }

        public bool ForcePackageRedeployment
        {
            get;
            set;
        }

        public string TaskId
        {
            get;
            set;
        }

        public string ProjectId
        {
            get;
            set;
        }

        public bool UseGuidedFailure
        {
            get;
            set;
        }

        public string Comments
        {
            get;
            set;
        }

        public DateTime QueueTime
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        }

        public DateTime Created
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