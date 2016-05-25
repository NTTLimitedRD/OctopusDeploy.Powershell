namespace DD.Cloud.OctopusDeploy.Powershell.Contracts
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// The data contract for a variable set
    /// </summary>
    public class LibraryVariableSet
    {
        // A unique identifier for this resource.
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

        // The id of the associated variable set.
        public string VariableSetId
        {
            get;
            set;
        }

        // Describes the purpose of the variable set (Variable/ScriptModule/etc)
        public string ContentType
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