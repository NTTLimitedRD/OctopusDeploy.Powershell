namespace DD.Cloud.OctopusDeploy.Powershell.Contracts
{
    /// <summary>
    /// The data contract for a variable
    /// </summary>
    public class Variable
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

        public string Value
        {
            get;
            set;
        }

        public Scope Scope
        {
            get;
            set;
        }

        public bool IsSensitive
        {
            get;
            set;
        }

        public bool IsEditable
        {
            get;
            set;
        }
    }
}