namespace DD.Cloud.OctopusDeploy.Powershell
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Management.Automation;
    using System.Net;
    using System.Threading.Tasks;
    using Contracts;
    using RestSharp;
    using Task = System.Threading.Tasks.Task;

    [Cmdlet(VerbsCommon.New, "OctoUser", ConfirmImpact = ConfirmImpact.Medium)]
    public class NewOctoUser : OctoCmdlet
    {
        public NewOctoUser()
        {
            //SpecificPackageVersions = new Hashtable();
            IsActive = true;
            Password = "Welcome1";
        }

        /// <summary>
        /// The new user's username
        /// </summary>
        [Parameter(Mandatory = true)]
        public string Username
        {
            get;
            set;
        }

        /// <summary>
        /// The new user's display name
        /// </summary>
        [Parameter(Mandatory = true)]
        public string DisplayName
        {
            get;
            set;
        }

        /// <summary>
        /// The new user's email address
        /// </summary>
        [Parameter(Mandatory = true)]
        public string EailAddress
        {
            get;
            set;
        }

        /// <summary>
        /// The new user's password
        /// </summary>
        [Parameter(Mandatory = false)]
        public string Password
        {
            get;
            set;
        }

        /// <summary>
        /// Is the new user an active user
        /// </summary>
        [Parameter(Mandatory = false)]
        public bool IsActive
        {
            get;
            set;
        }

        //[Parameter(ParameterSetName = "NewOctoReleaseByProject")]
        //[Parameter(ParameterSetName = "NewOctoReleaseByProjectId")]
        //public Hashtable SpecificPackageVersions
        //{
        //    get;
        //    set;
        //}

        /// <summary>
        ///		Asynchronously perform Cmdlet processing.
        /// </summary>
        /// <returns>
        ///		A <see cref="System.Threading.Tasks.Task"/> representing the asynchronous operation.
        /// </returns>
        protected override async Task ProcessRecordAsync()
        {
            var client = new RestClient(BaseUri);

            var request = new RestRequest("/api/users", Method.POST);
            request.AddHeader("X-Octopus-ApiKey", ApiKey);
            request.AddJsonBody(new Contracts.User
            {
                Username = Username,
                DisplayName = DisplayName,
                EmailAddress = EailAddress,
                IsActive = IsActive,
                Password = Password
            });

            var response = await client.ExecuteTaskAsync<Contracts.User>(request);
            if (response.StatusCode != HttpStatusCode.Created)
            {
                WriteError(new ErrorRecord(new Exception(response.ErrorMessage ?? response.Content), "Failed", ErrorCategory.OpenError, null));
            }
            else
            {
                WriteObject(response.Data);
            }
        }
    }
}