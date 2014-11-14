namespace DD.Cloud.OctopusDeploy.Powershell.Contracts
{
    /// <summary>
    /// The data contract for a package
    /// </summary>
    public class Package
    {
        public string Id
        {
            get;
            set;
        }

        public string NuGetPackageId
        {
            get;
            set;
        }

        public string Title
        {
            get;
            set;
        }

        public string Summary
        {
            get;
            set;
        }

        public string NuGetFeedId
        {
            get;
            set;
        }

        public string Version
        {
            get;
            set;
        }

        public string Description
        {
            get;
            set;
        }

        public string Published
        {
            get;
            set;
        }

        public string ReleaseNotes
        {
            get;
            set;
        }
    }
}