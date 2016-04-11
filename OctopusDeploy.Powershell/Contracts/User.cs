namespace DD.Cloud.OctopusDeploy.Powershell.Contracts
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// The data contract for a user
    /// </summary>
    public class User
    {
        public string Id
        {
            get;
            set;
        }
        public string Username
        {
            get;
            set;
        }
        public string DisplayName
        {
            get;
            set;
        }
        public string EmailAddress
        {
            get;
            set;
        }
        public string Password
        {
            get;
            set;
        }
        public bool IsActive
        {
            get;
            set;
        }
    }
}