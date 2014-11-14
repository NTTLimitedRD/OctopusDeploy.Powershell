namespace DD.Cloud.OctopusDeploy.Powershell.Contracts
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// The data contract for a variable set
    /// </summary>
    public class VariableSet
    {
        public string Id
        {
            get;
            set;
        }

        public string OwnerId
        {
            get;
            set;
        }
        public int Version
        {
            get;
            set;
        }

        public List<Variable> Variables
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