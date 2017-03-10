using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DD.Cloud.OctopusDeploy.Powershell.Contracts
{
    public class Lifecycles
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
        public List<Lifecycle> Items
        {
            get;
            set;
        }

    }
}
