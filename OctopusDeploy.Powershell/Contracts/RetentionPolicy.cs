using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DD.Cloud.OctopusDeploy.Powershell.Contracts
{
    /// <summary>
    /// The data contract for a RetentionPolicy
    /// </summary>
    public class RetentionPolicy
    {
        public string Unit
        {
            get;
            set;
        }
        public int QuantityToKeep
        {
            get;
            set;
        }
        public bool ShouldKeepForever
        {
            get;
            set;
        }

    }
}