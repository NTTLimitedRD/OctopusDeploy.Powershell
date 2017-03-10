using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DD.Cloud.OctopusDeploy.Powershell.Contracts
{
    /// <summary>
    /// The data contract for a variable
    /// </summary>
    public class Phase
    {
        public string Name
        {
            get;
            set;
        }

        public List<Environment> Environments
        {
            get;
            set;
        }
    }
}