namespace DD.Cloud.OctopusDeploy.Powershell.Contracts
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// The data contract for a release
    /// </summary>
    public class Release
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

        public DateTime Assembled
        {
            get;
            set;
        }

        public string ReleaseNotes
        {
            get;
            set;
        }

        public string ChannelId
        {
            get;
            set;
        }
        public string ProjectId
        {
            get;
            set;
        }

        public string Version
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

        public string ProjectVariableSetSnapshotId { get; set; }

        public List<string> LibraryVariableSetSnapshotIds
        {
            get;
            set;
        }

        public string ProjectDeploymentProcessSnapshotId
        {
            get;
            set;
        }

        public List<StepPackage> SelectedPackages
        {
            get;
            set;
        }
    }
}