namespace DD.Cloud.OctopusDeploy.Powershell.Contracts
{
    using System.Collections.Generic;

    /// <summary>
    /// The data contract for a releases
    /// </summary>
    public class Releases
    {
        public string ItemType
        {
            get;
            set;
        }

        public bool IsStale
        {
            get;
            set;
        }

        public int TotalResults
        {
            get;
            set;
        }

        public int ItemsPerPage
        {
            get;
            set;
        }
        public List<Release> Items
        {
            get;
            set;
        }
    }
}