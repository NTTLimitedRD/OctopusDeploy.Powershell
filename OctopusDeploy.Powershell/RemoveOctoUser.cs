namespace DD.Cloud.OctopusDeploy.Powershell
{
    using System;
    using System.Management.Automation;
    using System.Net;
    using RestSharp;
    using Task = System.Threading.Tasks.Task;

    [Cmdlet(VerbsCommon.Remove, "OctoUser", ConfirmImpact = ConfirmImpact.Medium)]
    public class RemoveOctoUser : OctoCmdlet
    {
        public RemoveOctoUser()
        {
        }

        /// <summary>
        /// The userId of the user we remove
        /// </summary>
        [Parameter(Mandatory = true)]
        public string UserId
        {
            get;
            set;
        }


        /// <summary>
        ///		Asynchronously perform Cmdlet processing.
        /// </summary>
        /// <returns>
        ///		A <see cref="System.Threading.Tasks.Task"/> representing the asynchronous operation.
        /// </returns>
        protected override async Task ProcessRecordAsync()
        {
            var client = new RestClient(BaseUri);

            var request = new RestRequest("/api/users/{id}", Method.DELETE);
            request.AddUrlSegment("id", UserId);
            request.AddHeader("X-Octopus-ApiKey", ApiKey);

            var response = await client.ExecuteTaskAsync(request);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                WriteError(new ErrorRecord(new Exception(response.ErrorMessage ?? response.Content), "Failed", ErrorCategory.OpenError, null));
            }
            else
            {
                WriteObject(response.Content);
            }
        }
    }
}