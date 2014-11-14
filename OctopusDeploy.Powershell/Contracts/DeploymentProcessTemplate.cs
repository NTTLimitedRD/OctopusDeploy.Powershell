namespace DD.Cloud.OctopusDeploy.Powershell.Contracts
{
    using System.Collections.Generic;

    /// <summary>
    /// The data contract for a deployment template
    /// </summary>
    public class DeploymentProcessTemplate
    {
        public string DeploymentProcessId
        {
            get;
            set;
        }

        public string LastReleaseVersion
        {
            get;
            set;
        }

        public string VersioningPackageStepName
        {
            get;
            set;
        }

        public List<PackageTemplate> Packages
        {
            get;
            set;
        }
    }
}