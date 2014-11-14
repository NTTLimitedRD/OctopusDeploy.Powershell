namespace DD.Cloud.OctopusDeploy.Powershell.Contracts
{
    using System;

    /// <summary>
    /// The data contract for specifying a package version. This is used for creating a release.
    /// </summary>
    public class SpecificPackageVersion
    {
        public string PackageId
        {
            get;
            set;
        }

        public Version Version
        {
            get;
            set;
        }
    }

    /// <summary>
    /// The data contract for a step package. This is used for creating a release.
    /// </summary>
    public class StepPackage
    {
        public string PackageId
        {
            get;
            set;
        }

        public string StepName
        {
            get;
            set;
        }

        public string Version
        {
            get;
            set;
        }
    }
}