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

    [Cmdlet(VerbsCommon.Get, "OctoUserPermissions", ConfirmImpact = ConfirmImpact.Low)]
    public class GetOctoUserPermissions : OctoCmdlet
    {
        /// <summary>
        /// The user Id.
        /// </summary>
        [Parameter(Mandatory = true, ParameterSetName = "GetOctoUserPermissionsByUserId")]
        public string UserId
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

            var uri = "/api/users";
            uri += "/{id}/permissions";
            var request = new RestRequest(uri, Method.GET);
            request.AddUrlSegment("id", UserId);
            request.AddHeader("X-Octopus-ApiKey", ApiKey);
            var response = await client.ExecuteTaskAsync<List<PermissionSets>>(request);

            if (response.StatusCode != HttpStatusCode.OK)
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
