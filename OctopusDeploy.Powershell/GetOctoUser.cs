namespace DD.Cloud.OctopusDeploy.Powershell
{
    using System;
    using System.Linq;
    using System.Management.Automation;
    using System.Net;
    using System.Threading.Tasks;
    using Contracts;
    using RestSharp;
    using Task = System.Threading.Tasks.Task;
    using System.Collections.Generic;

    [Cmdlet(VerbsCommon.Get, "OctoUser", ConfirmImpact = ConfirmImpact.Low)]
    public class GetOctoUser : OctoCmdlet
    {
        /// <summary>
        /// The user Id.
        /// </summary>
        [Parameter(Mandatory = false, ParameterSetName = "GetOctoUserByUserId")]
        public string UserId
        {
            get;
            set;
        }

        /// <summary>
        /// The Username of the user to retrieve.
        /// </summary>
        [Parameter(Mandatory = false, ParameterSetName = "GetOctoUserByUsername")]
        public string Username
        {
            get;
            set;
        }

        /// <summary>
        /// The EmailAddress of the user to retrieve.
        /// </summary>
        [Parameter(Mandatory = false, ParameterSetName = "GetOctoUserByEmailAddress")]
        public string EmailAddress
        {
            get;
            set;
        }

        [Parameter(Mandatory = false, ParameterSetName = "ListAvailable")]
        public SwitchParameter ListAvailable
        {
            get;
            set;
        }


        /// <summary>
        ///		Asynchronously perform Cmdlet processing.
        /// </summary>
        /// <returns>
        ///		A <see cref="Task"/> representing the asynchronous operation.
        /// </returns>
        protected override async Task ProcessRecordAsync()
        {
            var client = new RestClient(BaseUri);

            string userId = string.Empty;
            string filterByUsername = string.Empty;
            string filterByEmailAddress = string.Empty;

            switch (ParameterSetName)
            {
                case "GetOctoUserByUserId":
                {
                        userId = UserId;
                        break;
                }
                case "GetOctoUserByUsername":
                    {
                        filterByUsername = Username;
                        break;
                    }
                case "GetOctoUserByEmailAddress":
                    {
                        filterByEmailAddress = EmailAddress;
                        break;
                    }
            }
            var uri = "/api/users";
            if (userId != string.Empty)
            {
                uri += "/{id}";
                var request = new RestRequest(uri, Method.GET);
                request.AddUrlSegment("id", userId);
                request.AddHeader("X-Octopus-ApiKey", ApiKey);
                var response = await client.ExecuteTaskAsync<User>(request);

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    WriteError(new ErrorRecord(new Exception(response.ErrorMessage ?? response.Content), "Failed", ErrorCategory.OpenError, null));
                }
                else
                {
                    WriteObject(response.Data);
                }
            }
            else if (ListAvailable.IsPresent || filterByUsername != string.Empty || filterByEmailAddress != string.Empty)
            {
                uri += "/all";
                var request = new RestRequest(uri, Method.GET);
                request.AddHeader("X-Octopus-ApiKey", ApiKey);
                var response = await client.ExecuteGetTaskAsync<List<User>>(request);
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    WriteError(new ErrorRecord(new Exception(response.ErrorMessage ?? response.Content), "Failed", ErrorCategory.OpenError, null));
                }
                else
                {
                    if (filterByUsername == string.Empty && filterByEmailAddress == string.Empty)
                    {
                        WriteObject(response.Data);
                    }
                    else if (filterByUsername != string.Empty)
                    {
                        var allUsers = response;
                        WriteObject(allUsers.Data
                       .FirstOrDefault(
                           i => (string.Compare(i.Username, filterByUsername, StringComparison.InvariantCultureIgnoreCase) == 0)));
                    }
                    else
                    {
                        var allUsers = response;
                        foreach (var user in allUsers.Data.Where(user => user.EmailAddress != null && user.EmailAddress.ToLower().Contains(filterByEmailAddress.ToLower())))
                        {
                            WriteObject(user);
                            break;
                        }
                    }
                }
            }
        }
    }
}
