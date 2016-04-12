using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DD.Cloud.OctopusDeploy.Powershell.Contracts
{
    class Team
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

        public List<string> MemberUserIds
        {
            get;
            set;
        }

        public List<string> UserRoleIds
        {
            get;
            set;
        }

        public List<string> ProjectIds
        {
            get;
            set;
        }

        public List<string> EnvironmentIds
        {
            get;
            set;
        }
    }
}
