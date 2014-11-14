namespace DD.Cloud.OctopusDeploy.Powershell.Contracts
{
    /// <summary>
    /// The data contract for a package template
    /// </summary>
    public class PackageTemplate
    {
        public string StepName
        {
            get;
            set;
        }

        public string NuGetPackageId
        {
            get;
            set;
        }

        public string NuGetFeedId
        {
            get;
            set;
        }

        public string NuGetFeedName
        {
            get;
            set;
        }

        public string VersionSelectedLastRelease
        {
            get;
            set;
        }

        public bool IsResolvable
        {
            get;
            set;
        }
    }
}