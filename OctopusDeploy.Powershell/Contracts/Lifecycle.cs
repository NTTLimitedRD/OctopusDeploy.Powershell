using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DD.Cloud.OctopusDeploy.Powershell.Contracts
{
    public class Lifecycle
    {
        public string Id
        {
            get;
            set;
        }
        public string Name
        {
            get;
            set;
        }

        public List<Phase> Phases
        {
            get;
            set;
        }
    }
}
