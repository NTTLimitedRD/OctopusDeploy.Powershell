using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DD.Cloud.OctopusDeploy.Powershell.Contracts
{
    /// <summary>
    /// The data contract for a Phase
    /// </summary>
    public class Phase
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
        public List<string> AutomaticDeploymentTargets
        {
            get;
            set;
        }
        public List<string> OptionalDeploymentTargets
        {
            get;
            set;
        }
        public int MinimumEnvironmentsBeforePromotion
        {
            get;
            set;
        }
        public RetentionPolicy ReleaseRetentionPolicy
        {
            get;
            set;
        }

        public RetentionPolicy TentacleRetentionPolicy
        {
            get;
            set;
        }
    }
}