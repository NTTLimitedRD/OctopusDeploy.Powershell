using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DD.Cloud.OctopusDeploy.Powershell.Contracts
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// The data contract for a project
    /// </summary>
    public class Project
    {
        public string Id
        {
            get;
            set;
        }

        public string VariableSetId
        {
            get;
            set;
        }

        public string DeploymentProcessId
        {
            get;
            set;
        }

        public List<string> IncludedLibraryVariableSetIds
        {
            get;
            set;
        } 

        public string Name
        {
            get;
            set;
        }

        public bool DefaultToSkipIfAlreadyInstalled
        {
            get;
            set;
        }

        public string Description
        {
            get;
            set;
        }

        public bool IsDisabled
        {
            get;
            set;
        }

        public string ProjectGroupId
        {
            get;
            set;
        }

        public DateTime LastModifiedOn
        {
            get;
            set;
        }

        public string LastModifiedBy
        {
            get;
            set;
        }
    }
}
