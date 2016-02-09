using System;
using System.Collections.Generic;

namespace DD.Cloud.OctopusDeploy.Powershell.Contracts
{
    public class PagedResult<T>
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

        public bool ItemsPerPage
        {
            get;
            set;
        }

        public List<T> Items
        {
            get;
            set;
        }
    }
}