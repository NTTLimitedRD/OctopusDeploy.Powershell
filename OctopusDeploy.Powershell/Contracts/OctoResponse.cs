namespace DD.Cloud.OctopusDeploy.Powershell.Contracts
{
    using System.Collections.Generic;

    /// <summary>
    /// Generic octopus deploy response contract
    /// </summary>
    /// <typeparam name="TItem">
    /// The data contract type of each item.
    /// </typeparam>
    public class OctoResponse<TItem>
        where TItem : class
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

        public List<TItem> Items
        {
            get;
            set;
        } 
    }
}